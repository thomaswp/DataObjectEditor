namespace ObjectEditor.Json
{
    public enum FieldTags
    {
        None,
        Multiline,
        Image,
        File,
        Readonly,
        Refresh,
        Inline,
        Comment,
        Title,
        Hide
    }

    [System.AttributeUsage(System.AttributeTargets.Field | System.AttributeTargets.Property, AllowMultiple = true)]
    public class FieldTag : System.Attribute
    {
        public FieldTags flag;
        public string arg;

        public FieldTag(FieldTags tag, string arg = null)
        {
            this.flag = tag;
            this.arg = arg;
        }
    }

    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class Category : System.Attribute
    {
        public string[] categories;

        public Category(params string[] name)
        {
            this.categories = name;
        }
    }
}
