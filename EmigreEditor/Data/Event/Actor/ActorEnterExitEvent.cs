namespace Emigre.Data
{

    using ObjectEditor.Json;

    [Category("Actor")]
    public class ActorEnterExitEvent : StoryEvent
    {

        public enum Position
        {
            Left, Right, Remove
        }

        public readonly Reference<Character> character = new Reference<Character>();
        public Position position = Position.Left;

        public ActorEnterExitEvent() { }

        public ActorEnterExitEvent(string sprite, Position position)
        {
            this.position = position;
        }

        public override void AddFields(FieldData fields)
        {
            base.AddFields(fields);
            fields.addReference(character, "character");
            position = fields.addEnum(position, "position");
        }

        public override string ToString()
        {
            if (position == Position.Remove) return "<- " + character;
            else if (position == Position.Left) return "[" + character;
            return character + "]";
        }
    }
}