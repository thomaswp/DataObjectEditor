namespace Emigre.Data
{
    using ObjectEditor.Json;

    [Category("Data")]
    public class SetValueEvent : StoryEvent, IReadOnlyScriptable
    {
        public enum Operation
        {
            Set,
            Change
        }

        [FieldTag(FieldTags.Inline)]
        public SettableGameValue value = new ResourceValue();
        public Operation operation = Operation.Change;
        public int amount;

        public override void AddFields(FieldData fields)
        {
            base.AddFields(fields);
            value = fields.add(value, "gameValue");
            operation = fields.addEnum(operation, "operation");
            amount = fields.add(amount, "value");

            if (fields.readMode())
            {
                Resource resource = fields.addEnum(default(Resource), "resource");
                if (resource != default(Resource))
                {
                    ResourceValue rv = new ResourceValue();
                    rv.resource = resource;
                    value = rv;
                }
            }
        }

        public override string ToString()
        {
            return operation + " " + value + " " + 
                (operation == Operation.Set ? "to" : "by") + " " + amount;
        }
    }

}