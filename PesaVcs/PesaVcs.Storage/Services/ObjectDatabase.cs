using System;
using System.IO;
using System.Security.Cryptography;
using PesaVcs.Core.Interfaces;

namespace PesaVcs.Storage.Services
{
    public class FileSystemObjectDatabase : IObjectDatabase
    {
        private readonly string _objectsPath;
    
        public FileSystemObjectDatabase(string repositoryPath)
        {
            _objectsPath = Path.Combine(repositoryPath, ".pesavcs", "objects");
            
            // Ensure objects directory exists
            if (!Directory.Exists(_objectsPath))
            {
                Directory.CreateDirectory(_objectsPath);
            }
        }

        public void AddObject(string objectId, byte[] data)
        {
            if (string.IsNullOrEmpty(objectId))
                throw new ArgumentNullException(nameof(objectId), "Object ID cannot be null or empty");

            if (data == null)
                throw new ArgumentNullException(nameof(data), "Object data cannot be null");

            try
            {
                string objectPath = Path.Combine(_objectsPath, objectId);
               
                string directoryPath = Path.GetDirectoryName(objectPath) ?? string.Empty;
                
                if (!string.IsNullOrEmpty(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                File.WriteAllBytes(objectPath, data);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to add object {objectId}: {ex.Message}", ex);
            }
        }

        public byte[] GetObject(string objectId)
        {
            if (string.IsNullOrEmpty(objectId))
                throw new ArgumentNullException(nameof(objectId), "Object ID cannot be null or empty");

            string objectPath = Path.Combine(_objectsPath, objectId);

            if (!File.Exists(objectPath))
                throw new FileNotFoundException($"Object {objectId} not found");

            return File.ReadAllBytes(objectPath);
        }

        public bool ObjectExists(string objectId)
        {
            if (string.IsNullOrEmpty(objectId))
                return false;

            string objectPath = Path.Combine(_objectsPath, objectId);
            return File.Exists(objectPath);
        }

        public void DeleteObject(string objectId)
        {
            if (string.IsNullOrEmpty(objectId))
                throw new ArgumentNullException(nameof(objectId), "Object ID cannot be null or empty");

            string objectPath = Path.Combine(_objectsPath, objectId);

            if (File.Exists(objectPath))
            {
                File.Delete(objectPath);
            }
        }

        // Helper method to generate object ID (SHA-1 hash)
        public static string GenerateObjectId(byte[] data)
        {
            using (SHA1 sha1 = SHA1.Create())
            {
                byte[] hashBytes = sha1.ComputeHash(data);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }
    }
}