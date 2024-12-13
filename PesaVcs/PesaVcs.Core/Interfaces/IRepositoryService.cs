namespace PesaVcs.Core.Interfaces
{
    public interface IRepositoryService
    {
        void Initialize(string path);
        void Clone(string remoteUrl, string localPath);
        void Delete(string path);
        bool Exists(string path);

        public string GetRepositoryRoot();

    }
}
