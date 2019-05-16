using Emigre.Json;

namespace Emigre.Data
{
    [Category("Control")]
    public class EnableScenarioEvent : StoryEvent
    {
        public Reference<Scenario> scenario = new Reference<Scenario>();
        public bool enabled;

        public override void AddFields(Json.FieldData fields)
        {
            base.AddFields(fields);
            fields.addReference(scenario, "scenario");
            enabled = fields.add(enabled, "enabled");
        }

        public override string ToString()
        {
            return (enabled ? "Enable " : "Disable ") + scenario;
        }
    }
}
