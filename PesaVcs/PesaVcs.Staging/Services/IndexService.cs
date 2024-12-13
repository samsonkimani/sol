using System.Text.Json;
using PesaVcs.Core.Interfaces;
using PesaVcs.Storage.Services;

namespace PesaVcs.Staging.Services
{
    public class IndexService : IIndexService
    {
        private readonly string _repoPath;
        private readonly string _indexPath;
        private readonly FileSystemObjectDatabase _objectDatabase;

        // Index entry to track staged files
        public class IndexEntry
        {
            public string FilePath { get; set; } = string.Empty;
            public string ObjectId { get; set; } = string.Empty;
            public DateTime Timestamp { get; set; }
        }

        public IndexService(string repoPath)
        {
            _repoPath = repoPath;
            _indexPath = Path.Combine(repoPath, ".pesavcs", "index");
            _objectDatabase = new FileSystemObjectDatabase(repoPath);

            // Ensure .pesavcs directory exists
            string pesaVcsPath = Path.Combine(repoPath, ".pesavcs");
            if (!Directory.Exists(pesaVcsPath))
            {
                Directory.CreateDirectory(pesaVcsPath);
            }

            // Create index file if it doesn't exist
            if (!File.Exists(_indexPath))
            {
                File.Create(_indexPath).Close();
            }
        }

        public void StageAllChanges()
        {
            // Get all files in the repository
            var files = Directory.GetFiles(_repoPath, "*.*", SearchOption.AllDirectories)
                .Where(f => !f.Contains(".pesavcs"));

            foreach (var filePath in files)
            {
                StageFile(filePath);
            }
        }

        public void StageFile(string filePath)
        {
            // Validate file exists and is not in .pesavcs directory
            if (!File.Exists(filePath) || filePath.Contains(".pesavcs"))
                return;

            // Compute relative path
            string relativePath = Path.GetRelativePath(_repoPath, filePath);

            // Read file content
            byte[] fileContent = File.ReadAllBytes(filePath);

            // Generate object ID
            string objectId = FileSystemObjectDatabase.GenerateObjectId(fileContent);

            // Store file content in object database
            _objectDatabase.AddObject(objectId, fileContent);

            // Create index entry
            var indexEntry = new IndexEntry
            {
                FilePath = relativePath,
                ObjectId = objectId,
                Timestamp = DateTime.UtcNow
            };

            // Update index file
            UpdateIndexFile(indexEntry);
        }

        public void UnstageFile(string filePath)
        {
            // Compute relative path
            string relativePath = Path.GetRelativePath(_repoPath, filePath);

            // Read current index entries
            var indexEntries = ReadIndexFile();

            // Remove the specific file from index
            indexEntries.RemoveAll(entry => entry.FilePath == relativePath);

            // Rewrite the index file
            WriteIndexFile(indexEntries);
        }

        public Changes GetChanges()
        {
            var stagedChanges = new List<Change>();
            var untrackedChanges = new List<Change>();

            // Read current index entries
            var indexEntries = ReadIndexFile();

            // Staged changes are entries in the index
            stagedChanges = indexEntries.Select(entry => new Change
            {
                FilePath = Path.Combine(_repoPath, entry.FilePath),
                Status = "staged",
                Hash = entry.ObjectId
            }).ToList();

            // Find untracked files
            var allFiles = Directory.GetFiles(_repoPath, "*.*", SearchOption.AllDirectories)
                .Where(f => !f.Contains(".pesavcs"));

            var stagedFiles = indexEntries.Select(entry => Path.Combine(_repoPath, entry.FilePath)).ToHashSet();

            untrackedChanges = allFiles
                .Where(file => !stagedFiles.Contains(file))
                .Select(file => new Change
                {
                    FilePath = file,
                    Status = "untracked"
                })
                .ToList();

            return new Changes
            {
                StagedChanges = stagedChanges,
                UnstagedChanges = untrackedChanges
            };
        }

        public void ClearIndex()
        {
            // Clear the index file contents
            File.WriteAllText(_indexPath, string.Empty);
        }

        private void UpdateIndexFile(IndexEntry newEntry)
        {
            // Read current index entries
            var indexEntries = ReadIndexFile();

            // Remove existing entry for the same file if exists
            indexEntries.RemoveAll(entry => entry.FilePath == newEntry.FilePath);

            // Add new entry
            indexEntries.Add(newEntry);

            // Write updated entries back to index file
            WriteIndexFile(indexEntries);
        }

        private List<IndexEntry> ReadIndexFile()
        {
            try
            {
                // Read index file content
                string content = File.ReadAllText(_indexPath);

                // If file is empty, return empty list
                if (string.IsNullOrWhiteSpace(content))
                    return new List<IndexEntry>();

                // Deserialize index entries
                return JsonSerializer.Deserialize<List<IndexEntry>>(content) 
                       ?? new List<IndexEntry>();
            }
            catch
            {
                // If there's any issue reading the file, return empty list
                return new List<IndexEntry>();
            }
        }

        private void WriteIndexFile(List<IndexEntry> entries)
        {
            // Serialize and write entries to index file
            string jsonContent = JsonSerializer.Serialize(entries, new JsonSerializerOptions 
            { 
                WriteIndented = true 
            });

            File.WriteAllText(_indexPath, jsonContent);
        }
    }
}