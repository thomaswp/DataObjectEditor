using ObjectEditor.Json;
using System.Collections.Generic;
using System;

namespace Emigre.Data
{
    public class City : GameData, IScriptable
    {
        [FieldTag(FieldTags.Title, "#")]
        public string name = "City";
        public string country = "Country";

        [FieldTag(FieldTags.Readonly)]
        [FieldTag(FieldTags.Refresh)]
        public string FullName { get { return name + ", " + country; } }

        [FieldTag(FieldTags.Multiline)]
        public string description;

        [FieldTag(FieldTags.Image, Constants.DIR_IMAGES_MAPS)]
        public string map;

        public readonly Reference<Location> startLocation = new Reference<Location>();
        public readonly Reference<Location> startLodging = new Reference<Location>();

        [FieldTag(FieldTags.Inline)]
        public TestResources testResources;

        public readonly List<Location> locations = new List<Location>();
        public readonly List<Character> characters = new List<Character>();
        public readonly List<Variable> variables = new List<Variable>();
        public readonly List<Objective> objectives = new List<Objective>();
        public readonly List<GlobalEvent> cityEvents = new List<GlobalEvent>();
        public readonly List<Clue> clues = new List<Clue>();

        public override void AddFields(FieldData fields)
        {
            base.AddFields(fields);
            name = fields.add(name, "name");
            country = fields.add(country, "country");
            description = fields.add(description, "description");
            map = fields.add(map, "map");
            fields.addReference(startLocation, "startLocation");
            fields.addReference(startLodging, "startLodging");
            testResources = fields.add(testResources, "testResources");
            fields.addList(locations, "locations");
            fields.addList(characters, "characters");
            fields.addList(variables, "variables");
            fields.addList(objectives, "objectives");
            fields.addList(cityEvents, "cityEvents");
            fields.addList(clues, "clues");
        }

        public override string ToString()
        {
            return name;
        }
    }
}
