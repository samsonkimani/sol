using System.CommandLine;
using Microsoft.Extensions.DependencyInjection;
using PesaVcs.Core.Interfaces;

namespace PesaVcs.CLI.Commands
{
    public static class CommitCommand
    {
        public static Command CreateCommand(IServiceProvider serviceProvider)
        {
            // Declare options explicitly
            var messageOption = new Option<string>(
                "--message",
                description: "The commit message")
            {
                IsRequired = true
            };

            var authorOption = new Option<string>(
                "--author",
                description: "The name of the author")
            {
                IsRequired = true
            };

            var emailOption = new Option<string>(
                "--email",
                description: "The email of the author")
            {
                IsRequired = true
            };

            // Create the command and add options
            var commitCommand = new Command("commit", "Create a new commit")
            {
                messageOption,
                authorOption,
                emailOption
            };

            // Define the handler using SetHandler
            commitCommand.SetHandler(
                (string message, string author, string email) =>
                {
                    var commitService = serviceProvider.GetRequiredService<ICommitService>();
                    var commit = commitService.CreateCommit(message, author, email);

                    Console.WriteLine("Commit created successfully:");
                    Console.WriteLine($"  ID: {commit.Id}");
                    Console.WriteLine($"  Author: {commit.Author}");
                    Console.WriteLine($"  Date: {commit.Date}");
                    Console.WriteLine($"  Message: {commit.Message}");
                },
                messageOption,  
                authorOption,   
                emailOption 
            );

            return commitCommand;
        }
    }
}
