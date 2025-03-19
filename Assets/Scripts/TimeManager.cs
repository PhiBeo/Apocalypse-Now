using UnityEngine;
using MyBox;
using System;

public class TimeManager : MonoBehaviour
{
    [SerializeField] private int dayToSurvive = Global.maxDays;

    [ReadOnly, SerializeField] private int currentDay = 1;

    public Action ProcessDay;

    void Start()
    {
        ProcessDay?.Invoke();
    }

    public void NextDay()
    {
        currentDay++;

        ProcessDay?.Invoke();
        if (currentDay >= dayToSurvive)
        {
            GameManager.FindAnyObjectByType<GameManager>().Gamewin();
        }
    }

    public int GetMaxDay()
    {
        return dayToSurvive;
    }

    public int GetCurrentDay()
    {
        return currentDay;
    }
}
