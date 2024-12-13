using System.CommandLine;
using PesaVcs.Branches.Services;
using PesaVcs.Storage.Services;
using PesaVcs.Core.Interfaces;

namespace PesaVcs.CLI.Commands
{
    public static class BranchCommand
    {
        public static Command CreateCommand(IServiceProvider serviceProvider)
        {
            var branchCommand = new Command("branch", "Manage branches");

            // Create Branch Command
            var createCommand = new Command("create", "Create a new branch");
            var branchNameArgument = new Argument<string>("branchName", "Name of the branch to create");
            createCommand.AddArgument(branchNameArgument);
            createCommand.SetHandler((string branchName) =>
            {
                try
                {
                    string repoRoot = new RepositoryService().GetRepositoryRoot();
                    var branchService = new BranchService(repoRoot);
                    branchService.CreateBranch(branchName);
                    Console.WriteLine($"Branch '{branchName}' created successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error creating branch: {ex.Message}");
                }
            }, branchNameArgument);
            branchCommand.AddCommand(createCommand);

            // Delete Branch Command
            var deleteCommand = new Command("delete", "Delete a branch");
            var deleteBranchNameArgument = new Argument<string>("branchName", "Name of the branch to delete");
            deleteCommand.AddArgument(deleteBranchNameArgument);
            deleteCommand.SetHandler((string branchName) =>
            {
                try
                {
                    string repoRoot = new RepositoryService().GetRepositoryRoot();
                    var branchService = new BranchService(repoRoot);
                    branchService.DeleteBranch(branchName);
                    Console.WriteLine($"Branch '{branchName}' deleted successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error deleting branch: {ex.Message}");
                }
            }, deleteBranchNameArgument);
            branchCommand.AddCommand(deleteCommand);

            // List Branches Command
            var listCommand = new Command("list", "List all branches");
            listCommand.SetHandler(() =>
            {
                try
                {
                    string repoRoot = new RepositoryService().GetRepositoryRoot();
                    var branchService = new BranchService(repoRoot);
                    List<Branch> branches = branchService.ListBranches();
                    
                    Console.WriteLine("Branches:");
                    foreach (var branch in branches)
                    {
                        string currentIndicator = branch.IsCurrent == true ? "* " : "  ";
                        Console.WriteLine($"{currentIndicator}{branch.Name}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error listing branches: {ex.Message}");
                }
            });
            branchCommand.AddCommand(listCommand);

            // Switch Branch Command
            var switchCommand = new Command("switch", "Switch to a branch");
            var switchBranchNameArgument = new Argument<string>("branchName", "Name of the branch to switch to");
            switchCommand.AddArgument(switchBranchNameArgument);
            switchCommand.SetHandler((string branchName) =>
            {
                try
                {
                    string repoRoot = new RepositoryService().GetRepositoryRoot();
                    var branchService = new BranchService(repoRoot);
                    branchService.SwitchBranch(branchName);
                    Console.WriteLine($"Switched to branch '{branchName}'.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error switching branch: {ex.Message}");
                }
            }, switchBranchNameArgument);
            branchCommand.AddCommand(switchCommand);

            // Current Branch Command
            var currentCommand = new Command("current", "Show current branch");
            currentCommand.SetHandler(() =>
            {
                try
                {
                    string repoRoot = new RepositoryService().GetRepositoryRoot();
                    var branchService = new BranchService(repoRoot);
                    Branch currentBranch = branchService.GetCurrentBranch();
                    Console.WriteLine($"Current branch: {currentBranch.Name}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error getting current branch: {ex.Message}");
                }
            });
            branchCommand.AddCommand(currentCommand);

            return branchCommand;
        }
    }
}
