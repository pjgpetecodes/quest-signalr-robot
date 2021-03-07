using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.AspNetCore.SignalR.Client;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

public class RobotControl : MonoBehaviour
{

    private static HubConnection connection;
    private GameObject robotBody;
    private GameObject robotArm;

    // Start is called before the first frame update
    void Start()
    {

        StartSignalR();

        robotBody =  GameObject.Find("Robot-Body");
        robotArm =  GameObject.Find("Robot-Arm");

         //Application.OpenURL("https://bcsrobotdevuksapp.azurewebsites.net/");
        
    }

    public void SetRotation(float value)
    {
       if (connection == null)
       {
            StartSignalR();
        }
            
        Debug.Log("Rotation is " + value.ToString() );
        connection.SendAsync("SendMessage", "servo1", value.ToString());

        Rotation = value.ToString();

        robotBody.transform.Rotate(0.0f, value, 0.0f, Space.Self);
    }

    public void SetReach(float value)
    {
       if (connection == null)
       {
            StartSignalR();
        }
            
        Debug.Log("Reach is " + value.ToString() );
        connection.SendAsync("SendMessage", "servo2", value.ToString());

        Reach = value.ToString();

        robotArm.transform.Rotate(0.0f, 0.0f, value, Space.Self);
    }

    public void SetGrab(float value)
    {
       if (connection == null)
       {
            StartSignalR();
        }
            
        Debug.Log("Grab is " + value.ToString() );
        connection.SendAsync("SendMessage", "servo3", value.ToString());
        Grab = value.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
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
            
        });

        await connection.StartAsync();
    }

    public string Rotation;
    public string Reach;
    public string Grab;
}
