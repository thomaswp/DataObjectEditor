using Emigre.Json;

namespace Emigre.Data
{
    public class LocationScenario : Scenario, IHasStatus
    {
        public bool autostart;
        public bool dimWhenDone;
        public readonly Reference<IHasPortrait> reference = new Reference<IHasPortrait>();

        [FieldTag(FieldTags.Readonly)]
        [FieldTag(FieldTags.Refresh)]
        [FieldTag(FieldTags.Image, Constants.DIR_IMAGES_PORTRAITS)]
        public string Portrait
        {
            get
            {
                IHasPortrait reference = this.reference.Value;
                return reference == null ? null : reference.GetIcon();
            }
        }

        public override void AddFields(Json.FieldData fields)
        {
            base.AddFields(fields);
            autostart = fields.add(autostart, "autostart");
            dimWhenDone = fields.add(dimWhenDone, "dimWhenDone");
            fields.addReference(reference, "character");
        }

        public HighlightStatus DefaultStatus()
        {
            return dimWhenDone ? HighlightStatus.HighlightOnce :
                HighlightStatus.Highlight;
        }
    }
}
