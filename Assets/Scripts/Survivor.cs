using UnityEngine;
using MyBox;
using System;

public class Survivor : MonoBehaviour
{
    private string charName;
    private float hunger;
    private float thirst;
    private float health;

    private bool isAlive = true;
    private bool isInfected = false;
    private bool isExpedition = false;

    [ReadOnly, SerializeField] private int infectedDay = 0;
    [SerializeField] private int chanceOfInfection = 5;
    [SerializeField] private int dayOfExpedition = 0;

    private TimeManager timeManager;
    private ResourceManager resourceManager;

    [SerializeField] private GameObject characterMesh;

    private void Awake()
    {
        timeManager = FindAnyObjectByType<TimeManager>();
        resourceManager = FindAnyObjectByType<ResourceManager>();

        timeManager.ProcessDay += UpdateStats;
        timeManager.ProcessDay += ExpeditionUpdate;

        GenerateRandomName();
        hunger = Global.maxStats;
        thirst = Global.maxStats;
        health = Global.maxStats;
    }

    public void GenerateRandomName()
    {
        charName = Global.names[UnityEngine.Random.Range(0, Global.names.Length)];
    }

    public void UpdateStats()
    {
        if (!isAlive) return;
        if (timeManager.GetCurrentDay() == 1) return;

        StatsCal();

        if (hunger <= 0 || thirst <= 0)
        {
            health -= Global.healthDrain;

            if (health <= 0)
            {
                characterMesh.gameObject.SetActive(false);
                GameEvents.FindAnyObjectByType<GameEvents>().TriggerShelterEvent(ShelterSituation.SomeoneDie);
                isAlive = false;
            }
            return;
        }

        if(isInfected)
        {
            infectedDay++;
        }
    }

    private void StatsCal()
    {

        if(!isExpedition)
        {
            hunger -= Global.foodDrain;
            thirst -= Global.waterDrain;
        }

        if(isExpedition)
        {
            hunger -= Global.foodDrain / 2;
            thirst -= Global.waterDrain / 2;
        }

        hunger = Math.Clamp(hunger, 0, Global.maxStats);
        thirst = Math.Clamp(thirst, 0, Global.maxStats);
        health = Math.Clamp(health, 0, Global.maxStats);
    }

    private void ShelterEventTrigger()
    {
        if(hunger <= 0) GameEvents.FindAnyObjectByType<GameEvents>().TriggerShelterEvent(ShelterSituation.SomeoneHungry);
        if(thirst <= 0) GameEvents.FindAnyObjectByType<GameEvents>().TriggerShelterEvent(ShelterSituation.SomeoneThirsty);
        if(health <= 0) GameEvents.FindAnyObjectByType<GameEvents>().TriggerShelterEvent(ShelterSituation.SomeoneSick);
    }

    public void GiveSurvivorFood(int amount)
    {
        if (!resourceManager.HasResourceAmount(ItemType.Food, amount)) return;
        
        hunger += amount * Global.foodRecover;

        hunger = Math.Clamp(hunger, 0, Global.maxStats);

        resourceManager.UseResource(ItemType.Food, amount);
    }

    public void GiveSurvivorWater(int amount)
    {
        if (!resourceManager.HasResourceAmount(ItemType.Water, amount)) return;

        thirst += amount * Global.waterRecover;

        thirst = Math.Clamp(thirst, 0, Global.maxStats);

        resourceManager.UseResource(ItemType.Water, amount);
    }

    public void GiveSurvivorMed(int amount)
    {
        if (!resourceManager.HasResourceAmount(ItemType.Med, amount)) return;

        health += amount * Global.healthRecover;

        health = Math.Clamp(health, 0, Global.maxStats);

        resourceManager.UseResource(ItemType.Med, amount);
    }

    public void ExpeditionUpdate()
    {
        if (!isExpedition || !isAlive) return;

        InfectedDieRoll();
        dayOfExpedition--;
        if(dayOfExpedition <= 0)
        {
            isExpedition = false;
            GetBackFromExpedition();
        }

    }

    private void InfectedUpdate()
    {
        if (!isInfected) return;
    }

    public void Expedition()
    {
        characterMesh.SetActive(false);
        isExpedition = true;
        dayOfExpedition = Global.expeditionDay;
    }

    public void InfectedDieRoll()
    {
        int dieRoll = UnityEngine.Random.Range(1, 101);

        if (dieRoll <= chanceOfInfection)
        {
            isInfected = true;
        }
    }

    public void GetBackFromExpedition()
    {
        characterMesh.SetActive(true);
        ResourceManager resourceManager = FindAnyObjectByType<ResourceManager>();

        int amount = 0;

        for(int i = 0; i < Enum.GetValues(typeof(ItemType)).Length; i++)
        {
            if(resourceManager.GetItemClass((ItemType)i) == ItemClass.Resource)
            {
                amount = UnityEngine.Random.Range(1, Global.maxAmountOfResourceGain + 1);
            }
            else if (resourceManager.GetItemClass((ItemType)i) == ItemClass.Ultility)
            {
                amount = UnityEngine.Random.Range(0, Global.maxAmountOfUltilityGain + 1);
            }

            resourceManager.AddResource((ItemType)i, amount);
        }
    }

    public float GetHealth() { return health; }
    public float GetHunger() { return hunger; }
    public float GetThirst() { return thirst; }
    public string GetName() { return charName; }
    public bool IsExpedition() { return isExpedition; }
    public bool IsAlive() { return isAlive; }
}
