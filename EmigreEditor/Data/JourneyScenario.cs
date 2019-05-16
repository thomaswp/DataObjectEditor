using Emigre.Json;

namespace Emigre.Data
{
    public class JourneyScenario : Scenario
    {
        [FieldTag(FieldTags.Comment, "When the scenario occurs [0-1]")]
        public float timing;
        [FieldTag(FieldTags.Comment, "The variance allowed in the timing [0-1]")]
        public float range;
        [FieldTag(FieldTags.Comment, "The probability the even will occur [0-1]")]
        public float probablility = 1; // TODO: implement

        public JourneyScenario()
        {
            this.disableWhenDone = true;
        }

        public override void AddFields(Json.FieldData fields)
        {
            base.AddFields(fields);
            timing = fields.add(timing, "timing");
            range = fields.add(range, "range");
            probablility = fields.add(probablility, "probablility");
        }
    }
}
