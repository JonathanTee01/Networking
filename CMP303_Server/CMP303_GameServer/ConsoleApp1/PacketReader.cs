using System;
using System.Linq;
using System.Numerics;
using System.Text;

// Class made wih reference to https://github.com/tom-weiland/tcp-udp-networking/tree/tutorial-part4
// Class used to store a packet and process it
// Intended to have a packet passed in then to be called to read what is inside the packet
namespace GameServer
{
    // Class used to store a packet and process it
    // Intended to have a packet passed in then to be called to read what is inside the packet
    public class PacketReader
    {
        // Variables used to store the array and process it
        byte[] dataStore;
        int readPos;

        public PacketReader()
        {

        }

        // Read in a packet. Works as a setter but also cleans the previous packet
        public void ReadNewPacket(byte[] data)
        {
            // Get rid of the previous packet and prepare for the new one
            PacketClean(data.Length);

            // Stores the packet to be processed
            dataStore = data;
        }

        // Cleans the dataStore to be used for the next packet
        public void PacketClean(int dataSize)
        {
            // Create a new byte array that will store the new packet
            // Essentially this is just a vaiable length byte array
            dataStore = new byte[dataSize];

            // Also reset the read position for cycliing through the data
            readPos = 0;
        }

        public int UnreadLength()
        {
            return dataStore.Length - readPos;
        }

        // Return the packet being stored
        public byte[] GetPacket()
        {
            return dataStore;
        }

        // Return the size of the packet eing stored
        public int GetLength()
        {
            return dataStore.Length;
        }

        #region data reading
        public byte ReadByte(bool moveRead = true)
        {
            if (dataStore.Length > readPos)
            {
                // If there are unread bytes
                byte _value = dataStore[readPos]; // Get the byte at readPos' position
                if (moveRead)
                {
                    // If moveRead is true
                    readPos += 1; // Increase readPos by 1
                }
                return _value; // Return the byte
            }
            else
            {
                throw new Exception("Could not read value of type 'byte'!");
            }
        }

        // Reads an array of bytes from the packet.
        public byte[] ReadBytes(int length, bool moveRead = true)
        {
            if (dataStore.Length > readPos)
            {
                // If there are unread bytes
                int byteIndexCount = readPos + length;
                byte[] value = new byte[length];

                // Loop through each element needing accessed whicle incrementing the readPos
                for (int index = readPos; index < byteIndexCount; index++)
                {
                    value.Append<byte>(dataStore.ElementAt<byte>(index));
                    readPos++;
                }
                // If not wanting to move readPos then reset it
                if (moveRead)
                {
                    readPos -= length;
                }
                return value;
            }
            else
            {
                throw new Exception("Could not read value of type 'byte[]'!");
            }
        }

        // Reads a short from the packet.
        public short ReadShort(bool moveRead = true)
        {
            if (dataStore.Length > readPos)
            {
                // If there are unread bytes convert the bytes to a short
                short _value = BitConverter.ToInt16(dataStore, readPos); 
                if (moveRead)
                {
                    // If moveRead is true and there are unread bytes
                    readPos += 2;
                }
                return _value; 
            }
            else
            {
                throw new Exception("Could not read value of type 'short'!");
            }
        }

        // Reads an int from the packet.
        public int ReadInt(bool moveRead = true)
        {
            if (dataStore.Length > readPos)
            {
                // If there are unread bytes convert the bytes to an int
                int _value = BitConverter.ToInt32(dataStore, readPos); 
                if (moveRead)
                {
                    // If moveRead is true
                    readPos += 4; 
                }
                return _value; 
            }
            else
            {
                throw new Exception("Could not read value of type 'int'!");
            }
        }

        // Reads a long from the packet.
        public long ReadLong(bool moveRead = true)
        {
            if (dataStore.Length > readPos)
            {
                // If there are unread bytes convert the bytes to a long
                long _value = BitConverter.ToInt64(dataStore, readPos); 
                if (moveRead)
                {
                    // If moveRead is true
                    readPos += 8; 
                }
                return _value; 
            }
            else
            {
                throw new Exception("Could not read value of type 'long'!");
            }
        }

        // Reads a float from the packet.
        public float ReadFloat(bool moveRead = true)
        {
            if (dataStore.Length > readPos)
            {
                // If there are unread bytes convert the bytes to a float
                float _value = BitConverter.ToSingle(dataStore, readPos); 
                if (moveRead)
                {
                    // If moveRead is true
                    readPos += 4; 
                }
                return _value; 
            }
            else
            {
                throw new Exception("Could not read value of type 'float'!");
            }
        }

        // Reads a bool from the packet.
        public bool ReadBool(bool moveRead = true)
        {
            if (dataStore.Length > readPos)
            {
                // If there are unread bytes convert the bytes to a bool
                bool _value = BitConverter.ToBoolean(dataStore, readPos); 
                if (moveRead)
                {
                    // If moveRead is true
                    readPos += 1; 
                }
                return _value; 
            }
            else
            {
                throw new Exception("Could not read value of type 'bool'!");
            }
        }

        // Reads a string from the packet.
        public string ReadString(bool moveRead = true)
        {
            try
            {
                int length = ReadInt(); // Get the length of the string convert the bytes to a string
                string _value = Encoding.ASCII.GetString(dataStore, readPos, length); 
                if (moveRead && _value.Length > 0)
                {
                    // If moveRead is true string is not empty
                    readPos += length; 
                }
                return _value; 
            }
            catch
            {
                throw new Exception("Could not read value of type 'string'!");
            }
        }

        // Reads a Vector2
        public Vector2 ReadVector2(bool moveRead = true)
        {
            return new Vector2(ReadFloat(moveRead), ReadFloat(moveRead));
        }
        #endregion
    }
}
