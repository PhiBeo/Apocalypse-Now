using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum MinorEventType
{
    Trade,
    Random,
    Combat
}

public class GameEvents : MonoBehaviour
{
    [SerializeField] private List<RadioEvents> radioEvents;
    [SerializeField] private List<TradingEvent> tradingEvents;
    [SerializeField] private List<ShelterEvent> shelterEvents;
    [SerializeField] private List<RandomEvent> randomEvents;
    [SerializeField] private List<CombatEvent> combatEvents;
    [SerializeField] private RescueEvent rescueEvent;

    private Dictionary<int, int> radioDate;

    private Dictionary<MinorEventType, int> EventCoolDown = new Dictionary<MinorEventType, int>
    {
        {MinorEventType.Trade, Global.tradeEventCooldown },
        {MinorEventType.Combat, Global.combatEventCooldown },
        {MinorEventType.Random, 0}
    };

    private TimeManager timeManager;
    private UIManager uiManager;

    [SerializeField, MyBox.ReadOnly] private Event currentEvent;
    [SerializeField, MyBox.ReadOnly] private int currentRadioEvent = 0;

    private Dictionary<ShelterSituation, bool> shelterTrigger = new Dictionary<ShelterSituation, bool>
    { {ShelterSituation.SomeoneSick, false },
      {ShelterSituation.SomeoneThirsty, false },
      {ShelterSituation.SomeoneHungry, false },
      {ShelterSituation.SomeoneBecomeCrazy, false },
      {ShelterSituation.FoundSomething, false },
      {ShelterSituation.RunningLowOnSupplies, false },
      {ShelterSituation.SomeoneDie, false }
    };

    private Dictionary<ShelterSituation, int> shelterEventCoolDown = new Dictionary<ShelterSituation, int>
    {
        {ShelterSituation.SomeoneSick, Global.shelterSituationCooldown },
        {ShelterSituation.SomeoneThirsty, Global.shelterSituationCooldown },
        {ShelterSituation.SomeoneHungry, Global.shelterSituationCooldown },
        {ShelterSituation.SomeoneBecomeCrazy, Global.shelterSituationCooldown },
        {ShelterSituation.FoundSomething, Global.shelterSituationCooldown },
        {ShelterSituation.RunningLowOnSupplies, Global.shelterSituationCooldown },
        {ShelterSituation.SomeoneDie, Global.shelterSituationCooldown }
    };

    private void Start()
    {
        timeManager = FindAnyObjectByType<TimeManager>();
        uiManager = FindAnyObjectByType<UIManager>();
        radioDate = new Dictionary<int, int>();

        timeManager.ProcessDay += CoolDownLogic;
        timeManager.ProcessDay += PickNextEvent;

        RadioEventGenerate();
        PickNextEvent();
    }

    private void PickNextEvent()
    {
        currentEvent = null;

        //Prior events
        if(timeManager.GetCurrentDay() >= Global.maxDays)
        {
            RescueEventGenerate();
            uiManager.UpdateText(currentEvent);
            return;
        }

        if (currentRadioEvent < radioEvents.Count && radioDate[currentRadioEvent] == timeManager.GetCurrentDay())
        {
            currentEvent = radioEvents[currentRadioEvent];
            currentRadioEvent++;
            uiManager.UpdateText(currentEvent);
            return;
        }

        foreach(var trigger in shelterTrigger)
        {
            if(trigger.Value && shelterEventCoolDown[trigger.Key] <= 0)
            {
                ShelterEventGenerate(trigger.Key);
                uiManager.UpdateText(currentEvent);
                return;
            }
        }

        //Random events (Trading, combat and completely random)
        Debug.Log("No prior event found, generating random event");

        int randomTypeOfEvent = UnityEngine.Random.Range(0, Enum.GetNames(typeof(MinorEventType)).Length);

        if (EventCoolDown[(MinorEventType)randomTypeOfEvent] > 0)
        {
            randomTypeOfEvent = (int) MinorEventType.Random;
        }

        switch((MinorEventType)randomTypeOfEvent)
        {
            case MinorEventType.Trade:
                TradingEventGenerate();
                break;
            case MinorEventType.Random:
                RandomEventGenerate();
                break;
        }

        if (currentEvent == null) 
        {
            currentEvent = randomEvents[UnityEngine.Random.Range(0, randomEvents.Count)];
            Debug.Log("Fill in a random event");
        }

        uiManager.UpdateText(currentEvent);
    }

    //Radio event trigger
    private void RadioEventGenerate()
    {
        System.Random random = new System.Random();

        int baseSpacing = timeManager.GetMaxDay() / radioEvents.Count;

        radioDate.Add(0, 1);

        for(int i = 1; i < radioEvents.Count; i++)
        {
            int variation = random.Next(-3,4);
            int day = Math.Clamp((i * baseSpacing) + variation, 1, timeManager.GetMaxDay());

            radioDate.Add(i, day);
        }
    }

    //Trade event trigger
    private void TradingEventGenerate()
    {
        TradingEvent randTradeEvent = tradingEvents[UnityEngine.Random.Range(0, tradingEvents.Count)];
        currentEvent = randTradeEvent;

        TradingSystem tradingSystem = FindAnyObjectByType<TradingSystem>();

        tradingSystem.TradingTypeHandle(randTradeEvent.tradingType);

        EventCoolDown[MinorEventType.Trade] = Global.tradeEventCooldown;

        uiManager.SetTradingText(randTradeEvent);
    }

    //Shelter Event Trigger
    private void ShelterEventGenerate(ShelterSituation situation)
    {
        ShelterEvent shelterEvent = shelterEvents.Find(x => x.shelterSituation == situation);

        //Sprite sprite = itemImages.Find(x => x.itemName == type.ToString()).itemImage;
        currentEvent = shelterEvent;
        shelterTrigger[situation] = false;
        shelterEventCoolDown[situation] = Global.shelterSituationCooldown;

        if (currentEvent == null)
        {
            Debug.Log($"Cannot find: {situation}!");
        }
    }

    public void TriggerShelterEvent(ShelterSituation situation)
    {
        if (shelterEventCoolDown[situation] <= 0)
        {
            shelterTrigger[situation] = true;
        }
    }

    //Random Event Trigger
    public void RandomEventGenerate()
    {
        RandomEvent randomEvent = randomEvents[UnityEngine.Random.Range(0, randomEvents.Count)];
        currentEvent = randomEvent;
    }

    //Combat Event Trigger
    public void CombatEventGenerate()
    {
        CombatEvent combatEvent = combatEvents[UnityEngine.Random.Range(0, combatEvents.Count)];
        currentEvent = combatEvent;

        CombatSystem combatSystem = FindAnyObjectByType<CombatSystem>();
        combatSystem.CombatDataHandle(combatEvent);
        EventCoolDown[MinorEventType.Combat] = Global.combatEventCooldown;
        uiManager.SetCombatText(combatEvent);
    }

    //Rescue Event Trigger
    public void RescueEventGenerate()
    {
        currentEvent = rescueEvent;
    }

    public void CoolDownLogic()
    {
        Debug.Log("Event Cooldown Call");

        for (int i = 0; i < shelterEventCoolDown.Count; i++)
        {
            if (shelterEventCoolDown[(ShelterSituation)i] > 0)
            {
                shelterEventCoolDown[(ShelterSituation)i]--;
            }
        }

        for(int i = 0; i < EventCoolDown.Count; i++)
        {
            if (EventCoolDown[(MinorEventType)i] > 0)
            {
                EventCoolDown[(MinorEventType)i]--;
            }
        }
    }
}
