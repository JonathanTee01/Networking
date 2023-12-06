using TMPro;
using UnityEngine;

// Class to make use of the buttons and fields of the UI
public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public GameObject startMenu;
    public TMP_InputField usernameField;
    public TMP_InputField connectAddress;
    public Client client;

    private void Awake()
    {
        Application.runInBackground = true;
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

    // Function attached to a button that connects to a server
    public void ConnectToServer()
    {
        startMenu.SetActive(false);
        usernameField.interactable = false;
        connectAddress.interactable = false;
        client.SetParam(usernameField.text, connectAddress.text);
        client.ConnectToServer();
    }
}
