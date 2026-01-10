using System.Collections.Generic;
using Excel2dllCore.Export.Filter;

namespace Excel2dllCore.Export
{
    public interface ITableExporter
    {
        void ExportType(List<CellType> types, List<Record> records, string fullPath, string fileName, ICsFilter filter);
        void ExportRecord(List<CellType> types, List<Record> records, string fullPath, string fileName, ICsFilter filter);
    }
}