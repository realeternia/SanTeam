using System.Collections.Generic;
using Excel2dllCore.Export.CSharp.Item;
using Excel2dllCore.Export.Filter;

namespace Excel2dllCore.Export.CSharp
{
    public class CsExportItemManager
    {
        private static Dictionary<string, ITableExporter> userExporterDict = new Dictionary<string, ITableExporter>();
        private static ITableExporter _trivialTableExporter;
        static CsExportItemManager()
        {
            _trivialTableExporter = new CsCommonTableExporter();
            userExporterDict["GameConfig"] = new CsGameTableExporter();
        }

        public static void ExportType(List<CellType> types, List<Record> records, string fullPath, string fileName, ICsFilter filter)
        {
            var uselessCount = types.FindAll(a => filter.IsIgnore(a.CS)).Count;
            if (types.Count == uselessCount)
                return;

            ITableExporter tableExporter;
            if (userExporterDict.TryGetValue(fileName, out tableExporter))
            {
                tableExporter.ExportType(types, records, fullPath, fileName, filter);
            }
            else
            {
                _trivialTableExporter.ExportType(types, records, fullPath, fileName, filter);
            }
        }

        public static void ExportRecord(List<CellType> types, List<Record> records, string fullPath, string fileName, ICsFilter filter)
        {
            var uselessCount = types.FindAll(a => filter.IsIgnore(a.CS)).Count;
            if (types.Count == uselessCount)
                return;

            ITableExporter tableExporter;
            if (userExporterDict.TryGetValue(fileName, out tableExporter))
            {
                tableExporter.ExportRecord(types, records, fullPath, fileName, filter);
            }
            else if (fileName.ToLower().Substring(0, 2) == "bi" || fileName.ToLower().Substring(0, 2) == "ti")
            {
                tableExporter = userExporterDict["Multiple"];
                tableExporter.ExportRecord(types, records, fullPath, fileName, filter);
            }
            else
            {
                _trivialTableExporter.ExportRecord(types, records, fullPath, fileName, filter);
            }
        }

    }
}