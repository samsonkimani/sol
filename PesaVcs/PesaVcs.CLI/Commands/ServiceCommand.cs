using System.CommandLine;
using Microsoft.Extensions.DependencyInjection;
using PesaVcs.Core.Interfaces;

namespace PesaVcs.CLI.Commands
{
    public static class StageCommand
    {
        public static Command CreateCommand(IServiceProvider serviceProvider)
        {
            var stageCommand = new Command("stage", "Stage files for commit");

            var stageAllCommand = new Command("all", "Stage all changes");
            stageAllCommand.SetHandler(() =>
            {
                var indexService = serviceProvider.GetRequiredService<IIndexService>();
                indexService.StageAllChanges();
                Console.WriteLine("All changes staged.");
            });

            stageCommand.AddCommand(stageAllCommand);

            var filePathArgument = new Argument<string>("filePath", "Path to the file to stage");
            stageCommand.AddArgument(filePathArgument);

            stageCommand.SetHandler((string filePath) =>
            {
                var indexService = serviceProvider.GetRequiredService<IIndexService>();
                indexService.StageFile(filePath);
                Console.WriteLine($"File staged: {filePath}");
            }, filePathArgument);

            return stageCommand;
        }
    }

    public static class UnstageCommand
    {
        public static Command CreateCommand(IServiceProvider serviceProvider)
        {
            var unstageCommand = new Command("unstage", "Unstage a file");
            var filePathArgument = new Argument<string>("filePath", "Path to the file to unstage");
            unstageCommand.AddArgument(filePathArgument);

            unstageCommand.SetHandler((string filePath) =>
            {
                var indexService = serviceProvider.GetRequiredService<IIndexService>();
                indexService.UnstageFile(filePath);
                Console.WriteLine($"File unstaged: {filePath}");
            }, filePathArgument);

            return unstageCommand;
        }
    }

    public static class StatusCommand
    {
        public static Command CreateCommand(IServiceProvider serviceProvider)
        {
            var statusCommand = new Command("status", "Show the current status of changes");

            statusCommand.SetHandler(() =>
            {
                var indexService = serviceProvider.GetRequiredService<IIndexService>();
                var changes = indexService.GetChanges();

                Console.WriteLine("Staged Changes:");
                foreach (var change in changes.StagedChanges ?? new List<Change>())
                {
                    Console.WriteLine($"  {change.FilePath} ({change.Status})");
                }

                Console.WriteLine("\nUnstaged Changes:");
                foreach (var change in changes.UnstagedChanges ?? new List<Change>())
                {
                    Console.WriteLine($"  {change.FilePath} ({change.Status})");
                }
            });

            return statusCommand;
        }
    }
}
