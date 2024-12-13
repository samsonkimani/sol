using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using Microsoft.Extensions.DependencyInjection;
using PesaVcs.Core.Interfaces;
using PesaVcs.Storage.Services;
using PesaVcs.Staging.Services;
using PesaVcs.Network.Services;
using PesaVcs.CLI.Commands;
using PesaVcs.Branches.Services;




namespace PesaVcs.CLI
{
    class Program
    {
        static void Main(string[] args)
        {     


            var services = new ServiceCollection();
            ConfigureServices(services);
            var serviceProvider = services.BuildServiceProvider();

            // Create the root command
            var rootCommand = new RootCommand("PesaVcs - A Custom Version Control System");

            // Add the init command
            rootCommand.AddCommand(InitCommand.CreateCommand(serviceProvider));    
            rootCommand.AddCommand(StageCommand.CreateCommand(serviceProvider));  
            rootCommand.AddCommand(UnstageCommand.CreateCommand(serviceProvider));
            rootCommand.AddCommand(StatusCommand.CreateCommand(serviceProvider));   
            rootCommand.AddCommand(BranchCommand.CreateCommand(serviceProvider));  
            rootCommand.AddCommand(CommitCommand.CreateCommand(serviceProvider));
            rootCommand.AddCommand(DiffCommand.CreateCommand(serviceProvider));
            rootCommand.AddCommand(LogCommand.CreateCommand(serviceProvider));


            rootCommand.Invoke(args);
        }

        /// <summary>
        /// Configure dependency injection services
        /// </summary>
        static void ConfigureServices(IServiceCollection services)
        {
            // Register core services
            services.AddSingleton<IRepositoryService, RepositoryService>();
            services.AddSingleton<ICommitService, CommitService>();
            services.AddSingleton<IObjectDatabase, FileSystemObjectDatabase>();
            
            // // Staging services
            services.AddSingleton<IIndexService, IndexService>();
            services.AddSingleton<IStagingAreaManager, StagingAreaManager>();
            
            // // Network services
            // services.AddSingleton<IRemoteRepositoryService, RemoteRepositoryService>();
            
            // // Branch services
            services.AddSingleton<IBranchService, BranchService>();
        }
    }
}