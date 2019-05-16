using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Emigre.Json;

namespace Emigre.Editor.Reflect
{
    public class FieldAccessor : Accessor
    {

        private readonly FieldInfo info;
        private readonly object obj;

        public FieldAccessor(object obj, FieldInfo info)
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
                FieldInfo[] infos = t.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                var enabled = infos.Where((info) => !info.GetCustomAttributes(typeof(ObsoleteAttribute), false).Any());
                accessors.InsertRange(0, enabled.Select((info) => new FieldAccessor(obj, info)));
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

        public object Get()
        {
            return info.GetValue(obj);
        }

        public void Set(object value)
        {
            info.SetValue(obj, value);
        }

        public string GetName()
        {
            return info.Name;
        }


        public Type GetAccessorType()
        {
            return info.FieldType;
        }

        public bool IsReadOnly()
        {
            return info.IsInitOnly;
        }
    }
}
