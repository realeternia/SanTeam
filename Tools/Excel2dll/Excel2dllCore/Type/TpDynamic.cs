namespace Excel2dllCore.Type
{
    internal class TpDynamic : IDataType
    {
        public bool ProcessData(ref string value)
        {
            if (value.Length == 0)
            {
                if (ParseCommand.Language == "ts")
                    value = "null";
                else
                    value = "null";
            }
            return true;
        }

        public string TypeName { get { return "dynamic"; } }

    }
}