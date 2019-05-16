
using Emigre.Json;

namespace Emigre.Data
{
    [Category("Data")]
    public class SkillLevelUpEvent : StoryEvent
    {
        public readonly Reference<Skill> skill = new Reference<Skill>();
        public int levels = 1;

        public override void AddFields(FieldData fields)
        {
            base.AddFields(fields);
            fields.addReference(skill, "skill");
            levels = fields.add(levels, "levels");
        }

        public override string ToString()
        {
            return "Level up " + skill + " (" + levels + "x)";
        }

        public override Action GetTransitionAction()
        {
            return Action.NextEvent;
        }
    }
}
