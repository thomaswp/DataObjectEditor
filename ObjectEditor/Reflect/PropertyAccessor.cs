using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ObjectEditor.Json;

namespace ObjectEditor.Editor.Reflect
{
    public class PropertyAccessor : Accessor
    {
        private readonly PropertyInfo info;
        private readonly object obj;

        public PropertyAccessor(object obj, PropertyInfo info)
        {
            this.obj = obj;
            this.info = info;
        }

        public static List<Accessor> GetForObject(object obj)
        {
            Type t = obj.GetType();
            List<Accessor> accessors = new List<Accessor>();
            while (t != null)
            {
                PropertyInfo[] infos = t.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                var enabled = infos.Where((info) => 
                    {
                        return !info.GetCustomAttributes(typeof(ObsoleteAttribute), false).Any() &&
                            info.GetIndexParameters().Length == 0 &&
                            info.GetGetMethod() != null;
                    });
                accessors.InsertRange(0, enabled.Select((info) => new PropertyAccessor(obj, info)));
                t = t.BaseType;
            }

            return accessors;
        }

        public IEnumerable<FieldTag> GetTags()
        {
            return info.GetCustomAttributes(typeof(FieldTag), false).Cast<FieldTag>();
        }

        public bool HasFlag(FieldTags flag)
        {
            var attributes = info.GetCustomAttributes(typeof(FieldTag), false);
            foreach (FieldTag tag in attributes)
            {
                if (tag.flag.HasFlag(flag)) return true;
            }
            return false;
        }

        public bool IsReadOnly()
        {
            return info.GetSetMethod() == null;
        }

        public object Get()
        {
            return info.GetValue(obj, null);
        }

        public void Set(object value)
        {
            if (IsReadOnly()) return;
            info.SetValue(obj, value, null);
        }

        public string GetName()
        {
            return info.Name;
        }


        public Type GetAccessorType()
        {
            return info.PropertyType;
        }
    }
}
