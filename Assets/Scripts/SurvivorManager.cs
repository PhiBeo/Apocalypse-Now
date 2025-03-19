using System.Collections.Generic;
using UnityEngine;
using MyBox;

public class SurvivorManager : MonoBehaviour
{
    [SerializeField] private List<Survivor> survivors;

    private TimeManager timeManager;
    private GameManager gameManager;

    private void Start()
    {
        timeManager = FindAnyObjectByType<TimeManager>();
        gameManager = FindAnyObjectByType<GameManager>();

        timeManager.ProcessDay += CheckForSurvivors;
    }

    public void CheckForSurvivors()
    {
        for(int i = 0; i < survivors.Count; i++)
        {
            if (survivors[i].GetHealth() <= 0f)
            {
                survivors.Remove(survivors[i]);
            }
        }

        if(survivors.Count <= 0)
        {
            gameManager.Gameover();
        }
    }
}
