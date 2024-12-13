using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using Microsoft.Extensions.DependencyInjection;
using PesaVcs.Core.Interfaces;

namespace PesaVcs.CLI.Commands
{
    public static class InitCommand
    {
        public static Command CreateCommand(IServiceProvider serviceProvider)
        {
            // Define the "path" argument
            var pathArgument = new Argument<string>(
                "path",
                () => Environment.CurrentDirectory,
                "Path to initialize the repository"
            );

            // Create the "init" command
            var command = new Command("init", "Initialize a new PesaVcs repository")
            {
                pathArgument
            };

            // Define the command handler
            command.SetHandler(async (string path) =>
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

            return command;
        }
    }
}
