namespace Emigre.Data
{
    using ObjectEditor.Json;

    [Category("Data")]
    public class SetVariableEvent : StoryEvent
    {
        public readonly Reference<Variable> variable = new Reference<Variable>();
        public SetValueEvent.Operation operation;
        [FieldTag(FieldTags.Inline)]
        public GameValue value = new NumberValue();

        public override void AddFields(FieldData fields)
        {
            base.AddFields(fields);
            fields.addReference(variable, "varRef");
            operation = fields.addEnum(operation, "operation");
            value = fields.add(value, "gameValue");
            if (fields.readMode())
            {
                int v = fields.add(0, "value");
                if (v != 0)
                {
                    NumberValue nv = new NumberValue();
                    nv.value = v;
                    value = nv;
                }
            }
        }

        public override string ToString()
        {
            return operation + " " + variable + " " +
                (operation == SetValueEvent.Operation.Set ? "to" : "by") + " " + value;
        }

    }

}