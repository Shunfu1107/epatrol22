import argparse, numpy as np, yaml, lancedb, cv2
import degirum as dg, degirum_tools
from face_processor import FaceProcessor
from degirum_tools.video_support import (
    get_video_stream_properties
)
from degirum_tools.ui_support import Display
from face_recognition_schema import FaceRecognitionSchema
import os
import time

# Get the directory of the current script
current_directory = os.path.dirname(os.path.abspath(__file__))

# File to signal person detection
PERSON_FOUND_FILE = os.path.join(current_directory, "person_found.txt")

# Add the stop flag file path
STOP_FLAG_FILE = os.path.join(current_directory, "stop_flag.txt")

def should_stop():
    return os.path.exists(STOP_FLAG_FILE)

def person_found(name):
    with open(PERSON_FOUND_FILE, "w") as file:
        file.write(name)

def load_yaml(yaml_config):
    with open(yaml_config, "r") as file:
        return yaml.safe_load(file)

def main():
    parser = argparse.ArgumentParser(description="Face Detection and Cropping Tool")
    parser.add_argument(
        "--config", type=str, required=True, help="Path to the config file"
    )
    parser.add_argument(
        "--video_source",
        required=True,
        help="video source id - identifier of input video stream. It can be:"
                "  - cv2.VideoCapture object, already opened by open_video_stream()"
                "  - 0-based index for local cameras"
                "  - IP camera URL in the format 'rtsp://<user>:<password>@<ip or hostname>'"
                "  - Local path or URL to mp4 video file"
                "  - YouTube video URL"
    )
    parser.add_argument(
        "--display",
        type=bool,
        default=False,  # Default is False
        help="Enable visual display. This option is always True."
    )
    parser.add_argument(
        "--output_video_path",
        type=str,
        default="annotated_output_video.mp4",
        help="Path to save the annotated video",
    )
    parser.add_argument(
    "--save_output",
    action='store_true',  # This will set it to True if the flag is provided
    help="Enable visual display. This option is always True."
)
    parser.add_argument(
        "--threshold", type=str, default=0.30, help="Cosine distance threshold"
    )
    parser.add_argument("--fps", type=str, default=None, help="fps")

    args = parser.parse_args()

    config = load_yaml(args.config)

    uri = config.get("database", {}).get("name", "default_database")
    table_name = config.get("table", {}).get("name", "default_table")
    search_params = config.get("search_params", {})
    top_k = search_params.get("top_k", 1)
    field_name = search_params.get("field_name", "vector")
    metric_type = search_params.get("metric_type", "cosine")

    # Connect to the LanceDB database
    db = lancedb.connect(uri=uri)

    # Check if the table exists, create if not
    if table_name in db.table_names():
        """Open an existing table in the database."""
        tbl = db.open_table(table_name)
        schema_fields = [field.name for field in tbl.schema]
        if schema_fields != list(FaceRecognitionSchema.model_fields.keys()):
            raise RuntimeError(
                f"Table {table_name} has a different schema."
            )

    if args.display:
        win_name = f"Annotating {args.video_source}"
        display = Display(win_name)

    if args.video_source.isdigit():
        args.video_source = int(args.video_source)

    w, h, video_fps = get_video_stream_properties(args.video_source)

    # Overwrite the video stream's FPS if the fps argument is set.
    if args.fps:
        video_fps = args.fps
    if args.save_output:
        fourcc = cv2.VideoWriter_fourcc('m', 'p', '4', 'v')  # You can also use 'MJPG' or 'MP4V'
        writer = cv2.VideoWriter(args.output_video_path, fourcc, video_fps, (w, h))

    # Load Face detection and keypoints model
    face_keypoints_model = dg.load_model(
        model_name=config["face_det_kypts_model"]["model_name"],
        inference_host_address=config["hw_location"],
        zoo_url=config["face_det_kypts_model"]["model_zoo_url"],
        token=degirum_tools.get_token(),
        overlay_show_probabilities=True
    )

    # Load Face Reid model
    face_reid_model = dg.load_model(
        model_name=config["face_reid_model"]["model_name"],
        inference_host_address=config["hw_location"],
        zoo_url=config["face_reid_model"]["model_zoo_url"],
        token=degirum_tools.get_token()
    )

    # Initialize the face processing pipeline
    face_processor = FaceProcessor(
        face_keypoints_model, face_reid_model, most_centered_only=False
    )

    # Run the video inference
    for result in face_processor.predict_stream(args.video_source, fps=video_fps):
        if should_stop():
            break  

        for i, res in enumerate(result.results):
            search_result = tbl.search(np.array(res["embedding"][0]).astype(np.float32), vector_column_name = field_name) \
                            .metric(metric_type) \
                            .limit(top_k) \
                            .to_list()

            distance = round(1 - search_result[0]["_distance"], 2)
            if distance >= args.threshold:
                res["label"] = search_result[0]["entity_name"]
                person_found(f"{res['label']} ({args.video_source})")
                break
            else:
                res["label"] = "Unknown"

            res["score"] = distance
            result.results[i].pop("landmarks", None)
            result.results[i].pop("embedding", None)
        img = result.image_overlay
        if args.display:
            display.show(img)
        if args.save_output:
            writer.write(img)

    # Cleanup and finalize the video file
    if args.save_output:
        writer.release()  # Ensure the video file is saved properly when stopped

if __name__ == "__main__":
    main()
