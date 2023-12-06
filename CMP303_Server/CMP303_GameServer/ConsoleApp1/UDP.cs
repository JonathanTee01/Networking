using System;
using System.Net;
// Class made wih reference to https://github.com/tom-weiland/tcp-udp-networking/tree/tutorial-part4
namespace GameServer
{
    // Class to set up a UDP connection
    public class UDP
    {
        public IPEndPoint endPoint;

        private int id;

        public UDP(int clientId)
        {
            id = clientId;
        }

        // Connect to the client
        public void Connect(IPEndPoint clientEndPoint)
        {
            endPoint = clientEndPoint;
        }

        // Send data to the server
        public void SendData(Packet clientPacket)
        {
            Server.SendUDPData(id, endPoint, clientPacket);
        }

        // Cleans the data when disconnecting
        public void Disconnect()
        {
            endPoint = null;
        }

        // Handle the data read from the client packets
        public void HandleData()
        {
            int enumId = Server.packets[id].reader.ReadInt();

            switch (enumId)
            {
                case 1:
                    // Welcome received
                    ThreadManager.ExecuteOnMainThread(() =>
                    {
                        ServerHandle.WelcomeReveived(id);
                    });
                    break;
                case 2:
                    // Player movement
                    ServerHandle.PlayerMovement(id);
                    break;
                default:
                    // If the data is not within the enum parameters
                    Console.WriteLine($"Failed in receiving UDP data from client {id}");
                    break;
            }
        }
    }
}
