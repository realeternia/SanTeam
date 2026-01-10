using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Excel2dllCore.Check
{
    public class RulePreprocessor
    {
        public static string CheckText(string str)
        {
            string temp = str.Replace(" ", "").Replace("\t", "");
            if (temp.StartsWith(">="))
            {
                return string.Format("BiggerEqualThan({0})", temp.Substring(2));
            }
            if (temp.StartsWith(">"))
            {
                return string.Format("BiggerThan({0})", temp.Substring(1));
            }
            if (temp.StartsWith("<="))
            {
                return string.Format("SmallerEqualThan({0})", temp.Substring(2));
            }
            if (temp.StartsWith("<"))
            {
                return string.Format("SmallerThan({0})", temp.Substring(1));
            }
            if (temp.StartsWith("=="))
            {
                return string.Format("Equal({0})", temp.Substring(2));
            }
            if (temp.StartsWith("="))
            {
                return string.Format("Equal({0})", temp.Substring(1));
            }
            if (temp.Length > 0 && temp[0] >= '0' && temp[0] <= '9')
            {
                return string.Format("StringLike({0})", temp);
            }
            return temp;
        }
    }

}
