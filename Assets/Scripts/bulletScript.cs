using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class bulletScript : NetworkBehaviour
{
    //public ParticleSystem flash;
    public ParticleSystem hitPlayer;
    public ParticleSystem hitTerrain;

    Vector3 prevPosition;
    Vector3 startPosition;

    float dmgHead;
    float dmgTorso;
    float dmgLegsAndArms;

    bool disabled = false;

    // Start is called before the first frame update
    void Awake()
    {
        startPosition = transform.position;
        prevPosition = transform.position;

    }

    // Update is called once per frame
    void Update()
    {
        if (ToHighOrToLow() || disabled)
        {
            CmdSelfDestruct(0f);
            return;
        }
        if (gameObject.GetComponent<Rigidbody>().velocity.magnitude < 0.01)
        {
            CmdSelfDestruct(4f);
            return;
        }
        RaycastHit[] hits = Physics.RaycastAll(new Ray(prevPosition, (-prevPosition + transform.position).normalized), (prevPosition - transform.position).magnitude);
        int indexOfHits = -1;

        for (int i = 0; i < hits.Length; i++)
        {
            //Debug.Log(hits[i].collider.name);
            //Debug.Log((hits[i].point - startPosition).magnitude);
            if (i == 0)
            {
                indexOfHits = 0;
                continue;
            }
            if(hits[i].collider.gameObject.name != hits[i].collider.transform.root.name)
            {
                if ((hits[i].point - startPosition).magnitude < (hits[indexOfHits].point - startPosition).magnitude )
                {
                    indexOfHits = i;
                }
            }

        }

        if(indexOfHits != -1)
        {
            OnHit(hits[indexOfHits].collider, hits[indexOfHits].point, Quaternion.Euler((hits[indexOfHits].point - startPosition).normalized));
            disabled = true;

            //CmdSelfDestruct(0f);
            return;
        }
        prevPosition = transform.position;
        /*
        for (int i = 1; i < hits.Length; i++)
        {
            Debug.Log(hits[i].collider.name);
            Debug.Log((hits[i].point-startPosition).magnitude);
            

            
            if (!hits[i].collider.gameObject.layer.Equals("Local Player") && hits[i].collider.gameObject.name != hits[i].collider.transform.root.name)
            {
                if((hits[i].point - startPosition).magnitude < (hits[indexOfHits].point - startPosition).magnitude)
                {
                    indexOfHits = i;
                }

                Debug.Log(1);
            }
        }

        if(hits != null)
        {
            OnHit(hits[indexOfHits].collider, hits[indexOfHits].point, Quaternion.Euler((hits[indexOfHits].point - startPosition).normalized));
            CmdSelfDestruct(0f);
            return;
        }
        prevPosition = transform.position;*/

    }

    
    
    void OnHit(Collider collider, Vector3 position, Quaternion rotation)
    {


        if (collider.transform.root.gameObject.tag == "Player")
        {
            /*Hit = Instantiate(hitPlayer, position, rotation);

            Hit.transform.Rotate(Vector3.up * 180);
            */
            CmdHitPlayer(position, rotation);

            CmdPlayerHit(collider.transform.root.gameObject.name, getDmgAmmount(collider.gameObject.name));
        }
        if (collider.transform.root.gameObject.name == "Enviroment")
        {

            //Hit = Instantiate(hitTerrain, position, Quaternion.Euler(-90, 0, 0));
            CmdHitTerrain(position);
        }
        //NetworkServer.Spawn(Hit.gameObject);
    }

    [Command]
    public void CmdHitPlayer(Vector3 pos, Quaternion rot)
    {
        /*ParticleSystem Hit = null;
        Hit = Instantiate(hitPlayer, pos, rot);

        Hit.transform.Rotate(Vector3.up * 180);*/
        //NetworkServer.Spawn(Hit.gameObject);
        RpcHitPlayer(pos, rot);
    }

    [ClientRpc]
    public void RpcHitPlayer(Vector3 pos, Quaternion rot)
    {
        ParticleSystem Hit = null;
        Hit = Instantiate(hitPlayer, pos, rot);

        Hit.transform.Rotate(Vector3.up * 180);
        //NetworkServer.Spawn(Hit.gameObject);
    }

    [Command]
    public void CmdHitTerrain(Vector3 pos)
    {
       /* ParticleSystem Hit = null;
        Hit = Instantiate(hitTerrain, pos, Quaternion.Euler(-90, 0, 0));*/
        //NetworkServer.Spawn(Hit.gameObject);
        RpcHitTerrain(pos);
    }

    [ClientRpc]
    public void RpcHitTerrain(Vector3 pos)
    {
        ParticleSystem Hit = null;
        Hit = Instantiate(hitTerrain, pos, Quaternion.Euler(-90, 0, 0));
        //NetworkServer.Spawn(Hit.gameObject);
    }



    [Command]
    public void CmdSelfDestruct(float time)
    {
        StartCoroutine(DestroyYield(time));
        return;
    }


    IEnumerator DestroyYield(float time)
    {

        yield return new WaitForSeconds(time);
        NetworkServer.Destroy(gameObject);
        NetworkServer.objects.Remove(netId);

    }

    [Command]
    void CmdPlayerHit(string _playerID, float _DmgAmmount)
    {

        var _enemyPlayer = GameManager.GetPlayer(_playerID);

        _enemyPlayer.RpcTakeDmg("bullet", _DmgAmmount);
        Debug.Log(_playerID + " has been shot. Dmg: " + _DmgAmmount);
    }

    public float getDmgAmmount(string name)
    {
        float randomDMG;
        if (name == "mixamorig:Head")
        {
            Debug.Log("Head");
            randomDMG = Random.Range(dmgHead - 10, dmgHead + 10);
            return (randomDMG);

        }
        else if (name.Contains("Spine2"))
        {
            randomDMG = Random.Range(dmgTorso - 5, dmgTorso + 5);

            Debug.Log("Torso");
            return (randomDMG);

        }
        else
        {
            if (name.Contains("UpLeg") || (name == "mixamorig:LeftArm" || name == "mixamorig:RightArm"))
            {
                randomDMG = Random.Range(dmgLegsAndArms - 10, dmgLegsAndArms + 10);

                Debug.Log("UpperLegAndArms");
                return (randomDMG);
            }
            else
            {
                randomDMG = Random.Range(2f, dmgLegsAndArms - 5);

                Debug.Log("LowerLegAndArms");
                return (randomDMG);
            }
        }

    }


    public void setDmgToBulletInstance(float head, float torso, float legsAndArms)
    {
        dmgHead = head;
        dmgTorso = torso;
        dmgLegsAndArms = legsAndArms;
    }

    public bool ToHighOrToLow()
    {
        if(transform.position.y>1000f || transform.position.y < -1000f)
        {
            return true;
        }
        return false;
    }

}
