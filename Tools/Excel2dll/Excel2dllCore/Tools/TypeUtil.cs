namespace Excel2dllCore.Tools
{
    public class TypeUtil
    {
        public static string TypeCs2Ts(string typeName)
        {
            switch (typeName.ToLower())
            {
                case "byte": return "number";
                case "ushort": return "number";
                case "uint": return "number";
                case "short": return "number";
                case "int": return "number";
                case "long": return "number";
                case "ulong": return "number";
                case "double": return "number";
                case "float": return "number";
                case "byte[]": return "Array<number>";
                case "ushort[]": return "Array<number>";
                case "uint[]": return "Array<number>";
                case "short[]": return "Array<number>";
                case "int[]": return "Array<number>";
                case "long[]": return "Array<number>";
                case "ulong[]": return "Array<number>";
                case "float[]": return "Array<number>";
                case "double[]": return "Array<number>";
                case "bool": return "boolean";
                case "string": return "string";
                case "string[]": return "Array<string>";
                case "any": return "any";
            }

            return "AutoGen." + typeName;
        }
        public static string TypeCs2As(string typeName)
        {
            switch (typeName.ToLower())
            {
                case "byte": return "Number";
                case "ushort": return "Number";
                case "uint": return "Number";
                case "short": return "Number";
                case "int": return "Number";
                case "long": return "Number";
                case "ulong": return "Number";
                case "double": return "Number";
                case "float": return "Number";
                case "byte[]": return "Array";
                case "ushort[]": return "Array";
                case "uint[]": return "Array";
                case "short[]": return "Array";
                case "int[]": return "Array";
                case "long[]": return "Array";
                case "ulong[]": return "Array";
                case "float[]": return "Array";
                case "double[]": return "Array";
                case "bool": return "Boolean";
                case "string": return "String";
                case "string[]": return "Array";
                case "any": return "*";
            }

            return "AutoGen." + typeName;
        }
    }
    
}