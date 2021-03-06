﻿using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Net.Sockets;

public class TCPConnection : MonoBehaviour
{

    // The name of the connection, not required but better for overview if you have more than 1 connections running
    public string conName = "Localhost";

    // ip address of the server, 127.0.0.1 is for your own computer
    public string conHost = "127.0.0.1";

    // Port for the server, make sure to unblock this in your router firewall if you want to allow external connections
    public int conPort = 5005;

    // a true/false variable for connection status
    public bool socketReady = false;

    TcpClient mySocket;
    NetworkStream theStream;
    StreamWriter theWriter;
    StreamReader theReader;

    // Tries to initiate connection
    public void setupSocket()
    {
        try
        {
            mySocket = new TcpClient(conHost, conPort);
            theStream = mySocket.GetStream();
            theWriter = new StreamWriter(theStream);
            theReader = new StreamReader(theStream);
            socketReady = true;
        }
        catch (Exception e)
        {
            Debug.Log("Socket error:" + e);
        }
    }

    // Sends a message to server
    public void writeSocket(string theLine)
    {
        if (!socketReady)
            return;
        String tmpString = theLine + "\r";
        theWriter.Write(tmpString);
        theWriter.Flush();
    }

    // Reads the message from server
    public string readSocket()
    {
        String result = "";
        if (theStream.DataAvailable)
        {
            Byte[] inStream = new Byte[mySocket.SendBufferSize];
            theStream.Read(inStream, 0, inStream.Length);
            result += System.Text.Encoding.UTF8.GetString(inStream);
        }
        return result;
    }

    // Disconnects from the socket (Not used)
    public void closeSocket()
    {
        if (!socketReady)
            return;
        theWriter.Close();
        theReader.Close();
        mySocket.Close();
        socketReady = false;
    }

    // Keeps connection alive, reconnect if connection lost
    public void maintainConnection()
    {
        if (!theStream.CanRead)
        {
            setupSocket();
        }
    }


}