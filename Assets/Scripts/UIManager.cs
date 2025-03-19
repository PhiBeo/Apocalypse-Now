using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class TradingUI
{
    public TextMeshProUGUI tradingText;

    public GameObject skipButton;
   
    public string agree;
    public string disagree;
}

[Serializable]
public class CombatUI
{
    public string acceptText;

    public string diceChanceSucceedText;
    public string diceChanceFailText;
}

[Serializable]
public enum CombatOption
{
    Agree,
    Disagree
}

public class UIManager : MonoBehaviour
{
    private TimeManager timeManager;
    private GameEvents gameEvents;

    [SerializeField] private TradingUI tradingUI;
    [SerializeField] private CombatUI combatUI;

    [SerializeField] private TextMeshProUGUI currentDayText;
    [SerializeField] private TextMeshProUGUI eventDescriptionText;

    public TextMeshProUGUI resultText;
    public GameObject AfterLogPanel;

    public GameObject nextDayButton;

    public List<Button> Buttons;
    public List<ImageHolder> imageHolders;

    //General UI Logic
    private void Start()
    {
        timeManager = FindAnyObjectByType<TimeManager>();
        gameEvents = FindAnyObjectByType<GameEvents>();

        for (int i = 0; i < Buttons.Count; i++)
        {
           Buttons[i].gameObject.SetActive(false);
        }

        AfterLogPanel.SetActive(false);
        tradingUI.skipButton.SetActive(false);

        timeManager.ProcessDay += ResetUI;
    }

    public void UpdateText(Event _event)
    {
        if(_event == null)
        {
            Debug.Log("Event is null");
            return;
        }

        currentDayText.text = $"Day: {timeManager.GetCurrentDay()}";

        eventDescriptionText.text = _event.eventDescription;
    }

    public void ResetUI()
    {
        for (int i = 0; i < Buttons.Count; i++)
        {
            Buttons[i].gameObject.SetActive(false);
            Buttons[i].onClick.RemoveAllListeners();
        }

        tradingUI.tradingText.text = "";
        tradingUI.agree = "";
        tradingUI.disagree = "";
        tradingUI.tradingText.gameObject.SetActive(false);
    }

    //Trading UI Logic
    public void UpdateTradingUI()
    {
        TradingSystem tradingSystem = FindAnyObjectByType<TradingSystem>();

        tradingUI.tradingText.gameObject.SetActive(true);
        tradingUI.skipButton.SetActive(true);
        nextDayButton.SetActive(false);

        if (tradingSystem.GetTradeCount() <= 0) return;

        for(int i = 0; i < tradingSystem.GetTradeCount(); i++)
        {
            Buttons[i].gameObject.SetActive(true);
            
            Sprite sprite = imageHolders.Find(x => x.itemName == tradingSystem.GetTrades()[i].vendorTrade.item).itemImage;

            Buttons[i].GetComponent<Image>().sprite = sprite;

            Trading trading = tradingSystem.GetTrades()[i];
            Buttons[i].onClick.AddListener(() => tradingSystem.HandleTrade(trading));
            Buttons[i].onClick.AddListener(() => AcceptTrading());
        }

        if (tradingSystem.IsTradingInfo()) return;

        for (int i = 0; i < tradingSystem.GetTradeCount(); i++)
        {
            tradingUI.tradingText.text += $"\n{i + 1}. Offer {tradingSystem.GetTrades()[i].vendorTrade.item} x{tradingSystem.GetTrades()[i].vendorTrade.amount} " +
                $"for {tradingSystem.GetTrades()[i].playerTrade.item} x{tradingSystem.GetTrades()[i].playerTrade.amount}\n";
        }
    }

    public void SetTradingText(TradingEvent tradingEvent)
    {
        tradingUI.agree = tradingEvent.acceptTrading;
        tradingUI.disagree = tradingEvent.refuseTrading;
    }

    public void SkipTrading()
    {
        resultText.text = tradingUI.disagree;
        AfterLogPanel.SetActive(true);
        nextDayButton.SetActive(true);
    }

    public void AcceptTrading()
    {
        nextDayButton.SetActive(true);
        tradingUI.skipButton.SetActive(false);
        AfterLogPanel.SetActive(true);
        resultText.text = tradingUI.agree;
    }

    //Combat UI Logic
    public void UpdateCombatUI()
    {
        CombatSystem combatSystem = FindAnyObjectByType<CombatSystem>();

        nextDayButton.SetActive(false);

        for (int i = 0; i < Enum.GetValues(typeof(CombatOption)).Length; i++)
        {
            Buttons[i].gameObject.SetActive(true);

            Sprite sprite = imageHolders.Find(x => x.itemName == ((CombatOption)i).ToString()).itemImage;

            Buttons[i].GetComponent<Image>().sprite = sprite;

            if((CombatOption)i == CombatOption.Agree)
            {
                Buttons[i].onClick.AddListener(() => combatSystem.AcceptRaid());
            }
            else
            {
                Buttons[i].onClick.AddListener(() => combatSystem.RefuseRaid());
            }

            Buttons[i].onClick.AddListener(() => OnCombatPageOpen());
            Buttons[i].onClick.AddListener(() => UpdateCombatText(combatSystem));
        }
    }

    public void OnCombatPageOpen()
    {
        nextDayButton.SetActive(true);
        AfterLogPanel.SetActive(true);
        resultText.gameObject.SetActive(true);
    }

    public void SetCombatText(CombatEvent combatEvent)
    {
        combatUI.acceptText = combatEvent.acceptText;
        combatUI.diceChanceSucceedText = combatEvent.diceChanceSucceedText;
        combatUI.diceChanceFailText = combatEvent.diceChanceFailText;
    }

    private void UpdateCombatText(CombatSystem combatSystem)
    {
        //Accepted the terms of the raider
        if (combatSystem.IsAcceptTerm())
        {
            resultText.text = combatUI.acceptText;
            return;
        }

        //Not accepted the terms of the raider
        if(combatSystem.IsInvade())
        {
            resultText.text = combatUI.diceChanceFailText;
        }
        else
        {
            resultText.text = combatUI.diceChanceSucceedText;
        }
    }
}
