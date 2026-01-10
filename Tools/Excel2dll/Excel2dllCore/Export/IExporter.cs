namespace Excel2dllCore.Export
{
    public interface IExporter
    {
        void Prework();
        void ExportRecord();
        void ExportType();
        void ExportVersion();
    }
}