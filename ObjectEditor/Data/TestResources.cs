using Emigre.Json;

namespace Emigre.Data
{
    public class TestResources : GameData
    {
        public int day;
        public int money;
        public int food;
        public int wellbeing;
        public int socialCapital;

        public readonly Reference<Item> item1 = new Reference<Item>();
        public readonly Reference<Item> item2 = new Reference<Item>();
        public readonly Reference<Item> item3 = new Reference<Item>();
        public readonly Reference<Item> item4 = new Reference<Item>();
        public readonly Reference<Item> item5 = new Reference<Item>();

        public override void AddFields(Json.FieldData fields)
        {
            base.AddFields(fields);
            day = fields.add(day, "day");
            money = fields.add(money, "money");
            food = fields.add(food, "food");
            wellbeing = fields.add(wellbeing, "wellbeing");
            socialCapital = fields.add(socialCapital, "socialCapital");

            fields.addReference(item1, "item1");
            fields.addReference(item2, "item2");
            fields.addReference(item3, "item3");
            fields.addReference(item4, "item4");
            fields.addReference(item5, "item5");
        }
    }
}
