import cv2, numpy as np
from skimage import transform as trans
from degirum_tools.video_support import (
    video_source,
    open_video_stream
)

class FaceProcessor:
    def __init__(self, face_keypoints_model, face_reid_model, most_centered_only=False):
        self.face_keypoints_model = face_keypoints_model
        self.face_reid_model = face_reid_model
        self.most_centered_only = most_centered_only

    def align_and_crop(self, img, landmarks, image_size = 112):
        # img is the full image not the cropped bounding box.
        # landmarks should be in the same format as _arcface_ref_kps (left eye, right eye, nose, left lipcorner, right lip corner)
        # Image size should be 112 for the models we have in the zoo.
        # Returns the image you should provide to the model and the warping matrix
    
        _arcface_ref_kps = np.array(
            [
                [38.2946, 51.6963],
                [73.5318, 51.5014],
                [56.0252, 71.7366],
                [41.5493, 92.3655],
                [70.7299, 92.2041],
            ],
            dtype=np.float32,
        )
        assert len(landmarks) == 5
        assert image_size % 112 == 0 or image_size % 128 == 0

        # TODO: Understand this.
        if image_size % 112 == 0:
            ratio = float(image_size) / 112.0
            diff_x = 0
        else:
            ratio = float(image_size) / 128.0
            diff_x = 8.0 * ratio

        dst = _arcface_ref_kps * ratio
        dst[:, 0] += diff_x
        tform = trans.SimilarityTransform()
        tform.estimate(np.array(landmarks), dst)
        M = tform.params[0:2, :]

        aligned_img = cv2.warpAffine(img, M, (image_size, image_size), borderValue=0.0)

        return aligned_img, M

    def select_most_centered_face(self, results, image_size):
        """Select the most centered face from the detection results."""
        image_center = (image_size[0] / 2, image_size[1] / 2)
        min_distance = float("inf")
        most_centered_result = None

        for res in results:
            nose_kp = res["landmarks"][2]["landmark"]
            dist = (image_center[0] - nose_kp[0]) ** 2 + (
                image_center[1] - nose_kp[1]
            ) ** 2
            if dist < min_distance:
                min_distance = dist
                most_centered_result = res

        return most_centered_result

    def extract_embeddings(self, detection_results):
        """ Extract the face embeddings from aligned and cropped face"""
        if not detection_results.results:
            return detection_results  # Return the detection results as is if no faces are detected
        
        h, w, _ = detection_results.image.shape
        face_results = detection_results.results
        if self.most_centered_only and face_results:
            face_results = [self.select_most_centered_face(face_results, (w, h))]

        for result in face_results:  
            landmarks = [landmark["landmark"] for landmark in result["landmarks"]]
            aligned_img, _ = self.align_and_crop(detection_results.image, landmarks)
            embeddings = self.face_reid_model(aligned_img)
            result["embedding"] = embeddings.results[0]["data"].tolist()
        return detection_results

    def predict_batch(self, image_generator):
        """Process a batch of images from the given generator and yield results."""
        for result in self.face_keypoints_model.predict_batch(image_generator): 
            self.extract_embeddings(result)
            yield result

    def predict_stream(self,
        video_source_id,
        fps = None
    ):
        """Run a model on a video stream """

        with open_video_stream(video_source_id) as stream:
            for result in self.face_keypoints_model.predict_batch(video_source(stream, fps=fps)):  
                self.extract_embeddings(result)
                yield result

    def predict(self, image_input):
        """Process a single image from the given input and yield the results."""
        result = self.face_keypoints_model(image_input)
        self.extract_embeddings(result)
        return result

    def __call__(self, image_input):
        """Make the object callable and default to using the predict method."""
        return self.predict(image_input)
