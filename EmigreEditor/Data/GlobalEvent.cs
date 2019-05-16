using ObjectEditor.Json;

namespace Emigre.Data
{
    public class GlobalEvent : ConditionOutcome, IEnableable
    {
        public string name = "";
        public bool disableWhenDone;

        public override void AddFields(FieldData fields)
        {
            base.AddFields(fields);
            name = fields.add(name, "name");
            disableWhenDone = fields.add(disableWhenDone, "disableWhenDone");
        }

        public bool GetDefaultEnabled()
        {
            return true;
        }

        public override string ToString()
        {
            return name;
        }
    }
}
