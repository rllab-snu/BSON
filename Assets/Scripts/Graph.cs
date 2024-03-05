using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// INF = 1000000;

public class NodeHandler{
    private Vector3 mv3_Position;
    private List<NodeHandler> mL_ChildNodes = new List<NodeHandler>();
    private List<float> mL_Distance2Child = new List<float>();
    public bool mb_Occupied = false;
    public float mf32_DistanceFromObstacle;

    public NodeHandler mo_Parent = null;
    public float mf32_Distance = 10000.0f;
    public bool mb_Visited = false;

    public NodeHandler(Vector3 v3_Position, float f32_InitialDistanceFromObstacle){
        mv3_Position = v3_Position;
        mf32_DistanceFromObstacle = f32_InitialDistanceFromObstacle;
    }

    public Vector3 GetPosition(){
        return mv3_Position;
    }

    public float GetDistance(NodeHandler o_Node){
        return Vector3.Distance(mv3_Position, o_Node.GetPosition());
    }

    public float GetDistance(Vector3 v3_Position){
        return Vector3.Distance(mv3_Position, v3_Position);
    }

    public void AddChild(NodeHandler o_Node, float f32_Distance){
        mL_ChildNodes.Add(o_Node);
        mL_Distance2Child.Add(f32_Distance);
    }

    public List<NodeHandler> GetChildNodes(){
        return mL_ChildNodes;
    }

    public List<float> GetDistance2Child(){
        return mL_Distance2Child;
    }

    public void Clear(){
        mo_Parent = null;
        mf32_Distance = 10000.0f;
        mb_Visited = false;
    }

    public void Reset(){
        mL_ChildNodes.Clear();
        mL_Distance2Child.Clear();
        mb_Occupied = false;
    }
}

public class GraphHandler{

    private List<List<NodeHandler>> mL_Nodes = new List<List<NodeHandler>>();
    private float mf32_NodeConnectionCriterion;
    private float mf32_NodeInterval;
    private float mf32_HumanRadius;
    private float mf32_CostExpandDistance = 1.0f;

    private float mf32_minX;
    private float mf32_minZ;
    private float mf32_maxX;
    private float mf32_maxZ;
    
    public GraphHandler(GameObject GO_Floor, float f32_NodeInterval, float f32_HumanRadius){
        mf32_NodeInterval = f32_NodeInterval;
        mf32_HumanRadius = f32_HumanRadius + 0.05f;
        // Assume that floor gameobject is a plane
        Transform TF_Floor = GO_Floor.transform;
        Vector3 v3_LeftDownCorner = TF_Floor.position - new Vector3(5.0f * TF_Floor.localScale.x, 0.0f, 5.0f * TF_Floor.localScale.z);
        Vector3 v3_RightUpCorner = TF_Floor.position + new Vector3(5.0f * TF_Floor.localScale.x, 0.0f, 5.0f * TF_Floor.localScale.z); 

        mf32_minX = v3_LeftDownCorner.x + mf32_HumanRadius;
        mf32_minZ = v3_LeftDownCorner.z + mf32_HumanRadius;

        mf32_maxX = v3_RightUpCorner.x - mf32_HumanRadius;
        mf32_maxZ = v3_RightUpCorner.z - mf32_HumanRadius;

        for (float f32_NodeX = mf32_minX; f32_NodeX <= mf32_maxX; f32_NodeX += f32_NodeInterval){
            List<NodeHandler> L_NodesInSameRow = new List<NodeHandler>();
            for (float f32_NodeZ = mf32_minZ; f32_NodeZ <= mf32_maxZ; f32_NodeZ += f32_NodeInterval){
                Vector3 v3_NodePosition = new Vector3(f32_NodeX, 0.0f, f32_NodeZ);
                NodeHandler o_Node = new NodeHandler(v3_NodePosition, mf32_CostExpandDistance + f32_NodeInterval);
                L_NodesInSameRow.Add(o_Node);
            }
            mL_Nodes.Add(L_NodesInSameRow);
        }
    }

    private void DrawCircle(NodeHandler o_Node, float f32_Radius = 0.02f){
        bool b_occupied = o_Node.mb_Occupied;
        Vector3 v3_Center = o_Node.GetPosition();
        float f32_dtheta = 2 * Mathf.PI / 8;
        for (int i = 0; i < 8; i ++){
            Vector3 v3_StartPosition = v3_Center + new Vector3(f32_Radius * Mathf.Cos(f32_dtheta * i), 0.0f, f32_Radius * Mathf.Sin(f32_dtheta * i));
            Vector3 v3_EndPosition = v3_Center + new Vector3(f32_Radius * Mathf.Cos(f32_dtheta * (i+1)), 0.0f , f32_Radius * Mathf.Sin(f32_dtheta * (i+1)));
            if (b_occupied){
                Debug.DrawLine(v3_StartPosition, v3_EndPosition, Color.red, 200.0f);
            }
            else if(o_Node.mf32_DistanceFromObstacle <= mf32_CostExpandDistance){
                Debug.DrawLine(v3_StartPosition, v3_EndPosition, Color.green, 200.0f);
            }
            else{
                Debug.DrawLine(v3_StartPosition, v3_EndPosition, Color.black, 200.0f);
            } 
        }
    }

    public void BuildGraph(float f32_NodeConnectionCriterion, List<Obstacle> L_Obstacles){
        mf32_NodeConnectionCriterion = f32_NodeConnectionCriterion;
        int s32_NodeConnectionCriterion = (int) (f32_NodeConnectionCriterion / mf32_NodeInterval);
        DisableNodesOnObstacles(L_Obstacles);
        // pre-calculate the neighbor node idxs
        List<(int, int)> L_NeighborIdxs = new List<(int, int)>();
        for (int i = -s32_NodeConnectionCriterion; i < s32_NodeConnectionCriterion + 1; i++){
            float f32_DistanceX = Mathf.Abs(i) * mf32_NodeInterval;
            float f32_MaxDistanceZ = Mathf.Sqrt(Mathf.Pow(f32_NodeConnectionCriterion, 2) - Mathf.Pow(f32_DistanceX, 2));
            int s32_MaxIdxZ = (int) (f32_MaxDistanceZ / mf32_NodeInterval);
            for (int j = -s32_MaxIdxZ; j < s32_MaxIdxZ + 1; j++){
                if (i == 0 && j == 0) continue;
                L_NeighborIdxs.Add((i, j));
            }
        }

        // connect nodes
        for (int i = 0; i < mL_Nodes.Count; i++){
            for (int j = 0; j < mL_Nodes[i].Count; j++){
                DrawCircle(mL_Nodes[i][j]);
                if (mL_Nodes[i][j].mb_Occupied){
                    continue;
                }
                for (int k = 0; k < L_NeighborIdxs.Count; k++){
                    (int x, int z) Idxs = L_NeighborIdxs[k];
                    if (i + Idxs.x < 0 || i + Idxs.x >= mL_Nodes.Count || j + Idxs.z < 0 || j + Idxs.z >= mL_Nodes[i].Count){
                        continue;
                    }
                    if(mL_Nodes[i + Idxs.x][j + Idxs.z].mb_Occupied) continue;

                    float f32_Distance = Mathf.Sqrt(Idxs.x * Idxs.x + Idxs.z * Idxs.z) * mf32_NodeInterval;
                    int s32_NumPartition = (int) (2 * f32_Distance / mf32_NodeInterval);
                    bool b_EdgeOccupied = false;
                    float f32_MinDistance2Obstacle = mf32_CostExpandDistance + mf32_NodeInterval;

                    for (int s32_PartitionIdx = 0; s32_PartitionIdx < s32_NumPartition + 1; s32_PartitionIdx ++){
                        int s32_PartitionIdxX = (int) Mathf.Round(Idxs.x * s32_PartitionIdx / s32_NumPartition);
                        int s32_PartitionIdxZ = (int) Mathf.Round(Idxs.z * s32_PartitionIdx / s32_NumPartition);

                        if (mL_Nodes[i + s32_PartitionIdxX][j + s32_PartitionIdxZ].mb_Occupied){
                            b_EdgeOccupied = true;
                            break;
                        }
                        f32_MinDistance2Obstacle = Mathf.Min(f32_MinDistance2Obstacle, mL_Nodes[i + s32_PartitionIdxX][j + s32_PartitionIdxZ].mf32_DistanceFromObstacle);
                        if (mL_Nodes[i + s32_PartitionIdxX][j + s32_PartitionIdxZ].mf32_DistanceFromObstacle == 0){
                            Debug.Log(mL_Nodes[i + s32_PartitionIdxX][j + s32_PartitionIdxZ]);
                        }
                    }

                    if (b_EdgeOccupied) continue;

                    mL_Nodes[i][j].AddChild(mL_Nodes[i + Idxs.x][j + Idxs.z], mf32_NodeInterval + f32_Distance * (1 + mf32_CostExpandDistance / f32_MinDistance2Obstacle) / 2);
                    mL_Nodes[i + Idxs.x][j + Idxs.z].AddChild(mL_Nodes[i][j], mf32_NodeInterval + f32_Distance * (1 + mf32_CostExpandDistance / f32_MinDistance2Obstacle) / 2);
                    Debug.DrawLine(mL_Nodes[i][j].GetPosition(), mL_Nodes[i + Idxs.x][j + Idxs.z].GetPosition(), Color.blue, 200.0f);
                }
            }
        }
    }

    private void DisableNodesOnObstacles(List<Obstacle> L_Obstacles){
        foreach (Obstacle o_Obstacle in L_Obstacles){
            o_Obstacle.CalculateFootprint(mf32_NodeInterval, mf32_HumanRadius, mf32_CostExpandDistance);
            
            foreach(Vector3 v3_Footprint in o_Obstacle.CollisionFootprints){
                float f32_x = v3_Footprint.x;
                float f32_z = v3_Footprint.z;
                if (f32_x < mf32_minX || f32_x > mf32_maxX || f32_z < mf32_minZ || f32_z > mf32_maxZ) continue;
                int s32_xIdx = (int) Mathf.Round((f32_x - mf32_minX) / mf32_NodeInterval);
                int s32_zIdx = (int) Mathf.Round((f32_z - mf32_minZ) / mf32_NodeInterval);
                mL_Nodes[s32_xIdx][s32_zIdx].mb_Occupied = true;
                mL_Nodes[s32_xIdx][s32_zIdx].mf32_DistanceFromObstacle = 0.0f;
            }

            foreach((Vector3 v3_Footprint, float f32_Distance) FootprintAndDistance in o_Obstacle.CostFootprints){
                float f32_x = FootprintAndDistance.v3_Footprint.x;
                float f32_z = FootprintAndDistance.v3_Footprint.z;
                if (f32_x < mf32_minX || f32_x > mf32_maxX || f32_z < mf32_minZ || f32_z > mf32_maxZ) continue;
                int s32_xIdx = (int) Mathf.Round((f32_x - mf32_minX) / mf32_NodeInterval);
                int s32_zIdx = (int) Mathf.Round((f32_z - mf32_minZ) / mf32_NodeInterval);
                mL_Nodes[s32_xIdx][s32_zIdx].mf32_DistanceFromObstacle = Mathf.Min(FootprintAndDistance.f32_Distance, mL_Nodes[s32_xIdx][s32_zIdx].mf32_DistanceFromObstacle);
            }

        }
    }

    public NodeHandler GetNearestNode(Vector3 v3_Position){
        float f32_MinDistance = 1000;
        NodeHandler o_NearestNode = mL_Nodes[0][0];
        foreach (List<NodeHandler> L_NodesInRow in mL_Nodes){
            foreach(NodeHandler o_Node in L_NodesInRow){
                float f32_DistanceToNode = o_Node.GetDistance(v3_Position);
                if (f32_DistanceToNode < f32_MinDistance){
                    f32_MinDistance = f32_DistanceToNode;
                    o_NearestNode = o_Node;
                }
            }
        }
        return o_NearestNode;
    }

    // Clear() is called after the planning algorithm is done
    public void Clear(){
        foreach (List<NodeHandler> L_NodesInRow in mL_Nodes){
            foreach(NodeHandler o_Node in L_NodesInRow){
                o_Node.Clear();
            }
        }
    }

    // ResetGraph() is called after each episode to consider new obstacels
    public void ResetGraph(){
        foreach (List<NodeHandler> L_NodesInRow in mL_Nodes){
            foreach(NodeHandler o_Node in L_NodesInRow){
                o_Node.Reset();
            }
        }
    }

}

