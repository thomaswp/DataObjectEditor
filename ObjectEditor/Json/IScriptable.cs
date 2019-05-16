
namespace Emigre.Json
{
    public interface IScriptable : GuidDataObject
    {
    }

    public interface ICustomScriptable : IScriptable
    {
        string Write();
        void Read(string data);
        string Indent();
    }

    public interface IReadOnlyScriptable : IScriptable
    {
    }

}
