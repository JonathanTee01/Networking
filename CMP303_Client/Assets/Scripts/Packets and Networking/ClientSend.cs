using UnityEngine;

public class ClientSend : MonoBehaviour
{
    // Send data over tcp
    private static void SendTCPData()
    {
        Client.tcp.SendData();
    }

    #region Packets
    // Write a packet for the server with the clients id and name to ensure that the assumed id is correct
    public static void WelcomeReceived()
    {
        Client.packet.NewWrite();
        Client.packet.writer.Write((int)ClientPackets.welcomeReceived);
        Client.packet.writer.Write(Client.instance.myId);
        Client.packet.writer.Write(UIManager.instance.usernameField.text);

        SendTCPData();
    }
    #endregion
}
