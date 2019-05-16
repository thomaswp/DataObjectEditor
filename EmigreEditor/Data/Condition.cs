using Emigre.Json;
using System;

namespace Emigre.Data
{
    public abstract class Condition : GameData
    {
        public static void Register()
        {
            ReflectionConstructor<ConditionOutcome>.Register("ConditionOutcome");

            ReflectionConstructor<VariableCondition>.Register("VariableCondition");
            ReflectionConstructor<ObjectiveCondition>.Register("ObjectiveCondition");
            ReflectionConstructor<RandomCondition>.Register("RandomCondition");
            ReflectionConstructor<ItemCondition>.Register("ItemCondition");
            ReflectionConstructor<ItemCountCondition>.Register("ItemCountCondition");
            ReflectionConstructor<ResourceCondition>.Register("ResourceCondition");
            ReflectionConstructor<LocationCondition>.Register("LocationCondition");
            ReflectionConstructor<CityCondition>.Register("CityCondition");
            ReflectionConstructor<LodgingCondition>.Register("LodgingCondition");
            ReflectionConstructor<SkillCondition>.Register("SkillCondition");
            ReflectionConstructor<ClueCondition>.Register("ClueCondition");
            ReflectionConstructor<ComparisonCondition>.Register("ComparisonCondition");
        }
    }

    public enum Resource
    {
        Money,
        Food,
        Wellbeing,
        SocialCapital,
        Day,
        Time
    }

    public enum Comparison
    {
        Equals, 
        NotEquals, 
        GreaterThan, 
        LessThan, 
        AtLeast, 
        NoMoreThan
    }

    public class VariableCondition : Condition
    {
        public Reference<Variable> variable = new Reference<Variable>();
        public Comparison comparison;
        public int value;

        public override void AddFields(FieldData fields)
        {
            base.AddFields(fields);
            fields.addReference(variable, "varRef");
            comparison = fields.addEnum(comparison, "comparison");
            value = fields.add(value, "value");
        }

        public override string ToString()
        {
            return variable + " " + comparison + " " + value;
        }
    }

    public class ObjectiveCondition : Condition
    {
        public Reference<Objective> objective = new Reference<Objective>();
        public bool invert;
        public Objective.State value;

        public override void AddFields(FieldData fields)
        {
            base.AddFields(fields);
            fields.addReference(objective, "objective");
            invert = fields.add(invert, "invert");
            value = fields.addEnum(value, "value");
        }

        public override string ToString()
        {
            return objective + (invert ? " is not " : " is ") + value;
        }
    }

    public class RandomCondition : Condition
    {
        public float probability = 0.5f;

        public override void AddFields(FieldData fields)
        {
            base.AddFields(fields);
            probability = fields.add(probability, "probability");
        }

        public override string ToString()
        {
            return Math.Round(probability * 100, 2) + "% Chance";
        }
    }

    public class ItemCondition : Condition
    {
        public readonly Reference<Item> item = new Reference<Item>();
        public bool has = true;

        public override void AddFields(FieldData fields)
        {
            base.AddFields(fields);
            fields.addReference(item, "item");
            has = fields.add(has, "has");
        }

        public override string ToString()
        {
            return (has ? "Has " : "Does not have ") + item;
        }
    }

    public class ItemCountCondition : Condition
    {
        public Comparison comparison;
        public int value;

        public override void AddFields(FieldData fields)
        {
            base.AddFields(fields);
            comparison = fields.addEnum(comparison, "comparison");
            value = fields.add(value, "value");
        }

        public override string ToString()
        {
            return "Item count " + comparison + " " + value;
        }
    }

    public class ResourceCondition : Condition
    {
        public Resource resource;
        public Comparison comparison;
        public int value;

        public override void AddFields(FieldData fields)
        {
            base.AddFields(fields);
            resource = fields.addEnum(resource, "resource");
            comparison = fields.addEnum(comparison, "comparison");
            value = fields.add(value, "value");
        }

        public override string ToString()
        {
            return resource + " " + comparison + " " + value;
        }
    }

    public class LocationCondition : Condition
    {
        public readonly Reference<Location> location = new Reference<Location>();
        public bool isInLocation = true;

        public override void AddFields(FieldData fields)
        {
            base.AddFields(fields);
            fields.addReference(location, "location");
            isInLocation = fields.add(isInLocation, "isInLocation");
        }

        public override string ToString()
        {
            return (isInLocation ? "Is" : "Is not") + " in " + location;
        }
    }

    public class CityCondition : Condition
    {
        public readonly Reference<City> city = new Reference<City>();
        public bool isInCity = true;

        public override void AddFields(FieldData fields)
        {
            base.AddFields(fields);
            fields.addReference(city, "city");
            isInCity = fields.add(isInCity, "isInCity");
        }

        public override string ToString()
        {
            return (isInCity ? "Is" : "Is not") + " in " + city;
        }
    }

    public class LodgingCondition : Condition
    {
        public readonly Reference<Location> location = new Reference<Location>();
        public bool isLodging = true;

        public override void AddFields(FieldData fields)
        {
            base.AddFields(fields);
            fields.addReference(location, "location");
            isLodging = fields.add(isLodging, "isLodging");
        }
    }

    public class SkillCondition : Condition
    {
        public readonly Reference<Skill> skill = new Reference<Skill>();
        public Comparison comparison;
        public int value;
        
        public override void AddFields(FieldData fields)
        {
            base.AddFields(fields);
            fields.addReference(skill, "skill");
            comparison = fields.addEnum(comparison, "comparison");
            value = fields.add(value, "value");
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }

    public class ClueCondition : Condition
    {
        public readonly Reference<Clue> clue = new Reference<Clue>();
        public bool isFound;

        public override void AddFields(FieldData fields)
        {
            base.AddFields(fields);
            fields.addReference(clue, "clue");
            isFound = fields.add(isFound, "isFound");
        }

        public override string ToString()
        {
            return clue + " is " + (isFound ? "" : "not") + " found";
        }
    }

    public class ComparisonCondition : Condition
    {
        [FieldTag(FieldTags.Inline)]
        public GameValue value1;
        public Comparison comparison;
        [FieldTag(FieldTags.Inline)]
        public GameValue value2;

        public override void AddFields(FieldData fields)
        {
            base.AddFields(fields);
            value1 = fields.add(value1, "value1");
            comparison = fields.addEnum(comparison, "comparison");
            value2 = fields.add(value2, "value2");
        }

        public override string ToString()
        {
            return value1 + " " + comparison + " " + value2;
        }
    }
}
