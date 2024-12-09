## PesaVcs: A Custom Version Control System
### Project Overview

PesaVcs is a custom version control system inspired by Git, implemented in C# using a modular, component-based architecture. The project is designed to demonstrate version control concepts through a clean, extensible implementation.
Project Structure
### Modules

#### PesaVcs.CLI

Command-line interface for user interactions
Handles user commands and orchestrates other modules
Entry point for the application


#### PesaVcs.Core

Contains core domain models and abstractions
Defines interfaces and fundamental data structures
Provides base classes and contracts for other modules


#### PesaVcs.Storage

Manages repository and object storage
Handles file system interactions
Implements data persistence mechanisms


#### PesaVcs.Staging

Manages staging area and index
Handles file tracking and preparation for commits
Provides staging-related services


#### PesaVcs.Network

Manages network operations and remote interactions
Implements protocols for remote repository communication
Supports potential future network-related features


#### PesaVcs.Tests

Comprehensive test suite
Ensures reliability and correctness of implementation



##### Module Interconnections
###### Dependency Flow

PesaVcs.CLI
│
├── PesaVcs.Core (Interfaces & Models)
│   │
│   ├── PesaVcs.Storage
│   ├── PesaVcs.Staging
│   └── PesaVcs.Network
Key Interactions


###### CLI Module

Receives user commands
Uses dependency injection to resolve services
Coordinates between different modules


###### Core Module

Defines interfaces that other modules implement
Provides base models and contracts
Acts as a central coordination point


###### Storage Module

Implements repository and object storage interfaces
Provides concrete implementations for data persistence
Handles file system interactions


###### Staging Module

Manages files to be committed
Tracks changes and prepares staging area
Implements index management


###### Network Module

Provides remote repository interactions
Implements communication protocols
Supports potential distributed version control features



### Getting Started
-------- Prerequisites ----------------

.NET 6.0 SDK or later
Visual Studio 2022 or Visual Studio Code (recommended)

### Installation

Clone the repository

```cd PesaVcs```

Restore NuGet packages

```dotnet restore```

Running the Project
From Command Line
```Build the project```

```dotnet build```

# Run the CLI
```dotnet run --project PesaVcs.CLI```

### Sample commands
in the root directory

```dotnet run --project PesaVcs.CLI init .```

pesavcs init: Initialize a new repository
pesavcs commit: Create a new commit
pesavcs branch: Manage branches
pesavcs status: Show repository status


Key Design Principles

Separation of Concerns
Dependency Injection
Testability
Extensibility

#### Development Roadmap

Core model and interface implementations
Storage and persistence mechanisms
Staging area management
Commit and branching logic
Network and remote repository support
Comprehensive test coverage

#### Contributing


Fork the repository
Create your feature branch (git checkout -b feature/AmazingFeature)

Commit your changes (git commit -m 'Add some AmazingFeature')

Push to the branch (git push origin feature/AmazingFeature)

Open a Pull Request
