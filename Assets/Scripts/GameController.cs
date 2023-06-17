using System;
using System.Data;
using UnityEngine;

[RequireComponent(typeof(MazeConstructor))]
public class GameController : MonoBehaviour
{
    private MazeConstructor constructor;
    public GameObject playerPrefab;
    public GameObject monsterPrefab;
    private AIController aIController;

    [SerializeField] private int rows;
    [SerializeField] private int cols;

    private void Awake()
    {
        constructor = GetComponent<MazeConstructor>();
        aIController = GetComponent<AIController>();
    }

    private void Start()
    {
        constructor.GenerateNewMaze(rows, cols, OnTreasureTrigger);
        aIController.Graph = constructor.graph;
        aIController.Player = CreatePlayer();
        aIController.Monster = CreateMonster(OnMonsterTrigger);
        aIController.HallWidth = constructor.hallWidth;
        aIController.StartAI();
    }

    private GameObject CreatePlayer()
    {
        Vector3 playerStartPosition = new Vector3(constructor.hallWidth, 1, constructor.hallWidth);
        GameObject player = Instantiate(playerPrefab, playerStartPosition, Quaternion.identity);
        player.tag = "Generated";
        return player;
    }

    private GameObject CreateMonster(TriggerEventHandler onKillCallback)
    {
        Vector3 monsterPosition = new Vector3(
            constructor.goalCol * constructor.hallWidth,
            0f,
            constructor.goalRow * constructor.hallWidth);

        GameObject monster = Instantiate(monsterPrefab, monsterPosition, Quaternion.identity);
        TriggerEventRouter tc = monster.GetComponent<TriggerEventRouter>();
        tc.callback = onKillCallback;
        monster.tag = "Generated";
        return monster;
    }

    private void OnTreasureTrigger(GameObject trigger, GameObject other)
    {
        Debug.Log("You Won!");
        aIController.StopAI();
    }

    private void OnMonsterTrigger(GameObject trigger, GameObject other)
    {
        Debug.Log("The monster got you :(");
    }

}
