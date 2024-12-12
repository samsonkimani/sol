using System.Security.Cryptography;
using System.Text;

namespace PesaVcs.Core.Services
{
    public class TreeEntry
    {
        // Required properties
        public string Type { get; set; } = string.Empty;
        public string Hash { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;

        // Optional properties
        public long Size { get; set; } = 0;

        // Validation method
        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(Type))
                throw new ArgumentException("Tree entry type is required", nameof(Type));

            if (string.IsNullOrWhiteSpace(Hash))
                throw new ArgumentException("Tree entry hash is required", nameof(Hash));

            if (string.IsNullOrWhiteSpace(Name))
                throw new ArgumentException("Tree entry name is required", nameof(Name));

            // Validate type is either "blob" or "tree"
            if (Type != "blob" && Type != "tree")
                throw new ArgumentException("Tree entry type must be 'blob' or 'tree'", nameof(Type));
        }
    }


  public class Commit
    {
        // Required properties
        public string TreeHash { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;

        // Optional properties
        public string Hash { get; set; } = string.Empty;
        public string ParentHash { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        // Validation method
        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(TreeHash))
                throw new ArgumentException("Commit tree hash is required", nameof(TreeHash));

            if (string.IsNullOrWhiteSpace(Author))
                throw new ArgumentException("Commit author is required", nameof(Author));

            if (string.IsNullOrWhiteSpace(Message))
                throw new ArgumentException("Commit message is required", nameof(Message));

            // Optional validation for author format (example)
            if (!IsValidAuthorFormat(Author))
                throw new ArgumentException("Invalid author format. Use 'Name <email>'", nameof(Author));
        }

        private bool IsValidAuthorFormat(string author)
        {
            // Simple validation for author format: Name <email>
            return System.Text.RegularExpressions.Regex.IsMatch(
                author, 
                @"^[^<>]+\s*<[^<>]+>$"
            );
        }
    }


    public class RepositoryTreeService
    {
        private readonly string _repoPath;
        private readonly string _objectsPath;
        private readonly string _commitsPath;

        public RepositoryTreeService(string repositoryPath)
        {
            // Validate repository path
            if (string.IsNullOrWhiteSpace(repositoryPath))
                throw new ArgumentException("Repository path cannot be empty", nameof(repositoryPath));

            _repoPath = repositoryPath;
            _objectsPath = Path.Combine(repositoryPath, ".pesavcs", "objects");
            _commitsPath = Path.Combine(repositoryPath, ".pesavcs", "commits");

            Directory.CreateDirectory(_objectsPath);
            Directory.CreateDirectory(_commitsPath);
        }

        public string CreateTree(List<TreeEntry> entries)
        {
            // Validate input
            if (entries == null || entries.Count == 0)
                throw new ArgumentException("Tree must contain at least one entry", nameof(entries));

            // Validate each tree entry
            foreach (var entry in entries)
            {
                entry.Validate();
            }

            // Serialize tree entries
            var sortedEntries = entries.OrderBy(e => e.Name);
            var treeContent = new StringBuilder();

            foreach (var entry in sortedEntries)
            {
                treeContent.AppendLine($"{entry.Type} {entry.Hash} {entry.Name}");
            }

            // Generate tree hash
            byte[] treeBytes = Encoding.UTF8.GetBytes(treeContent.ToString());
            string treeHash = ComputeSHA1Hash(treeBytes);

            // Store tree object
            File.WriteAllBytes(Path.Combine(_objectsPath, treeHash), treeBytes);

            return treeHash;
        }


        public Commit CreateCommit(Commit commitDetails)
        {
            // Validate commit details
            commitDetails.Validate();

            // Serialize commit
            var commitContent = new StringBuilder();
            commitContent.AppendLine($"tree {commitDetails.TreeHash}");
            
            // parent hash
            if (!string.IsNullOrWhiteSpace(commitDetails.ParentHash))
                commitContent.AppendLine($"parent {commitDetails.ParentHash}");
            
            commitContent.AppendLine($"author {commitDetails.Author}");
            commitContent.AppendLine($"timestamp {commitDetails.Timestamp:O}");
            commitContent.AppendLine();
            commitContent.AppendLine(commitDetails.Message);

            byte[] commitBytes = Encoding.UTF8.GetBytes(commitContent.ToString());
            commitDetails.Hash = ComputeSHA1Hash(commitBytes);

            // Store commit object
            File.WriteAllBytes(Path.Combine(_objectsPath, commitDetails.Hash), commitBytes);
            File.WriteAllText(Path.Combine(_commitsPath, commitDetails.Hash), commitContent.ToString());

            return commitDetails;
        }

        public List<Commit> GetCommitLog(int limit = 10)
        {
            var commits = new List<Commit>();

            var commitFiles = Directory.GetFiles(_commitsPath)
                .OrderByDescending(f => new FileInfo(f).CreationTime)
                .Take(limit);

            foreach (var commitFile in commitFiles)
            {
                var commitContent = File.ReadAllLines(commitFile);
                var commit = ParseCommitFromLines(commitContent);
                commits.Add(commit);
            }

            return commits;
        }

        public List<string> GetFileDifferences(string commitHash1, string commitHash2)
        {
            var differences = new List<string>();

            // Retrieve tree hashes for both commits
            var tree1 = GetTreeFromCommit(commitHash1);
            var tree2 = GetTreeFromCommit(commitHash2);

            // Compare tree entries
            var entries1 = ParseTreeEntries(tree1);
            var entries2 = ParseTreeEntries(tree2);

            // Find differences
            var diff1 = entries1.Where(e => !entries2.Any(n => n.Name == e.Name && n.Hash == e.Hash));
            var diff2 = entries2.Where(e => !entries1.Any(n => n.Name == e.Name && n.Hash == e.Hash));

            differences.AddRange(diff1.Select(d => $"Removed/Changed: {d.Name}"));
            differences.AddRange(diff2.Select(d => $"Added/Changed: {d.Name}"));

            return differences;
        }

        private string GetTreeFromCommit(string commitHash)
        {
            var commitPath = Path.Combine(_commitsPath, commitHash);
            if (!File.Exists(commitPath))
                throw new FileNotFoundException($"Commit {commitHash} not found");

            var commitLines = File.ReadAllLines(commitPath);
            var treeLinePrefix = "tree ";
            var treeLine = commitLines.FirstOrDefault(l => l.StartsWith(treeLinePrefix));

            if (treeLine == null)
                throw new InvalidOperationException($"No tree found in commit {commitHash}");

            return treeLine.Substring(treeLinePrefix.Length).Trim();
        }

        private List<TreeEntry> ParseTreeEntries(string treeHash)
        {
            var treeEntries = new List<TreeEntry>();
            var treePath = Path.Combine(_objectsPath, treeHash);

            if (!File.Exists(treePath))
                throw new FileNotFoundException($"Tree {treeHash} not found");

            var treeContent = File.ReadAllLines(treePath);
            foreach (var line in treeContent)
            {
                var parts = line.Split(' ');
                if (parts.Length == 3)
                {
                    treeEntries.Add(new TreeEntry
                    {
                        Type = parts[0],
                        Hash = parts[1],
                        Name = parts[2]
                    });
                }
            }

            return treeEntries;
        }

        private Commit ParseCommitFromLines(string[] commitLines)
        {
            var commit = new Commit();
            foreach (var line in commitLines)
            {
                if (line.StartsWith("tree "))
                    commit.TreeHash = line.Substring(5).Trim();
                else if (line.StartsWith("parent "))
                    commit.ParentHash = line.Substring(7).Trim();
                else if (line.StartsWith("author "))
                    commit.Author = line.Substring(7).Trim();
                else if (line.StartsWith("timestamp "))
                    commit.Timestamp = DateTime.Parse(line.Substring(10).Trim());
            }

            // Last line or lines after blank line is the commit message
            var messageLines = commitLines.SkipWhile(l => !string.IsNullOrWhiteSpace(l)).Skip(1);
            commit.Message = string.Join(Environment.NewLine, messageLines).Trim();

            return commit;
        }

       private static string ComputeSHA1Hash(byte[] data)
        {
            using (var sha1 = SHA1.Create())
            {
                var hashBytes = sha1.ComputeHash(data);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }
    }
}