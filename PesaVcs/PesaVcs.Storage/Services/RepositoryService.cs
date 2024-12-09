using System;
using System.IO;
using PesaVcs.Core.Interfaces;

namespace PesaVcs.Storage.Services
{
    public class RepositoryService : IRepositoryService
    {
        private const string REPO_FOLDER = ".pesavcs";

        public void Initialize(string path)
        {
           try
            {
                // Ensure path exists
                Directory.CreateDirectory(path);

                // Create .pesavcs directory
                var repoPath = Path.Combine(path, REPO_FOLDER);
                Directory.CreateDirectory(repoPath);

                // Create subdirectories
                Directory.CreateDirectory(Path.Combine(repoPath, "objects"));
                Directory.CreateDirectory(Path.Combine(repoPath, "refs", "heads")); // Ensure intermediate directories exist

                // Create initial HEAD file
                File.WriteAllText(Path.Combine(repoPath, "HEAD"), "ref: refs/heads/main");

                // Create initial main branch file
                var mainBranchPath = Path.Combine(repoPath, "refs", "heads", "main");
                File.WriteAllText(mainBranchPath, ""); // Create an empty file for the main branch
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing repository: {ex.Message}");
                throw; // Re-throw exception to ensure visibility in higher layers
            }
        }

        public bool IsRepository()
        {
            var currentDir = Environment.CurrentDirectory;
            return Directory.Exists(Path.Combine(currentDir, REPO_FOLDER));
        }

        public string GetRepositoryRoot()
        {
            var currentDir = Environment.CurrentDirectory;
            
            while (currentDir != null)
            {
                if (Directory.Exists(Path.Combine(currentDir, REPO_FOLDER)))
                {
                    return currentDir;
                }
                currentDir = Directory.GetParent(currentDir)?.FullName;
            }

            throw new InvalidOperationException("Not a PesaVcs repository");
        }


        public void Clone(string remoteUrl, string localPath)
        {
            // Implementation for cloning a remote repository to a local path
        }

        public void Delete(string path)
        {
            // Implementation for deleting a repository at the specified path
        }

        public bool Exists(string path)
        {
            // Implementation to check if a repository exists at the specified path
            return Directory.Exists(path);
        }
    }
}