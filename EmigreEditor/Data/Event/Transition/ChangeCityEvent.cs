namespace Emigre.Data
{

    using ObjectEditor.Json;

    [Category("Transition")]
    public class ChangeCityEvent : StoryEvent
    {
        public Reference<City> city = new Reference<City>();
        public bool slowFade;

        public override void AddFields(FieldData fields)
        {
            base.AddFields(fields);
            fields.addReference(city, "city");
            slowFade = fields.add(slowFade, "slowFade");
        }

        public override string ToString()
        {
            return "Go to " + city;
        }
    }
}
