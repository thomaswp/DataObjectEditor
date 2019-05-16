using Emigre.Json;
using System;

namespace Emigre.Data
{
    public abstract class GameData : GuidDataObject, IIgnorable
    {
        public Guid Guid
        {
            get;
            private set;
        }

        [FieldTag(FieldTags.Hide)]
        public bool Ignored
        {
            get;
            set;
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
            if (fields.readMode() || Ignored) Ignored = fields.add(Ignored, "Ignored");
        }
        
        public static void Register<T>(string key = null) where T : GameData
        {
            ReflectionConstructor<T>.Register(key);
        }
    }
}
