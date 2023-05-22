# Procedural Flight Sim

![output_gifsicle](https://user-images.githubusercontent.com/123111159/218338536-8e3800f7-f691-4634-85f1-57e6e8d8618e.gif)

--- 
## How to play

Procedural Flight Sim is a procedurally generated flight simulator meant for taking a cup of
tea and relaxing while playing it. As such you can enjoy a peaceful voyage through the
skies with nice views and dynamically changing lighting conditions.
---
## Tech
Procedural Flight Sim builds it's worth upon ability to create beautiful terrains as well as good looking and sky
that feels alive. As souch it consists of those parts:

### Terrain generation

Terrain generation is divided into two parts that work together:

- Main Unity thread
  - Schedules chunks to generate on separate threads
    - Runs coroutines for
      - Spawning chunks
      - Spawning clouds and pigeons
- New threads spawned for
  - Generating chunk data
  - Calculating chunks that are to be destroyed



### License

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![Made with Unity](https://img.shields.io/badge/Made%20with-Unity-57b9d3.svg?style=flat&logo=unity)](https://unity3d.com)
