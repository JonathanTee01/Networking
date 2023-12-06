namespace GameServer
{
    // Class made wih reference to https://github.com/tom-weiland/tcp-udp-networking/tree/tutorial-part4
    class ServerSend
    {
        // Send a packet to a specific client
        private static void SendTCPData(int clientId)
        {
            Server.clients[clientId].tcp.SendData(Server.packets[clientId]);
        }

        private static void SendUDPData(int clientId)
        {
            Server.clients[clientId].udp.SendData(Server.packets[clientId]);
        }

        // Send a packet to all clients
        private static void SendTCPDataToAll(int packetId)
        {
            for (int i = 1; i <= Server.capacity; i++)
            {
                if (!Server.clients[i].open)
                {
                    Server.clients[i].tcp.SendData(Server.packets[packetId]);
                }
            }
        }

        // Overloaded version to send a packet to all clients except one
        private static void SendTCPDataToAll(int packetId, int exception)
        {
            for (int i = 1; i <= Server.capacity; i++)
            {
                if (i != exception)
                {
                    if (!Server.clients[i].open && Server.packets[packetId] != null)
                    {
                        Server.clients[i].tcp.SendData(Server.packets[packetId]);
                    }
                }
            }
        }

        // Function to send a packet to all clients except the one whos packet it is
        private static void SendUDPDataToAll(int clientId)
        {
            for (int i = 1; i <= Server.capacity; i++)
            {
                if (i != clientId)
                {
                    if (!Server.clients[i].open && Server.packets[clientId] != null)
                    {
                        Server.clients[i].udp.SendData(Server.packets[clientId]);
                    }
                }
            }
        }

        // Function to write and send a welcome message to the client
        #region Packets
        public static void Welcome(int capacity, int clientId, string _msg)
        {
            // Write the enum as as packet ID to say what's being sent
            Server.packets[clientId].NewWrite();
            Server.packets[clientId].writer.SetEnumId((int)ServerPackets.welcome);

            Server.packets[clientId].writer.Write(_msg);
            Server.packets[clientId].writer.Write(clientId);
            Server.packets[clientId].writer.Write(capacity);

            // Sent through TCP so we don't lose the welcome message
            SendTCPData(clientId);
        }

        // Function to send the required data to the client to spawn the player
        public static void SpawnPlayer(int clientId, Player player)
        {
            // Write the enum as as packet ID to say what's being sent
            Server.packets[clientId].NewWrite();
            Server.packets[clientId].writer.SetEnumId((int)ServerPackets.spawnPlayer);

            // Let the client know their player ID so they can identify themselves during udp
            Server.packets[clientId].writer.Write(player.id);
            Server.packets[clientId].writer.Write(player.username);
            Server.packets[clientId].writer.Write(player.position);
            Server.packets[clientId].writer.Write(player.velocity);

            // Sent through TCP so we don't lose the welcome message
            Server.clients[clientId].open = false;

            SendTCPDataToAll(clientId);
        }

        public static void SendPlayerPosition(int clientId)
        {
            // Data has already been written and stored in the clients relevant packet
            SendUDPDataToAll(clientId);
        }

        // Send a packet to clients so they can despawn a player who left
        public static void DespawnPlayer(int clientId)
        {
            Server.packets[clientId].NewWrite();
            Server.packets[clientId].writer.Write((int)ServerPackets.despawnPlayer);
            Server.packets[clientId].writer.Write(clientId);

            SendTCPDataToAll(clientId);
        }
        #endregion
    }
}
