
using Emigre.Json;

namespace Emigre.Data
{
    [Category("Sprite")]
    public class FadeSpriteEvent : StoryEvent
    {
        public Reference<Character> character = new Reference<Character>();
        public float duration;
        public float alpha;

        public override void AddFields(FieldData fields)
        {
            base.AddFields(fields);
            fields.addReference(character, "character");
            duration = fields.add(duration, "duration");
        }

        public override string ToString()
        {
            return string.Format("Fade {0} to {1} alpha over {2}s", character, alpha, duration);
        }

        public override Action GetTransitionAction()
        {
            return Action.NextEvent;
        }
    }
}
