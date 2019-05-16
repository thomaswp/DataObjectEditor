
using ObjectEditor.Json;

namespace Emigre.Data
{
    public enum SpriteTransition
    {
        InAndOut,
        Overlay,
        Crossfade
    }

    [Category("Sprite")]
    public class ChangeSpriteEvent : StoryEvent
    {
        public Reference<Character> character = new Reference<Character>();
        public float duration;
        public SpriteTransition transition;
        public bool waitToAdvance = true;

        [FieldTag(FieldTags.Image, Constants.DIR_IMAGES_POSES)]
        public string image;

        public override void AddFields(FieldData fields)
        {
            base.AddFields(fields);
            fields.addReference(character, "character");
            duration = fields.add(duration, "duration");
            transition = fields.addEnum(transition, SpriteTransition.InAndOut, "transition");
            if (fields.readMode())
            {
                bool crossfade = fields.add(false, "crossfade");
                if (crossfade) transition = SpriteTransition.Crossfade;
            }
            image = fields.add(image, "image");
            waitToAdvance = fields.add(waitToAdvance, "waitToAdvance");
        }

        public override string ToString()
        {
            return string.Format("Change {0}'s sprite to {1}", character, image);
        }

        public override Action GetTransitionAction()
        {
            return waitToAdvance ? Action.NextEvent : Action.Immediately;
        }
    }
}
