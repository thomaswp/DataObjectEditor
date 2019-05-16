using ObjectEditor.Json;
using System.Collections.Generic;

namespace Emigre.Data
{
    public class Outcome : GameData, IHasEvents
    {
        public readonly List<StoryEvent> outcomeEvents = new List<StoryEvent>();
        public readonly Reference<StoryEvent> nextEvent = new Reference<StoryEvent>();
        
        public List<StoryEvent> GetEvents()
        {
            return outcomeEvents;
        }

        public override void AddFields(FieldData fields)
        {
            base.AddFields(fields);
            fields.addList(outcomeEvents, "outcomeEvents");
            fields.addReference(nextEvent, "nextEvent");
        }
    }
}
