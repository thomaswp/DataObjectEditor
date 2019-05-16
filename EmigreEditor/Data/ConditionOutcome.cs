using ObjectEditor.Json;
using System.Collections.Generic;
using System;

namespace Emigre.Data
{
    public class ConditionOutcome : Outcome, ICustomScriptable
    {
        public readonly List<Condition> extraConditions = new List<Condition>();

        public bool meetAllConditions = true;
        
        [FieldTag(FieldTags.Inline)]
        public Condition condition1;
        [FieldTag(FieldTags.Inline)]
        public Condition condition2;

        public override void AddFields(FieldData fields)
        {
            base.AddFields(fields);
            fields.addList(extraConditions, "extraConditions");
            meetAllConditions = fields.add(meetAllConditions, "meetAllConditions");
            condition1 = fields.add(condition1, "condition");
            condition2 = fields.add(condition2, "condition2");
        }

        public override string ToString()
        {
            return condition1 == null ? "Outcome" : condition1.ToString();
        }

        public string Write()
        {
            return "If " + condition1 + (condition2 == null ? "" : " and " + condition2);
        }

        public void Read(string data)
        {
        }

        public string Indent()
        {
            return ">";
        }
    }
}
