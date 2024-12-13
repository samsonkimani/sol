using System.CommandLine;
using Microsoft.Extensions.DependencyInjection;
using PesaVcs.Core.Interfaces;
using PesaVcs.Storage.Services;
using PesaVcs.Staging.Services;


namespace PesaVcs.CLI.Commands
{
    public static class CommitCommand
    {
        public static Command CreateCommand(IServiceProvider serviceProvider)
        {
            var commitCommand = new Command("commit", "Create a new commit");

            // Message option
            var messageOption = new Option<string>(
                new[] { "-m", "--message" }, 
                "Commit message"
            ) { IsRequired = true };
            commitCommand.AddOption(messageOption);

            // Author option
            var authorOption = new Option<string>(
                new[] { "-a", "--author" }, 
                "Author name"
            ) { IsRequired = true };
            commitCommand.AddOption(authorOption);

            // Email option
            var emailOption = new Option<string>(
                new[] { "-e", "--email" }, 
                "Author email"
            ) { IsRequired = true };
            commitCommand.AddOption(emailOption);

            // Console.WriteLine(message);


            // Handler for commit command
            commitCommand.SetHandler((string message, string author, string email) =>
            {
                try
                {
                    // Get required services
                    string repoPath = Directory.GetCurrentDirectory();
                
                
                    var indexService = new IndexService(repoPath);

                    // var commitService = serviceProvider.GetRequiredService<ICommitService>();
                    var repositoryService = serviceProvider.GetRequiredService<IRepositoryService>();
                    // Console.WriteLine(repositoryService);
                    var treeService = new RepositoryTreeService(repositoryService.GetRepositoryRoot());

                    // Get staged changes
                    var stagedChanges = indexService.GetChanges().StagedChanges;
                    
                    if (stagedChanges == null || stagedChanges.Count == 0)
                    {
                        Console.WriteLine("No changes to commit.");
                        return;
                    }

                    Console.WriteLine(stagedChanges.Count);

                    var treeEntries = stagedChanges
                        .Where(entry => 
                            !string.IsNullOrWhiteSpace(entry.FilePath))
                        .Select(change => new TreeEntry
                        {
                            Type = "blob",
                            Name = change.FilePath!, 
                            Hash = change.Hash!  
                        }).ToList();

                    
                        

                    // Console.WriteLine(treeEntries.Count);

                    // Create tree
                    var treeHash = treeService.CreateTree(treeEntries);

                    // Making this obsolete 
                    // var commit = commitService.CreateCommit(message, author, email);

                    var commit = treeService.CreateCommit(new PesaVcs.Storage.Services.Commit
                    {
                        TreeHash = treeHash,
                        Author = $"{author} <{email}>",
                        Message = message,        
                    });

                    // Clear staging area
                    indexService.ClearIndex();

                    Console.WriteLine($"Commit created: {commit.Hash}");
                    Console.WriteLine($"Message: {message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Commit failed: {ex.Message}");
                }
            }, messageOption, authorOption, emailOption);

            return commitCommand;
        }
    }

    public static class DiffCommand
    {
        public static Command CreateCommand(IServiceProvider serviceProvider)
        {
            var diffCommand = new Command("diff", "Show differences between commits or changes");

            var commit1Option = new Option<string>(
                new[] { "-c1", "--commit1" }, 
                "First commit hash"
            );
            diffCommand.AddOption(commit1Option);

            var commit2Option = new Option<string>(
                new[] { "-c2", "--commit2" }, 
                "Second commit hash"
            );
            diffCommand.AddOption(commit2Option);

            diffCommand.SetHandler((string commit1, string commit2) =>
            {
                try
                {
                    var repositoryService = serviceProvider.GetRequiredService<IRepositoryService>();
                    var treeService = new RepositoryTreeService(repositoryService.GetRepositoryRoot());

                    // If no commits specified, show unstaged changes
                    if (string.IsNullOrWhiteSpace(commit1) && string.IsNullOrWhiteSpace(commit2))
                    {
                        var indexService = serviceProvider.GetRequiredService<IIndexService>();
                        var unstagedChanges = indexService.GetChanges().UnstagedChanges;

                        Console.WriteLine("Unstaged Changes:");
                        foreach (var change in unstagedChanges ?? new List<Change>())
                        {
                            Console.WriteLine($"  {change.FilePath} - {change.Status}");
                        }
                        return;
                    }

                    // Compare two commits
                    if (!string.IsNullOrWhiteSpace(commit1) && !string.IsNullOrWhiteSpace(commit2))
                    {
                        var differences = treeService.GetFileDifferences(commit1, commit2);
                        Console.WriteLine($"Differences between commits {commit1} and {commit2}:");
                        foreach (var diff in differences)
                        {
                            Console.WriteLine($"  {diff}");
                        }
                        return;
                    }

                    Console.WriteLine("Please provide either two commit hashes or no hashes to show unstaged changes.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Diff failed: {ex.Message}");
                }
            }, commit1Option, commit2Option);

            return diffCommand;
        }
    }

    public static class LogCommand
    {
        public static Command CreateCommand(IServiceProvider serviceProvider)
        {
            var logCommand = new Command("log", "Show commit history");

            // Limit option
            var limitOption = new Option<int>(
                new[] { "-n", "--number" }, 
                () => 10,
                "Number of commits to show"
            );
            logCommand.AddOption(limitOption);

            // Handler for log command
            logCommand.SetHandler((int limit) =>
            {
                try
                {
                    var repositoryService = serviceProvider.GetRequiredService<IRepositoryService>();
                    var treeService = new RepositoryTreeService(repositoryService.GetRepositoryRoot());

                    var commits = treeService.GetCommitLog(limit);

                    Console.WriteLine("Commit History:");
                    foreach (var commit in commits)
                    {
                        Console.WriteLine($"Commit: {commit.Hash}");
                        Console.WriteLine($"Author: {commit.Author}");
                        Console.WriteLine($"Date: {commit.Timestamp}");
                        Console.WriteLine($"Message: {commit.Message}");
                        Console.WriteLine();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Log failed: {ex.Message}");
                }
            }, limitOption);

            return logCommand;
        }
    }
}
