using ObjectEditor.Json;

namespace Emigre.Data
{
    public class Variable : GameData
    {
        public string name = "var";
        public int startValue;
        [FieldTag(FieldTags.Multiline)]
        public string comment;

        public override void AddFields(FieldData fields)
        {
            base.AddFields(fields);
            name = fields.add(name, "name");
            startValue = fields.add(startValue, "startValue");
            comment = fields.add(comment, "comment");
        }

        public override string ToString()
        {
            return name;
        }
    }
}
