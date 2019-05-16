using System;

namespace ObjectEditor.Json
{
    public interface GuidDataObject : DataObject
    {
        Guid GetGuid();
    }
}
