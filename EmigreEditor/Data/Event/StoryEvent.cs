namespace Emigre.Data
{
    using ObjectEditor.Json;

    public abstract class StoryEvent : GameData, IEnableable
    {
        public enum Action
        {
            Immediately,
            NextEvent,
            Choice,
            InventoryOpened,
            InventoryClosed,
            ScreenClick,
            Nothing,
            NoFastMode
        }

        public static void Load()
        {
            ReflectionConstructor<TextEvent>.Register("TextEvent");

            ReflectionConstructor<TextChoiceEvent>.Register("TextChoiceEvent");
            ReflectionConstructor<ImageChoiceEvent>.Register("ImageChoiceEvent");

            ReflectionConstructor<SetVariableEvent>.Register("SetVariableEvent");
            ReflectionConstructor<SetValueEvent>.Register("ResourceEvent");
            ReflectionConstructor<SetValueEvent>.Register("SetValueEvent");
            ReflectionConstructor<SkillLevelUpEvent>.Register("SkillLevelUpEvent");
            ReflectionConstructor<ShopEvent>.Register("ShopEvent");
            ReflectionConstructor<ItemEvent>.Register("ItemEvent");
            ReflectionConstructor<SleepEvent>.Register("SleepEvent");
            ReflectionConstructor<SetLodgingEvent>.Register("SetLodgingEvent");
            ReflectionConstructor<OverlayEvent>.Register("OverlayEvent");
            ReflectionConstructor<ObjectiveEvent>.Register("ObjectiveEvent");

            ReflectionConstructor<ChangeCityEvent>.Register("ChangeCityEvent");
            ReflectionConstructor<ChangeLocationEvent>.Register("ChangeLocationEvent");
            ReflectionConstructor<StartJourneyEvent>.Register("StartJourneyEvent");
            ReflectionConstructor<EndScenarioEvent>.Register("EndScenarioEvent");

            ReflectionConstructor<IfEvent>.Register("IfEvent");
            ReflectionConstructor<CheckPagesEvent>.Register("CheckPagesEvent");

            ReflectionConstructor<MusicEvent>.Register("MusicEvent");
            ReflectionConstructor<CutsceneEvent>.Register("CutsceneEvent");
            ReflectionConstructor<BackgroundEvent>.Register("BackgroundEvent");
            ReflectionConstructor<ActorEnterExitEvent>.Register("CharacterSpriteEvent");

            ReflectionConstructor<EnableScenarioEvent>.Register("EnableScenarioEvent");
            ReflectionConstructor<EnablePageEvent>.Register("EnablePageEvent");
            ReflectionConstructor<EnableChoiceEvent>.Register("EnableChoiceEvent");
            ReflectionConstructor<DiscoverLocationEvent>.Register("DiscoverLocationEvent");
            ReflectionConstructor<FindClueEvent>.Register("FindClueEvent");
            ReflectionConstructor<ChangeStatusEvent>.Register("ChangeLocationStatusEvent");
            ReflectionConstructor<ChangeStatusEvent>.Register("ChangeStatusEvent");

            ReflectionConstructor<AlignmentEvent>.Register("AlignmentEvent");

            ReflectionConstructor<WaitEvent>.Register("WaitEvent");
            ReflectionConstructor<MinigameEvent>.Register("MinigameEvent");
            ReflectionConstructor<EndGameEvent>.Register("EndGameEvent");

            Register<ChangeSpriteEvent>();
            Register<TransitionTriggerEvent>();
            Register<LoadSceneEvent>();
            Register<ScreenEvent>();

            // Subway Events
            Register<ShowTitleEvent>();
            Register<ShowBookEvent>();
            Register<PhoneEvent>();
        }

        public bool once;

        public override void AddFields(FieldData fields)
        {
            base.AddFields(fields);
            once = fields.add(once, "once");
        }

        public virtual bool cut()
        {
            return false;
        }

        public virtual Action GetTransitionAction()
        {
            return Action.Immediately;
        }

        public override string ToString()
        {
            return GetType().Name;
        }

        public bool GetDefaultEnabled()
        {
            return true;
        }
    }
}