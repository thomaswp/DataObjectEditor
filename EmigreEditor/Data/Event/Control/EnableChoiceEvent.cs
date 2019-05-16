namespace Emigre.Data
{

    using Json;

    [Category("Control", "Text")]
    public class EnableChoiceEvent : StoryEvent
    {

        public readonly Reference<Choice> choice = new Reference<Choice>();
        public bool enable;

        public override void AddFields(FieldData fields)
        {
            base.AddFields(fields);
            fields.addReference(choice, "choiceRef");
            enable = fields.add(enable, "enable");
        }

        public override string ToString()
        {
            return (enable ? "Enable " : "Disable ") + choice;
        }
    }

}