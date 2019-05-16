

using ObjectEditor.Json;

namespace Emigre.Data
{
    [Category("Transition")]
    public class LoadSceneEvent : StoryEvent
    {
        public string scene;
        public float duration;

        public override void AddFields(FieldData fields)
        {
            base.AddFields(fields);
            scene = fields.add(scene, "scene");
            duration = fields.add(duration, "duration");
        }

        public override string ToString()
        {
            return "Load " + scene;
        }
    }
}
