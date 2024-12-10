
using PesaVcs.Core.Interfaces;

namespace PesaVcs.Staging.Services
{
    public class StagingAreaManager : IStagingAreaManager
    {
        private readonly IndexService _indexService;

        public StagingAreaManager(IndexService indexService)
        {
            _indexService = indexService;
        }

        public void AddToStaging(string filePath)
        {
            _indexService.StageFile(filePath);
        }

        public void RemoveFromStaging(string filePath)
        {
            _indexService.UnstageFile(filePath);
        }

        public void ClearStaging()
        {
            _indexService.ClearIndex();
        }

        public List<string> ListStagedFiles()
        {
            var changes = _indexService.GetChanges();
            return changes.StagedChanges?
                    .Where(c => c.FilePath != null)
                    .Select(c => c.FilePath!)
                    .ToList() 
                ?? new List<string>();
        }

    }

}

