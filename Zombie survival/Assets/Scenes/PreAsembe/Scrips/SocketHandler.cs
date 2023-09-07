using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class SocketHandler
{
    private UdpClient udpClient;
    private ConcurrentDictionary<IPEndPoint, string> connectedClients = new ConcurrentDictionary<IPEndPoint, string>();
    private Dictionary<string, Action<IPEndPoint, string>> eventHandlers = new Dictionary<string, Action<IPEndPoint, string>>();
    private IPEndPoint serverEndpoint;

    public void Start(int port)
    {
        udpClient = new UdpClient(port);
        Console.WriteLine($"UDP server started on port {port}");

        Thread receiveThread = new Thread(ReceiveLoop);
        receiveThread.Start();
    }
    public void Dispose()
    {

        try
        {
            udpClient.Close();
        }
        catch (Exception)
        {


        }
    }
    public void Connect(string serverIpAddress, int serverPort, string clientId)
    {
        udpClient = new UdpClient();

        serverEndpoint = new IPEndPoint(IPAddress.Parse(serverIpAddress), serverPort);

        Thread receiveThread = new Thread(ReceiveLoop);
        receiveThread.Start();
    }
    public void Emit(IPEndPoint clientEndpoint, string eventName, string data)
    {
        byte[] eventData = Encoding.UTF8.GetBytes($"{eventName}|{data}");
        udpClient.Send(eventData, eventData.Length, clientEndpoint);
    }
    public void Emit(string eventName, string data)
    {
        byte[] eventData = Encoding.UTF8.GetBytes($"{eventName}|{data}");
        udpClient.Send(eventData, eventData.Length, serverEndpoint);
    }
    public void Broadcast(string eventName, string data)
    {
        byte[] eventData = Encoding.UTF8.GetBytes($"{eventName}|{data}");

        foreach (var clientEndpoint in connectedClients.Keys)
        {
            udpClient.Send(eventData, eventData.Length, clientEndpoint);
        }
    }
    public void On(string eventName, Action<IPEndPoint, string> handler)
    {
        eventHandlers[eventName] = handler;
    }
    private void ReceiveLoop()
    {
        while (true)
        {
            IPEndPoint remoteEndpoint;
            if (serverEndpoint == null)
                remoteEndpoint = new IPEndPoint(IPAddress.Any, 0);
            else
            {
                remoteEndpoint = serverEndpoint;
            }
            byte[] receivedData = udpClient.Receive(ref remoteEndpoint);
            string message = Encoding.UTF8.GetString(receivedData);

            // Parsing received data format: "eventName|data"
            string[] parts = message.Split('|');
            if (parts.Length >= 2)
            {
                string eventName = parts[0];
                string eventData = parts[1];

                if (!connectedClients.ContainsKey(remoteEndpoint))
                {
                    connectedClients.TryAdd(remoteEndpoint, eventData);
                    Console.WriteLine($"New client connected: {remoteEndpoint}");
                }

                Console.WriteLine($"Received event '{eventName}' from {remoteEndpoint}: {eventData}");

                if (eventHandlers.TryGetValue(eventName, out Action<IPEndPoint, string> handler))
                {
                    handler(remoteEndpoint, eventData);
                }
            }
        }
    }
}
