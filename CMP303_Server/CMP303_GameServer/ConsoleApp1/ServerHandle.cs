using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer
{
    // Handles data read in from a packet
    class ServerHandle
    {
        // sendDataQueue for data being sent
        public static Queue<byte[]> sendDataQueue = new Queue<byte[]>();

        public static void WelcomeReveived(int clientId)
        {

            // Read the data
            int clientIdCheck = Server.packets[clientId].reader.ReadInt();
            string username = Server.packets[clientId].reader.ReadString();

            Console.WriteLine($"{Server.clients[clientId].tcp.socket.Client.RemoteEndPoint} connected succesfully and is now player {clientId}");

            // If the client
            if (clientId != clientIdCheck)
            {
                Console.WriteLine($"Player \"{username}\" (ID: {clientId}) has assumed the wrong client ID ({clientIdCheck})!");
            }
            
            Server.clients[clientId].SendIntoGame(username);
        }

        // Reads in the player movement and tracks their last known data then sends it to the other players
        public static void PlayerMovement(int clientId)
        {
            Server.clients[clientId].player.Move();
            ServerSend.SendPlayerPosition(clientId);
        }
    }
}
