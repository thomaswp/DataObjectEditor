
using Emigre.Json;

namespace Emigre.Data
{
    public abstract class GameValue : DataObject
    {
        public static void Register()
        {
            ReflectionConstructor<ResourceValue>.Register("ResourceValue");
            ReflectionConstructor<NumberValue>.Register("NumberValue");
            ReflectionConstructor<VariableValue>.Register("VariableValue");
            ReflectionConstructor<ItemCountValue>.Register("ItemCountValue");
            ReflectionConstructor<RandomValue>.Register("RandomValue");
            ReflectionConstructor<SkillValue>.Register("SkillValue");
            ReflectionConstructor<CluesValue>.Register("CluesValue");
        }

        public virtual void AddFields(FieldData fields) { }
    }

    public abstract class SettableGameValue : GameValue
    {

    }

    public class ResourceValue : SettableGameValue
    {
        public Resource resource;

        public override void AddFields(FieldData fields)
        {
            base.AddFields(fields);
            resource = fields.addEnum(resource, "resource");
        }

        public override string ToString()
        {
            return "" + resource;
        }
    }

    public class NumberValue : GameValue
    {
        public int value;
        
        public override void AddFields(FieldData fields)
        {
            base.AddFields(fields);
            value = fields.add(value, "value");
        }

        public override string ToString()
        {
            return "" + value;
        }
    }

    public class VariableValue : SettableGameValue
    {
        public readonly Reference<Variable> variable = new Reference<Variable>();

        public override void AddFields(FieldData fields)
        {
            base.AddFields(fields);
            fields.addReference(variable, "variable");
        }

        public override string ToString()
        {
            return variable.ToString();
        }
    }

    public class ItemCountValue : GameValue
    {
        public override string ToString()
        {
            return "Item Count";
        }
    }

    public class RandomValue : GameValue
    {
        public int min, max;

        public override void AddFields(FieldData fields)
        {
            base.AddFields(fields);
            min = fields.add(min, "min");
            max = fields.add(max, "max");
        }
    }

    public class SkillValue : SettableGameValue
    {
        public readonly Reference<Skill> skill = new Reference<Skill>();

        public override void AddFields(FieldData fields)
        {
            base.AddFields(fields);
            fields.addReference(skill, "skill");
        }

        public override string ToString()
        {
            return skill.ToString();
        }
    }

    public class CluesValue : GameValue
    {
        public bool found;
        public readonly Reference<City> inCity = new Reference<City>();

        public override void AddFields(FieldData fields)
        {
            base.AddFields(fields);
            fields.addReference(inCity, "inCity");
            found = fields.add(found, "found");
        }

        public override string ToString()
        {
            return "Number of clues " + (found ? "found" : "not found") + " in " + inCity;
        }
    }
}
