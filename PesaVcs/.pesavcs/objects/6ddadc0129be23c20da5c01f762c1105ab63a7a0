namespace PesaVcs.Core.Interfaces
{
    public interface IBranchService
    {
        void CreateBranch(string name);
        void DeleteBranch(string name);
        List<Branch> ListBranches();
        Branch GetCurrentBranch();
        void SwitchBranch(string name);
    }

    public class Branch
    {
        public string? Name { get; set; }
        public bool? IsCurrent { get; set; }
    }
}
