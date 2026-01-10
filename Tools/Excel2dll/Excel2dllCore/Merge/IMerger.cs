namespace Excel2dllCore.Merge
{
    public interface IMerger
    {
        void Process(string path);
        bool Check(string path);
    }
}