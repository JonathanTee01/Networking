using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

// Class made wih reference to https://github.com/tom-weiland/tcp-udp-networking/tree/tutorial-part4
// UDP class to send and receive packets
public class UDP
{
    public UdpClient socket;
    public IPEndPoint endPoint;

    public UDP(string ip, int port)
    {
        endPoint = new IPEndPoint(IPAddress.Parse(ip), port);
    }

    // Connect to a server using the supplied port number
    public void Connect(int port)
    {
        socket = new UdpClient(port);

        socket.Connect(endPoint);
        socket.BeginReceive(ReceiveCallback, null);

        SendData();
    }

    // Send a packet
    public void SendData()
    {
        try
        {
            if (socket != null)
            {
                socket.BeginSend(Client.packet.writer.GetPacket(), Client.packet.writer.GetLength(), null, null);
            }
        }
        catch (Exception ex)
        {
            Debug.Log($"Error sending data to server via UDP: {ex}");
        }
    }

    // Recieve the data from the packet and prepare to handle it
    // Ran asyncrenously and calls back to itself. Essentialy waits until a udp packet come in and then handles it before accepting another
    private void ReceiveCallback(IAsyncResult result)
    {
        try
        {
            byte[] data = socket.EndReceive(result, ref endPoint);

            if (data.Length < 4)
            {
                Client.instance.Disconnect();
                socket.BeginReceive(ReceiveCallback, null);
                return;
            }

            HandleData(data);
            socket.BeginReceive(ReceiveCallback, null);
        }
        catch
        {
            Disconnect();
        }
    }

    // Cleans the udp data before disconnecting
    private void Disconnect()
    {
        Client.instance.Disconnect();

        endPoint = null;
        socket = null;
    }

    // Process the data from the packet
    private void HandleData(byte[] data)
    {
        Client.packet.NewRead(data);
        int enumId = Client.packet.reader.ReadInt();

        // Based on the packets enumID we know what we're wanting to read
        switch (enumId)
        {
            case 1:
                // Welcome
                ClientHandle.Welcome();
                break;
            case 2:
                // Spawn
                ThreadManager.ExecuteOnMainThread(() =>
                {
                    ClientHandle.SpawnPlayer();
                });
                break;
            case 3:
                // Position
                ClientHandle.UpdatePlayer();
                break;
            case 4:
                // Position
                ClientHandle.DespawnPlayer();
                break;
            default:
                // Unpredicted data
                Debug.Log("Error receiving data from server");
                break;
        }
    }
}

