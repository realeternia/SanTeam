using System;
using Excel2dllCore.Tools;
using System.Collections.Generic;
using Excel2dllCore.Load;

namespace Excel2dllCore.Check
{

    public class Checker
    {
        public string[] Vals;
        public Dictionary<int, int> RefOtherCol;
        public string Rule;
        public virtual bool Check(string text)
        {
            return true;
        }
    }

    public class BetweenChecker : Checker
    {
        public BetweenChecker(string[] value)
        {
            Vals = value;
            Rule = string.Format("Between({0},{1})", Vals[0], Vals[1]);
        }
        public override bool Check(string text)
        {
            double vmin = double.Parse(Vals[0]);
            double vmax = double.Parse(Vals[1]);

            double p = double.Parse(text);
            return p >= vmin && p <= vmax;
        }
    }
    class BiggerThanChecker : Checker
    {        
        public BiggerThanChecker(string[] value)
        {
            Vals = value;
            Rule = string.Format("BiggerThan({0})", Vals[0]);
        }
        public override bool Check(string text)
        {
            double v = double.Parse(Vals[0]);

            double p = double.Parse(text);
            return p > v;
        }
    }

    class BiggerEqualThanChecker : Checker
    {
        public BiggerEqualThanChecker(string[] value)
        {
            Vals = value;
            Rule = string.Format("BiggerEqualThan({0})", Vals[0]);
        }

        public override bool Check(string text)
        {
            double v = double.Parse(Vals[0]);

            double p = double.Parse(text);
            return p >= v;
        }
    }

    class SmallerThanChecker : Checker
    {
        public SmallerThanChecker(string[] value)
        {
            Vals = value;
            Rule = string.Format("SmallerThan({0})", Vals[0]);
        }
        public override bool Check(string text)
        {
            double v = double.Parse(Vals[0]);

            double p = double.Parse(text);
            return p < v;
        }
    }

    class SmallerEqualThanChecker : Checker
    {
        public SmallerEqualThanChecker(string[] value)
        {
            Vals = value;
            Rule = string.Format("SmallerEqualThan({0})", Vals[0]);
        }
        public override bool Check(string text)
        {
            double v = double.Parse(Vals[0]);

            double p = double.Parse(text);
            return p <= v;
        }
    }

    class EqualChecker : Checker
    {
        public EqualChecker(string[] value)
        {
            Vals = value;
            Rule = string.Format("Equal({0})", Vals[0]);
        }
        public override bool Check(string text)
        {
            double v = double.Parse(Vals[0]);

            double p = double.Parse(text);
            return p == v;
        }
    }

    class ArrayLengthChecker : Checker
    {
        public ArrayLengthChecker(string[] value)
        {
            Vals = value;
            Rule = string.Format("ArrayLength({0})", Vals[0]);
        }
        public override bool Check(string text)
        {
            if (Vals == null)
                return true; //没填为空不检查
            var valLen = Vals[0] == "" ? 0 : Vals[0].Split(',').Length;
            var textLen = text == "" ? 0 : text.Split(',').Length;
            return valLen == textLen;
        }
    }

    class StringNotEmptyChecker : Checker
    {
        public StringNotEmptyChecker()
        {
            Rule = "StringNotEmpty()";
        }
        public override bool Check(string text)
        {
            return text != "";
        }
    }

    //数组长度等于
    class ArrayLengthEqualChecker : Checker
    {
        public ArrayLengthEqualChecker(string[] value)
        {
            Vals = value;
            Rule = string.Format("ArrayLengthEqual({0})", Vals[0]);
        }
        public override bool Check(string text)
        {
            return text.Split(',').Length == uint.Parse(Vals[0]);
        }
    }

    //数组长度大于
    class ArrayLengthBiggerThanChecker : Checker
    {
        public ArrayLengthBiggerThanChecker(string[] value)
        {
            Vals = value;
            Rule = string.Format("ArrayLengthBiggerThan({0})", Vals[0]);
        }
        public override bool Check(string text)
        {
            return text.Split(',').Length > uint.Parse(Vals[0]);
        }
    }

    //数组长度小于
    class ArrayLengthSmallerThanChecker : Checker
    {
        public ArrayLengthSmallerThanChecker(string[] value)
        {
            Vals = value;
            Rule = string.Format("ArrayLengthSmallerThan({0})", Vals[0]);
        }
        public override bool Check(string text)
        {
            return text.Split(',').Length < uint.Parse(Vals[0]);
        }
    }

    //MailConfig中的附件数量都要小于GeneralItemConfig中的最大可堆叠数
    class ArraySmallerEqualChecker : Checker
    {
        public ArraySmallerEqualChecker(string[] value)
        {
            Vals = value;
            Rule = string.Format("ArraySmallerEqual({0},{1},{2},{3})", Vals[0], Vals[1], Vals[2], Vals[3]);
        }
        public override bool Check(string text)
        {
            if (DataCheck.DataExistCheck == false)
            {
                return true;
            }
            try
            {
                if (text == "" || text == "0" || text == "0.0") //空格
                {
                    return true;
                }

                string[] attachArray = Vals[0].Split(',');
                string[] attachNumArray = Vals[1].Split(',');
                uint cellValue;

                List<Record> records;
                if (Global.DataDict.TryGetValue(Vals[3], out records))
                {
                    foreach (var oneRecord in records)
                    {
                        if (oneRecord.Id == text)
                        {
                            foreach (var oneCell in oneRecord.Values)
                            {
                                if (oneCell.Type.FieldName == Vals[4])
                                {
                                    cellValue = uint.Parse(oneCell.Value);
                                    int i = 0;
                                    foreach (var oneAttach in attachArray)
                                    {
                                        if (oneAttach == text)
                                        {
                                            if (uint.Parse(attachNumArray[i]) > cellValue)
                                            {
                                                return false;
                                            }
                                            return true;
                                        }
                                        ++i;
                                    }
                                }
                            }
                        }
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                Logger.Error(string.Format("ArraySmallerEqual异常，请检查表：{0}， 列：{1} 是否存在！是否表名列名填错了？ 加$ ？", Vals[3], Vals[4]));
                Logger.Error(e.Message);
                return false;
            }
        }
    }

    //string长度等于
    class StringLengthEqualChecker : Checker
    {
        public StringLengthEqualChecker(string[] value)
        {
            Vals = value;
            Rule = string.Format("StringLengthEqual({0})", Vals[0]);
        }
        public override bool Check(string text)
        {
            return text.Length == uint.Parse(Vals[0]);
        }
    }

    //string长度大于
    class StringLengthBiggerThanChecker : Checker
    {
        public StringLengthBiggerThanChecker(string[] value)
        {
            Vals = value;
            Rule = string.Format("StringLengthBiggerThan({0})", Vals[0]);
        }
        public override bool Check(string text)
        {
            return text.Length > uint.Parse(Vals[0]);
        }
    }

    //string长度小于
    class StringLengthSmallerThanChecker : Checker
    {
        public StringLengthSmallerThanChecker(string[] value)
        {
            Vals = value;
            Rule = string.Format("StringLengthSmallerThan({0})", Vals[0]);
        }
        public override bool Check(string text)
        {
            return text.Length < uint.Parse(Vals[0]);
        }
    }

    class StringLikeChecker : Checker
    {
        public StringLikeChecker(string[] value)
        {
            Vals = value;
            Rule = string.Format("StringLike({0})", Vals[0]);
        }
        public override bool Check(string text)
        {
            if (Vals[0] == null || text == null || Vals[0].Length != text.Length)
            {
                return false;
            }
            for (int i = 0; i < Vals[0].Length; i++)
            {
                if (Vals[0][i] == '?')
                {
                    continue;
                }
                if (Vals[0][i] != text[i])
                {
                    return false;
                }
            }

            return true;
        }
    }

    class DataExistChecker : Checker
    {
        public DataExistChecker(string[] value)
        {
            Vals = value;
            Rule = string.Format("DataExist({0})", Vals[0]);
        }
        public override bool Check(string text)
        {
            if (DataCheck.DataExistCheck == false)
                return true;

            if (text == "" || text == "0" || text == "0.0") //空格
                return true;

            List<Record> records;
            if (!Global.DataDict.TryGetValue(Vals[0], out records))
            {
                if (!DataLoader.TryLoadConfigIdCol(Vals[0]))
                    return false;
                Global.DataDict.TryGetValue(Vals[0], out records);
            }

            if (records == null)
                return false;

            foreach (var oneRecord in records)
            {
                if (oneRecord.Id == text)
                {
                    if (Vals.Length == 1)
                    {
                        return true;
                    }
                    foreach (var oneCell in oneRecord.Values)
                    {
                        if (oneCell.Type.FieldName == Vals[1])
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }

    public class NullAsChecker : Checker
    {
        public NullAsChecker(string[] value)
        {
            Vals = value;
            Rule = string.Format("NullAs({0})", Vals[0]);
        }
        public override bool Check(string text)
        {
            if (Vals[0] == "" || Vals[0] == "0")
            {
                return text == "" || text == "0";
            }
            return text != "" && text != "0";
        }
    }
}
