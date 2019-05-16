using System.Collections.Generic;

namespace Emigre.Json
{
    public class DataObjectList : List<DataObject>, DataObject
    {
        public void AddFields(FieldData fields)
        {
            fields.addList(this, "objects");
        }

        public class Constructor : Json.Constructor
        {
            public override DataObject Construct()
            {
                return new DataObjectList();
            }
        }
    }
}
