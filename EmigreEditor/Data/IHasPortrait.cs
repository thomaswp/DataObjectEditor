using Emigre.Json;

namespace Emigre.Data
{
    public interface IHasPortrait : GuidDataObject
    {
        string GetIcon();
    }
}
