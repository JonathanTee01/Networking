using System;
using System.Net.Sockets;
// Class made wih reference to https://github.com/tom-weiland/tcp-udp-networking/tree/tutorial-part4
namespace GameServer
{
    // Class to set up a TCP connection
    public class TCP
    {
        // Client socket
        // Data is sent from this socket
        public TcpClient socket;

        // Variables for the client ID and the network stream
        // The network stream is what carries the data
        private readonly int id;
        public NetworkStream stream;
        private byte[] receiveBuffer;
        private int bufferSize;

        public TCP(int _id)
        {
            id = _id;
        }

        // Function to connect the client to the server
        public void Connect(TcpClient _socket)
        {
            socket = _socket;
            bufferSize = Globals.BUFFER_SIZE;
            socket.ReceiveBufferSize = bufferSize;
            socket.SendBufferSize = bufferSize;

            stream = socket.GetStream();

            receiveBuffer = new byte[bufferSize];

            stream.BeginRead(receiveBuffer, 0, bufferSize, Receive, null);

            ServerSend.Welcome(Server.capacity, id, "Welcome to the server!");
        }

        // Function to prepare a packet to send
        public void SendData(Packet packet)
        {
            try
            {
                if (socket != null)
                {
                    stream.BeginWrite(packet.writer.GetPacket(), 0, packet.writer.GetLength(), null, null);
                }
            }
            catch
            {
                Console.WriteLine($"Error sending data to player {id} via TCP");
            }
        }

        private void Receive(IAsyncResult result)
        {
            // Attempt to recieve data
            try
            {
                int byteLength = stream.EndRead(result);
                if (byteLength <= 0)
                {
                Server.clients[id].Disconnect();
                    return;
                }

                byte[] data = new byte[byteLength];
                Array.Copy(receiveBuffer, data, byteLength);

                HandleData(data);
                stream.BeginRead(receiveBuffer, 0, bufferSize, Receive, null);
            }
            // If it failed then this catches it
            catch (Exception ex)
            {
                Console.WriteLine($"Error receiving TCP data: {ex}");
                Server.clients[id].Disconnect();
            }
        }

        // Cleans the data when disconnecting
        public void Disconnect()
        {
            socket.Close();
            stream = null;
            receiveBuffer = null;
            socket = null;
        }


        // Handle data read from your packet
        private void HandleData(byte[] data)
        {
            Server.packets[id].NewRead(data);

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
                    ThreadManager.ExecuteOnMainThread(() =>
                    {
                        ServerHandle.PlayerMovement(id);
                    });
                    break;
                default:
                    // If the data is not within the enum parameters
                    Console.WriteLine($"Failed in receiving TCP data from client {id}");
                    break;

            }
        }
    }
}
