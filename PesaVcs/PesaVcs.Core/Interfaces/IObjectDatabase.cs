namespace PesaVcs.Core.Interfaces
{
    public interface IObjectDatabase
    {
        void AddObject(string objectId, byte[] data);
        byte[] GetObject(string objectId);
        bool ObjectExists(string objectId);
        void DeleteObject(string objectId);
    }
}
