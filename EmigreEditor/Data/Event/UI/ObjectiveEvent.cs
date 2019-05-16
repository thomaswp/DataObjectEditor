namespace Emigre.Data
{

    using ObjectEditor.Json;

    [Category("UI")]
    public class ObjectiveEvent : AbstractOverlayEvent, IReadOnlyScriptable
    {

        public readonly Reference<Objective> objective = new Reference<Objective>();
        public Objective.State state;

        public override void AddFields(FieldData fields)
        {
            base.AddFields(fields);
            fields.addReference(objective, "objective");
            state = fields.addEnum(state, "state");
        }

        public override string ToString()
        {
            return state + " Objective: " + objective;
        }
    }

}