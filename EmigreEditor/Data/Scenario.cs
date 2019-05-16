namespace Emigre.Data
{

    using System.Collections.Generic;
    using Emigre.Json;

    public class Scenario : GameData, IHasEvents, IEnableable, IScriptable
    {
        [FieldTag(FieldTags.Title, "#")]
        public string name = "Scenario";
        public bool enabled = true;
        public bool disableWhenDone;
        public readonly List<StoryEvent> events = new List<StoryEvent>();
        public readonly List<Page> pages = new List<Page>();
        public readonly List<Actor> actors = new List<Actor>();

        public Scenario() { }

        public Scenario(string name)
        {
            this.name = name;
        }

        public override void AddFields(FieldData fields)
        {
            base.AddFields(fields);
            name = fields.add(name, "name");
            enabled = fields.add(enabled, "enabled");
            disableWhenDone = fields.add(disableWhenDone, "disableWhenDone");
            fields.addList(events, "events");
            fields.addList(pages, "pages");
            fields.addList(actors, "actors");
        }

        public override string ToString()
        {
            return name;
        }

        public List<StoryEvent> GetEvents()
        {
            return events;
        }

        public bool GetDefaultEnabled()
        {
            return enabled;
        }
    }

}