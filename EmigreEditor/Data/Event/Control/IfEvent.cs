namespace Emigre.Data
{
    using System.Collections.Generic;
    using Json;

    [Category("Control")]
    public class IfEvent : StoryEvent, IScriptable
    {
        public readonly List<ConditionOutcome> outcomes = new List<ConditionOutcome>();

        public override void AddFields(FieldData fields)
        {
            base.AddFields(fields);
            fields.addList(outcomes, "conditions");
        }

        public override string ToString()
        {
            string text =  "If: ";
            for (int i = 0; i < outcomes.Count; i++)
            {
                if (i > 0) text += " / ";
                text += outcomes[i].ToString();
            }
            return text;
        }
    }

}