using Emigre.Json;

namespace Emigre.Data
{
    [Category("Data")]
    public class AlignmentEvent : StoryEvent
    {
        public Reference<Alignment> alignmnet = new Reference<Alignment>();
        public int shift;

        public override void AddFields(FieldData fields)
        {
            base.AddFields(fields);
            fields.addReference(alignmnet, "alignment");
            shift = fields.add(shift, "shift");
        }

        public override string ToString()
        {
            return shift + " to " + alignmnet;
        }
    }
}
