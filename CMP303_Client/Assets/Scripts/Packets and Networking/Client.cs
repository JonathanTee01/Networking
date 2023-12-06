using UnityEngine;
using System;

// Class made wih reference to https://github.com/tom-weiland/tcp-udp-networking/tree/tutorial-part4
public class Client : MonoBehaviour
{
    public static Client instance;
    public static int dataBufferSize = 4096;

    public static Packet packet;

    // Initialise networking stuff
    public string username;
    public string ip = "127.0.0.1";
    public int port = 26950;
    public int myId = 0;
    public static TCP tcp;
    public static UDP udp;
    public static int capacity;
    private bool connected = false;

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

    // On start create a TCP and UDP item
    private void Start()
    {
        tcp = new TCP();
        udp = new UDP(ip, port);
    }

    // When quiting we want to tell the server
    private void OnApplicationQuit()
    {
        Disconnect();
    }

    // Establish a TCP connection to the server
    public void ConnectToServer()
    {
        InitClientData();
        connected = true;
        tcp.Connect(ip, port);
    }    

    // Read the input fields to variables
    public void SetParam(string name, string address)
    {
        // Store the inputed username
        username = name;

        // Split the entered address into the IP and port
        string[] splitString = address.Split(":");
        ip = splitString[0];
        port = Convert.ToInt32(splitString[1]);
    }

    // Initialise the client
    private void InitClientData()
    {
        packet = new Packet();
        Debug.Log("Initialised packets");
    }

    // Clean the sockets on disconnect
    public void Disconnect()
    {
        if (connected)
        {
            connected = false;
            tcp.socket.Close();
            udp.socket.Close();
        }
    }
}
