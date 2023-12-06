using System;

namespace GameServer
{
    // Client data that is stored on the server
    // NOT used by client side
    public class ClientData
    {
        // Client id and TCP channel that will be used
        public int id;
        public Player player;
        public TCP tcp;
        public UDP udp;
        public bool open = true;
        public float lastSec = -1;
        public float lastMil = -1;

        // Class to initialise the client
        public ClientData(int _clientId)
        {
            id = _clientId;
            tcp = new TCP(id);
            udp = new UDP(id);
        }    

        // Send players into the game
        public void SendIntoGame(string playerName)
        {
            player = new Player(id, playerName, new System.Numerics.Vector2(0, -2));

            if (player != null)
            {
                ServerSend.SpawnPlayer(id, player);
            }
           
        }
       
        // Disconnect a client
        public void Disconnect()
        {
            Console.WriteLine($"{id} has disconnected...");

            player = null;

            tcp.Disconnect();
            tcp = new TCP(id);

            udp.Disconnect();
            udp = new UDP(id);

            open = true;

            ServerSend.DespawnPlayer(id);
            Server.clients[id] = new ClientData(id);
        }
    }
}