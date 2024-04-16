# Mythic Empire - Realtime Server

## Description
Mythic Empire - Realtime Server is a project that utilizes SignalR, a library of ASP.NET developed by Microsoft, to build a real-time feedback system. The project aims to establish real-time connections among players in the game [Mythic Empire](https://github.com/viethuynh713/TowerDefense-Unity), enhancing the authentic experience when two players battle each other.

## Technologies Used
- [SignalR](https://dotnet.microsoft.com/en-us/apps/aspnet/signalr)
- [ASP.NET core](https://dotnet.microsoft.com/en-us/apps/aspnet)

## Main Features
- Create queues and find suitable opponents for players.
- Create and manage matches between players.
- Automatic map generation: The system automatically generates random maps that meet the criteria every time a new game session starts.
- Bi-directional data exchange with players in the game.
- Signal processing and reply mechanism for player actions.
- Real-time Response: Use SignalR to provide real-time reply to players, enhancing the gaming experience.
- Packet Handling: The system processes packets sent from the client and server to ensure efficient data transmission.
- AI System: Integration of an AI system utilizing behavior trees to make decisions for bots in the Adventure mode of the game, including:
    - Card selection decisions for gameplay.
    - Tactical strategy choices for bots.
    - Utilization of cards in various game scenarios.


## Installation and Running
1. Clone this repository.
2. Open the solution in Visual Studio.
3. Install necessary dependencies.
4. Build and run the project.

## Contribution
We welcome contributions from the community. Please create a Pull Request to contribute to this project.

## Author
- Contact: viethuynh713@gmail.com
