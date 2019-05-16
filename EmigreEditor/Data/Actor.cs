using ObjectEditor.Json;

namespace Emigre.Data
{
    public class Actor : GameData
    {
        public readonly Reference<Character> character = new Reference<Character>();
        public bool rightSide;

        public override void AddFields(FieldData fields)
        {
            base.AddFields(fields);
            fields.addReference(character, "character");
            rightSide = fields.add(rightSide, "rightSide");
        }

        public override string ToString()
        {
            string s = character.ToString();
            if (rightSide) s += "]";
            else s = "[" + s;
            return s;
        }
    }
}
