
using Emigre.Json;

namespace Emigre.Data
{
    [Category("Transition")]
    public class SleepEvent : StoryEvent
    {
        public bool decision = true;
        public int cost;

        private Outcome compiled;
        
        public override void AddFields(FieldData fields)
        {
            base.AddFields(fields);
            cost = fields.add(cost, "cost");
            decision = fields.add(decision, "decision");
        }

        public override Action GetTransitionAction()
        {
            return decision ? base.GetTransitionAction() : Action.NextEvent;
        }

        public override string ToString()
        {
            return (decision ? "" : "Auto ") + "Sleep ($" + cost + ")";
        }

        public Outcome Compile(int maxTime)
        {
            if (compiled != null) return compiled;

            compiled = new Outcome();

            IfEvent checkTime = new IfEvent();
            compiled.outcomeEvents.Add(checkTime);

            ConditionOutcome tooSoon = new ConditionOutcome();
            checkTime.outcomes.Add(tooSoon);

            ResourceCondition condition = new ResourceCondition();
            condition.resource = Resource.Time;
            condition.comparison = Comparison.LessThan;
            condition.value = maxTime;
            tooSoon.condition1 = condition;

            TextEvent sayTooSoon = new TextEvent();
            sayTooSoon.text = "The day isn't over yet. Come back to sleep when it's night time.";
            tooSoon.outcomeEvents.Add(sayTooSoon);

            EndScenarioEvent end = new EndScenarioEvent();
            tooSoon.outcomeEvents.Add(end);

            TextChoiceEvent choice = new TextChoiceEvent();
            choice.prompt = "Sleep?";
            if (cost != 0) choice.prompt += " (" + cost + " $dollars per night)";
            compiled.outcomeEvents.Add(choice);

            Choice yes = new Choice();
            yes.text = "Yes";
            choice.choices.Add(yes);

            Choice no = new Choice();
            no.text = "No";
            choice.choices.Add(no);

            SleepEvent sleep = new SleepEvent();
            sleep.decision = false;
            sleep.cost = cost;
            yes.outcomeEvents.Add(sleep);

            return compiled;
        }
    }
}
