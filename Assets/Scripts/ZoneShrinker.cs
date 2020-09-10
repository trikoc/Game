using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ZoneShrinker : NetworkBehaviour
{
    [SyncVar]
    public Vector3 newCenter;

    public bool canShrink = true;
    [SyncVar]
    public float newScale;


    private void Start()
    {
        
        transform.localScale = Vector3.one * 500f;
        newScale = 500f;
        //gameObject.SetActive(true);
    }
    // Update is called once per frame
    void Update()
    {
        if (canShrink)
        {
            if (transform.localScale.x > 100f)
            {


                StartCoroutine(ZoneTimer());


                //transform.localScale = Vector3.one * currentScale * Time.deltaTime;


            }
            else
            {
                canShrink = false;
                CmdDisableZoneTimer();
            }
        }
    }

    [Command]
    void CmdDisableZoneTimer()
    {
        RpcDisableZoneTimer();
    }

    [ClientRpc]
    void RpcDisableZoneTimer()
    {
        GameObject.Find("ZoneTimer").SetActive(false);
        
    }

    IEnumerator ZoneTimer()
    {

        canShrink = false;

        Debug.Log("\nZone will shrink in 60sec");
        //yield return new WaitForSeconds(60f);
        /* for (int i = 0; i <= 60; i++)
         {
             GameObject.Find("Messages").GetComponent<Text>().text += "\nZone will get smaller in " + (60-i) + " seconds";
             yield return new WaitForSeconds(1);
         }*/

        CmdTimer();
        yield return new WaitForSeconds(60);

        CmdShrink();

    }
    [Command]
    void CmdTimer()
    {
        StartCoroutine(TimerYield());
    }

    [ClientRpc]
    void RpcTimer(int time)
    {
        Text zoneTimerText = GameObject.Find("ZoneTimer").GetComponent<Text>();
        if (time > 0)
        {
           zoneTimerText.text = "Zone will get smaller in " + time + " seconds";
        }
        else
        {
            zoneTimerText.text = "<color=red> Restricting Zone </color>";
        }
        
    }

    IEnumerator TimerYield()
    {
        for (int i = 0; i <= 60; i++)
        {
            RpcTimer(60 - i);
            yield return new WaitForSeconds(1f);
        }

    }

    IEnumerator ZoneShrinkerTimer()
    {

        //GetNewCenter();
        Debug.Log("Shrinking");
        while (transform.localScale.x - newScale > 0.05f)
        {

            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * (newScale-20f), .0005f);
            RpcUpdateShrinkOnClients(transform.localScale);

            yield return new WaitForSeconds(.02f);
        }
        Debug.Log("END Shrinking");
        
        transform.localScale = Vector3.one * newScale;
        canShrink = true;
    }

    IEnumerator ZoneCenterTimer()
    {
        float mult = 1;
        Debug.Log("Centering");
        // overCenter omogoča enakomerno spreminjanje centra ko je trenutno središče Zone blizu novega centra
        Vector3 overCenter = (newCenter - transform.position).normalized * 2f; 

        while (Mathf.Abs(transform.position.x - newCenter.x) > 0.1f && Mathf.Abs(transform.position.z - newCenter.z ) > 0.1f)
        {
            
            transform.position = Vector3.Lerp(transform.position, newCenter + overCenter, .0005f*mult);
            RpcUpdateCenterOnClients(transform.position);

            yield return new WaitForSeconds(.02f);
            mult += 0.002f;
        }
        Debug.Log("END Centering");

        transform.position = newCenter;
        
    }
     
    [Command]
    public void CmdShrink()
    {

        newScale -= 50f;
        GetNewCenter();
        StartCoroutine(ZoneShrinkerTimer());
        StartCoroutine(ZoneCenterTimer());


    }

    [ClientRpc]
    public void RpcUpdateShrinkOnClients(Vector3 scale)
    {
        transform.localScale = scale;
    }

    [ClientRpc]
    public void RpcUpdateCenterOnClients(Vector3 position)
    {
        transform.position = position;
    }


    public void GetNewCenter()
    {
        float diff = (newScale / 10f) - 5f;
        Vector3 pos = transform.position;
        float newX = Random.Range(pos.x - diff, pos.x + diff);
        float newY = Random.Range(pos.z - diff, pos.z + diff);
        newCenter = new Vector3(newX, 0f, newY);

    }


}
