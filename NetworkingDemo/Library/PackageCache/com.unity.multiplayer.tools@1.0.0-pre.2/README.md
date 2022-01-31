# com.unity.multiplayer.tools

The Multiplayer Tools package provides a suite of tools used in multiplayer game development. 

## Prerequisites

This package requires a Unity version of 2020.3 or above.

## Required Packages

This package must be used with a supported multiplayer development package. On its own, this package will provide tools that display no data.

### Supported Multiplayer Solutions

- Netcode for GameObjects

## Included Tools

### Network Profiler

The Network Profiler provides a view into the messages being sent by the network solution. This can be accessed through the Unity Profiler via Window/Analysis/Profiler.

On 2021.2 and above, selecting a frame in the profiler will display a detailed hierarchy view of network traffic that occurred on a given frame. The tree can be filtered using a search bar.

On 2021.1 and below, only simple stats are displayed.