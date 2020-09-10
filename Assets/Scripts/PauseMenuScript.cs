using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;


public class PauseMenuScript : MonoBehaviour
{

    public static bool IsOn = false;

    private NetworkManager networkManager;


    // Start is called before the first frame update
    void Start()
    {
        networkManager = NetworkManager.singleton;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void LeaveRoom()
    {
        /*if (networkManager.client.connection.playerControllers[0].gameObject.GetComponent<NetworkBehaviour>().isServer)
        {*/
            GameManager.UnRegisterPlayer(networkManager.client.connection.playerControllers[0].gameObject.name);
            MatchInfo matchInfo = networkManager.matchInfo;
            networkManager.matchMaker.DropConnection(matchInfo.networkId, matchInfo.nodeId, 0, networkManager.OnDropConnection);
            //networkManager.matchMaker.DestroyMatch(matchInfo.networkId, 0, networkManager.OnDestroyMatch);
            networkManager.StopHost();
            
        /*}
        else
        {

            MatchInfo matchInfo = networkManager.matchInfo;
            networkManager.matchMaker.DropConnection(matchInfo.networkId, matchInfo.nodeId, 0, networkManager.OnDropConnection);
            //networkManager.StopClient();

        } */
        

        MainMenu.returned = true;


    }

}
