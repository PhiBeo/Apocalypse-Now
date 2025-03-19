using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public struct TradeItem
{
    public string item;
    public int amount;
}

[Serializable]
public struct Trading
{
    public TradeItem playerTrade;
    public TradeItem vendorTrade;

    public Trading(TradeItem player, TradeItem vendor)
    {
        playerTrade = player;
        vendorTrade = vendor;
    }
}

public class TradingSystem : MonoBehaviour
{
    private ResourceManager resourceManager;
    private List<Trading> tradingList;

    private bool isTradingInfo = false;

    private void Start()
    {
        resourceManager = FindAnyObjectByType<ResourceManager>();

        tradingList = new List<Trading>();
    }

    public void TradingTypeHandle(TradingType type)
    {
        tradingList.Clear();

        isTradingInfo = false;

        switch (type)
        { 
            case TradingType.Resource:
                ResourceTrading();
                break;
            case TradingType.Infomation:
                InfomationTrading();
                isTradingInfo = true;
                break;
            case TradingType.Item:
                ItemTrading();
                break;
        }

        UIManager.FindAnyObjectByType<UIManager>().UpdateTradingUI();
    }

    private void ResourceTrading()
    {
        List<Resource> availableResource = new List<Resource>();

        for (int i = 0; i < Enum.GetValues(typeof(ItemType)).Length; i++)
        {
            if (!resourceManager.IsClass((ItemType)i, ItemClass.Resource)) continue;
            if (!resourceManager.HasResource((ItemType)i)) continue;

            availableResource.Add(new Resource(Enum.GetNames(typeof(ItemType))[i], resourceManager.GetResourceAmount((ItemType)i), resourceManager.GetItemClass((ItemType)i)));
        }

        if (availableResource.Count <= 0) return;

        System.Random random = new System.Random();
        int tradeCount = 9;

        if (availableResource.Count >= 2)
        {
            tradeCount = 0;
        }

        while (tradeCount < 2)
        {
            Resource randomPickedResource = availableResource[random.Next(0, availableResource.Count)];
            availableResource.Remove(randomPickedResource);

            //Calculate player item to trade
            TradeItem playerTradeItem = new TradeItem();

            int randomAmount = random.Next(1, 3);
            randomAmount = Math.Clamp(randomAmount, 1, randomPickedResource.amount);

            playerTradeItem.item = randomPickedResource.itemName;
            playerTradeItem.amount = randomAmount;

            //vendor trade offer
            TradeItem vendorTradeItem = new TradeItem();
            List<string> tradableItem = new List<string>();
            tradableItem = resourceManager.GetItemsFromClass(ItemClass.Resource);
            tradableItem.Remove(randomPickedResource.itemName);

            string randomPickedItem = tradableItem[random.Next(0, tradableItem.Count)];
            int randomOffset = random.Next(-1, 2);
            int vendorAmount = Math.Clamp(randomOffset + randomAmount, 1, 5);

            vendorTradeItem.item = randomPickedItem;
            vendorTradeItem.amount = vendorAmount;

            tradingList.Add(new Trading(playerTradeItem, vendorTradeItem));

            tradeCount++;
        }
    }

    private void InfomationTrading()
    {
        List<Resource> availableResource = new List<Resource>();

        for (int i = 0; i < Enum.GetValues(typeof(ItemType)).Length; i++)
        {
            if (!resourceManager.HasResource((ItemType)i)) continue;

            availableResource.Add(new Resource(Enum.GetNames(typeof(ItemType))[i], resourceManager.GetResourceAmount((ItemType)i), resourceManager.GetItemClass((ItemType)i)));
        }

        if (availableResource.Count <= 0) return;

        for(int i = 0; i < availableResource.Count; i++)
        {
            TradeItem playerTradeItem = new TradeItem();
            TradeItem vendorTradeItem = new TradeItem();

            playerTradeItem.item = availableResource[i].itemName;
            playerTradeItem.amount = 1;

            vendorTradeItem.item = availableResource[i].itemName;
            vendorTradeItem.amount = 0;

            tradingList.Add(new Trading(playerTradeItem, vendorTradeItem));
        }
    }

    private void ItemTrading()
    {
        List<Resource> availableResource = new List<Resource>();

        for (int i = 0; i < Enum.GetValues(typeof(ItemType)).Length; i++)
        {
            if (!resourceManager.IsClass((ItemType)i, ItemClass.Ultility)) continue;
            if (!resourceManager.HasResource((ItemType)i)) continue;

            availableResource.Add(new Resource(Enum.GetNames(typeof(ItemType))[i], resourceManager.GetResourceAmount((ItemType)i), resourceManager.GetItemClass((ItemType)i)));
        }

        if (availableResource.Count <= 0) return;

        System.Random random = new System.Random();
        int tradeCount = 9;

        if (availableResource.Count >= 2)
        {
            tradeCount = 0;
        }

        while(tradeCount < 2)
        {
            Resource randomPickedResource = availableResource[random.Next(0, availableResource.Count)];
            availableResource.Remove(randomPickedResource);

            //Calculate player item to trade
            TradeItem playerTradeItem = new TradeItem();

            playerTradeItem.item = randomPickedResource.itemName;
            playerTradeItem.amount = 1;

            //vendor trade offer
            TradeItem vendorTradeItem = new TradeItem();
            List<string> tradableItem = new List<string>();
            tradableItem = resourceManager.GetItemsFromClass(ItemClass.Ultility);
            tradableItem.Remove(randomPickedResource.itemName);

            string randomPickedItem = tradableItem[random.Next(0, tradableItem.Count)];
            int vendorAmount = 1;

            vendorTradeItem.item = randomPickedItem;
            vendorTradeItem.amount = vendorAmount;

            tradingList.Add(new Trading(playerTradeItem, vendorTradeItem));

            tradeCount++;
        }
    }

    public void HandleTrade(Trading trading)
    {
        ItemType itemType;

        if (Enum.TryParse(trading.playerTrade.item, out itemType))
        {
            resourceManager.UseResource(itemType, trading.playerTrade.amount);
        }
        else
        {
            Debug.Log("Invalid String for enum conversion");
        }

        if (Enum.TryParse(trading.vendorTrade.item, out itemType))
        {
            resourceManager.AddResource(itemType, trading.vendorTrade.amount);
        }
        else
        {
            Debug.Log("Invalid String for enum conversion");
        }

        tradingList.Clear();
    }

    public int GetTradeCount()
    {
        return tradingList.Count;
    }

    public List<Trading> GetTrades()
    {
        return tradingList;
    }

    public bool IsTradingInfo()
    {
        return isTradingInfo;
    }
}
