using System;
using ObjectEditor.Json;

namespace HearthEditor.Data
{
    class Game : GameData, IScriptable
    {
        public string name;

        public static void Load()
        {
            ReflectionConstructor<Game>.Register("Game");
        }

        public override void AddFields(FieldData fields)
        {
            base.AddFields(fields);
            name = fields.add(name, "name");
        }
    }
}
