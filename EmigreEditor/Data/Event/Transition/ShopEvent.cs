using ObjectEditor.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Emigre.Data
{
    [Category("Transition")]
    public class ShopEvent : SpeakingEvent, IReadOnlyScriptable
    {
        public readonly Reference<Character> buyer = new Reference<Character>(); 

        [FieldTag(FieldTags.Multiline)]
        public string intro;
        public string thanks;

        public readonly List<Trade> buying = new List<Trade>();
        public readonly List<Trade> selling = new List<Trade>();

        private ShopOutcome compiled;

        static ShopEvent()
        {
            ReflectionConstructor<ResourceGood>.Register("ResourceGood");
            ReflectionConstructor<ItemGood>.Register("ItemGood");
            ReflectionConstructor<Trade>.Register("Trade");
        }

        public override void AddFields(ObjectEditor.Json.FieldData fields)
        {
            base.AddFields(fields);
            fields.addReference(buyer, "buyer");
            intro = fields.add(intro, "intro");
            thanks = fields.add(thanks, "thanks");
            fields.addList(buying, "buying");
            fields.addList(selling, "selling");
        }

        public override string ToString()
        {
            return "[Shop]: " + intro;
        }

        public Outcome Compile()
        {
            if (compiled != null) return compiled;

            List<StoryEvent> events = new List<StoryEvent>();

            TextEvent prompt = new TextEvent();
            prompt.text = intro;
            prompt.speaker.Value = speaker;
            events.Add(prompt);

            List<StoryEvent> buyEvents = new List<StoryEvent>();
            List<StoryEvent> sellEvents = new List<StoryEvent>();

            buyEvents.Add(compileTrades(buying, true, prompt));
            sellEvents.Add(compileTrades(selling, false, prompt));

            if (buying.Count == 0) events.AddRange(sellEvents);
            else if (selling.Count == 0) events.AddRange(buyEvents);
            else
            {
                TextChoiceEvent buyOrSell = new TextChoiceEvent();
                buyOrSell.prompt = "What would you like to do?";
                events.Add(buyOrSell);

                Choice buy = new Choice();
                buy.text = "I'd like to buy.";
                buy.outcomeEvents.AddRange(buyEvents);
                buy.meetAllConditions = false;
                buy.conditions.AddRange(buying.Select(trade => trade.MakeCondition(true)));
                buyOrSell.choices.Add(buy);

                Choice sell = new Choice();
                sell.text = "I'd like to sell.";
                sell.outcomeEvents.AddRange(sellEvents);
                sell.meetAllConditions = false;
                sell.conditions.AddRange(buying.Select(trade => trade.MakeCondition(false)));
                buyOrSell.choices.Add(sell);

                Choice back = new Choice();
                back.text = "Nevermind.";
                buyOrSell.choices.Add(back);
            }


            return compiled = new ShopOutcome(events);
        }

        private StoryEvent compileTrades(List<Trade> trades, bool buying, StoryEvent restart)
        {
            TextChoiceEvent choiceEvent = new TextChoiceEvent();
            choiceEvent.prompt = "What would you like to " + (buying ? "buy" : "sell") + "?";

            foreach (Trade trade in trades)
            {
                Choice choice = new Choice();
                choice.text = trade.MakeChoice(buying);
                choice.conditions.Add(trade.MakeCondition(buying));
                choiceEvent.choices.Add(choice);

                IfEvent checkQuantity = new IfEvent();
                choice.outcomeEvents.Add(checkQuantity);

                ConditionOutcome limitReachedOutcome = new ConditionOutcome();
                limitReachedOutcome.condition1 = new CanTradeCondition(trade, true);
                limitReachedOutcome.nextEvent.Value = choiceEvent;
                checkQuantity.outcomes.Add(limitReachedOutcome);

                TextEvent soldOut = new TextEvent();
                soldOut.speaker.Value = speaker;
                soldOut.text = buying ? "I'm sorry, I'm all sold out of that." : "I'm sorry, I think I have enough of that.";
                if (trades.Count > 1) soldOut.text += " Maybe something else?";
                limitReachedOutcome.outcomeEvents.Add(soldOut);

                ConditionOutcome enoughOutcome = new ConditionOutcome();
                enoughOutcome.condition1 = new CanTradeCondition(trade, false);
                checkQuantity.outcomes.Add(enoughOutcome);

                TextChoiceEvent tradeChoice = new TextChoiceEvent();
                tradeChoice.prompt = trade.prompt;
                enoughOutcome.outcomeEvents.Add(tradeChoice);

                Choice accept = new Choice();
                accept.text = trade.acceptResponse;
                tradeChoice.choices.Add(accept);

                accept.outcomeEvents.AddRange(trade.MakeOutcomes(buying));
                if (!String.IsNullOrEmpty(thanks))
                {
                    TextEvent acceptMessage = new TextEvent();
                    acceptMessage.text = thanks;
                    acceptMessage.speaker.Value = speaker;
                    accept.outcomeEvents.Add(acceptMessage);
                }
                accept.nextEvent.Value = choiceEvent;

                Choice decline = new Choice();
                decline.text = trade.declineResponse;
                tradeChoice.choices.Add(decline);

                Choice backToTrade = new Choice();
                backToTrade.text = "Maybe another item...";
                backToTrade.nextEvent.Value = choiceEvent;
                tradeChoice.choices.Add(backToTrade);
            }

            Choice back = new Choice();
            back.text = "On second thought, nevermind.";
            choiceEvent.choices.Add(back);

            return choiceEvent;
        }

        private class ShopOutcome : Outcome
        {
            public ShopOutcome(List<StoryEvent> events)
            {
                outcomeEvents.AddRange(events);
            }
        }

        public abstract class Good : DataObject
        {
            public virtual void AddFields(FieldData fields) { }
            protected internal abstract Condition MakeCondition();
            protected internal abstract StoryEvent MakeOutcome(bool gain);
            protected internal abstract string MakeChoice();
        }

        public class ResourceGood : Good
        {
            public Resource resource;
            public int amount;

            public override void AddFields(FieldData fields)
            {
                resource = fields.addEnum(resource, "resource");
                amount = fields.add(amount, "amount");
            }

            public override string ToString()
            {
                return amount + " " + resource;
            }

            protected internal override Condition MakeCondition()
            {
                ResourceCondition condition = new ResourceCondition();
                condition.comparison = Comparison.AtLeast;
                condition.resource = resource;
                condition.value = amount;
                return condition;
            }

            protected internal override StoryEvent MakeOutcome(bool gain)
            {
                SetValueEvent e = new SetValueEvent();
                ResourceValue rv = new ResourceValue();
                rv.resource = resource;
                e.value = rv;
                e.amount = amount * (gain ? 1 : -1);
                e.operation = SetValueEvent.Operation.Change;
                return e;
            }

            protected internal override string MakeChoice()
            {
                switch (resource)
                {
                    case Resource.Food: return "Food";
                    case Resource.Money: return "Money";
                }
                return "{Cannot sell " + resource + "!}";
            }
        }

        public class ItemGood : Good
        {
            public readonly Reference<Item> item = new Reference<Item>();

            public override void AddFields(FieldData fields)
            {
                fields.addReference(item, "item");
            }

            public override string ToString()
            {
                return item.ToString();
            }

            protected internal override Condition MakeCondition()
            {
                ItemCondition condition = new ItemCondition();
                condition.item.Value = item;
                condition.has = true;
                return condition;
            }

            protected internal override StoryEvent MakeOutcome(bool gain)
            {
                ItemEvent e = new ItemEvent();
                e.item.Value = item;
                e.action = gain ? ItemEvent.ItemAction.Add : ItemEvent.ItemAction.Remove;
                return e;
            }

            protected internal override string MakeChoice()
            {
                if (item.IsNull) return "{Item cannot be null}";
                return item.Value.ToString();
            }
        }

        public class Trade : GameData
        {
            public string prompt;
            public string acceptResponse;
            public string declineResponse;
            [FieldTag(FieldTags.Comment, "Values less than 1 are treated as limitless.")]
            public int limit;

            [FieldTag(FieldTags.Inline)]
            public Good good;
            [FieldTag(FieldTags.Inline)]
            public Good cost;

            public override void AddFields(FieldData fields)
            {
                base.AddFields(fields);
                prompt = fields.add(prompt, "prompt");
                acceptResponse = fields.add(acceptResponse, "acceptResponse");
                declineResponse = fields.add(declineResponse, "declineResponse");
                good = fields.add(good, "good");
                cost = fields.add(cost, "cost");
                limit = fields.add(limit, "limit");
            }

            public override string ToString()
            {
                return good + " for " + cost;
            }

            protected internal Condition MakeCondition(bool buying)
            {
                return buying ? cost.MakeCondition() : good.MakeCondition();
            }

            protected internal IEnumerable<StoryEvent> MakeOutcomes(bool buying)
            {
                yield return good.MakeOutcome(buying);
                yield return cost.MakeOutcome(!buying);
                yield return new TradeMadeEvent(this);
            }

            protected internal string MakeChoice(bool buying)
            {
                return good.MakeChoice();
            }
        }

        // These two classes don't need to be serialized, since they're generated only on compile

        public class TradeMadeEvent : StoryEvent
        {
            public readonly Trade trade;

            public TradeMadeEvent(Trade trade)
            {
                this.trade = trade;
            }
        }

        public class CanTradeCondition : Condition
        {
            public readonly Trade trade;
            public readonly bool invert;

            public CanTradeCondition(Trade trade, bool invert)
            {
                this.trade = trade;
                this.invert = invert;
            }
        }
    }
}
