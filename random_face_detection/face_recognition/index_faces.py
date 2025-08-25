import yaml, argparse, lancedb
import degirum as dg, degirum_tools
from pathlib import Path
from face_processor import FaceProcessor
from face_recognition_schema import FaceRecognitionSchema

def load_config(config_path):
    """Load configuration from a YAML file."""
    with open(config_path, "r") as file:
        return yaml.safe_load(file)

def image_generator(input_path, identity_name = None):
    """Generate image paths from a given directory or a single image file."""
    path = Path(input_path)
    # If the input path is a single file, yield it if it's an image along with its entity name
    if path.is_file() and path.suffix.lower() in (".png", ".jpg", ".jpeg"):
        entity_name = identity_name if identity_name is not None else path.stem.split("_")[0]
        yield str(path), {"image_path": str(path), "entity_name": entity_name}
    # If it's a directory, yield all image files found within along with its entity name
    else:
        for file in path.rglob("*"):
            if file.suffix.lower() in (".png", ".jpg", ".jpeg"):
                entity_name = file.stem.split("_")[0]
                yield str(file), {"image_path": str(file), "entity_name": entity_name}

def main():
    """Main function to process images and index them into the database."""
    parser = argparse.ArgumentParser(description="Indexing a Database with faces")
    parser.add_argument(
        "--config", type=str, required=True, help="Path to the config file"
    )
    parser.add_argument(
        "--input_path",
        type=str,
        required=True,
        help="Path to the folder of images or a single image file",
    )
    parser.add_argument("--identity_name", type=str, default=None, help="Identity name")
    args = parser.parse_args()

    # Load the configuration
    config = load_config(args.config)
    uri = config.get("database", {}).get("name", "default_database")
    table_name = config.get("table", {}).get("name", "default_table")

    # Connect to the LanceDB database
    db = lancedb.connect(uri=uri)

    # Check if the table exists, create if not
    if table_name not in db.table_names():
        """Create a new table in the database."""
        tbl = db.create_table(table_name, schema=FaceRecognitionSchema)
    else:
        """Open an existing table in the database."""
        tbl = db.open_table(table_name)
        schema_fields = [field.name for field in tbl.schema]
        if schema_fields != list(FaceRecognitionSchema.model_fields.keys()):
            raise RuntimeError(
                f"Table {table_name} has a different schema."
            )

    # Load Face detection and keypoints model
    face_keypoints_model = dg.load_model(
        model_name=config["face_det_kypts_model"]["model_name"],
        inference_host_address=config["hw_location"],
        zoo_url=config["face_det_kypts_model"]["model_zoo_url"],
        token=degirum_tools.get_token()
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
        face_keypoints_model, face_reid_model, most_centered_only=True
    )

    num_entities =  0 # Count the number of entities
    # Process images in batches
    for result in face_processor.predict_batch(image_generator(args.input_path, args.identity_name)):
        # Format data for the FaceRecognitionSchema
        data = FaceRecognitionSchema.format_data(result)
        if len(data) > 0:
            # Add the LanceSchema data to the table
            tbl.add(data=data)
        num_entities+=len(data)

    # Prints the number of entities added to the table
    print (f"Successfully added {num_entities} entities to the {table_name} table.")
    # Prints the total number of entities in the table
    print(f"{table_name} table contains a total of {tbl.count_rows()} entities.")

if __name__ == "__main__":
    main()

# python face_recognition/index_faces.py --config face_recognition/config.yaml --input_path <path_to_the_database>

# ffmpeg -i rtsp://admin:mquest_123@192.168.1.18/media/0 -c:v libx264 -preset veryfast -crf 23 -g 60 -hls_time 2 -hls_list_size 0 -f hls -c copy -f mp4 C:\Users\shaun\Desktop\Stream\output.mp4
