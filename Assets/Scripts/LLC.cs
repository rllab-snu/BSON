using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LLC : MonoBehaviour
{
    private GameObject mGO_Object;
    private Transform mTF_Object;

    private List<Vector3> mL_Waypoints = new List<Vector3>();
    private int ms32_WaypointIdx = 0;
    // Start is called before the first frame update
    void Start()
    {
        mGO_Object = this.gameObject;
        mTF_Object = mGO_Object.transform;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Vector3.Distance(mTF_Object.position, mL_Waypoints[ms32_WaypointIdx]) < 0.02f){
            ms32_WaypointIdx = Mathf.Min(ms32_WaypointIdx + 1, mL_Waypoints.Count - 1);
        }
        else{
            mTF_Object.position = mTF_Object.position + 0.02f * (mL_Waypoints[ms32_WaypointIdx] - mTF_Object.position).normalized;
        }
        
    }

    public void SetWaypoints(List<Vector3> L_Waypoints){
        mL_Waypoints = L_Waypoints;
    }
}
