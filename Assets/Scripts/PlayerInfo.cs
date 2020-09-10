using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerInfo : NetworkBehaviour
{

    [SerializeField]
    private float maxHealth = 100f;

    [SyncVar (hook = "OnHealthChanged")]
    public float currentHealth;

    public override void OnStartClient()
    {
        OnHealthChanged(maxHealth);
    }


    public void OnHealthChanged(float value)
    {
        /* 
         * pomembna uporaba if stavka, ker če ne kot value dobi neko čudno številko (cca -490 ???)   !!!!!!!
         * Z if stavkom smo omejili le na veljavne vrednosti 0-100
         */

        
        if (value <= 100 && value >= 0)
        {
            currentHealth = value;
        }
        else
        {
            currentHealth = currentHealth;
        }
    }

    public float getHealthPct()
    {
        
        return (float) currentHealth / maxHealth;
    }

    [SyncVar]
    public bool _isDead;

    [SerializeField]
    private Animator _animator;

    [SerializeField]
    private Behaviour[] disableOnDeath;
    private bool[] wasEnabled;


    public void Setup()
    {
		if (isLocalPlayer)
		{
			//Switch cameras
			GameManager.instance.SetSceneCameraActive(false);
			GetComponent<PlayerSetup>().playerUIInstance.SetActive(true);
		}
        CmdBroadcastNewPlayer();
    }

    // send to server that new player was set up
    [Command]
    private void CmdBroadcastNewPlayer()
    {
        RpcSetupPlayerOnAllClients();
    }

    //
    [ClientRpc]
    private void RpcSetupPlayerOnAllClients()
    {
        wasEnabled = new bool[disableOnDeath.Length];
        for (int i = 0; i < wasEnabled.Length; i++)
        {
            wasEnabled[i] = disableOnDeath[i].enabled;
        }


        SetDefaults();

    }

    [ClientRpc]
    public void RpcTakeDmg(string name, float ammount)
    {
        if (IsDead)
        {
            return;
        }
        //Debug.Log(name.ToUpper() + " damaged you by " + ammount);
        currentHealth = Mathf.Max(0f, (currentHealth - ammount));

        if (currentHealth == 0)
        {
            //Debug.Log(currentHealth + " DEAD");
            Die();
        }

    }

    public void SetDefaults()
    {
        IsDead = false;

        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = wasEnabled[i];
        }

        // če želi dodat coliderje
    }

    [Command]
    public void CmdInitializeSyncVars()
    {
        IsDead = false;
        currentHealth = maxHealth;
    }

    public void Die()
    {
        IsDead = true;
        _animator.SetBool("End Game", true);
        if (isLocalPlayer)
        {
            GameObject.Find("PlayerUI").GetComponent<PlayerUI>().ActivateWinnerOrLoserMenu(false);
        }
        GetComponent<GunManager>().returnToBaseCam();
        StartCoroutine(waitYield());
        /*for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = false;
        }
        Cursor.lockState = CursorLockMode.None;*/

    }

    public void Win()
    {

        // animacija za zmago
        if (isLocalPlayer)
        {
            GameObject.Find("PlayerUI").GetComponent<PlayerUI>().ActivateWinnerOrLoserMenu(true);
        }
        //GetComponent<PlayerMotor>().stopMove();
        GetComponent<GunManager>().returnToBaseCam();

        StartCoroutine(waitYield());
       /*for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = false;
        }
        Cursor.lockState = CursorLockMode.None;*/
        
    }
    IEnumerator waitYield()
    {
        yield return new WaitForSeconds(2);
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = false;
        }
        Cursor.lockState = CursorLockMode.None;
    }

    public bool IsDead
    {
        get
        {
            return _isDead;
        }
        protected set
        {
            _isDead = value;
        }
    }

}
