namespace PesaVcs.Core.Interfaces
{
    public interface IIndexService
    {
        void StageAllChanges();
        void StageFile(string filePath);
        void UnstageFile(string filePath);
        Changes GetChanges();
        void ClearIndex();
    }

    public class Changes
    {
        public List<Change>? StagedChanges { get; set; }
        public List<Change>? UnstagedChanges { get; set; }
    }

    public class Change
    {
        public string? FilePath { get; set; }
        public string? Status { get; set; }

         public string? Hash { get; set; }
    }
}
