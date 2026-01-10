namespace Excel2dllCore.Export.Filter
{
    public class FakeCsFilter : ICsFilter
    {
        public string Name
        {
            get { return "a"; }
        }

        public FakeCsFilter()
        {

        }
        
        public bool IsIgnore(string s)
        {
            return false;
        }
    }
}