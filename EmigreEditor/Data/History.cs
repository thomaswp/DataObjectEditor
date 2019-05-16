using ObjectEditor.Json;

namespace Emigre.Data
{
    public class History : GameData, IHasPortrait
    {
        public string name = "History";
        [FieldTag(FieldTags.Image, Constants.DIR_IMAGES_PORTRAITS)]
        public string image;

        [FieldTag(FieldTags.Multiline)]
        public string description;

        public override void AddFields(FieldData fields)
        {
            base.AddFields(fields);
            image = fields.add(image, "image");
            name = fields.add(name, "name");
            description = fields.add(description, "description");
        }

        public override string ToString()
        {
            return name;
        }

        public string GetIcon()
        {
            return image;
        }
    }
}
