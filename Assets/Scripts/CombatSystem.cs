using UnityEngine;

public class CombatSystem : MonoBehaviour
{
    private ResourceManager resourceManager;

    [SerializeField, MyBox.ReadOnly] private int currentCombatSuccessChance = 0;

    private bool isInvade = false;
    private bool isAcceptTerm = false;

    private void Start()
    {
        resourceManager = FindAnyObjectByType<ResourceManager>();
    }

    public void CombatDataHandle(CombatEvent combatEvent)
    {
        currentCombatSuccessChance = combatEvent.succeedChance;
    }

    public void AcceptRaid()
    {
        isAcceptTerm = true;
    }

    public void RefuseRaid()
    {
        isAcceptTerm = false;

        int diceRoll = Random.Range(0, 100);

        if (diceRoll <= currentCombatSuccessChance)
        {
            isInvade = false;
            return;
        }

        isInvade = true;
    }

    public void CombatResult()
    {
        if(isAcceptTerm)
        {
            resourceManager.DeleteHalfResource();
            return;
        }

        if (!isInvade) return;

        resourceManager.DeleteEverything();

        //Todo: Add effect to survivors
    }

    public bool IsInvade()
    {
        return isInvade;
    }

    public bool IsAcceptTerm()
    {
        return isAcceptTerm;
    }
}
