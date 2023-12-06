using System.Net;
using UnityEngine;

// Class to handle received data
public class ClientHandle : MonoBehaviour
{
    public static void Welcome()
    {
        // Read the welcome message in the same order as it was sent
        string msg = Client.packet.reader.ReadString();
        int myId = Client.packet.reader.ReadInt();
        Client.capacity = Client.packet.reader.ReadInt();

        Debug.Log($"Message from server: {msg}");
        Client.instance.myId = myId;

        // Send a packet acknowledging that the player has their id
        ClientSend.WelcomeReceived();

        // Connect through udp
        Client.udp.Connect(((IPEndPoint)Client.tcp.socket.Client.LocalEndPoint).Port);
    }

    // Unpack and spawn a player
    public static void SpawnPlayer()
    {
        // Read the player data
        int id = Client.packet.reader.ReadInt();
        string username = Client.packet.reader.ReadString();
        Vector2 position = Client.packet.reader.ReadVector2();
        Vector2 velocity = Client.packet.reader.ReadVector2();

        // Spawn the new player through the player manager
        PlayerManager.instance.SpawnPlayer(id, position, velocity);

        // Debug
        Debug.Log($"Spawning Player {username} with ID {id}");
    }

    // Updatea player based on their id
    public static void UpdatePlayer()
    {
        PlayerManager.UpdatePlayerMovement();
    }

    // A player disconnected so get rid of their old body
    public static void DespawnPlayer()
    {
        int id = Client.packet.reader.ReadInt();
        Destroy(PlayerManager.players[id]);
        PlayerManager.players.Remove(id);
    }
}
