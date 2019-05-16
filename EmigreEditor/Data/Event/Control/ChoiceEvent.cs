namespace Emigre.Data
{
    using System.Linq;
    using System.Collections.Generic;
    using ObjectEditor.Json;
    
    public abstract class ChoiceEvent : StoryEvent, ISearchable, IScriptable
    {
        public abstract IEnumerable<Choice> GetChoices();

        [FieldTag(FieldTags.Multiline)]
        [FieldTag(FieldTags.Title)]
        public string prompt = "";

        public override void AddFields(FieldData fields)
        {
            base.AddFields(fields);
            prompt = fields.add(prompt, "prompt");
        }

        public override StoryEvent.Action GetTransitionAction()
        {
            return Action.NextEvent;
        }

        public override string ToString()
        {
            return "\"" + prompt + "\"";
        }
    }

    public abstract class ChoiceEvent<T> : ChoiceEvent where T : Choice
    {

        static ChoiceEvent()
        {
            ReflectionConstructor<Choice>.Register("Choice");
        }

        public readonly List<T> choices = new List<T>();

        public override IEnumerable<Choice> GetChoices()
        {
            return choices.Select((choice) => (Choice)choice);
        }

        public override void AddFields(FieldData fields)
        {
            base.AddFields(fields);
            fields.addList(choices, "choices");
        }

        public override bool cut()
        {
            return true;
        }
    }

    public class Choice : Outcome, IEnableable, IScriptable
    {
        [FieldTag(FieldTags.Title, ">")]
        [FieldTag(FieldTags.Multiline)]
        public string text;
        public string shortText;
        public bool enabled = true;

        public readonly Reference<Skill> skillRequisite = new Reference<Skill>();
        public int requiredSkillLevel;

        public readonly List<Condition> conditions = new List<Condition>();
        public bool meetAllConditions = true;

        public override void AddFields(FieldData fields)
        {
            base.AddFields(fields);
            text = fields.add(text, "text");
            shortText = fields.add(shortText, "shortText");
            enabled = fields.add(enabled, "enabled");
            fields.addReference(skillRequisite, "skillRequisite");
            requiredSkillLevel = fields.add(requiredSkillLevel, "requiredSkillLevel");
            fields.addList(conditions, "conditions");
            meetAllConditions = fields.add(meetAllConditions, "meetAllConditions");
        }

        public override string ToString()
        {
            return string.IsNullOrEmpty(shortText) ? text : shortText;
        }

        public bool GetDefaultEnabled()
        {
            return enabled;
        }
    }

}