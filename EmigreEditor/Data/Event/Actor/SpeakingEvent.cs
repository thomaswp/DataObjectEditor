namespace Emigre.Data
{
    using ObjectEditor.Json;

    public abstract class SpeakingEvent : StoryEvent
    {
        public readonly Reference<Character> speaker = new Reference<Character>();

        public override void AddFields(FieldData fields)
        {
            base.AddFields(fields);
            fields.addReference(speaker, "speaker");
        }

        public override StoryEvent.Action GetTransitionAction()
        {
            return Action.NextEvent;
        }
    }
}
