
using PesaVcs.Core.Interfaces;

namespace PesaVcs.Staging.Services
{
    public class IndexService : IIndexService
    {
        private readonly string _repoPath;
        private readonly string _indexPath;

        public IndexService(string repoPath)
        {
            _repoPath = repoPath;
            _indexPath = Path.Combine(repoPath, "index");
            if (!Directory.Exists(_indexPath))
            {
                Directory.CreateDirectory(_indexPath);
            }
        }

        public void StageAllChanges()
        {
            // Logic to stage all changes in the repository
            var files = Directory.GetFiles(_repoPath, "*.*", SearchOption.AllDirectories);
            foreach (var filePath in files)
            {
                StageFile(filePath);
            }
        }

        public void StageFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                var relativePath = Path.GetRelativePath(_repoPath, filePath);
                var stagedFilePath = Path.Combine(_indexPath, relativePath);
                Directory.CreateDirectory(Path.GetDirectoryName(stagedFilePath));
                File.Copy(filePath, stagedFilePath, true);
            }
        }

        public void UnstageFile(string filePath)
        {
            var relativePath = Path.GetRelativePath(_repoPath, filePath);
            var stagedFilePath = Path.Combine(_indexPath, relativePath);
            if (File.Exists(stagedFilePath))
            {
                File.Delete(stagedFilePath);
            }
        }

        public Changes GetChanges()
        {
            var stagedChanges = new List<Change>();
            var untrackedChanges = new List<Change>();

            // List all staged files
            if (Directory.Exists(_indexPath))
            {
                var stagedFiles = Directory.GetFiles(_indexPath, "*.*", SearchOption.AllDirectories);
                foreach (var filePath in stagedFiles)
                {
                    stagedChanges.Add(new Change { FilePath = filePath, Status = "staged" });
                }
            }

            // List untracked files (files not in the index)
            var allFiles = Directory.GetFiles(_repoPath, "*.*", SearchOption.AllDirectories);
            foreach (var filePath in allFiles)
            {
                var relativePath = Path.GetRelativePath(_repoPath, filePath);
                var stagedFilePath = Path.Combine(_indexPath, relativePath);
                if (!File.Exists(stagedFilePath))
                {
                    untrackedChanges.Add(new Change { FilePath = filePath, Status = "untracked" });
                }
            }

            return new Changes
            {
                StagedChanges = stagedChanges,
                UnstagedChanges = untrackedChanges
            };
        }

        public void ClearIndex()
        {
            // Clear the staging area
            if (Directory.Exists(_indexPath))
            {
                Directory.Delete(_indexPath, true);
            }
        }
    }
}
