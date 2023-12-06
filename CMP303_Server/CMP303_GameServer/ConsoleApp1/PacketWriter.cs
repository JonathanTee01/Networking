using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

// Class made wih reference to https://github.com/tom-weiland/tcp-udp-networking/tree/tutorial-part4
// Class used to write to a packet
namespace GameServer
{
    public class PacketWriter
    {
        private List<byte> dataStore;
        private byte[] readable;

        // Constructor to initialise the dataStore
        public PacketWriter()
        {
            dataStore = new List<byte>();
        }

        // Clean the packet for further use
        public void PacketClean()
        {
            dataStore = new List<byte>();
        }
        
        // Writes the packet out as an array
        public byte[] GetPacket()
        {
            readable = null; 
            readable = dataStore.ToArray();
            return readable;
        }

        // Add an ID to tell the client what it is they're receiving
        public void SetEnumId(int enumID)
        {
            Write(enumID);
        }

        // Returns the length of the stored packet
        public int GetLength()
        {
            return dataStore.Count;
        }

        #region Write Functions
        /// Inserts the length of the packet's content at the start of the buffer.
        public void WriteLength()
        {
            dataStore.InsertRange(0, BitConverter.GetBytes(dataStore.Count)); // Insert the byte length of the packet at the very beginning
        }

        /// Adds a byte to the packet.
        public void Write(byte value)
        {
            dataStore.Add(value);
        }
        /// Adds an array of bytes to the packet.
        public void Write(byte[] value)
        {
            dataStore.AddRange(value);
        }
        /// Adds a short to the packet.
        public void Write(short value)
        {
            dataStore.AddRange(BitConverter.GetBytes(value));
        }
        /// Adds an int to the packet.
        /// The int to add.
        public void Write(int value)
        {
            dataStore.AddRange(BitConverter.GetBytes(value));
        }
        /// Adds a long to the packet.
        public void Write(long value)
        {
            dataStore.AddRange(BitConverter.GetBytes(value));
        }
        /// Adds a float to the packet.
        public void Write(float value)
        {
            dataStore.AddRange(BitConverter.GetBytes(value));
        }
        /// Adds a bool to the packet.
        public void Write(bool value)
        {
            dataStore.AddRange(BitConverter.GetBytes(value));
        }
        /// Adds a string to the packet.
        public void Write(string value)
        {
            Write(value.Length); // Add the length of the string to the packet
            dataStore.AddRange(Encoding.ASCII.GetBytes(value)); // Add the string itself
        }
        /// Adds a Vector2 to the packet.
        public void Write(Vector2 value)
        {
            Write(value.X);
            Write(value.Y);
        }
        #endregion 
    }
}
