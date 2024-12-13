using PesaVcs.Core.Interfaces;
using System.Text.Json;

namespace PesaVcs.Storage.Services
{
    public class CommitService : ICommitService
    {
        private readonly IObjectDatabase _objectDatabase;

        public CommitService(IObjectDatabase objectDatabase)
        {
            _objectDatabase = objectDatabase;
        }

        /// <summary>
        /// Creates a new commit and saves it to the object database.
        /// </summary>
        public PesaVcs.Core.Interfaces.Commit CreateCommit(string message, string author, string email)
        {
            ValidateCommitInput(message, author, email);

            var commit = new PesaVcs.Core.Interfaces.Commit
            {
                Id = Guid.NewGuid().ToString(),
                Date = DateTime.UtcNow,
                Author = $"{author} <{email}>",
                Message = message
            };

            // Serialize and save the commit to the database
            var commitData = JsonSerializer.SerializeToUtf8Bytes(commit);
            _objectDatabase.AddObject(commit.Id, commitData);

            return commit;
        }

        /// <summary>
        /// Retrieves a commit by its ID from the object database.
        /// </summary>
        public Commit GetCommitById(string commitId)
        {
            if (!_objectDatabase.ObjectExists(commitId))
            {
                throw new CommitNotFoundException(commitId);
            }

            var data = _objectDatabase.GetObject(commitId);
            return JsonSerializer.Deserialize<Commit>(data) 
                   ?? throw new Exception("Failed to deserialize commit.");
        }

        /// <summary>
        /// Retrieves all commits by iterating through all stored objects.
        /// Requires a mechanism for retrieving all stored commit IDs.
        /// </summary>
        public List<Commit> GetAllCommits()
        {
            // Implement an index or storage system to track commit IDs
            throw new NotImplementedException("Requires indexing mechanism for object IDs.");
        }

        /// <summary>
        /// Validates the input for creating a commit.
        /// </summary>
        private void ValidateCommitInput(string message, string author, string email)
        {
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException("Commit message cannot be empty.");

            if (string.IsNullOrWhiteSpace(author))
                throw new ArgumentException("Author name cannot be empty.");

            if (!email.Contains("@") || string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Invalid email address.");
        }

        /// <summary>
        /// Custom exception for commit-related errors.
        /// </summary>
        public class CommitNotFoundException : Exception
        {
            public CommitNotFoundException(string commitId)
                : base($"Commit with ID {commitId} not found.")
            {
            }
        }
    }
}
