using ObjectEditor.Json;

namespace Emigre.Data
{
    public class Character : GameData, IHasPortrait
    {
        public string name = "Character";
        [FieldTag(FieldTags.Image, Constants.DIR_IMAGES_CHARACTERS)]
        public string image;
        [FieldTag(FieldTags.Image, Constants.DIR_IMAGES_PORTRAITS)]
        public string portrait;

        [FieldTag(FieldTags.Multiline)]
        public string description;

        public override void AddFields(FieldData fields)
        {
            base.AddFields(fields);
            image = fields.add(image, "image");
            portrait = fields.add(portrait, "portrait");
            name = fields.add(name, "name");
            description = fields.add(description, "description");
        }

        public override string ToString()
        {
            return name;
        }

        public string GetIcon()
        {
            return portrait;
        }
    }
}
