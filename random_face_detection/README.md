
# Face Recognition

## Quickstart 
1. Clone the repository:
      ```
      git clone https://github.com/DeGirum/degirum_face_recognition.git
      cd degirum_face_recognition
      ```
2. Install the required dependencies:
      ```
      pip install -r requirements.txt
      ```
3. Install the package locally:
      ```
      pip install .
      ```
## Usage

#### Setting the DEGIRUM_CLOUD_TOKEN Environment Variable
To access hardware options and model zoos in the DeGirum Cloud Platform with degirum_cli, you need to pass the DEGIRUM_CLOUD_TOKEN variable to the functions. The token can be set as an environment variable instead of passing it as an argument to every function. For detailed instructions on how to set this environment variable across various systems (including Linux, macOS, Windows, and virtual environments), please refer to [this guide](https://gist.github.com/shashichilappagari/ab856f4ed85fbfb623bc949cf453925b). Rest of the user guide below assumes that the token is set as an environment variable.



## Create a Database
Steps to follow to create a database of images : <br>
1. **Add a folder of images to the database** (input_path = Path to the database) <br>
    * Gather all the images you want to include in the database and place them in a single folder. <br>
    * Ensure that each image filename starts with the identity name,  followed by an underscore (“_”), and then include any integers or strings that you prefer. <br>
    * Most importantly, ensure that the filenames are unique, following the format 'uniqueName_1', where each 'uniqueName' should differ. <br>
    (If two people in the database has the same name, just add the lastname along with the uniquename, For example : 'uniqueNameLastName_1').

        Example of how the folder structure should look like,

        ```
        folder
        ├── John_1.jpg
        ├── John_2.jpg 
        ├── Michael_1.jpg
        ├── Michael_a.jpg
        ├── MichaelKeaton_1.jpg
        ├── MichaelKeaton_a.jpg
        ```
      **CLI**
        ```
        python face_recognition/index_faces.py --config face_recognition/config.yaml --input_path <path_to_the_database>
        ````

**OR**  

2. **Add a single image to the database** (input_path = Path to the single image file) <br>
    * Ensure that the image filename starts with the identity name,  followed by an underscore (“_”), and then include any integers or strings that you prefer. <br>
        ```
        For example:
            input_path = "John_1.jpg"
        ```

        **CLI**
        ```
        python face_recognition/index_faces.py --config face_recognition/config.yaml --input_path <path_to_the_image_file>
        ```

    **OR** <br>

    * Make sure to include the identity name along with the filename if it does not adhere to the above specified naming structure.
        ```
        For example:
            identity_name = "John"
            input_path = "image1.jpg"
        ```
        **CLI**
        ```
        python face_recognition/index_faces.py --config face_recognition/config.yaml --input_path <path_to_the_image_file> --identity_name <identity_name>
        ```
## Video Inference

Specify the **config file**, **video source**, **threshold** (default= 0.35), **fps** , **display**, **save_output** and the **output path** (default = "annotated_output_video.mp4"). <br>
**Input video source** can be:
  - cv2.VideoCapture object, already opened by open_video_stream()
  - 0-based index for local cameras
  - IP camera URL in the format 'rtsp://<user>:<password>@<ip or hostname>'
  - Local path or URL to mp4 video file
  - YouTube video URL <br>

Provide **--save_output** in the CLI to write the annotated video.<br>
You can also play around with the `threshold` parameter as this defines the cosine distance threshold.<br>

**CLI**
  ```
  python face_recognition/identify_faces.py --config face_recognition/config.yaml --video_source <input_video_source>
  ```

**CONFIG**

There is a config yaml file (**config.yaml**) with all the parameters to index a Database.
  ```
  hw_location : "@cloud"
  face_det_kypts_model:
    model_zoo_url: "degirum/models_n2x"
    model_name: "yolov8n_relu6_widerface_kpts--640x640_quant_n2x_orca1_1"
  face_reid_model:
    model_zoo_url: "degirum/models_n2x"
    model_name : "mbf_w600k--112x112_float_n2x_orca1_1"
  database:
    name: "face_recognition.db"
  table:
    name: "faces"
  search_params:
      top_k: 1
      field_name: "vector"
      metric_type: "cosine"
  ```
The parameters that can be modified are:
* Database Name: Specify the name of the database in the config file. Default database name: "default_database" <br>
    For example: <br>
    ```
    database:
        name: "face_recognition.db"
    ```
* Table Name: Specify the name of the table in the config file. Default table name: "default_table" <br>
    For example: <br>
    ```
    collection:
        name: "faces"
    ```

## Models

**Face Recognition** is a three step process:
1. *Face Detection with Keypoints* - The face detection with keypoints model provides a bounding box for each detected face along with its 5 landmarks(Left Eye, Right Eye, Nose, Left Lip, Right Lip). 
2. The face is cropped and aligned based on the corresponding five landmarks using the *align_and_crop* method.
3. *Face Re-identification* - The face reid model provides a feature embedding for the above aligned and cropped face. <br>

Below are the default entries: <br>
  * Inference Host : @cloud <br>
  * Face Detection and Keypoints Model <br>
      * Model Zoo : degirum/models_n2x <br>
      * Model Name : yolov8n_relu6_widerface_kpts--640x640_quant_n2x_orca1_1 <br>

  * Face Re-identification Model
      * Model Zoo : degirum/models_n2x <br>
      * Model Name : mbf_w600k--112x112_float_n2x_orca1_1 <br>
