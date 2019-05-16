using ObjectEditor.Json;

namespace Emigre.Data
{
    [Category("Transition")]
    public class CutsceneEvent : StoryEvent
    {
        [FieldTag(FieldTags.File, Constants.DIR_VIDEO)]
        public string video;

        public override void AddFields(FieldData fields)
        {
            base.AddFields(fields);
            video = fields.add(video, "video");
        }

        public override StoryEvent.Action GetTransitionAction()
        {
 	         return Action.NextEvent;
        }

        public override string ToString()
        {
            return "\u1408 " + video;
        }
    }
}
