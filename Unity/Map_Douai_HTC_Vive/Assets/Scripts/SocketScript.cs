using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;
using System.Linq;

public class SocketScript : MonoBehaviour
{
    // Variables
    private TCPConnection myTCP;
    public ObjectCreation Creation;
    private string serverMsg;
    public string msgToServer;


    void Awake()
    {
        // Adds a copy of TCPConnection to this game object
        myTCP = gameObject.AddComponent<TCPConnection>();
    }
    

    void Start()
    {
        
    }
    

    void Update()
    {
        // Keeps checking the server for messages, if a message is received from server, it gets logged in the Debug console (see function below)
        SocketResponse();

    }
    

    void OnGUI()
    {
        // If no connection has been made, displays button to connect
        if (myTCP.socketReady == false)
        {
            
            if (GUILayout.Button("Connect"))
            {
                // Tries to connect
                Debug.Log("Attempting to connect..");
                myTCP.setupSocket();
            }

        }
        
        // Once a connection has been made, displays editable text field with a button to send that string to the server (see function below)
        if (myTCP.socketReady == true)
        {
            msgToServer = GUILayout.TextField(msgToServer);

            if (GUILayout.Button("Write to server", GUILayout.Height(30)))
            {
                SendToServer(msgToServer);
            }
        }

    }

    // Socket reading script
    void SocketResponse()
    {
        string serverSays = myTCP.readSocket();

        if (serverSays != "")
        {
            // Spliting the String to seperate the vehicles. The last element from temp is removed as it is always EOF.
            String[] temp = serverSays.Split('|');
            String[] elements = new String[temp.Length - 1];
            Array.Copy(temp, elements, temp.Length - 1);
            foreach (String element in elements)
            {
                if (!element.Equals(""))
                    Creation.UpdateVehicule(element);
            }
        }
    }
    
    // Sends a message to the server
    public void SendToServer(string str)
    {
        myTCP.writeSocket(str);
        Debug.Log("[CLIENT] -> " + str);
    }

}
