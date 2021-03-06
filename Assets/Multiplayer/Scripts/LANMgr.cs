using Unity.Netcode;
using Unity.Netcode.Transports.UNET;
using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.Net.Sockets;

public class LANMgr : MonoBehaviour
{
    private bool connection = false;
    public GameObject hostButton;
    public GameObject clientButton;
    public GameObject moveButton;
    
    public Text text_Status;
    public InputField inputF_IP;
    private string currentIP;
    public GameObject cloudAnchorMgrPrefab;
    public GameObject gameStatusMgrPrefab;

    public void StartGame()
    {
        var cloudAnchorMgrObj = Instantiate(cloudAnchorMgrPrefab,Vector3.zero,Quaternion.identity);
        cloudAnchorMgrObj.GetComponent<NetworkObject>().Spawn();
        var gameStatusMgrObj = Instantiate(gameStatusMgrPrefab, Vector3.zero,Quaternion.identity);
        gameStatusMgrObj.GetComponent<NetworkObject>().Spawn();
    }

    public void StartHost()
    {
        connection = NetworkManager.Singleton.StartHost();
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                currentIP = ip.ToString();
                break;
            }
        }
        UIMgr.Singleton.startGameButton.SetActive(true);
    }

    public void StartClient()
    {
        NetworkManager.Singleton.GetComponent<UNetTransport>().ConnectAddress = inputF_IP.text;
        connection = NetworkManager.Singleton.StartClient();
    }

    private void ActivateStartButtons()
    {
        hostButton.SetActive(true);
        clientButton.SetActive(true);
    }

    private void DeactivateStartButtons()
    {
        hostButton.SetActive(false);
        clientButton.SetActive(false);
    }

    public void SubmitNewPosition()
    {
        if (NetworkManager.Singleton.IsServer && !NetworkManager.Singleton.IsClient)
        {
            foreach (ulong uid in NetworkManager.Singleton.ConnectedClientsIds)
            {
                NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(uid).GetComponent<PlayerNetworkMgr>().Move();
            }
        }
        else
        {
            var playerObject = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();
            var player = playerObject.GetComponent<PlayerNetworkMgr>();
            player.Move();
        }
    }


    private void Update()
    {
        DeactivateStartButtons();
        moveButton.SetActive(false);

        string status = "";
        status += $"Connection: {connection.ToString()}\n";
        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
        {
            ActivateStartButtons();
        }
        else
        {
            var mode = NetworkManager.Singleton.IsHost ?
            "Host" : NetworkManager.Singleton.IsServer ? "Server" : "Client";

            status += "Transport: " + NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetType().Name + "\n";
            status += "Mode: " + mode + "\n";

            moveButton.SetActive(true);

            if (NetworkManager.Singleton.IsServer)
            {
                status += $"IP: {currentIP}\n";
                text_Status.text = currentIP;
                status += "ConnectedClients: \n";
                int clientCount = NetworkManager.Singleton.ConnectedClients.Count;

                for (int i =1; i <= UIMgr.Singleton.userIcons.Count; i++)
                {
                    UIMgr.Singleton.userIcons[i-1].SetActive(i<=clientCount);
                }

                foreach (ulong uid in NetworkManager.Singleton.ConnectedClientsIds)
                {
                    //status += $"{uid.ToString()}: {NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(uid).transform.position}\n";
                    status += $"uid: {uid.ToString()}\n";
                }
            }
            else
            {
                //client
            }
        }

        //text_Status.text = status;
    }
}
