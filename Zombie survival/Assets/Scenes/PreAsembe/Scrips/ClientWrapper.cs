using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class ClientWrapper
{

    private UdpClient udpClient;
    private IPEndPoint serverEndpoint;
    private string clientId;
    private Dictionary<string, Action<string>> eventHandlers = new Dictionary<string, Action<string>>();

    public ClientWrapper(string serverIpAddress, int serverPort, string clientId)
    {
        this.clientId = clientId;
        udpClient = new UdpClient();
        serverEndpoint = new IPEndPoint(IPAddress.Parse(serverIpAddress), serverPort);
        Thread receiveThread = new Thread(ReceiveLoop);
        receiveThread.Start();


    }
    public void On(string eventName, Action<string> handler)
    {
        eventHandlers[eventName] = handler;
    }

    internal void Close()
    {
        udpClient.Close();
    }

    public void Send(string eventName, string data)
    {
        string message = $"{eventName}|{data}";
        byte[] messageBytes = Encoding.UTF8.GetBytes(message);
        udpClient.Send(messageBytes, messageBytes.Length, serverEndpoint);
    }
    public void SendHealthCheck()
    {
        string message = "heartbeat";
        byte[] messageBytes = Encoding.UTF8.GetBytes(message);
        udpClient.Send(messageBytes, messageBytes.Length, serverEndpoint);
    }
    public IEnumerator HealthCheck()
    {
        while (true)
        {

            SendHealthCheck();


            yield return new WaitForSeconds(4);

        }
    }
    public void ReceiveLoop()
    {

        while (true)
        {
            try
            {
                byte[] receivedData = udpClient.Receive(ref serverEndpoint);

                IPEndPoint remoteEndpoint = new IPEndPoint(IPAddress.Any, 0);
                string message = Encoding.UTF8.GetString(receivedData);

                // Parsing received data format: "eventName|data"
                string[] parts = message.Split('|');
                if (parts.Length >= 2)
                {
                    string eventName = parts[0];
                    string eventData = parts[1];


                    Debug.Log($"Received event '{eventName}' from {remoteEndpoint}: {eventData}");

                    if (eventHandlers.TryGetValue(eventName, out Action<string> handler))
                    {
                        handler(eventData);
                    }
                }
            }
            catch (Exception)
            {

                ReceiveLoop();
            }
        }

    }
}

