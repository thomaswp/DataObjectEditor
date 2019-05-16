namespace Emigre.Json
{
    using System;
    using System.Collections.Generic;

    public interface DataObject
    {
        void AddFields(FieldData fields);
    }

    public abstract class Constructor
    {
        private static Dictionary<String, Constructor> constructorMap =
                new Dictionary<String, Constructor>();
        private static Dictionary<Type, String> classMap =
                new Dictionary<Type, String>();

        public static IEnumerable<Type> RegisteredTypes
        {
            get
            {
                return classMap.Keys;
            }
        }

        public abstract DataObject Construct();

        static Constructor()
        {
            new DataObjectList.Constructor().Register(typeof(DataObjectList), "DataObjectList");
        }

        public static void Register(Type clazz, Constructor constructor)
        {
            Register(clazz, clazz.Name, constructor);
        }

        public static void Register(Type clazz, String key, Constructor constructor)
        {
            constructorMap[key] = constructor;
            classMap[clazz] = key;
        }

        public static DataObject Construct(Type clazz)
        {
            return Construct(classMap[clazz]);
        }

        public static DataObject Construct(String key, bool quietFail = false)
        {
            if (!constructorMap.ContainsKey(key))
            {
                if (quietFail) return null;
                throw new ParseDataException("No constructor for type: " + key);
            }
            Constructor constructor = constructorMap[key];
            return constructor.Construct();
        }

        public static String ClassName(Type clazz)
        {
            if (clazz == null) return "null";
            return classMap[clazz];
        }

        public Constructor Register(Type clazz, String key)
        {
            Register(clazz, key, this);
            return this;
        }

        public Constructor Register(Type clazz)
        {
            Register(clazz, this);
            return this;
        }

        public void Load() { }
    }

}