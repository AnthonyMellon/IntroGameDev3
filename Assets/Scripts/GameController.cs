using System;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

[RequireComponent(typeof(MazeConstructor))]
public class GameController : MonoBehaviour
{
    private MazeConstructor constructor;
    public GameObject playerPrefab;
    public GameObject monsterPrefab;
    private AIController aIController;
    private List<GameObject> waypoints = new List<GameObject>();

    [SerializeField] private int rows;
    [SerializeField] private int cols;

    private void Awake()
    {
        constructor = GetComponent<MazeConstructor>();
        aIController = GetComponent<AIController>();
    }

    private void Start()
    {
        StartNewGame();
    }

    private void Update()
    {
        if (Input.GetKeyDown("f"))
        {
            Debug.Log("Helping the player find the exit");
            CreateWayPointPath();
        }
    }

    private void StartNewGame()
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

    private GameObject CreateWaypoint(Vector3 position)
    {
        GameObject waypoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        waypoint.GetComponent<SphereCollider>().enabled = false;
        waypoint.transform.position = position;
        waypoint.name = "Waypoint";
        waypoint.tag = "Generated";

        return waypoint;
    }

    private void CreateWayPointPath()
    {
        DestroyAllWaypoints();

        int playerCol = Mathf.RoundToInt(aIController.Player.transform.position.x / aIController.HallWidth);
        int playerRow = Mathf.RoundToInt(aIController.Player.transform.position.z / aIController.HallWidth);        

        //Debug.Log($"Finding path from {playerCol} {playerCol} to {endPosition.x} {endPosition.y}");
        List<Node> path = aIController.FindPath(playerRow, playerCol, constructor.goalRow, constructor.goalCol);

        for(int i = 0; i < path.Count; i++)
        {
            Node node = path[i];
            Vector3 position = new Vector3(node.y * constructor.hallWidth, 0.5f, node.x * constructor.hallWidth);
            waypoints.Add(CreateWaypoint(position));
        }
    }

    private void DestroyAllWaypoints()
    {
        if (waypoints.Count == 0) return;

        for(int i = 0; i < waypoints.Count; i++)
        {
            Destroy(waypoints[i]);            
        }
         
        waypoints = new List<GameObject>();
    }

    private void OnTreasureTrigger(GameObject trigger, GameObject other)
    {
        Debug.Log("You Won!");
        aIController.StopAI();
    }

    private void OnMonsterTrigger(GameObject trigger, GameObject other)
    {
        Debug.Log("The monster got you :(");
        StartNewGame();
    }

}
