using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ObstacleShape{Rectangle, Circle, CustomCircle, CustomRectangle}
public enum ObstacleControl{GetEnum, GetTranform}
public class Obstacle : MonoBehaviour
{
    private List<LineObstacle> mL_LineObstacles = new List<LineObstacle>();
    public ObstacleShape me_Shape = ObstacleShape.Circle;
    
    [SerializeField] private float mf32_length_x;
    public float length_x{
        get{
            return mf32_length_x;
        }
    }
    [SerializeField] private float mf32_length_z;
    public float length_z{
        get{
            return mf32_length_z;
        }
    }
    [SerializeField] private float mf32_radius;
    public float radius{
        get{
            return mf32_radius;
        }
    }

    private GameObject mGO_Obstacle;
    private Transform mTF_Obstacle;
    public Transform tranform{
        get{
            return mTF_Obstacle;
        }
    }

    private List<Vector3> mL_CollisionFootprints = new List<Vector3>();
    public List<Vector3> CollisionFootprints{
        get{
            return mL_CollisionFootprints;
        }
    }
    private List<(Vector3, float)> mL_CostFootprints = new List<(Vector3, float)>();
    public List<(Vector3, float)> CostFootprints{
        get{
            return mL_CostFootprints;
        }
    }
    // Start is called before the first frame update
    void Awake()
    {
        mGO_Obstacle = this.gameObject;
        mTF_Obstacle = mGO_Obstacle.transform;
        SetGeometryInfo();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SetGeometryInfo(){
        switch(me_Shape){
            case ObstacleShape.Rectangle:
                mf32_length_x = mTF_Obstacle.localScale.x;
                mf32_length_z = mTF_Obstacle.localScale.z;
                break;
            case ObstacleShape.Circle:
                mf32_radius = mTF_Obstacle.localScale.x / 2;
                break;
            case ObstacleShape.CustomRectangle:
                mf32_length_x *= mTF_Obstacle.localScale.x;
                mf32_length_z *= mTF_Obstacle.localScale.z;
                me_Shape = ObstacleShape.Rectangle;
                break;
            case ObstacleShape.CustomCircle:
                mf32_radius *= mTF_Obstacle.localScale.x;
                me_Shape = ObstacleShape.Circle;
                break;
        }
    }

    public void CalculateFootprint(float f32_NodeInterval, float f32_HumanRadius, float f32_CostExpansionDistance){
        float f32_FootprintInterval = f32_NodeInterval / 2;
        switch(me_Shape){
            case ObstacleShape.Rectangle:
                int s32_NumFootprintX = (int) ((mf32_length_x + 2 * f32_HumanRadius) / f32_FootprintInterval);
                int s32_NumFootprintZ =  (int) ((mf32_length_z + 2 * f32_HumanRadius) / f32_FootprintInterval);

                float f32_FootprintIntervalX = (mf32_length_x + 2 * f32_HumanRadius) / s32_NumFootprintX;
                float f32_FootprintIntervalZ = (mf32_length_z + 2 * f32_HumanRadius) / s32_NumFootprintZ;

                Vector3 v3_LeftRearCorner = mTF_Obstacle.position - mTF_Obstacle.forward * mf32_length_z / 2 
                                                                    - mTF_Obstacle.right * mf32_length_x / 2;
                Vector3 v3_RightRearCorner = mTF_Obstacle.position - mTF_Obstacle.forward * mf32_length_z / 2 
                                                                    + mTF_Obstacle.right * mf32_length_x / 2;
                Vector3 v3_LeftFrontCorner = mTF_Obstacle.position + mTF_Obstacle.forward * mf32_length_z / 2 
                                                                    - mTF_Obstacle.right * mf32_length_x / 2;
                Vector3 v3_RightFrontCorner = mTF_Obstacle.position + mTF_Obstacle.forward * mf32_length_z / 2 
                                                                    + mTF_Obstacle.right * mf32_length_x / 2;

                // edge
                Vector3 v3_Base = v3_LeftRearCorner - mTF_Obstacle.forward * f32_HumanRadius - mTF_Obstacle.right * f32_HumanRadius;
                int s32_NumFootprintInRaidus_X = 1 + (int) (f32_HumanRadius / f32_FootprintIntervalX);
                int s32_NumFootprintInRaidus_Z = 1 + (int) (f32_HumanRadius / f32_FootprintIntervalZ);
                for (int i = s32_NumFootprintInRaidus_X; i < s32_NumFootprintX + 1 - s32_NumFootprintInRaidus_X; i++){
                    for(int j = 0; j < s32_NumFootprintZ + 1; j++){
                        Vector3 v3_Footprint = v3_Base + i * f32_FootprintIntervalX * mTF_Obstacle.right + j * f32_FootprintIntervalZ * mTF_Obstacle.forward;
                        mL_CollisionFootprints.Add(v3_Footprint); 
                    }
                }
                for (int i = 0; i < s32_NumFootprintInRaidus_X; i++){
                    for(int j = s32_NumFootprintInRaidus_Z; j < s32_NumFootprintZ + 1 - s32_NumFootprintInRaidus_Z; j++){
                        Vector3 v3_Footprint = v3_Base + i * f32_FootprintIntervalX * mTF_Obstacle.right + j * f32_FootprintIntervalZ * mTF_Obstacle.forward;
                        mL_CollisionFootprints.Add(v3_Footprint); 
                    }
                }
                for (int i = s32_NumFootprintX + 1 - s32_NumFootprintInRaidus_X; i < s32_NumFootprintX + 1; i++){
                    for(int j = s32_NumFootprintInRaidus_Z; j < s32_NumFootprintZ + 1 - s32_NumFootprintInRaidus_Z; j++){
                        Vector3 v3_Footprint = v3_Base + i * f32_FootprintIntervalX * mTF_Obstacle.right + j * f32_FootprintIntervalZ * mTF_Obstacle.forward;
                        mL_CollisionFootprints.Add(v3_Footprint); 
                    }
                }

                int s32_CostExpansionX = (int) (f32_CostExpansionDistance / f32_FootprintIntervalX);
                int s32_CostExpansionZ = (int) (f32_CostExpansionDistance / f32_FootprintIntervalZ);

                for (int i = s32_NumFootprintInRaidus_X; i < s32_NumFootprintX + 1 - s32_NumFootprintInRaidus_X; i++){
                    for(int j = 1; j < s32_CostExpansionZ + 1; j++){
                        Vector3 v3_Footprint = v3_Base + i * f32_FootprintIntervalX * mTF_Obstacle.right + (s32_NumFootprintZ + j) * f32_FootprintIntervalZ * mTF_Obstacle.forward;
                        mL_CostFootprints.Add((v3_Footprint, j * f32_FootprintIntervalZ)); 
                        v3_Footprint = v3_Base + i * f32_FootprintIntervalX * mTF_Obstacle.right + (-j) * f32_FootprintIntervalZ * mTF_Obstacle.forward;
                        mL_CostFootprints.Add((v3_Footprint, j * f32_FootprintIntervalZ)); 
                    }
                }

                for (int i = 1; i < s32_CostExpansionX + 1; i++){
                    for(int j = s32_NumFootprintInRaidus_Z; j < s32_NumFootprintZ + 1 - s32_NumFootprintInRaidus_Z; j++){
                        Vector3 v3_Footprint = v3_Base + (s32_NumFootprintX + i) * f32_FootprintIntervalX * mTF_Obstacle.right + j * f32_FootprintIntervalZ * mTF_Obstacle.forward;
                        mL_CostFootprints.Add((v3_Footprint, i * f32_FootprintIntervalX)); 
                        v3_Footprint = v3_Base + (-i) * f32_FootprintIntervalX * mTF_Obstacle.right + j * f32_FootprintIntervalZ * mTF_Obstacle.forward;
                        mL_CostFootprints.Add((v3_Footprint, i * f32_FootprintIntervalX)); 
                    }
                }
                // corner
                int s32_NumFootprintInRadius = (int) (f32_HumanRadius / f32_FootprintInterval);
                float f32_FootprintIntervalRadius = f32_HumanRadius / s32_NumFootprintInRadius;
                for (int i = 1; i < s32_NumFootprintInRadius + 1; i++){
                    float dtheta = Mathf.PI / (4*i);
                    float f32_r = f32_FootprintIntervalRadius * i;
                    for (int j = 0; j < 2 * i; j++){
                        Vector3 v3_Footprint = v3_RightFrontCorner + mTF_Obstacle.forward * f32_r * Mathf.Cos(dtheta * j) + mTF_Obstacle.right * f32_r * Mathf.Sin(dtheta * j);
                        mL_CollisionFootprints.Add(v3_Footprint);

                        v3_Footprint = v3_LeftFrontCorner + mTF_Obstacle.forward * f32_r * Mathf.Cos(dtheta * j) - mTF_Obstacle.right * f32_r * Mathf.Sin(dtheta * j);
                        mL_CollisionFootprints.Add(v3_Footprint);

                        v3_Footprint = v3_RightRearCorner - mTF_Obstacle.forward * f32_r * Mathf.Cos(dtheta * j) + mTF_Obstacle.right * f32_r * Mathf.Sin(dtheta * j);
                        mL_CollisionFootprints.Add(v3_Footprint);

                        v3_Footprint = v3_LeftRearCorner - mTF_Obstacle.forward * f32_r * Mathf.Cos(dtheta * j) - mTF_Obstacle.right * f32_r * Mathf.Sin(dtheta * j);
                        mL_CollisionFootprints.Add(v3_Footprint); 
                    }
                }

                int s32_CostExpansionRadius = (int) (f32_CostExpansionDistance / f32_FootprintIntervalRadius);
                for (int i = 1; i < s32_CostExpansionRadius + 1; i++){
                    float dtheta = Mathf.PI / (4 * (i + s32_NumFootprintInRadius));
                    float f32_r = f32_FootprintIntervalRadius * (i + s32_NumFootprintInRadius);

                    for (int j = 0; j < 2 * (i + s32_NumFootprintInRadius); j++){
                        Vector3 v3_Footprint = v3_RightFrontCorner + mTF_Obstacle.forward * f32_r * Mathf.Cos(dtheta * j) + mTF_Obstacle.right * f32_r * Mathf.Sin(dtheta * j);
                        mL_CostFootprints.Add((v3_Footprint, i * f32_FootprintIntervalRadius));

                        v3_Footprint = v3_LeftFrontCorner + mTF_Obstacle.forward * f32_r * Mathf.Cos(dtheta * j) - mTF_Obstacle.right * f32_r * Mathf.Sin(dtheta * j);
                        mL_CostFootprints.Add((v3_Footprint, i * f32_FootprintIntervalRadius));

                        v3_Footprint = v3_RightRearCorner - mTF_Obstacle.forward * f32_r * Mathf.Cos(dtheta * j) + mTF_Obstacle.right * f32_r * Mathf.Sin(dtheta * j);
                        mL_CostFootprints.Add((v3_Footprint, i * f32_FootprintIntervalRadius));

                        v3_Footprint = v3_LeftRearCorner - mTF_Obstacle.forward * f32_r * Mathf.Cos(dtheta * j) - mTF_Obstacle.right * f32_r * Mathf.Sin(dtheta * j);
                        mL_CostFootprints.Add((v3_Footprint, i * f32_FootprintIntervalRadius)); 
                    }
                }

                break;
            case ObstacleShape.Circle:
                
                s32_NumFootprintInRadius = (int) (f32_HumanRadius + mf32_radius / f32_FootprintInterval);
                f32_FootprintIntervalRadius = (f32_HumanRadius + mf32_radius) / s32_NumFootprintInRadius;
                for (int i = 1; i < s32_NumFootprintInRadius + 1; i++){
                    float dtheta = Mathf.PI / (4*i);
                    float f32_r = f32_FootprintIntervalRadius * i;
                    for (int j = 0; j < 8 * i; j++){
                        Vector3 v3_Footprint = mTF_Obstacle.position + mTF_Obstacle.forward * f32_r * Mathf.Cos(dtheta * j) + mTF_Obstacle.right * f32_r * Mathf.Sin(dtheta * j);
                        mL_CollisionFootprints.Add(v3_Footprint); 
                    }
                }

                s32_CostExpansionRadius = (int) (f32_CostExpansionDistance / f32_FootprintIntervalRadius);
                for (int i = 1; i < s32_CostExpansionRadius + 1; i++){
                    float dtheta = Mathf.PI / (4 * (i + s32_NumFootprintInRadius));
                    float f32_r = f32_FootprintIntervalRadius * (i + s32_NumFootprintInRadius);

                    for (int j = 0; j < 8 * (i + s32_NumFootprintInRadius); j++){
                        Vector3 v3_Footprint = mTF_Obstacle.position + mTF_Obstacle.forward * f32_r * Mathf.Cos(dtheta * j) + mTF_Obstacle.right * f32_r * Mathf.Sin(dtheta * j);
                        mL_CostFootprints.Add((v3_Footprint, i * f32_FootprintIntervalRadius));
                    }
                }
                break;
        }
    }
}

public class LineObstacle
{
    Vector3 mv3_StartPosition;
    Vector3 mv3_EndPosition;
    public LineObstacle(Vector3 v3_StartPosition, Vector3 v3_EndPosition){
        mv3_StartPosition = v3_StartPosition;
        mv3_EndPosition = v3_EndPosition;
    }
}
