using System.Collections.Generic;
using Emigre.Json;

namespace Emigre.Data
{
    public class Item : History
    {
        public bool canDrop = true;
        public readonly List<StoryEvent> useEvents = new List<StoryEvent>();

        public override void AddFields(FieldData fields)
        {
            base.AddFields(fields);
            canDrop = fields.add(canDrop, "canDrop");
            fields.addList(useEvents, "useEvents");
        }
    }
}
