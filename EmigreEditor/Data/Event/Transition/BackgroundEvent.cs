namespace Emigre.Data
{
    using ObjectEditor.Json;

    [Category("Transition")]
    public class BackgroundEvent : StoryEvent
    {
        [FieldTag(FieldTags.Image, Constants.DIR_IMAGES_BG)]
        public string background = "";
        public bool clearTextHistory = true;

        public BackgroundEvent() { }

        public BackgroundEvent(string background)
        {
            this.background = background;
        }

        public override void AddFields(FieldData fields)
        {
            base.AddFields(fields);
            background = fields.add(background, "background");
            clearTextHistory = fields.add(clearTextHistory, "clearTextHistory");
        }

        public override bool cut()
        {
            return clearTextHistory;
        }

        public override string ToString()
        {
            return "Change BG to: " + background;
        }
    }
}