namespace Emigre.Data
{

    using ObjectEditor.Json;

    [Category("Data")]
    public class ItemEvent : StoryEvent, IReadOnlyScriptable
    {

        public enum ItemAction
        {
            Add, Remove
        }

        public readonly Reference<Item> item = new Reference<Item>();
        public ItemAction action = ItemAction.Add;

        public override void AddFields(FieldData fields)
        {
            base.AddFields(fields);
            fields.addReference(item, "item");
            action = fields.addEnum(action, "action");
        }

        public override string ToString()
        {
            return action + " " + item.ToString();
        }

        public override StoryEvent.Action GetTransitionAction()
        {
            return Action.NextEvent;
        }
    }

}