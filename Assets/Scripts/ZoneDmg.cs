using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class ZoneDmg : NetworkBehaviour
{

    [SerializeField]
    private Transform zone;
    private bool inZone = true;
    public PlayerInfo player;

    private bool canUpdate = true;


    // Start is called before the first frame update
    void Start()
    {
        zone = GameObject.FindGameObjectWithTag("Zone").transform;
        player = transform.GetComponent<PlayerInfo>();

    }
    private void OnTriggerEnter(Collider other)
    {
        inZone = true;
        //Debug.Log(player.name + " In zone ");
    }
    private void OnTriggerExit(Collider other)
    {
        inZone = false;
    }
    

    // Update is called once per frame
    void Update()
    {

        if (!inZone && canUpdate && !player.IsDead)
        {
            canUpdate = false;
            TakeDmg();
        }
    }

    [Client]
    public void TakeDmg()
    {
        CmdTakeDmg();
    }


    [Command]
    public void CmdTakeDmg()
    {
        StartCoroutine(TakeDmgTime());
       
    }


    IEnumerator TakeDmgTime()
    {
        while (!inZone && !player.IsDead)
        {
            player.RpcTakeDmg("Zone", 1f);
            Debug.Log("zone");
            yield return new WaitForSeconds(2f);
            
        }
    }
}
