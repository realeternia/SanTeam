namespace Excel2dllCore.Export.Filter
{
    public interface ICsFilter
    {
        string Name { get; }
        bool IsIgnore(string s);
    }
}