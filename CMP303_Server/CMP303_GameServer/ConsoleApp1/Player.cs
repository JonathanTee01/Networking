using System.Numerics;

// Class to store the player information
namespace GameServer
{
    public class Player
    {
        public int id;
        public string username;

        public Vector2 position;
        public Vector2 velocity;

        public Player(int playerId, string playerName, Vector2 spawnPostion)
        {
            id = playerId;
            username = playerName;
            position = spawnPostion;
        }

        public void Move()
        {
            // Creates a new set of data for the packet of the client who sent the data
            Server.packets[id].NewWrite();
            Server.packets[id].writer.Write((int)ServerPackets.playerPosition);
            Server.packets[id].writer.Write(id);

            // Write the clients time from when they sent a packet
            Server.packets[id].writer.Write(Server.clients[id].lastSec);
            Server.packets[id].writer.Write(Server.clients[id].lastMil);

            // Set and write the position and velocity
            position = Server.packets[id].reader.ReadVector2();
            Server.packets[id].writer.Write(position);
            velocity = Server.packets[id].reader.ReadVector2();
            Server.packets[id].writer.Write(velocity);
        }
    }
}
