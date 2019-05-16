using Emigre.Json;
using System;
using System.Collections.Generic;

namespace Emigre.Data
{
    public class Page : GameData, IHasEvents, IEnableable, IHasStatus, ICustomScriptable
    {
        public string name = "Page";
        public readonly List<StoryEvent> events = new List<StoryEvent>();
        public readonly List<Condition> extraConditions = new List<Condition>();
        public bool enabled = true;
        public HighlightStatus defaultStatus = HighlightStatus.Highlight;

        public bool meetAllConditions = true;
        [FieldTag(FieldTags.Inline)]
        public Condition condition1;
        [FieldTag(FieldTags.Inline)]
        public Condition condition2;


        public override void AddFields(FieldData fields)
        {
            base.AddFields(fields);
            name = fields.add(name, "name");
            fields.addList(events, "events");
            fields.addList(extraConditions, "extraConditions");
            enabled = fields.add(enabled, "enabled");
            defaultStatus = fields.addEnum(defaultStatus, "defaultStatus");
            meetAllConditions = fields.add(meetAllConditions, "meetAllConditions");
            condition1 = fields.add(condition1, "condition1");
            condition2 = fields.add(condition2, "condition2");
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

        public HighlightStatus DefaultStatus()
        {
            return defaultStatus;
        }
        
        public string Write()
        {
            string r = name;
            if (condition1 != null) r += " (If " + condition1 + ")";
            return r;
        }

        public void Read(string data)
        {
            int index = data.IndexOf(" (If");
            if (index >= 0) data = data.Substring(0, index);
            name = data;
        }

        public string Indent()
        {
            return "#";
        }
    }
}
