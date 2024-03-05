# BSON: Benchmark for Socially-Aware Navigation

## The entire projects will be uploaded soon.

BSON is a platform to train and evaluate the socially-aware navigation algorithms in realistic and diverse social environments, based on the Unity 3D game engine. It is designed to provide the following features:

- **Various pedestrian motion models**: BSON utilizes social force, ORCA, and MAC-ID to generate diverse pedestrian behaviors. It ensures a fair evaluation by preventing socially-aware navigation algorithms specialized in a particular pedestrian model from receiving inflating scores.

- **Various sensors**: BSON offers support for RGB-D cameras, 3D Lidar, 2D Lidar, and IMU, enabling users to evaluate their algorithms using sensor configurations consistent with those of their real robot.

- **Supporting ROS2 and gym**: We provide a simple python API based on gym for training and evaluating learning-based algorithms. If you use the ROS2 branch, you can obtain sensor information and control the robot via ROS2 messages, just like implementing a real robot system.

- **Easy customization**: Designing custom environments and pedestrian trajectories is easily accessible to users.


### Benchmark Description
The task of BSON is point goal navigation, which aims to navigate the robot towards a given point goal in a crowded environment. In this work, we have used the Jackal robot platform, bur it is possible to utilize different robots using [URDF Importer](https://github.com/Unity-Technologies/URDF-Importer).

### 