using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace GameServer
{
    class Server
    {
        // Variables to hold the max players and port number
        public static int capacity;
        public static int port;

        // Dictionaries to store and handle data from multiple clients
        public static Dictionary<int, ClientData> clients = new Dictionary<int, ClientData>();

        // Create a packet and a dictionary to store each clients packet
        public static Dictionary<int, Packet> packets = new Dictionary<int, Packet>();
        public static Packet tempPacket;

        // Listeners for recieving data from clients
        private static TcpListener tcpListener;
        private static UdpClient udpListener;

        // Start the server
        public static void Start(int playerCount, int portNo)
        {
            // Store player capacity and port number
            capacity = playerCount;
            port = portNo;

            // Initialise the packets
            InitPackets();

            // Initialise the server
            InitServerData();
            tcpListener = new TcpListener(IPAddress.Any, port);

            // Create a listener for tcp
            tcpListener.Start();
            tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnect), null);

            // Creates a listener for udp
            udpListener = new UdpClient(port);
            udpListener.BeginReceive(UDPReceive, null);

            // Tell the user that the server has been successfully
            Console.WriteLine($"Server Started on {port} successfully with a max player count of {capacity}!");
        }

        // Function to connect a client through TCP
        private static void TCPConnect(IAsyncResult result)
        {
            // Wait for an incoming connection. This fuction is called asyncronously so it doesn't hold up the other code
            TcpClient client = tcpListener.EndAcceptTcpClient(result);

            // Once a client is accepted call back to the finction to wait for another connection asyncronous
            tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnect), null);
            Console.WriteLine($"Incoming conection from {client.Client.RemoteEndPoint}...");

            // If not at capacity accept a player on the highest open slot
            for (int i = 1; i <= capacity; i++)
            {
                if (clients[i].tcp.socket == null)
                {
                    clients[i].tcp.Connect(client);
                    return;
                }
            }

            // If the client tries to connect but the server was full output a message
            Console.WriteLine($"{client.Client.RemoteEndPoint} failed to connect: Server full!");
        }

        // Receive data 
        // Ran asyncrenously and calls back to itself.Essentialy waits until a udp packet come in and then handles it before accepting another
        private static void UDPReceive(IAsyncResult result)
        {
            try
            {
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);
                byte[] _data = udpListener.EndReceive(result, ref endPoint);

                // if length of the message is less than 4 then it is empty and nothing was received
                if (_data.Length < 4)
                {
                    return;
                }

                // Read in the new packet and assign it to the correct client
                tempPacket.NewRead(_data);
                int clientId = tempPacket.reader.ReadInt();

                // If client id is 0 then we don't know who it came from
                if (clientId == 0)
                {
                    return;
                }

                // Set up the connection to the client if not currently recognised
                if (clients[clientId].udp.endPoint == null)
                {
                    clients[clientId].udp.Connect(endPoint);
                    packets[clientId] = tempPacket;
                    udpListener.BeginReceive(UDPReceive, null);
                    return;
                }

                // If the user has already been set up then make sure it's the correct client then handle the data
                if (clients[clientId].udp.endPoint.ToString() == endPoint.ToString())
                {
                    float newSec = tempPacket.reader.ReadInt();
                    float newMil = tempPacket.reader.ReadInt();

                    if (clients[clientId].lastSec != -1)
                    {
                        // If new second isn't behind the last recieved on
                        // OR if it's reset after getting to 60 it should be less than the last one minus 30 seconds
                        if (newSec >= clients[clientId].lastSec || newSec < clients[clientId].lastSec - 30)
                        {
                            // Same for miliseconds as above
                            if (newMil > clients[clientId].lastMil || newMil < clients[clientId].lastMil - 500)
                            {
                                packets[clientId] = tempPacket;
                                clients[clientId].lastSec = newSec;
                                clients[clientId].lastMil = newMil;
                                clients[clientId].udp.HandleData();
                            }
                        }
                    }
                    else
                    {
                        clients[clientId].lastSec = newSec;
                        clients[clientId].lastMil = newMil;
                    }
                }

                // Callback
                udpListener.BeginReceive(UDPReceive, null);
            }
            catch (Exception _ex)
            {
                Console.WriteLine($"Error receiving UDP data from client: {_ex}");
            }
        }

        // Send data through UDP
        public static void SendUDPData(int clientId, IPEndPoint _clientEndPoint, Packet _packet)
        {
            try
            {
                // So long as the client end point exists send data to them
                if (_clientEndPoint != null)
                {
                    udpListener.BeginSend(packets[clientId].writer.GetPacket(), packets[clientId].writer.GetPacket().Length, _clientEndPoint, null, null);
                }
            }
            catch (Exception _ex)
            {
                Console.WriteLine($"Error sending data to {_clientEndPoint} via UDP: {_ex}");
            }
        }

        // Initialise the server data
        private static void InitServerData()
        {
            tempPacket = new Packet();
            for (int i = 1; i <= capacity; i++)
            {
                clients.Add(i, new ClientData(i));
            }
        }

        // Initialise the packets that will be used
        private static void InitPackets()
        {
            for (int i = 1; i <= capacity; i++) 
            {
                packets[i] = new Packet();
            }
        }

    }
}
