namespace Emigre.Data
{

    using ObjectEditor.Json;

    [Category("Control")]
    public class ImageChoiceEvent : ChoiceEvent<ImageChoiceEvent.ImageChoice>
    {

        static ImageChoiceEvent()
        {
            ReflectionConstructor<ImageChoice>.Register("ImageChoice");
        }

        public class ImageChoice : Choice
        {
            [FieldTag(FieldTags.Image, Constants.DIR_IMAGES_PORTRAITS)]
            public string image;

            public override void AddFields(FieldData fields)
            {
                base.AddFields(fields);
                image = fields.add(image, "image");
            }
        }
    }

}