# Changelog
All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [1.0.0-pre.2] - 2021-10-19

- Updated documentation files in preparation for publish

## [1.0.0-pre.1] - 2021-08-18

### *Netcode Profiler*.

This release adds the Netcode Profiler to the Multiplayer Tools Package. This tool is used to inspect the network activity of **Netcode for GameObjects**.

#### Activity Section
- View detailed metrics about custom messages, scene transitions, and server logs
- View activity related to individual game objects, including network variable updates, rpcs, spawn and destroy events, and ownership changes

#### Messages Section
- View the raw messages that are being sent to the transport interface

#### Other Usability
- Connect to built players to inspect netcode activity remotely
- Filter results by name, type, number of bytes, and network direction
- Correlate network objects in the profiler with objects in the scene hierarchy
- View key metrics in graph form in the profiler charts