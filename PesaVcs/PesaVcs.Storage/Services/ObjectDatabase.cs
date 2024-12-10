using PesaVcs.Core.Interfaces;

namespace PesaVcs.Storage.Services
{
    public class FileSystemObjectDatabase : IObjectDatabase
    {
        private readonly string _storagePath;

        public FileSystemObjectDatabase(string storagePath)
        {
            _storagePath = storagePath;

            // Ensure the storage directory exists
            if (!Directory.Exists(_storagePath))
            {
                Directory.CreateDirectory(_storagePath);
            }
        }

        public void AddObject(string objectId, byte[] data)
        {
            string filePath = GetFilePath(objectId);

            // If the object already exists, throw an exception
            if (File.Exists(filePath))
            {
                throw new InvalidOperationException($"Object with ID '{objectId}' already exists.");
            }

            File.WriteAllBytes(filePath, data);
        }

        public byte[] GetObject(string objectId)
        {
            string filePath = GetFilePath(objectId);

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Object with ID '{objectId}' not found.");
            }

            return File.ReadAllBytes(filePath);
        }

        public bool ObjectExists(string objectId)
        {
            string filePath = GetFilePath(objectId);
            return File.Exists(filePath);
        }

        public void DeleteObject(string objectId)
        {
            string filePath = GetFilePath(objectId);

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Object with ID '{objectId}' not found.");
            }

            File.Delete(filePath);
        }

        private string GetFilePath(string objectId)
        {
            // Generate a file path based on the object ID
            return Path.Combine(_storagePath, objectId);
        }
    }
}
