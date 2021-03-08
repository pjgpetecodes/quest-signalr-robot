using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.AspNetCore.SignalR.Client;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System;
using UnityEngine.UI;
using System.Timers;

public class SignalRController : MonoBehaviour
{

    private static HubConnection connection;
    private ArticulationBody baseArticulation;
    private ArticulationBody armArticulation;
    private RobotController robotController;

    public GameObject robot;
    public string Rotation;
    public string Reach;
    public string Grab;

    private bool remoteMoveRotation = false;
    private bool remoteMoveReach = false;
    private bool remoteMoveGrab = false;

    private System.Timers.Timer disableRotationTimer = new System.Timers.Timer(1000);
    private System.Timers.Timer disableReachTimer = new System.Timers.Timer(1000);
    private System.Timers.Timer disableGrabTimer = new System.Timers.Timer(1000);

    // Start is called before the first frame update
    void Start()
    {

        StartSignalR();
       
         //Application.OpenURL("https://bcsrobotdevuksapp.azurewebsites.net/");
        
    }

    public void SetRotation(float value)
    {
        Rotation = value.ToString();
        Debug.Log("Rotation is " + value.ToString() );

        if (remoteMoveRotation == false)
        {
            if (connection == null)
            {
                StartSignalR();
            }
            
            connection.SendAsync("SendMessage", "servo1", value.ToString());        
            setRemoteRotate((int)value);
        }
        else
        {
            remoteMoveRotation = false;            
        }

        //robotBody.transform.Rotate(0.0f, value, 0.0f, Space.Self);
    }

    public void SetReach(float value)
    {
            
        Reach = value.ToString();
        Debug.Log("Reach is " + value.ToString() );
        
        if (remoteMoveReach == false)
        {
            if (connection == null)
            {
                StartSignalR();
            }
        
            connection.SendAsync("SendMessage", "servo2", value.ToString());
            setRemoteReach((int)value);
        }
        else
        {
            remoteMoveReach = false;            
        }

        //robotArm.transform.Rotate(0.0f, 0.0f, value, Space.Self);
    }

    public void SetGrab(float value)
    {
        Grab = value.ToString();
        Debug.Log("Grab is " + value.ToString() );

        if (remoteMoveGrab == false)
        {
            if (connection == null)
            {
                StartSignalR();
            }            

            connection.SendAsync("SendMessage", "servo3", value.ToString());
            setRemoteGrab((int)value);
        }
        else
        {
            remoteMoveGrab = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void setRemoteRotate(int value)
    {
        var drive = baseArticulation.xDrive;
        drive.target = value;
        baseArticulation.xDrive = drive;

        remoteMoveRotation = true;
        GameObject.Find("RotateSlider").GetComponent<Slider>().value = value;
    }

    void setRemoteReach(int value)
    {
        var drive = armArticulation.xDrive;
        drive.target = value-90;
        armArticulation.xDrive = drive;

        remoteMoveReach = true;
        GameObject.Find("ReachSlider").GetComponent<Slider>().value = value;
        
    }

    void setRemoteGrab(int value)
    {
        float grabValue;

        grabValue = (float)value;
        grabValue = (grabValue-75)/105;

        robotController.joints[6].robotPart.GetComponent<PincherController>().grip = grabValue;
        GameObject.Find("GrabSlider").GetComponent<Slider>().value = value;
        Debug.Log("Grab at: " + grabValue);

    }

    void rotationDisableTimerExpired(System.Object source, ElapsedEventArgs e) {
        GameObject.Find("RotateSlider").GetComponent<Slider>().interactable = true;
    }

    void reachDisableTimerExpired(System.Object source, ElapsedEventArgs e) {
        GameObject.Find("ReachSlider").GetComponent<Slider>().interactable = true;
    }

    void grabDisableTimerExpired(System.Object source, ElapsedEventArgs e) {
        GameObject.Find("GrabSlider").GetComponent<Slider>().interactable = true;
    }

    async void StartSignalR() {

        connection = new HubConnectionBuilder()
            .WithUrl("https://bcsrobotdevuksapp.azurewebsites.net/chathub")
            .WithAutomaticReconnect()                
            .Build();

        connection.On<string, string>("ReceiveMessage", (user, message) =>
        {
            var encodedMsg = $"{user}: {message}";
            Debug.Log(encodedMsg);

            switch (user)
            {
                case "servo1":

                    setRemoteRotate(Int32.Parse(message));
                    remoteMoveRotation = true;
                    GameObject.Find("RotateSlider").GetComponent<Slider>().interactable = false;
                    disableRotationTimer.AutoReset = false;
                    disableRotationTimer.Start();
                    break;

                case "servo2":

                    setRemoteReach(Int32.Parse(message));
                    remoteMoveReach = true;
                    GameObject.Find("ReachSlider").GetComponent<Slider>().interactable = false;
                    disableReachTimer.AutoReset = false;
                    disableReachTimer.Start();
                    break;

                case "servo3":

                    setRemoteGrab(Int32.Parse(message));
                    remoteMoveGrab = true;
                    GameObject.Find("GrabSlider").GetComponent<Slider>().interactable = false;
                    disableGrabTimer.AutoReset = false;
                    disableGrabTimer.Start();
                    
                    break;
            }
            
        });

        robotController = robot.GetComponent<RobotController>();
        baseArticulation = robotController.joints[0].robotPart.GetComponent<ArticulationBody>();
        armArticulation = robotController.joints[1].robotPart.GetComponent<ArticulationBody>();
        
        disableRotationTimer.Elapsed += rotationDisableTimerExpired;
        disableReachTimer.Elapsed += reachDisableTimerExpired;
        disableGrabTimer.Elapsed += grabDisableTimerExpired;

        await connection.StartAsync();
    }


}
