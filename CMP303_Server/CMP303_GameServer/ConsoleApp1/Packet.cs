// Class made wih reference to https://github.com/tom-weiland/tcp-udp-networking/tree/tutorial-part4
namespace GameServer
{
    // Sent from server to clients
    public enum ServerPackets
    {
        welcome = 1,
        spawnPlayer,
        playerPosition,
        despawnPlayer
    }

    // Sent from client to server
    // Not used in this side of the connection but helps with reading the data handling
    public enum ClientPackets
    {
        welcomeReceived = 1,
        playerMovement
    }

    // Reusable packet class
    public class Packet
    {
        // Reader and writer. They both store packets of their own that can be editted from outwith their class
        public PacketReader reader;
        public PacketWriter writer;

        public bool busy = false;

        #region constructors
        // Constructor to create an empty packet
        public Packet()
        {
            reader = new PacketReader();
            writer = new PacketWriter();
        }

        // Overloaded constructor to create a packet with an ID
        public Packet(int id)
        {
            // id will be used to determine the type of packet being sent
            writer.Write(id);
        }

        // Overloaded constructor for a packet that will accept data while receiving
        public Packet(byte[] receivedData)
        {
            reader.ReadNewPacket(receivedData);
        }
        #endregion

        #region reset packets
        // Prepare the reader to read a new packet
        public void NewRead(byte[] receivedData)
        {
            reader.ReadNewPacket(receivedData);
        }

        // Prepare teh packet to write to a fresh state
        public void NewWrite()
        {
            writer.PacketClean();
        }
        #endregion
    }
}
