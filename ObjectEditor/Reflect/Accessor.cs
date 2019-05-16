using ObjectEditor.Json;
using System;
using System.Collections.Generic;

namespace ObjectEditor.Editor.Reflect
{
    public interface Accessor
    {
        bool IsReadOnly();
        object Get();
        void Set(object value);
        Type GetAccessorType();
        string GetName();
        IEnumerable<FieldTag> GetTags();
    }
}
