using ObjectEditor.Json;
using System;

namespace HearthEditor.Data
{
    public abstract class GameData : GuidDataObject
    {
        public Guid Guid
        {
            get;
            private set;
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
            Guid = fields.addGuid(Guid, "Guid");
        }
        
        public static void Register<T>(string key = null) where T : GameData
        {
            ReflectionConstructor<T>.Register(key);
        }
    }
}
