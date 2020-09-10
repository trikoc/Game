using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Networking.Match;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(PlayerInfo))]

public class PlayerSetup : NetworkBehaviour
{

    [SerializeField]
    GameObject[] WeaponPrefabs;

    [SerializeField]
    GameObject zonePrefab;

    [SerializeField]
    Behaviour[] componentsToDisable;

    //[SerializeField]
    const string LOCAL_PLAYER_LAYER = "Local Player";
    const string REMOTE_PLAYER_LAYER = "Remote Player";
    const string DONT_DRAW_LAYER = "DontDraw";
    public GameObject playerGraphics;


    [SerializeField]
    GameObject playerUIPrefab;
    public GameObject playerUIInstance;

    public GameObject SniperObj;
    public GameObject CrosshairObj;
    public GameObject BulletObj;


    [SerializeField]
    GameObject bulletsUIPrefab;
    public GameObject bulletsUIInstance;

    [SerializeField]
    GameObject sniperUIPrefab;
    public GameObject sniperUIInstance;


    bool rifleAddedToFirstSlot = false;
    bool rifleAddedToSecondSlot = false;
    public Camera sceneCamera;

    private string localPlayerName;

    NetworkConnection id;


    // Start is called before the first frame update
    void Start()
    {

        GetComponent<GunChanger>().SetWeaponTransform();

        if (!isLocalPlayer)
        {
            DisableComponents();
            AssignRemoteLayer();


        }
        else
        {
            id = connectionToServer;
            localPlayerName = gameObject.name;
            //AddWeaponsToPlayer();
            //addStartWeapons();
            sceneCamera = Camera.main;
            if (sceneCamera != null)
            {
                sceneCamera.gameObject.SetActive(false);
            }
            // disable player graphics for local player - fps camera
            setLayerRecursively(playerGraphics, LayerMask.NameToLayer(DONT_DRAW_LAYER));

            //create player UI - crosshair
            playerUIInstance = Instantiate(playerUIPrefab);
            playerUIInstance.name = playerUIPrefab.name;

            playerUIInstance.transform.Find("Winner Menu").gameObject.SetActive(false);
            playerUIInstance.transform.Find("Loser Menu").gameObject.SetActive(false);
            playerUIInstance.transform.Find("Pause Menu").gameObject.SetActive(false);

            SniperObj = playerUIInstance.transform.Find("Sniper").gameObject;
            SniperObj.SetActive(false);
            CrosshairObj = playerUIInstance.transform.Find("Crosshair").gameObject;
            BulletObj = playerUIInstance.transform.Find("Bullets").gameObject;

            PlayerUI ui = playerUIInstance.GetComponent<PlayerUI>();
            ui.setPlayerInfo(GetComponent<PlayerInfo>());

            //create player UI - bullet
            /*bulletsUIInstance = Instantiate(bulletsUIPrefab);
            bulletsUIInstance.name = bulletsUIPrefab.name;*/
            
           /* //create player UI - sniper
            sniperUIInstance = Instantiate(sniperUIPrefab);
            sniperUIInstance.name = sniperUIPrefab.name;
            sniperUIInstance.SetActive(false);

            */
            
            GetComponent<PlayerInfo>().Setup();

            if (isServer)
            {
                GameObject zoneInstance = GameObject.Instantiate(zonePrefab, Vector3.zero, Quaternion.identity);
                NetworkServer.Spawn(zoneInstance);
                StartCoroutine(JoinTimerYield());
            }



        }
        for (int i = 0; i < GetComponent<Animator>().parameterCount; i++)
        {
            GetComponent<NetworkAnimator>().SetParameterAutoSend(i, true);
        }

         




    }
    /*[Command]
    public void CmdSetMessage(string message, int time)
    {
        RpcSetMessage(message, time);
    }*/

    [ClientRpc]
    public void RpcSetMessage(/*string message,*/ int time)
    {

        Text joinTimerText = GameObject.Find("JoinTimer").GetComponent<Text>();
        
        if (time > 0)
        {
            
            
            joinTimerText.text = "Other players have " + time + " seconds to join";
        }
        else
        {
            GameManager.canMove = true;
            joinTimerText.text = "";
        }


    }




    IEnumerator JoinTimerYield()
    {
        
        for (int i = 0; i <= 60; i++)
        {
            //MessagesObjText.text = "Other players have " + (60 - i) + "seconds to join";
            RpcSetMessage(/*"Join", */60 - i);
            yield return new WaitForSeconds(1);
        }
        //MessagesObjText.text = "";
        RpcSetMessage(/*"",*/0);

        //yield return new WaitForSeconds(60);
        NetworkManager.singleton.matchMaker.ListMatches(0, 20, "", true, 0, 0, OnMatchList);

    }


    public void OnMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matchList)
    {
        MatchInfoSnapshot match = matchList[matchList.Count - 1];
        NetworkManager.singleton.matchMaker.SetMatchAttributes(match.networkId, false, 0, NetworkManager.singleton.OnSetMatchAttributes);
    }



    void setLayerRecursively(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;
        foreach (Transform child in obj.transform)
        {
            setLayerRecursively(child.gameObject,newLayer);

        }
    }




    public override void OnStartClient()
    {
        
        base.OnStartClient();

        string _netID = GetComponent<NetworkIdentity>().netId.ToString();
        PlayerInfo _player = GetComponent<PlayerInfo>();
        
        GameManager.RegisterPlayer(_netID, _player);


        //Debug.Log("Join is enabled: " + GameManager.joinEnabled);
    }


    

    // all enemy players are assigned to Remote Layer
    void AssignRemoteLayer()
    {

        setLayerRecursively(gameObject, LayerMask.NameToLayer(REMOTE_PLAYER_LAYER));
        //gameObject.layer = LayerMask.NameToLayer(REMOTE_PLAYER_LAYER);
    }

    //disable components script
    void DisableComponents()
    {
        for (int i = 0; i < componentsToDisable.Length; i++)
        {
            componentsToDisable[i].enabled = false;
        }
        transform.Find("Camera").gameObject.SetActive(false);

        
    }

    
    // player destroyed
    void OnDisable()
    {
        Destroy(playerUIInstance);
        //Destroy(sniperUIInstance);
        //Destroy(bulletsUIInstance);

        if (sceneCamera != null)
        {
            sceneCamera.gameObject.SetActive(true);
        }

        //GameManager.UnRegisterPlayer(transform.name);
        
    }

    public void togglePlayerUI()
    {
        //playerUIInstance.SetActive(!playerUIInstance.activeSelf);
        //playerUIInstance.transform.Find("Crosshair").gameObject.SetActive(!playerUIInstance.transform.Find("Crosshair").gameObject.activeSelf);
        CrosshairObj.SetActive(!CrosshairObj.activeSelf);
    }
    public void toggleSniperUI()
    {
        //sniperUIInstance.SetActive(!sniperUIInstance.activeSelf);
        //playerUIInstance.transform.Find("Sniper").gameObject.SetActive(!playerUIInstance.transform.Find("Sniper").gameObject.activeSelf);
        SniperObj.SetActive(!SniperObj.activeSelf);
    }
    public void disableSniperUI()
    {
        //sniperUIInstance.SetActive(false);
        if (SniperObj == null)
        {
            return;
        }
        SniperObj.SetActive(false);
        
    }

    public void displayBullets(string _text)
    {
        //bulletsUIInstance.GetComponentInChildren<Text>().text = _text;
        BulletObj.GetComponentInChildren<Text>().text = _text;
    }


    [Client]
    public void addStartWeapons()
    {
        string[] paths = { GetGameObjectPath(GameObject.Find("Rifle1")), GetGameObjectPath(GameObject.Find("Rifle2")), GetGameObjectPath(GameObject.Find("Pistol")) };
        CmdWeaponInstances(paths);
      
    }
    [Command]
    public void CmdWeaponInstances(string [] paths)
    {
        int x = 0;
        GameObject[] weaponsInstances = new GameObject[3];
        for (int i = 0; i < WeaponMenu.weaponsToGet.Length; i++)
        {
            for (int j = 0; j < WeaponPrefabs.Length; j++)
            {
                if (WeaponMenu.weaponsToGet[i] == WeaponPrefabs[j].GetComponent<Gun>().name)
                {
                    Instantiate(WeaponPrefabs[j], GameObject.Find(localPlayerName + paths[x++]).transform);
                }
            }
            
        }
          /*  {
            
            Instantiate(WeaponPrefabs[0], GameObject.Find(localPlayerName + paths[0]).transform), // AK-47
            Instantiate(WeaponPrefabs[1], GameObject.Find(localPlayerName + paths[1]).transform), // L-96 Sniper
            Instantiate(WeaponPrefabs[3], GameObject.Find(localPlayerName + paths[2]).transform) //pistol

            };*/

        for (int i = 0; i < weaponsInstances.Length; i++)
        {
            Debug.Log(weaponsInstances[i].name);
            weaponsInstances[i].name = localPlayerName +": "+ weaponsInstances[i].name;
            weaponsInstances[i].transform.localPosition = Vector3.zero;
            weaponsInstances[i].transform.localRotation = Quaternion.identity;
            NetworkServer.Spawn(weaponsInstances[i]);
            
        }

    }



    [Client]
    public void AddWeaponsToPlayer()
    {
        
        var weaponsToGet = WeaponMenu.weaponsToGet;
        Debug.Log(weaponsToGet[0] + "   " + weaponsToGet[1] +"   "+ weaponsToGet[2]);

        for (int i = 0; i < weaponsToGet.Length; i++)
        {
            if (!weaponsToGet[i].Equals("") || weaponsToGet[i] != null)
            {
                int index = getPrefab(weaponsToGet[i]);
                if(index != -1)
                {
                    
                    if (WeaponPrefabs[index].GetComponent<Gun>().type.Equals("Pistol"))
                    {
                        
                        CmdInstantiatePistolPrefabOnPlayer(WeaponPrefabs[index]);
                       
                    }
                    else
                    {
                        CmdInstantiateMainWeaponPrefabOnPlayer(WeaponPrefabs[index]);
                    }
                }
                
            }
        }
    }

    [Command]
    public void CmdInstantiateMainWeaponPrefabOnPlayer(GameObject _weaponPrefab)
    {
        string path = "";
        
        if (!rifleAddedToFirstSlot)
        {
            path=GetGameObjectPath(GameObject.Find("Rifle1"));

            
            var weaponInstance = Instantiate(_weaponPrefab, GameObject.Find(localPlayerName + path).transform);
            weaponInstance.name = localPlayerName + " " + weaponInstance.name;
            
            NetworkServer.Spawn(weaponInstance);
            setLayerRecursively(weaponInstance, LayerMask.NameToLayer(LOCAL_PLAYER_LAYER));
            //weaponInstance.transform.SetParent(GameObject.Find("Rifle1").transform);
            
            rifleAddedToFirstSlot = true;
        }
        else if (!rifleAddedToSecondSlot)
        {
            path = GetGameObjectPath(GameObject.Find("Rifle2"));
            
            var weaponInstance = Instantiate(_weaponPrefab, GameObject.Find(localPlayerName + path).transform);
            weaponInstance.name = localPlayerName + " " + weaponInstance.name;
            
            NetworkServer.Spawn(weaponInstance);
            setLayerRecursively(weaponInstance, LayerMask.NameToLayer(LOCAL_PLAYER_LAYER));
            rifleAddedToSecondSlot = true;
        }
        else
        {
            Debug.Log("Couldn't add weapons to player...");
        }




    }

    // path of gameObject
    public static string GetGameObjectPath(GameObject obj)
    {
        var rootName = obj.transform.root.gameObject.name;
        string path = "/" + obj.name;
        while (obj.transform.parent != null)
        {
            
            obj = obj.transform.parent.gameObject;
            if (!obj.name.Equals(rootName))
            {
                path = "/" + obj.name + path;
            }
        }
        return path;
    }

    [Command]
    public void CmdInstantiatePistolPrefabOnPlayer(GameObject _weaponPrefab)
    {
        string path = GetGameObjectPath(GameObject.Find("Pistol"));
        var weaponInstance = Instantiate(_weaponPrefab, GameObject.Find(localPlayerName + path).transform);
        weaponInstance.name = localPlayerName + " " + weaponInstance.name;
        
        NetworkServer.Spawn(weaponInstance);
        setLayerRecursively(weaponInstance, LayerMask.NameToLayer(LOCAL_PLAYER_LAYER));
        //weaponInstance.transform.SetParent(GameObject.Find("Pistol").transform);
        //weaponInstance.transform.localPosition = Vector3.zero;
        //weaponInstance.transform.localRotation = Quaternion.identity;
    }

    public int getPrefab(string _weapon)
    {
        for (int i = 0; i < WeaponPrefabs.Length; i++)
        {
            if(WeaponPrefabs[i].name.StartsWith(_weapon.Substring(0, 2)))
            {
                return i;
            }

            
        }
        return -1;
    }


}
