using UnityEngine;

[CreateAssetMenu(fileName = "New Combat Event", menuName = "Events/Combat")]
public class CombatEvent : Event
{
    [TextArea(3, 10)] public string acceptText;

    [TextArea(3, 10)] public string diceChanceSucceedText;
    [TextArea(3, 10)] public string diceChanceFailText;

    public int succeedChance;
}
