using Emigre.Json;
using System.Collections.Generic;
using System;

namespace Emigre.Data
{
    public class Location : GameData, IEnableable, IHasStatus, IScriptable
    {
        [FieldTag(FieldTags.Title, "#")]
        public string name = "Location";
        public bool discovered;
        public string description;
        [FieldTag(FieldTags.Image, Constants.DIR_IMAGES_BG)]
        public string background;
        [FieldTag(FieldTags.Image, Constants.DIR_IMAGES_ICONS)]
        public string icon;
        public MapPoint mapPoint = new MapPoint();
        public readonly List<LocationScenario> scenarios = new List<LocationScenario>();
        public readonly List<Actor> actors = new List<Actor>();

        static Location()
        {
            ReflectionConstructor<MapPoint>.Register("MapPoint");
        }

        public override void AddFields(FieldData fields)
        {
            base.AddFields(fields);
            name = fields.add(name, "name");
            discovered = fields.add(discovered, "discovered");
            description = fields.add(description, "description");
            background = fields.add(background, "background");
            icon = fields.add(icon, "icon");
            mapPoint = fields.add(mapPoint, "mapPoint");
            fields.addList(scenarios, "scenarios");
            fields.addList(actors, "actors");
        }

        public override string ToString()
        {
            return name;
        }

        public bool GetDefaultEnabled()
        {
            return discovered;
        }

        public class MapPoint : Point2D
        {
            public readonly Reference<Location> location = new Reference<Location>();

            public override void AddFields(FieldData fields)
            {
                base.AddFields(fields);
                fields.addReference(location, "location");
            }
        }

        public HighlightStatus DefaultStatus()
        {
            return HighlightStatus.HighlightOnce;
        }
    }
}
