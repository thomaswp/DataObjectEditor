﻿using ObjectEditor.Json;
using ObjectEditor.Editor.Reflect;
using System;
using System.Linq;

namespace HearthEditor.Data
{
    public abstract class GameData : GuidDataObject
    {
        public Guid Guid
        {
            get; set;
        }

        Guid GuidDataObject.GetGuid()
        {
            return Guid;
        }

        public GameData()
        {
            Guid = Guid.NewGuid();
        }

        public virtual void AddFields(FieldData fields)
        {
            ReflectionFieldAdder.addFields(this, fields);
        }
        
        public static void Register<T>(string key = null) where T : GameData
        {
            ReflectionConstructor<T>.Register(key);
        }

        public static void LoadAll()
        {
            string ns = typeof(GameData).Namespace;
            var allTypes = AppDomain.CurrentDomain.GetAssemblies()
                            .SelectMany(t => t.GetTypes())
                            .Where(t => t.IsClass && t.IsSubclassOf(typeof(GameData)) && t.Namespace == ns);
            foreach (Type type in allTypes)
            {
                new NoArgsTypeConstructor(type).Register(type);
                //Console.WriteLine(type.Name);
            }
        }
    }

    class NoArgsTypeConstructor : Constructor
    {
        Type type;

        public NoArgsTypeConstructor(Type type)
        {
            if (!typeof(DataObject).IsAssignableFrom(type))
            {
                throw new Exception("Type must be a subclass of DataObject");
            }
            this.type = type;
        }

        public override DataObject Construct()
        {
            return (DataObject)Activator.CreateInstance(type);
        }
    }
}
