using System;
using UnityEngine;

[Serializable]
public enum TradingType
{
    Resource,
    Infomation,
    Item
}

[CreateAssetMenu(fileName = "New Trading Event", menuName = "Events/Trade")]
public class TradingEvent : Event
{
    [TextArea(3,10)] public string acceptTrading;
    [TextArea(3,10)] public string refuseTrading;

    public TradingType tradingType;
}
