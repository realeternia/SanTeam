namespace Excel2dllCore.Export.Filter
{
    public class CommonCsFilter : ICsFilter
    {
        private string myCs;

        public string Name
        {
            get { return myCs; }
        }

        public CommonCsFilter(string fValue)
        {
            myCs = fValue;
        }

        public bool IsIgnore(string s)
        {
            return !s.Contains(myCs);
        }
    }
}