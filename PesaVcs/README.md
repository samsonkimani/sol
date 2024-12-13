## PesaVcs: A Custom Version Control System
### Project Overview

PesaVcs is a custom version control system inspired by Git, implemented in C# using a modular, component-based architecture. The project is designed to demonstrate version control concepts through a clean, extensible implementation. This is also a good opportunity for me to practice my
c# skills. I mostly work with python, nodejs and occasionally php. 


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

#### PesaVcs.Branches

Manages branch creation, deletion, switching, listing branches and displaying current branch





### Getting Started
-------- Prerequisites ----------------

.NET 6.0 SDK or later
Visual Studio 2022 or Visual Studio Code (recommended)

### Installation

Clone the repository

Then

```cd PesaVcs```

Restore NuGet packages

```dotnet restore```

Running the Project
From Command Line

```dotnet build```

# Run the CLI
```dotnet run --project PesaVcs.CLI```

### Sample commands
in the root directory

Sample output is a direct copy from my terminal

## pesavcs init: Initialize a new repository

```dotnet run --project PesaVcs.CLI init```

this will create a .pesavcs folder

sample output
```sam@Sam:~/sol/PesaVcs$ dotnet run --project PesaVcs.CLI init
Initialized empty PesaVcs repository in /home/sam/sol/PesaVcs```

## pesavcs branch
On initialization, main branch was created

To view newly created branch

### current
```
dotnet run --project PesaVcs.CLI branch current
```
sample outout
```sam@Sam:~/sol/PesaVcs$ dotnet run --project PesaVcs.CLI branch current
Current branch: main
```

### Creating new branch

```
dotnet run --project PesaVcs.CLI branch create samson
```

sample output

```sam@Sam:~/sol/PesaVcs$ dotnet run --project PesaVcs.CLI branch create samson
Branch 'samson' created successfully.
sam@Sam:~/sol/PesaVcs$ 
```

### Listing all branches

```
dotnet run --project PesaVcs.CLI branch list
```

sample output
```sam@Sam:~/sol/PesaVcs$ dotnet run --project PesaVcs.CLI branch list
Branches:
* main
  samson
```

### switching branch

```dotnet run --project PesaVcs.CLI branch switch samson```

sample output
    Here i have combined a couple of commands to demonstrate switching

```
sam@Sam:~/sol/PesaVcs$ dotnet run --project PesaVcs.CLI branch switch samson
Switched to branch 'samson'.
sam@Sam:~/sol/PesaVcs$ dotnet run --project PesaVcs.CLI branch current
Current branch: samson
sam@Sam:~/sol/PesaVcs$ dotnet run --project PesaVcs.CLI branch list
Branches:
  main
* samson
sam@Sam:~/sol/PesaVcs$ 
```

### deleting branch

Note: cannot delete cuttent branch

```
dotnet run --project PesaVcs.CLI branch delete main
```

``` dotnet run --project PesaVcs.CLI branch delete samson
Error deleting branch: Cannot delete current branch```

can delete another branch

```sam@Sam:~/sol/PesaVcs$ dotnet run --project PesaVcs.CLI branch delete main
Branch 'main' deleted successfully.
```

### showing current branch

```
dotnet run --project PesaVcs.CLI branch current
```

sample output

```
sam@Sam:~/sol/PesaVcs$ dotnet run --project PesaVcs.CLI branch current
Current branch: samson
sam@Sam:~/sol/PesaVcs$ 
```

## staging

Here i have implemented stage all and stage a file

### staging file
    ```
     dotnet run --project PesaVcs.CLI stage README.md 
     ```

     sample output

     ```
     sam@Sam:~/sol/PesaVcs$ dotnet run --project PesaVcs.CLI stage README.md 
File staged: README.md
```

### showing staged and unstaged file in active directory


```
dotnet run --project PesaVcs.CLI status
```

sample output displaying previous staged file

```sam@Sam:~/sol/PesaVcs$ dotnet run --project PesaVcs.CLI status
Staged Changes:
  /home/sam/sol/PesaVcs/README.md (staged)

Unstaged Changes:
  /home/sam/sol/PesaVcs/.gitignore (untracked)
  /home/sam/sol/PesaVcs/PesaVcs.sln (untracked)
  /home/sam/sol/PesaVcs/PesaVcs.Tests/PesaVcs.Tests.csproj (untracked)
```

### unstaging a file

```dotnet run --project PesaVcs.CLI unstage README.md```

sample output

```
sam@Sam:~/sol/PesaVcs$ dotnet run --project PesaVcs.CLI unstage README.md 
File unstaged: README.md
sam@Sam:~/sol/PesaVcs$ dotnet run --project PesaVcs.CLI status
Staged Changes:

Unstaged Changes:
  /home/sam/sol/PesaVcs/.gitignore (untracked)
  /home/sam/sol/PesaVcs/PesaVcs.sln (untracked)
```

### staging all

```dotnet run --project PesaVcs.CLI stage all```

sample output

```sam@Sam:~/sol/PesaVcs$ dotnet run --project PesaVcs.CLI stage all
All changes staged.
sam@Sam:~/sol/PesaVcs$ dotnet run --project PesaVcs.CLI status
Staged Changes:
  /home/sam/sol/PesaVcs/.gitignore (staged)
  /home/sam/sol/PesaVcs/PesaVcs.sln (staged)
  /home/sam/sol/PesaVcs/README.md (staged)
.................
```

## commit

```dotnet run --project PesaVcs.CLI -- commit -m "sample commit" -a "samson kimani" -e "samsonkimani43@gmail.com"```

sample output 

```sam@Sam:~/sol/PesaVcs$ dotnet run --project PesaVcs.CLI -- commit -m "sample commit" -a "samson kimani" -e "samsonkimani43@gmail.com"
331
Commit created: 5c1345e38a41fe8ce6e8ad9f4de44befe187bc78
Message: sample commit
sam@Sam:~/sol/PesaVcs$```


## log

```dotnet run --project PesaVcs.CLI log
```

sample output 

```sam@Sam:~/sol/PesaVcs$ dotnet run --project PesaVcs.CLI log
Commit History:
Commit: 5c1345e38a41fe8ce6e8ad9f4de44befe187bc78
Author: samson kimani <samsonkimani43@gmail.com>
Date: 12/13/2024 8:37:58 PM
Message: sample commit
Hash 5c1345e38a41fe8ce6e8ad9f4de44befe187bc78
```


## diff

```dotnet run --project PesaVcs.CLI -- diff -c1 "030f34d0e6dc01e73fb51292dc397e7f196a129e" -c2 "5c1345e38a41fe8ce6e8ad9f4de44befe187bc78"```

This command will only show files that have been changed

To demonstrate diff, i will add content in Readme and stage and commit

i will add "This is some test content to see diff"

sample output

``` dotnet run --project PesaVcs.CLI stage all
All changes staged.
sam@Sam:~/sol/PesaVcs$ dotnet run --project PesaVcs.CLI -- commit -m "another test commit" -a "samson kimani" -e "samsonkimani43@gmail.com"
331
Commit created: 030f34d0e6dc01e73fb51292dc397e7f196a129e
Message: another test commit
sam@Sam:~/sol/PesaVcs$ dotnet run --project PesaVcs.CLI log
Commit History:
Commit: 030f34d0e6dc01e73fb51292dc397e7f196a129e
Author: samson kimani <samsonkimani43@gmail.com>
Date: 12/13/2024 8:43:29 PM
Message: another test commit
Hash 030f34d0e6dc01e73fb51292dc397e7f196a129e

Commit: f9ecff4cb1e452eccb3ddf512e98ceb34be7a211
Author: samson kimani <samsonkimani43@gmail.com>
Date: 12/13/2024 8:42:57 PM
Message: another commit
Hash f9ecff4cb1e452eccb3ddf512e98ceb34be7a211

Commit: 5c1345e38a41fe8ce6e8ad9f4de44befe187bc78
Author: samson kimani <samsonkimani43@gmail.com>
Date: 12/13/2024 8:37:58 PM
Message: sample commit
Hash 5c1345e38a41fe8ce6e8ad9f4de44befe187bc78

sam@Sam:~/sol/PesaVcs$ dotnet run --project PesaVcs.CLI -- diff -c1 "030f34d0e6dc01e73fb51292dc397e7f196a129e" -c2 "5c1345e38a41fe8ce6e8ad9f4de44befe187bc78"
Differences between commits 030f34d0e6dc01e73fb51292dc397e7f196a129e and 5c1345e38a41fe8ce6e8ad9f4de44befe187bc78:
  Removed/Changed: /home/sam/sol/PesaVcs/README.md
  Added/Changed: /home/sam/sol/PesaVcs/README.md
sam@Sam:~/sol/PesaVcs$ 
```
