using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public enum EventTypeDe
{
    Trade,
    HiddenSupply,
    SomeoneGotSick,
    BreakIn,
    Raid,
    AidOther,
    Radio,
    SomeoneBecomeCrazy,
    Moral,
    Infected
}

public class Event : ScriptableObject
{
    [TextArea(3, 50)] public string eventDescription;
}
