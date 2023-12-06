using System.Collections.Generic;
using UnityEngine;

// Class that managegs player behavious
public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;

    // Player prefabs for spawning
    public GameObject playerPrefab;
    public GameObject otherPlayerPrefab;

    // Player data storage
    public static Dictionary<int, GameObject> players = new Dictionary<int, GameObject>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destorying objects!");
            Destroy(this);
        }
    }

    // Spawn a new player
    public void SpawnPlayer(int id, Vector2 spawnPosition, Vector2 spawnVelocity)
    {
        ThreadManager.ExecuteOnMainThread(() =>
        {
            GameObject player;

            // If the same as the clients ID then spawn their player
            if (id == Client.instance.myId)
            {
                player = Instantiate(playerPrefab);
            }
            // Otherwise it's a different player so spawn a different one
            else
            {
                player = Instantiate(otherPlayerPrefab);
            }


            // Set the new players variables
            players.Add(id, player);

            players[id].GetComponent<PlayerData>().InitData(id, spawnPosition, spawnVelocity, player.GetComponent<Rigidbody2D>());
        });
    }

    // Update the movement received about a player
    public static void UpdatePlayerMovement()
    {
        int id = Client.packet.reader.ReadInt();
        int sec = Client.packet.reader.ReadInt();
        int mil = Client.packet.reader.ReadInt();
        Vector2 pos = Client.packet.reader.ReadVector2();
        Vector2 vel = Client.packet.reader.ReadVector2();

        // Spawn the player if they don't yet exist
        try
        {
            players[id].GetComponent<PlayerData>().UpdateOtherData(sec, mil, pos, vel);
        }
        catch
        {
            instance.SpawnPlayer(id, pos, vel);
        }
    }

    private void FixedUpdate()
    {
        // Loop to update all exisiting player
        for (int i = 1; i <= Client.capacity; i++)
        {
            // Record and send up to date information about the client
            if (i == Client.instance.myId)
            {
                players[i].GetComponent<PlayerData>().UpdateData();
                players[i].GetComponent<PlayerData>().WriteData();
                Client.udp.SendData();
            }
            else
            {
                // If they exist then update them
                try
                {
                    // If updated is true then their position was updated via a packet
                    if (players[i].GetComponent<PlayerData>().updated)
                    {
                        players[i].GetComponent<PlayerData>().updated = false;
                    }
                    // Otherwise we must have missed it
                    else
                    {
                        players[i].GetComponent<PlayerData>().UpdateOtherData();
                    }
                }
                catch { }
            }
        }
    }
}
