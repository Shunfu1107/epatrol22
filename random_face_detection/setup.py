from setuptools import setup, find_packages

setup(
    name="degirum_face_recognition",
    version="0.1.0",
    description="Database Indexing and Facial Recognition on video source",
    # long_description=open("README.md").read(),  # Assumes you have a README.md
    packages=find_packages(),
    install_requires=[
        "lancedb",
        "scikit-image",
        "PyYAML",
        "degirum",
        "degirum-tools",
    ],
    classifiers=["Programming Language :: Python :: 3"],
    python_requires=">=3.6",
)
