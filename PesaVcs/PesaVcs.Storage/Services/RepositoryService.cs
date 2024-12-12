using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Collections.Generic;
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
                Directory.CreateDirectory(Path.Combine(repoPath, "refs", "heads"));

                // Create initial HEAD file
                File.WriteAllText(Path.Combine(repoPath, "HEAD"), "ref: refs/heads/main");

                // Create initial main branch file
                var mainBranchPath = Path.Combine(repoPath, "refs", "heads", "main");
                File.WriteAllText(mainBranchPath, ""); // Create an empty file for the main branch
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing repository: {ex.Message}");
                throw;
            }
        }

        public void Clone(string source, string localPath)
        {
            try
            {
                // Validate inputs
                if (string.IsNullOrWhiteSpace(source))
                    throw new ArgumentException("Source cannot be empty", nameof(source));

                if (string.IsNullOrWhiteSpace(localPath))
                    throw new ArgumentException("Local path cannot be empty", nameof(localPath));

                // Ensure local path exists
                Directory.CreateDirectory(localPath);

                // Check if the local directory is already a repository
                if (Directory.Exists(Path.Combine(localPath, REPO_FOLDER)))
                {
                    throw new InvalidOperationException("Destination is already a PesaVcs repository");
                }

                // Initialize the repository structure
                Initialize(localPath);

                // Determine if source is a URL or a local path
                if (IsValidUrl(source))
                {
                    CloneFromRemote(source, localPath);
                }
                else
                {
                    CloneFromLocalPath(source, localPath);
                }

                Console.WriteLine($"Cloned repository from {source} to {localPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error cloning repository: {ex.Message}");
                throw;
            }
        }

        private bool IsValidUrl(string source)
        {
            return Uri.TryCreate(source, UriKind.Absolute, out var uriResult) 
                   && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }

        private void CloneFromRemote(string remoteUrl, string localPath)
        {
            using (var httpClient = new HttpClient())
            {
                // Fetch repository metadata
                string repositoryMetadataUrl = $"{remoteUrl.TrimEnd('/')}/repository-metadata.json";
                var metadataResponse = httpClient.GetStringAsync(repositoryMetadataUrl).Result;

                // Parse repository metadata
                var repositoryMetadata = JsonSerializer.Deserialize<RepositoryMetadata>(metadataResponse);

                if (repositoryMetadata == null)
                    throw new InvalidOperationException("Could not fetch repository metadata");

                // Save repository metadata
                File.WriteAllText(
                    Path.Combine(localPath, REPO_FOLDER, "remote-metadata.json"), 
                    metadataResponse
                );

                foreach (var fileEntry in repositoryMetadata.Files)
                {
                    string fileUrl = $"{remoteUrl.TrimEnd('/')}/{fileEntry.Path}";
                    if (fileEntry.Path == null)
                        throw new InvalidOperationException("File entry path cannot be null");

                    string localFilePath = Path.Combine(localPath, fileEntry.Path);

                    // Ensure directory exists
                    var directoryPath = Path.GetDirectoryName(localFilePath);
                    if (directoryPath != null)
                    {
                        Directory.CreateDirectory(directoryPath);
                    }

                    // Download file
                    var fileContent = httpClient.GetStringAsync(fileUrl).Result;
                    File.WriteAllText(localFilePath, fileContent);
                }

                // Save remote configuration
                File.WriteAllText(
                    Path.Combine(localPath, REPO_FOLDER, "remote-config"), 
                    $"url = {remoteUrl}"
                );
            }
        }

        private void CloneFromLocalPath(string sourcePath, string localPath)
        {
            // Verify source is a valid repository
            var sourceRepoPath = Path.Combine(sourcePath, REPO_FOLDER);
            if (!Directory.Exists(sourceRepoPath))
            {
                throw new InvalidOperationException("Source is not a PesaVcs repository");
            }

            // Copy entire repository structure
            CopyDirectory(sourcePath, localPath);

            // Update remote config to reflect local source
            File.WriteAllText(
                Path.Combine(localPath, REPO_FOLDER, "remote-config"), 
                $"path = {sourcePath}"
            );
        }

        private void CopyDirectory(string sourceDir, string destDir)
        {
            // Get all subdirectories
            var dirs = Directory.GetDirectories(sourceDir, "*", SearchOption.AllDirectories);

            // Get all files
            var files = Directory.GetFiles(sourceDir, "*.*", SearchOption.AllDirectories);

            foreach (string dirPath in dirs)
            {
                Directory.CreateDirectory(dirPath.Replace(sourceDir, destDir));
            }

            foreach (string filePath in files)
            {
                // Exclude .pesavcs directory from direct copying
                if (!filePath.Contains(REPO_FOLDER))
                {
                    File.Copy(filePath, filePath.Replace(sourceDir, destDir), true);
                }
            }

            // Ensure .pesavcs is properly initialized in the new location
            var sourceRepoPath = Path.Combine(sourceDir, REPO_FOLDER);
            var destRepoPath = Path.Combine(destDir, REPO_FOLDER);
            
            // Copy specific .pesavcs files if needed
            CopyRepositoryMetadata(sourceRepoPath, destRepoPath);
        }

        private void CopyRepositoryMetadata(string sourceRepoPath, string destRepoPath)
        {
            // Copy specific files from .pesavcs that are important
            string[] metadataFiles = { "HEAD", "config", "description" };

            foreach (var file in metadataFiles)
            {
                var sourcePath = Path.Combine(sourceRepoPath, file);
                var destPath = Path.Combine(destRepoPath, file);

                if (File.Exists(sourcePath))
                {
                    File.Copy(sourcePath, destPath, true);
                }
            }
        }

        public void Delete(string path)
        {
            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(path))
                    throw new ArgumentException("Path cannot be empty", nameof(path));

                // Verify it's a PesaVcs repository before deleting
                var repoPath = Path.Combine(path, REPO_FOLDER);
                if (!Directory.Exists(repoPath))
                {
                    throw new InvalidOperationException("Not a PesaVcs repository");
                }

                // This is just to confirm actual deletion
                var confirmationFile = Path.Combine(repoPath, "REPO_DELETION_CONFIRMED");
                File.WriteAllText(confirmationFile, DateTime.UtcNow.ToString());

                // Delete the entire .pesavcs directory
                Directory.Delete(repoPath, true);

                Console.WriteLine($"Deleted PesaVcs repository at {path}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting repository: {ex.Message}");
                throw;
            }
        }

        public bool Exists(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return false;

            try
            {
                // Check for .pesavcs directory
                var repoPath = Path.Combine(path, REPO_FOLDER);
                return Directory.Exists(repoPath);
            }
            catch
            {                
                return false;
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
    }

    public class RepositoryMetadata
    {
        public string RepositoryName { get; set; }
        public string DefaultBranch { get; set; }
        public List<FileEntry> Files { get; set; }

        public RepositoryMetadata(string repositoryName, string defaultBranch, List<FileEntry> files)
        {
            RepositoryName = repositoryName ?? throw new ArgumentNullException(nameof(repositoryName));
            DefaultBranch = defaultBranch ?? throw new ArgumentNullException(nameof(defaultBranch));
            Files = files ?? throw new ArgumentNullException(nameof(files));
        }
    }

    public class FileEntry
    {
        public string? Path { get; set; }
        public string? Hash { get; set; }
        public long? Size { get; set; }
    }
}