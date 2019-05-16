using System;
using ObjectEditor.Json;

namespace Emigre.Data
{
    [Category("Text", "Control")]
    public class ReflectionConstructor<T> : Constructor where T : DataObject
    {
        public override DataObject Construct()
        {
            return Activator.CreateInstance<T>();
        }

        public static Constructor Register(string key = null)
        {
            if (key == null) key = typeof(T).Name;
            return new ReflectionConstructor<T>().Register(typeof(T), key);
        }
    }
}
