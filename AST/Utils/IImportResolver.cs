namespace EsotericDevZone.Celesta.AST.Utils
{
    public interface IImportResolver
    {
        string GetSourcePath(string source, bool isFileName);      
    }
}
