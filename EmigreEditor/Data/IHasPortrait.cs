using ObjectEditor.Json;

namespace Emigre.Data
{
    public interface IHasPortrait : GuidDataObject
    {
        string GetIcon();
    }
}
