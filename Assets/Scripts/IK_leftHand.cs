using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(GunChanger))]
[RequireComponent(typeof(GunManager))]

public class IK_leftHand : NetworkBehaviour
{

    protected Animator anim;
    protected GunChanger _gc;
    [SerializeField]
    Transform _equippedGun;
    [SerializeField]
    Transform leftHand;

    [SyncVar(hook = "OnTargetChanged")]
    public bool set;

    void OnTargetChanged(bool r)
    {
        if (r)
        {
            targetObj = _equippedGun.GetChild(0).Find("1_holder").transform;
            set = r;
            

        }
        else
        {
            targetObj = null;
            set = r;
        }


    }

    //public bool set = false;
    public Transform targetObj = null;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        _gc = GetComponent<GunChanger>();

    }


    private void OnAnimatorIK()
    {
        if (set)
        {
            
        // Set the right hand target position and rotation, if one has been assigned
       
            if (targetObj != null)
            {
               
                anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
                anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
                anim.SetIKPosition(AvatarIKGoal.LeftHand, targetObj.position);
                anim.SetIKRotation(AvatarIKGoal.LeftHand, targetObj.rotation);
                

               
            }

        }



    }


    public void setLeftHandIK(int weapon)
    {
        if (weapon > 0)
        {

            set = true;

        }
        else
        {
            set = false;
        }
        CmdSet(weapon);
       /* if (weapon > 0)
        {
            //targetObj = _equippedGun.GetChild(0).Find("1_holder").transform;
            rif = weapon;
            set = true;

        }
        else
        {
            resetLeftHandIK();
        }*/
    }
    [Command]
    void CmdSet(int weapon)
    {
        set = weapon > 0 ? true : false;
        RpcSet(weapon);
    }
    [ClientRpc]
    void RpcSet(int weapon)
    {
        set = weapon > 0 ? true : false;
        if (weapon > 0)
        {
            targetObj = _equippedGun.GetChild(0).Find("1_holder").transform;
        }
        else
        {
            targetObj = null;
        }

    }
/*
    public void resetLeftHandIK()
    {
        targetObj = null;
        set = false;
    }
    */
    // Update is called once per frame
    void Update()
    {
        
    }



}
