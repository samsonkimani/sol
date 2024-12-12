using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PesaVcs.Core.Interfaces;

namespace PesaVcs.Branches.Services
{
    public class BranchService : IBranchService
    {
        private readonly string _repoPath;
        private readonly string _refsPath;
        private readonly string _headPath;

        public BranchService(string repositoryPath)
        {
            _repoPath = repositoryPath;
            _refsPath = Path.Combine(repositoryPath, ".pesavcs", "refs", "heads");
            _headPath = Path.Combine(repositoryPath, ".pesavcs", "HEAD");

            // Ensure refs directory exists
            Directory.CreateDirectory(_refsPath);
        }

        public void CreateBranch(string name)
        {
            ValidateBranchName(name);

            string branchPath = Path.Combine(_refsPath, name);
            
            if (File.Exists(branchPath))
                throw new InvalidOperationException($"Branch {name} already exists");

            // Create branch file with current HEAD commit
            string currentCommit = GetCurrentCommit();
            File.WriteAllText(branchPath, currentCommit);
        }

        public void DeleteBranch(string name)
        {
            ValidateBranchName(name);

            if (name == GetCurrentBranch().Name)
                throw new InvalidOperationException("Cannot delete current branch");

            string branchPath = Path.Combine(_refsPath, name);
            
            if (!File.Exists(branchPath))
                throw new FileNotFoundException($"Branch {name} not found");

            File.Delete(branchPath);
        }

        public List<Branch> ListBranches()
        {
            string[] branchFiles = Directory.GetFiles(_refsPath);
            var currentBranch = GetCurrentBranch();
            string currentBranchName = currentBranch?.Name ?? string.Empty;

            return branchFiles.Select(bf => new Branch
            {
                Name = Path.GetFileName(bf),
                IsCurrent = Path.GetFileName(bf) == currentBranchName
            }).ToList();
        }

        public Branch GetCurrentBranch()
        {
            if (!File.Exists(_headPath))
                throw new InvalidOperationException("HEAD file not found");

            string headContent = File.ReadAllText(_headPath).Trim();
            string currentBranchName = headContent.Replace("ref: refs/heads/", "");

            return new Branch
            {
                Name = currentBranchName,
                IsCurrent = true
            };
        }

        public void SwitchBranch(string name)
        {
            ValidateBranchName(name);

            string branchPath = Path.Combine(_refsPath, name);
            
            if (!File.Exists(branchPath))
                throw new FileNotFoundException($"Branch {name} not found");

            // Update HEAD to point to the new branch
            File.WriteAllText(_headPath, $"ref: refs/heads/{name}");
        }

        private void ValidateBranchName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Branch name cannot be empty", nameof(name));

            if (name.Contains(" ") || name.Contains("/"))
                throw new ArgumentException("Invalid branch name. Cannot contain spaces or '/'", nameof(name));
        }

        private string GetCurrentCommit()
        {
            // This would typically read the current commit hash from HEAD or the current branch ref
            // For now, i will return empty but should implement a more robust functionality to get the commit
            return string.Empty;
        }
    }
}