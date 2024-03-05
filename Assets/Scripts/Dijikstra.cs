using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphPlanner
{
    private GraphHandler mo_Graph;
    // Start is called before the first frame update
    public GraphPlanner(GraphHandler o_Graph){
        mo_Graph = o_Graph;
    }

    public List<Vector3> Dijkstra(Vector3 v3_StartPosition, Vector3 v3_EndPosition){
        NodeHandler o_StartNode = mo_Graph.GetNearestNode(v3_StartPosition);
        NodeHandler o_EndNode = mo_Graph.GetNearestNode(v3_EndPosition);
        PriorityQueue<NodeHandler> PQ_NodePriorityQueue = new PriorityQueue<NodeHandler>();

        o_StartNode.mf32_Distance = 0.0f;
        o_StartNode.mo_Parent = null;
        PQ_NodePriorityQueue.Enqueue(o_StartNode, o_StartNode.mf32_Distance);

        int s32_count = 0;
        while(PQ_NodePriorityQueue.Count > 0 && !o_EndNode.mb_Visited){
            s32_count ++;
            NodeHandler o_CurrentNode = PQ_NodePriorityQueue.Dequeue();
            if (o_CurrentNode.mb_Visited){
                continue;
            }
            o_CurrentNode.mb_Visited = true;
            List<NodeHandler> L_ChildNodes = o_CurrentNode.GetChildNodes();
            List<float> L_Distance2Child = o_CurrentNode.GetDistance2Child();

            for(int i = 0; i < L_ChildNodes.Count; i++){
                NodeHandler o_ChildNode = L_ChildNodes[i];
                if (o_ChildNode.mb_Visited){
                    continue;
                }
                if (o_ChildNode.mf32_Distance <= L_Distance2Child[i] + o_CurrentNode.mf32_Distance){
                    continue;
                }

                o_ChildNode.mo_Parent = o_CurrentNode;
                o_ChildNode.mf32_Distance = L_Distance2Child[i] + o_CurrentNode.mf32_Distance;
                PQ_NodePriorityQueue.Enqueue(o_ChildNode, o_ChildNode.mf32_Distance);
            }
        }
        
        List<Vector3> L_ShortestPath = new List<Vector3>();

        if(!o_EndNode.mb_Visited){
            Debug.Log("no feasible path!");
        }
        else{
            NodeHandler o_CurrentNode = o_EndNode;
            while(o_CurrentNode != null){
                L_ShortestPath.Insert(0, o_CurrentNode.GetPosition());
                o_CurrentNode = o_CurrentNode.mo_Parent;
            }
        }

        mo_Graph.Clear();

        return L_ShortestPath;
    }
}

public class PriorityQueue<T>
{
    List<T> mL_Elements = new List<T>();
    List<float> mL_Values = new List<float>();

    public int Count{
        get{return mL_Elements.Count;}
    }

    public void Enqueue(T element, float value){
        if (mL_Elements.Count == 0){
            mL_Elements.Add(element);
            mL_Values.Add(value);
        }
        else{
            int s = 0;
            int e = mL_Elements.Count;
            while(e - s > 0){
                int m = (s + e) / 2;
                if (mL_Values[m] <= value){
                    s = m + 1;
                }
                else{
                    e = m;
                }
            }
            mL_Elements.Insert(e, element);
            mL_Values.Insert(e, value);
        }
    }

    public T Dequeue(){
        T PriorElement = mL_Elements[0];
        mL_Elements.RemoveAt(0);
        mL_Values.RemoveAt(0);
        return PriorElement;
    }
}