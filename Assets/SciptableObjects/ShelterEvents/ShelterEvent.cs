using System;
using UnityEngine;

[Serializable]
public enum ShelterSituation
{
    SomeoneSick,
    SomeoneThirsty,
    SomeoneHungry,
    SomeoneBecomeCrazy,
    FoundSomething,
    RunningLowOnSupplies,
    SomeoneDie
}

[CreateAssetMenu(fileName = "New Shelter Event", menuName = "Events/Shelter")]
public class ShelterEvent : Event
{
    public ShelterSituation shelterSituation;
}
