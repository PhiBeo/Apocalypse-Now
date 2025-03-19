using System.Collections.Generic;
using UnityEngine;
using MyBox;
using UnityEngine.UI;
using System;

[Serializable]
public enum CharacterUIButtonOp
{ 
    Heal,
    Feed,
    Drink,
    Expedition
}


public class CharacterUIManager : MonoBehaviour
{
    [SerializeField] private List<Survivor> survivors;
    [SerializeField] private List<CharacterUI> characterUIs;

    [SerializeField, ReadOnly] private Survivor selectedSurvivor;

    [SerializeField] private Button expeditionButton;

    private TimeManager timeManager;

    private void Start()
    {
        timeManager = FindAnyObjectByType<TimeManager>();

        timeManager.ProcessDay += UpdateStat;

        for(int i = 0; i < characterUIs.Count; i ++)
        {
            characterUIs[i].SetName(survivors[i].GetName());
        }

        UpdateStat();
    }

    private void Update()
    {
        if (!selectedSurvivor || selectedSurvivor.IsExpedition()) expeditionButton.interactable = false;
        else expeditionButton.interactable = true;
    }

    public void UpdateStat()
    {
        for(int i = 0; i < survivors.Count; i++)
        {
            characterUIs[i].UpdateStat(survivors[i].GetHealth(), survivors[i].GetHunger(), survivors[i].GetThirst());
        }

        for (int i = 0; i < survivors.Count; i++)
        {
            if (!survivors[i].IsAlive() || survivors[i].IsExpedition())
            {
                characterUIs[i].gameObject.SetActive(false);
            }
            else
            {
                characterUIs[i].gameObject.SetActive(true);
            }
        }


    }

    public void SelectedSurvivor(int i)
    {
        DeselectedAll();
        
        selectedSurvivor = survivors[i];

        characterUIs[i].OnSelectedCharacter();
    }

    public void DeselectedAll()
    {
        for (int j = 0; j < characterUIs.Count; j++)
        {
            characterUIs[j].OnDeselectedCharacter();
        }

        selectedSurvivor = null;
    }

    public void Operation(int index)
    {
        CharacterUIButtonOp op = (CharacterUIButtonOp)index;

        if (selectedSurvivor == null) return;

        switch (op)
        {
            case CharacterUIButtonOp.Heal:
                selectedSurvivor.GiveSurvivorMed(1);
                break;
            case CharacterUIButtonOp.Feed:
                selectedSurvivor.GiveSurvivorFood(1);
                break;
            case CharacterUIButtonOp.Drink:
                selectedSurvivor.GiveSurvivorWater(1);
                break;
            case CharacterUIButtonOp.Expedition:
                selectedSurvivor.Expedition();
                break;
        }

        UpdateStat();
    }
}
