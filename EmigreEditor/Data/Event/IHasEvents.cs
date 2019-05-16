using System.Collections.Generic;

namespace Emigre.Data
{
    public interface IHasEvents
    {
        List<StoryEvent> GetEvents();
    }
}
