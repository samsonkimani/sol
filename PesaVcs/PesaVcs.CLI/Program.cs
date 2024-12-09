using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using Microsoft.Extensions.DependencyInjection;
using PesaVcs.Core.Interfaces;
using PesaVcs.Storage.Services;
using PesaVcs.Staging.Services;
using PesaVcs.Network.Services;
using PesaVcs.CLI.Commands;

namespace PesaVcs.CLI
{
    class Program
    {
        static void Main(string[] args)
        {
            
            var services = new ServiceCollection();
            ConfigureServices(services);
            var serviceProvider = services.BuildServiceProvider();

            // creating the root command
            var rootCommand = new RootCommand("PesaVcs - A Custom Version Control System");

            // Path
            var pathArgument = new Argument<string>(
                "path",
                () => Environment.CurrentDirectory,
                "Path to initialize the repository"
            );

            var initCommand = new Command("init", "Initialize a new PesaVcs repository")
            {
                pathArgument
            };

            initCommand.SetHandler(async (string path) =>
            {
                try
                {
                    var repoService = serviceProvider.GetRequiredService<IRepositoryService>();
                    await Task.Run(() => repoService.Initialize(path));
                    Console.WriteLine($"Initialized empty PesaVcs repository in {path}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error initializing repository: {ex.Message}");
                }
            }, pathArgument);

            rootCommand.AddCommand(initCommand);


            // Commit command
            // var commitCommand = new Command("commit", "Record changes to the repository")
            // {
            //     new Option<string>(new[] { "-m", "--message" }, "Commit message"),
            //     new Option<string>(new[] { "-a", "--author" }, () => Environment.UserName, "Author name"),
            //     new Option<string>(new[] { "-e", "--email" }, () => $"{Environment.UserName}@localhost", "Author email")
            // };
            // commitCommand.SetHandler((message, author, email) => 
            // {
            //     try 
            //     {
            //         var commitService = serviceProvider.GetRequiredService<ICommitService>();
            //         var indexService = serviceProvider.GetRequiredService<IIndexService>();

            //         // Stage all changes if -a flag is used (similar to git)
            //         indexService.StageAllChanges();

            //         // Create commit
            //         var commit = commitService.CreateCommit(message, author, email);
            //         Console.WriteLine($"Commit created: {commit.Id}");
            //     }
            //     catch (Exception ex)
            //     {
            //         Console.WriteLine($"Error creating commit: {ex.Message}");
            //     }
            // }, 
            // commitCommand.Options[0], 
            // commitCommand.Options[1], 
            // commitCommand.Options[2]);

            // Status command
            // var statusCommand = new Command("status", "Show the working tree status");
            // statusCommand.SetHandler(() => 
            // {
            //     try 
            //     {
            //         var indexService = serviceProvider.GetRequiredService<IIndexService>();
            //         var changes = indexService.GetChanges();
                    
            //         Console.WriteLine("Repository Status:");
            //         Console.WriteLine("Changes to be committed:");
            //         foreach (var change in changes.StagedChanges)
            //         {
            //             Console.WriteLine($"  {change.Status}: {change.FilePath}");
            //         }
                    
            //         Console.WriteLine("\nUnstaged changes:");
            //         foreach (var change in changes.UnstagedChanges)
            //         {
            //             Console.WriteLine($"  {change.Status}: {change.FilePath}");
            //         }
            //     }
            //     catch (Exception ex)
            //     {
            //         Console.WriteLine($"Error retrieving status: {ex.Message}");
            //     }
            // });

            // Branch command
            // var branchCommand = new Command("branch", "List, create, or delete branches")
            // {
            //     new Argument<string>("name", () => null, "Branch name to create"),
            //     new Option<bool>(new[] { "-d", "--delete" }, "Delete a branch")
            // };
            // branchCommand.SetHandler((name, isDelete) => 
            // {
            //     try 
            //     {
            //         var branchService = serviceProvider.GetRequiredService<IBranchService>();
                    
            //         if (string.IsNullOrEmpty(name))
            //         {
            //             // List branches
            //             var branches = branchService.ListBranches();
            //             Console.WriteLine("Branches:");
            //             foreach (var branch in branches)
            //             {
            //                 Console.WriteLine($"{(branch.IsCurrent ? "* " : "  ")}{branch.Name}");
            //             }
            //         }
            //         else if (isDelete)
            //         {
            //             // Delete branch
            //             branchService.DeleteBranch(name);
            //             Console.WriteLine($"Deleted branch {name}");
            //         }
            //         else
            //         {
            //             // Create branch
            //             branchService.CreateBranch(name);
            //             Console.WriteLine($"Created branch {name}");
            //         }
            //     }
            //     catch (Exception ex)
            //     {
            //         Console.WriteLine($"Error handling branch: {ex.Message}");
            //     }
            // }, 
            // branchCommand.Arguments[0], 
            // branchCommand.Options[0]);

            // Add commands to root
            // rootCommand.AddCommand(initCommand);
            // rootCommand.AddCommand(commitCommand);
            // rootCommand.AddCommand(statusCommand);
            // rootCommand.AddCommand(branchCommand);

            // Parse and invoke command
            rootCommand.Invoke(args);
        }

        /// <summary>
        /// Configure dependency injection services
        /// </summary>
        static void ConfigureServices(IServiceCollection services)
        {
            // Register core services
            services.AddSingleton<IRepositoryService, RepositoryService>();
            // services.AddSingleton<ICommitService, CommitService>();
            // services.AddSingleton<IObjectDatabase, FileSystemObjectDatabase>();
            
            // // Staging services
            // services.AddSingleton<IIndexService, IndexService>();
            // services.AddSingleton<IStagingAreaManager, StagingAreaManager>();
            
            // // Network services
            // services.AddSingleton<IRemoteRepositoryService, RemoteRepositoryService>();
            
            // // Branch services
            // services.AddSingleton<IBranchService, BranchService>();
        }
    }
}