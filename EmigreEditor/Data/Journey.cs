using ObjectEditor.Json;
using System.Collections.Generic;

namespace Emigre.Data
{
    public class Journey : GameData
    {
        public readonly Reference<City> from = new Reference<City>();
        public readonly Reference<City> to = new Reference<City>();

        [FieldTag(FieldTags.Comment, "Duration of the journey for the player, in milliseconds")]
        public int durationMS;
        [FieldTag(FieldTags.Comment, "Numer of days the journey will take the player")]
        public int durationDays;
        [FieldTag(FieldTags.Comment, "Numer of additional time increments the journey will take the player")]
        public int durationTime;

        [FieldTag(FieldTags.Multiline)]
        public string description;

        public JourneyPath path = new JourneyPath();

        [FieldTag(FieldTags.Inline)]
        public TestResources testResources;

        public readonly List<JourneyScenario> scenarios = new List<JourneyScenario>();
        public readonly List<Character> characters = new List<Character>();
        public readonly List<Variable> variables = new List<Variable>();

        static Journey()
        {
            ReflectionConstructor<JourneyPath>.Register("JourneyPath");
        }

        public override void AddFields(FieldData fields)
        {
            base.AddFields(fields);
            fields.addReference(from, "from");
            fields.addReference(to, "to");
            durationMS = fields.add(durationMS, "durationMS");
            durationDays = fields.add(durationDays, "durationDays");
            durationTime = fields.add(durationTime, "durationTime");
            description = fields.add(description, "description");
            path = fields.add(path, "path");
            testResources = fields.add(testResources, "testResources");
            fields.addList(scenarios, "scenarios");
            fields.addList(characters, "characters");
            fields.addList(variables, "variables");
        }

        public override string ToString()
        {
            return from + " -> " + to;
        }
    }

    public class JourneyPath : DataObject
    {
        public string map;
        public readonly List<Point2D> points = new List<Point2D>();

        public void AddFields(FieldData fields)
        {
            map = fields.add(map, "map");
            fields.addList(points, "points");
        }

        public void Bound(int xMin, int yMin, int xMax, int yMax)
        {
            for (int i = 0; i < points.Count; i++)
            {
                points[i] = points[i].Bound(xMin, yMin, xMax, yMax);
            }
        }
    }
}
