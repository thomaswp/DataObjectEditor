
using ObjectEditor.Json;

namespace Emigre.Data
{
    [Category("Data")]
    public class FindClueEvent : StoryEvent
    {
        public readonly Reference<Clue> clue = new Reference<Clue>();

        public override void AddFields(FieldData fields)
        {
            base.AddFields(fields);
            fields.addReference(clue, "clue");
        }

        public override string ToString()
        {
            return "Find " + clue;
        }

        public override Action GetTransitionAction()
        {
            return Action.NextEvent;
        }
    }
}
