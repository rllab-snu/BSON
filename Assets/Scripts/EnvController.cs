using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvController : MonoBehaviour
{
    [SerializeField] private bool b_SpawnRandomObstacles = true;
    [SerializeField] private int s32_NumObstacles = 20;
    [SerializeField] private float f32_ObstacleScale = 0.5f;
    [SerializeField] private bool b_SpawnRandomHumans = true;
    [SerializeField] private int s32_NumHumans = 50;

    [SerializeField] GameObject mGO_Floor;
    [SerializeField] List<Obstacle> mL_Obstacles = new List<Obstacle>();

    private GraphHandler mo_GraphHandler;
    [SerializeField] float mf32_NodeInterval = 0.08f;
    [SerializeField] float mf32_HumanRadius = 0.3f;
    [SerializeField] float mf32_NodeConnectionCriterion = 0.19f;
    [SerializeField] float mf32_CostExpandDistance = 1.0f;
    [SerializeField] float mf32_MaxCostRatio = 2.0f;

    [SerializeField] private GameObject mGO_Human;    
    [SerializeField] private Vector3 mv3_Goal;

    // Start is called before the first frame update
    void Start()
    {
        mo_GraphHandler = new GraphHandler(mGO_Floor, mf32_NodeInterval, mf32_HumanRadius);
        mo_GraphHandler.BuildGraph(mf32_NodeConnectionCriterion, mL_Obstacles);

        GraphPlanner o_GraphPlanner = new GraphPlanner(mo_GraphHandler);
        List<Vector3> L_Waypoints = o_GraphPlanner.Dijkstra(mGO_Human.transform.position, mv3_Goal);
        
        LLC HumanController = mGO_Human.GetComponent<LLC>();
        HumanController.SetWaypoints(L_Waypoints);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
