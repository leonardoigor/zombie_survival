using Microsoft.Extensions.Configuration;
using Server.Entities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public class SocketHandler
{
    private UdpClient udpClient;
    private List<Action<IPEndPoint>> disconnectHandlers = new List<Action<IPEndPoint>>();
    private ConcurrentDictionary<IPEndPoint, string> connectedClients = new ConcurrentDictionary<IPEndPoint, string>();
    private ConcurrentDictionary<IPEndPoint, DateTime> clientHeartbeats = new ConcurrentDictionary<IPEndPoint, DateTime>();
    private Dictionary<string, Action<IPEndPoint, string>> eventHandlers = new Dictionary<string, Action<IPEndPoint, string>>();
    public ConcurrentDictionary<IPEndPoint, UserEntity> players = new ConcurrentDictionary<IPEndPoint, UserEntity>();
    private IPEndPoint serverEndpoint;

    public IConfiguration Configuration { get; }

    public SocketHandler(IConfiguration configuration)
    {
        Configuration = configuration;
    }
    public void Start(int port)
    {
        udpClient = new UdpClient(port);
        Console.WriteLine($"UDP server started on port {port}");

        Thread receiveThread = new Thread(ReceiveLoop);
        receiveThread.Start();
    }
    public void OnDisconnect(Action<IPEndPoint> disconnectHandler)
    {
        disconnectHandlers.Add(disconnectHandler);
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
    public void SendHeartbeat(IPEndPoint clientEndpoint)
    {
        // Send a heartbeat message to the client
        string heartbeatMessage = "heartbeat";
        byte[] heartbeatData = Encoding.UTF8.GetBytes(heartbeatMessage);
        udpClient.Send(heartbeatData, heartbeatData.Length, clientEndpoint);

        // Update the last heartbeat timestamp for the client
        clientHeartbeats.AddOrUpdate(clientEndpoint, DateTime.UtcNow, (_, existingTimestamp) => DateTime.UtcNow);
    }
    public Thread WatcherHealth()
    {
        Thread th = new Thread(async () =>
        {
            while (true)
            {
                //Console.WriteLine($"Check Health of Users");

                CheckClientHealth();
                await Task.Delay(1 * 1000);
            }
        });
        th.Start();
        return th;
    }

    public void CheckClientHealth()
    {
        DateTime now = DateTime.UtcNow;
        var len = clientHeartbeats.Keys.Count;
        Console.WriteLine($"Total of Users Connecteds: {len}");
        int TimeOutConnection = 10;
        if (int.TryParse(Configuration.GetSection("TimeOutConnection").Value, out int v))
        {
            TimeOutConnection = v;
        }

        foreach (var clientEndpoint in clientHeartbeats.Keys)
        {
            var timer = (now - clientHeartbeats[clientEndpoint]).TotalSeconds;
            if (timer > TimeOutConnection) // Adjust the timeout as needed
            {
                // Client has not sent a heartbeat for more than 60 seconds, consider it disconnected
                HandleDisconnect(clientEndpoint);
                Console.WriteLine($"Check Health of User: {clientEndpoint}, Disconnected");


            }
        }
    }
    private void HandleDisconnect(IPEndPoint clientEndpoint)
    {
        if (connectedClients.TryRemove(clientEndpoint, out _))
        {
            Console.WriteLine($"Client disconnected: {clientEndpoint}");
            // Notify registered disconnect handlers
            foreach (var disconnectHandler in disconnectHandlers)
            {
                disconnectHandler(clientEndpoint);
            }
            var heatb = clientHeartbeats.Where(e => e.Key == clientEndpoint).FirstOrDefault();
            clientHeartbeats.TryRemove(heatb);
        }

    }
    private void ReceiveLoop()
    {
        while (true)
        {
            IPEndPoint remoteEndpoint = new IPEndPoint(IPAddress.Any, 0);
            byte[] receivedData = udpClient.Receive(ref remoteEndpoint);
            Console.WriteLine("Get message");
            string message = Encoding.UTF8.GetString(receivedData);

            // Check if it's a heartbeat message
            if (message == "heartbeat")
            {
                // Update the last heartbeat timestamp for the client
                clientHeartbeats.AddOrUpdate(remoteEndpoint, DateTime.UtcNow, (_, existingTimestamp) => DateTime.UtcNow);
            }
            else
            {
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

                    //Console.WriteLine($"Received event '{eventName}' from {remoteEndpoint}: {eventData}");

                    if (eventHandlers.TryGetValue(eventName, out Action<IPEndPoint, string> handler))
                    {
                        handler(remoteEndpoint, eventData);
                    }
                }
                else if (parts.Length == 1 && parts[0] == "disconnect")
                {
                    // Handle a disconnect message
                    HandleDisconnect(remoteEndpoint);
                }
            }
        }
    }

}
