using Emigre.Json;
using System.Collections.Generic;
using System;

namespace Emigre.Data
{
    public class Story : GameData, IScriptable
    {
        [FieldTag(FieldTags.Title, "#")]
        public string name;
        public readonly Reference<City> startCity = new Reference<City>();
                
        [FieldTag(FieldTags.Comment, "Sets a temporary starting City for testing purposes.")]
        public readonly Reference<City> testCity = new Reference<City>();
        [FieldTag(FieldTags.Comment, "Sets a temporary starting Journey for testing purposes.")]
        public readonly Reference<Journey> testJourney = new Reference<Journey>();

        [FieldTag(FieldTags.Multiline)]
        public string moneyDescription, socialCapitalDescription, wellbeingDescription, itemsDescription;

        public readonly List<Item> items = new List<Item>();
        public readonly List<History> histories = new List<History>();
        public readonly List<City> cities = new List<City>();
        public readonly List<Journey> journeys = new List<Journey>();
        public readonly List<Character> characters = new List<Character>();
        public readonly List<Variable> variables = new List<Variable>();
        public readonly List<Objective> objectives = new List<Objective>();
        public readonly List<GlobalEvent> storyEvents = new List<GlobalEvent>();
        public readonly List<Skill> skills = new List<Skill>();
        public readonly List<Alignment> alignments = new List<Alignment>();

        public static void Load()
        {
            ReflectionConstructor<Story>.Register("Story");
            ReflectionConstructor<Character>.Register("Character");
            ReflectionConstructor<Item>.Register("Item");
            ReflectionConstructor<History>.Register("History");
            ReflectionConstructor<Journey>.Register("Journey");
            ReflectionConstructor<Variable>.Register("Variable");
            ReflectionConstructor<City>.Register("City");
            ReflectionConstructor<Location>.Register("Location");
            ReflectionConstructor<LocationScenario>.Register("Scene");
            ReflectionConstructor<LocationScenario>.Register("LocationScenario");
            ReflectionConstructor<JourneyScenario>.Register("JourneyScenario");
            ReflectionConstructor<Page>.Register("Page");
            ReflectionConstructor<Actor>.Register("ScenarioActor");
            ReflectionConstructor<Objective>.Register("Objective");
            ReflectionConstructor<Alignment>.Register("Alignment");
            ReflectionConstructor<Point2D>.Register("Point2D");
            ReflectionConstructor<ConditionOutcome>.Register("Condition");
            ReflectionConstructor<ConditionOutcome>.Register("ConditionOutcome");
            ReflectionConstructor<GlobalEvent>.Register("GlobalEvent");
            ReflectionConstructor<TestResources>.Register("TestResources");
            ReflectionConstructor<Skill>.Register("Skill");
            ReflectionConstructor<Clue>.Register("Clue");
            Condition.Register();
            GameValue.Register();
            StoryEvent.Load();
        }

        public override void AddFields(FieldData fields)
        {
            base.AddFields(fields);
            name = fields.add(name, "name");
            fields.addReference(startCity, "startCity");
            fields.addReference(testCity, "testCity");
            fields.addReference(testJourney, "testJourney");
            moneyDescription = fields.add(moneyDescription, "moneyDescription");
            wellbeingDescription = fields.add(wellbeingDescription, "wellbeingDescription");
            socialCapitalDescription = fields.add(socialCapitalDescription, "socialCapitalDescription");
            itemsDescription = fields.add(itemsDescription, "itemsDescription");
            fields.addList(items, "items");
            fields.addList(histories, "histories");
            fields.addList(characters, "characters");
            fields.addList(variables, "variables");
            fields.addList(objectives, "objectives");
            fields.addList(alignments, "alignments");
            fields.addList(journeys, "journeys");
            fields.addList(storyEvents, "storyEvents");
            fields.addList(cities, "cities");
            fields.addList(skills, "skills");
        }

        public override string ToString()
        {
            return name;
        }
    }
}
