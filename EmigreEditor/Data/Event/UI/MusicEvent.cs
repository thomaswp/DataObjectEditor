namespace Emigre.Data
{

    using ObjectEditor.Json;

    [Category("UI")]
    public class MusicEvent : StoryEvent
    {
        [FieldTag(FieldTags.File, Constants.DIR_SOUND_BG)]
        public string music;

        public override void AddFields(FieldData fields)
        {
            base.AddFields(fields);
            music = fields.add(music, "music");
        }

        public override string ToString()
        {
            return "\u266B " + music;
        }
    }
}