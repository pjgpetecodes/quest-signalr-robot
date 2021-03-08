using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotControl : MonoBehaviour
{    

    public GameObject robot;
    private SignalRController robotSignalRController;
    public string Rotation;
    public string Reach;
    public string Grab;

    // Start is called before the first frame update
    void Start()
    {
        robotSignalRController = robot.GetComponent<SignalRController>();
    }

    public void SetRotation(float value)
    {
        Rotation = value.ToString();
        Debug.Log("Rotation is " + value.ToString() );

        robotSignalRController.SetRotation(value);
    }

    public void SetReach(float value)
    {
            
        Reach = value.ToString();
        Debug.Log("Reach is " + value.ToString() );
        
        robotSignalRController.SetReach(value);
    }

    public void SetGrab(float value)
    {
        Grab = value.ToString();
        Debug.Log("Grab is " + value.ToString() );
        robotSignalRController.SetGrab(value);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
