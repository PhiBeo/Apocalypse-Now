using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public enum ItemType
{
    Food,
    Water,
    Med,
    Axe,
    Knife
}

[Serializable]
public enum ItemClass
{ 
    Resource,
    Ultility,
}


[Serializable]
public class Resource
{
    public string itemName;
    public int amount;
    public ItemClass itemClass;

    public Resource(string name, int amount, ItemClass iClass)
    {
        itemName = name;
        this.amount = amount;
        itemClass = iClass;
    }
}


public class ResourceManager : MonoBehaviour
{
    private Dictionary<ItemType, ItemClass> resourceClass = new Dictionary<ItemType, ItemClass>
    {
        {ItemType.Food, ItemClass.Resource},
        {ItemType.Water, ItemClass.Resource},
        {ItemType.Med, ItemClass.Resource},
        {ItemType.Axe, ItemClass.Ultility},
        {ItemType.Knife, ItemClass.Ultility},
    };

    [SerializeField] private List<Resource> resources = new List<Resource>();

    private TimeManager timeManager;
    private ResourceUIManager resourceUIManager; 

    private void Awake()
    {
        timeManager = FindAnyObjectByType<TimeManager>();
        resourceUIManager = FindAnyObjectByType<ResourceUIManager>();

        for (int i = 0; i < Enum.GetNames(typeof(ItemType)).Length; i++)
        {
            resources.Add(new Resource(Enum.GetNames(typeof(ItemType))[i], 5, resourceClass[(ItemType)i]));
        }

        timeManager.ProcessDay += DailyResourceCheck;
    }

    public bool UseResource(ItemType type, int amount)
    {
        Resource item = resources.Find(x => x.itemName == type.ToString());

        if (item.amount <= 0)
        {
            Debug.Log("Not enough resources");
            return false;
        }

        item.amount -= amount;

        resourceUIManager.UpdateResourceUI((int)type);

        return true;
    }

    public void AddResource(ItemType type, int amount)
    {
        Resource item = resources.Find(x => x.itemName == type.ToString());

        item.amount += amount;

        resourceUIManager.UpdateResourceUI((int)type);
    }

    public bool HasResourceAmount(ItemType type, int amount)
    {
        Resource item = resources.Find(x => x.itemName == type.ToString());

        return item.amount >= amount;
    }

    public bool HasResource(ItemType type)
    {
        Resource item = resources.Find(x => x.itemName == type.ToString());

        return item.amount > 0;
    }   

    public int GetResourceAmount(ItemType type)
    {
        Resource item = resources.Find(x => x.itemName == type.ToString());

        return item.amount;
    }

    public ItemClass GetItemClass(ItemType type)
    {
        return resourceClass[type];
    }

    public bool IsClass(ItemType itemType, ItemClass itemClass)
    {
        return resourceClass[itemType] == itemClass;
    }

    public List<string> GetItemsFromClass(ItemClass itemClass)
    {
        return resources.Where(x => x.itemClass == itemClass).Select(x => x.itemName).ToList();
    }

    public void DailyResourceCheck()
    {
        for (int i = 0; i < resources.Count; i++)
        {
            if (resources[i].amount <= 1)
            {
                GameEvents.FindAnyObjectByType<GameEvents>().TriggerShelterEvent(ShelterSituation.RunningLowOnSupplies);
            }
        }
    }

    public void DeleteHalfResource()
    {
        for (int i = 0; i < resources.Count; i++)
        {
            resources[i].amount /= 2;
        }
    }

    public void DeleteEverything()
    {
        for (int i = 0; i < resources.Count; i++)
        {
            resources[i].amount = 0;
        }
    }
}
