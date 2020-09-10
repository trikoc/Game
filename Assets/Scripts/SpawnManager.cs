using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SpawnManager : NetworkBehaviour
{

    /// <summary>
    ///  Spawn manager is used to spawn prefabs in the scene
    /// </summary>

    [SerializeField]
    GameObject[] weaponPrefabs;
    private Dictionary<string, GameObject> weapons = new Dictionary<string, GameObject>();


    [SerializeField]
    GameObject[] bulletsPrefabs;
    private Dictionary<string, GameObject> bullets = new Dictionary<string, GameObject>();

    [SerializeField]
    GameObject flashPrefab;

    //GameObject[] bulletPrefab;
    // Start is called before the first frame update
    void Start()
    {
        foreach(GameObject b in bulletsPrefabs)
        {
            bullets.Add(b.name, b);
        }
        foreach (GameObject w in weaponPrefabs)
        {
            bullets.Add(w.name, w);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }



    [Command]
    public void CmdspawnBullet(string name, Vector3 pos, Quaternion rot, Vector3 point, float bulletSpeed, float dmgHead, float dmgTorso, float dmgLegsAndArms)
    {
        var bullet = Instantiate(bullets[name], pos, rot);
        bullet.GetComponent<bulletScript>().setDmgToBulletInstance(dmgHead, dmgTorso, dmgLegsAndArms);
        NetworkServer.Spawn(bullet);


        bullet.GetComponent<Rigidbody>().AddForce((point - pos).normalized * bulletSpeed);

    }
    /*
    [Command]
    public void CmdspawnBullet(string name, Vector3 pos, Quaternion rot, Vector3 point, float bulletSpeed, float dmgHead, float dmgTorso, float dmgLegsAndArms)
    {
        var bullet = Instantiate(bullets[name], pos, rot);
        bullet.GetComponent<bulletScript>().setDmgToBulletInstance(dmgHead, dmgTorso, dmgLegsAndArms);
        NetworkServer.Spawn(bullet);


        bullet.GetComponent<Rigidbody>().AddForce((point - pos).normalized * bulletSpeed);

    }*/

    [Command]
    public void CmdspawnFlash(Vector3 pos, Quaternion rot)
    {
        //var flash = Instantiate(flashPrefab, pos, rot);
        RpcspawnFlash(pos, rot);
        //NetworkServer.Spawn(flash);
    }

    [ClientRpc]
    public void RpcspawnFlash(Vector3 pos, Quaternion rot)
    {
        var flash = Instantiate(flashPrefab, pos, rot);
        //NetworkServer.Spawn(flash);
    }



    [Command]
    public void CmdspawnWeapon(string name, Vector3 pos, Quaternion rot)
    {
        var weapon = Instantiate(weapons[name], pos, rot);
        NetworkServer.Spawn(weapon);

    }



}
