using UnityEngine;
using UnityEngine.Networking;

using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class HostGame : NetworkBehaviour
{

    [SerializeField]
    private uint roomSize = 8;

    [SerializeField]
    private InputField sizeInput;

    private string roomName;

    private NetworkManager networkManager;

    void Start()
    {
        networkManager = NetworkManager.singleton;
        if (networkManager.matchMaker == null)
        {
            networkManager.StartMatchMaker();
        }
    }

    public void SetRoomName(InputField _name)
    {
        roomName = _name.text;
        
    }

    public void SetRoomName(string _name)
    {
        roomName = _name;

    }
    public void SetRoomSize(string _size)
    {
        if (_size.Equals(""))
        {
            roomSize = 8;
            return;
        }
        var size = (uint)int.Parse(_size);
        if (size < 2)
        {
            size = 2;
        }
        if (size > 8)
        {
            size = 8;
        }
        sizeInput.text = "" + size;

        roomSize = size;

    }


    public void CreateRoom()
    {
        if (roomName != "" && roomName != null)
        {
            Debug.Log("Creating Room: " + roomName + " with room for " + roomSize + " players.");
            networkManager.matchMaker.CreateMatch(roomName, roomSize, true, "", "", "", 0, 0, networkManager.OnMatchCreate);
            

        }
    }


}