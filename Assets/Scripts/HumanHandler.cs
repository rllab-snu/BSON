using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanHandler : MonoBehaviour
{
    private GameObject mGO_Human;
    private Transform mTF_Human;
    private Rigidbody mRB_Human;
    
    [SerializeField] private bool mb_Cyclic = false;
    [SerializeField] private bool mb_ReachGoal = false;
    [SerializeField] private bool mb_DestroyAfterGoal = false;

    private List<Vector3> mL_Milestones = new List<Vector3>();
    private List<Vector3> mL_Waypoints = new List<Vector3>();

    void Awake()
    {
        mGO_Human = this.gameObject;
        mTF_Human = mGO_Human.transform;
        mRB_Human = mGO_Human.GetComponent<Rigidbody>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Step()
    {
        
    }
}