using System;
using ObjectEditor.Json;
using System.Linq;
using System.Collections.Generic;

namespace HearthEditor.Data
{
    class Game : GameData, IScriptable
    {
        public string name;
        public readonly List<Resource> resources = new List<Resource>();
        public readonly List<Building> buildings = new List<Building>();

        public override void AddFields(FieldData fields)
        {
            base.AddFields(fields);
            name = fields.add(name, "name");
            fields.addList(resources, "resources");
            fields.addList(buildings, "buildings");
        }
    }
}
