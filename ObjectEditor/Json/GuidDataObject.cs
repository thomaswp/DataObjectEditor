using System;

namespace Emigre.Json
{
    public interface GuidDataObject : DataObject
    {
        Guid GetGuid();
    }
}
