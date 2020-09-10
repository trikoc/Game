using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class GameManager : NetworkBehaviour
{

    public static GameManager instance;

    public MatchSettings matchSettings;

    [SerializeField]
    GameObject[] WeaponPrefabs;

    [SerializeField]
    private GameObject sceneCamera;

    public static bool canJoin;

    public bool canUpdate;

    public static bool canMove = false;


    #region Player tracking

    private const string PLAYER_ID_PREFIX = "Player ";

    private static Dictionary<string, PlayerInfo> players = new Dictionary<string, PlayerInfo>();

    public static void RegisterPlayer(string _netID, PlayerInfo _player)
    {


        string _playerID = PLAYER_ID_PREFIX + /*(players.Count() + 1);*/_netID;
        players.Add(_playerID, _player);
        _player.transform.name = _playerID;
    }

    public static void UnRegisterPlayer(string _playerID)
    {
        players.Remove(_playerID);
    }

    public static PlayerInfo GetPlayer(string _playerID)
    {
        return players[_playerID];
    }

    public static PlayerInfo[] GetAllPlayers()
    {
        return players.Values.ToArray();
    }
    /*
    void OnGUI ()
    {
        GUILayout.BeginArea(new Rect(200, 200, 200, 500));
        GUILayout.BeginVertical();

        foreach (string _playerID in players.Keys)
        {
            GUILayout.Label(_playerID + "  -  " + players[_playerID].transform.name);
        }

        GUILayout.EndVertical();
        GUILayout.EndArea();
    }
    */
    #endregion


    void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("More than one GameManager in scene.");
        }
        else
        {
            instance = this;

            //if (NetworkManager.singleton.client.connection.playerControllers[0].gameObject.GetComponent<NetworkBehaviour>().isServer)
            {
                canJoin = true;
                canUpdate = true;
            }


        }
    }


    private void Update()
    {

        if(!canUpdate && players.Count() > 1)
        {
            canUpdate = true;
        }

        if (canUpdate)
        {
            if (canJoin)
            {
                canUpdate = false;
                canJoin = false;
                Debug.Log("Other players have 60 seconds to join");
                StartCoroutine(JoinTimer());
                return;
            }

            int counter = 0;
            string playerID = "";
            foreach(string player in players.Keys)
            {
                if (players[player].currentHealth > 0)
                {
                    counter++;
                    playerID = player;
                }
                
                if (counter > 1)
                {

                    return;
                }
            }
            //Debug.Log(playerID);
            players[playerID].Win();
            canUpdate = false;
            //canJoin = true;
            players = new Dictionary<string, PlayerInfo>();
        }
        
    }


    IEnumerator JoinTimer()
    {
        
        yield return new WaitForSeconds(60f);
        canJoin = false;
        canUpdate = true;
    }



    public void SetSceneCameraActive(bool isActive)
    {
        if (sceneCamera == null)
            return;

        sceneCamera.SetActive(isActive);
    }


}
