using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using System;
using UnityEngine.UI;

public enum HumanAlgo{SocialForce, ORCA, MAC_ID}
public class HumanController : Agent
{

    [SerializeField] private List<HumanHandler> mL_HumanHandler = new List<HumanHandler>();
    
    public override void Initialize(){
        
    }

    private void StartNewGame(){

    }

    public override void OnEpisodeBegin(){

    }
    public override void CollectObservations(Unity.MLAgents.Sensors.VectorSensor sensor)
    {
    }

    public override void OnActionReceived(ActionBuffers actions)
    {

        MoveAgent(actions.ContinuousActions);

    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {

    }
    public void MoveAgent(ActionSegment<float> act)
    { 
    }
    IEnumerator RevertMaterial()
    {
        yield return new WaitForSeconds(0.2f);

    }
}