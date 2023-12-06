using System;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

// Class made wih reference to https://github.com/tom-weiland/tcp-udp-networking/tree/tutorial-part4
// TCP class to send and reciecve data packets
public class TCP
{
    public TcpClient socket;

    private NetworkStream stream;
    private byte[] receiveBuffer;
    private Queue<byte[]> dataQueue;

    // Connect to the server through the provided ip address
    public void Connect(string ip, int port)
    {
        socket = new TcpClient
        {
            ReceiveBufferSize = Client.dataBufferSize,
            SendBufferSize = Client.dataBufferSize
        };

        dataQueue = new Queue<byte[]>();
        receiveBuffer = new byte[Client.dataBufferSize];
        socket.BeginConnect(ip, port, ConnectCallback, socket);
    }

    // Allows the connection and disconnection from a socket
    // Ran asyncrenously and calls back to itself. Essentialy waits until a tcp connection happens then handles the packet before waiting for another
    private void ConnectCallback(IAsyncResult result)
    {
        socket.EndConnect(result);

        if (!socket.Connected)
        {
            return;
        }

        stream = socket.GetStream();

        Client.packet = new Packet();

        stream.BeginRead(receiveBuffer, 0, Client.dataBufferSize, ReceiveCallback, null);
    }

    // Add a packet to the data stream
    public void SendData()
    {
        try
        {
            if (socket != null)
            {
                stream.BeginWrite(Client.packet.writer.GetPacket(), 0, Client.packet.writer.GetLength(), null, null);
            }
        }
        catch (Exception ex)
        {
            Debug.Log($"Error sending data to server via TCP: {ex}");
        }
    }

    // Wait to recieve a packet asyncrenously then handle it and wait for another
    private void ReceiveCallback(IAsyncResult result)
    {
        try
        {
            int byteLength = stream.EndRead(result);
            if (byteLength <= 0)
            {
                Client.instance.Disconnect();
                return;
            }

            byte[] data = new byte[byteLength];
            Array.Copy(receiveBuffer, data, byteLength);

            HandleData(data);
            stream.BeginRead(receiveBuffer, 0, Client.dataBufferSize, ReceiveCallback, null);
        }
        catch
        {
            Disconnect();
        }
    }

    // Cleans the data when disconnecting
    private void Disconnect()
    {
        Client.instance.Disconnect();

        stream = null;
        receiveBuffer = null;
        socket = null;
    }

    // Unpack the data from the packet received
    private void HandleData(byte[] data)
    {
        // Add data to a queue
        dataQueue.Enqueue(data);

        // If busy then we are already dealing with a packet so don't want to risk race conditions
        if (Client.packet.busy)
        {
            return;
        }

        // Runs so long as there is data waiting to be processed
        // The function is accessed asyncrenously so data can be added at any point and still be exectued in here
        while (dataQueue.Count > 0)
        {
            // Set to busy to prevent race conditions
            Client.packet.busy = true;
            Client.packet.NewRead(dataQueue.Dequeue());
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
                    ClientHandle.SpawnPlayer();
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
        // Now we're done set false to allow it to be run again
        Client.packet.busy = false;
    }
}