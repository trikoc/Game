using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Networking;
using Globals;

public class Gun : MonoBehaviour
{
    private const string PLAYER_TAG = "Player";

    public GameObject bulletReady;
    public Transform gunBarrel;
    [SerializeField]
    private int maxBullets;
    private int allBullets;
    public int currentBullets;

    public string type;
    public float bulletSpeed = 733;

    public float despawnTime = 5.0f;
    public bool shootAble = true;

    public string name;
    public float fireRate;

    public float bulletDMGHead;
    public float bulletDMGTorso;
    public float bulletDMGLegsAndArms;


    [SerializeField]
    private AudioClip shootSound;

    // Start is called before the first frame update
    void Start()
    {
        currentBullets = maxBullets;
        AllBullets = (int)(2.5f * maxBullets);
    }


    public int getBullets()
    {
        return currentBullets;
    }

    public int AllBullets{
        get { return allBullets; }
        set { allBullets = value; }
    }
    public int MaxBullets
    {
        get { return maxBullets; }
    }


    public AudioClip getShootAudio()
    {
        return shootSound;
    }
}
