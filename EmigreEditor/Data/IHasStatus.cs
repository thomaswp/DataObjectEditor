using Emigre.Json;

namespace Emigre.Data
{
    public enum HighlightStatus
    {
        NoHighlight,
        HighlightOnce,
        Highlight,
    }

    public interface IHasStatus : GuidDataObject
    {
        HighlightStatus DefaultStatus();
    }
}
