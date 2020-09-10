using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

using Globals;

public class GunChanger : NetworkBehaviour
{
    private Animator _animator;
    private NetworkAnimator networkAnimator;



    [SerializeField]
    Transform _rifle1;
    [SerializeField]
    Transform _rifle2;
    [SerializeField]
    Transform _pistol;
    [SerializeField]
    Transform _equippedGun;



    public Transform _rifle1Transform;
    public Transform _rifle2Transform;
    public Transform _pistolTransform;


    string _rifle1Path;
    string _rifle2Path;
    string _pistolPath;
    string _equippedPath;

    public int currentWeapon = 0;
    public int newWeapon = 1;
    public bool canGrab = false;
    public bool changed = false;



    [SerializeField]
    NetworkTransformChild ch;

   
    AnimatorStateInfo layer0;
    AnimatorStateInfo layer1;

    string localPlayerName;

    // Start is called before the first frame update

    void Start()
    {

        _animator = GetComponent<Animator>();
        networkAnimator = GetComponent<NetworkAnimator>();
        localPlayerName = ClientScene.FindLocalObject(netId).name;
        _rifle1Path = localPlayerName + GetGameObjectPath(_rifle1.gameObject);
        _rifle2Path = localPlayerName + GetGameObjectPath(_rifle2.gameObject);
        _pistolPath = localPlayerName + GetGameObjectPath(_pistol.gameObject);
        _equippedPath = localPlayerName + GetGameObjectPath(_equippedGun.gameObject);
        /*
        _rifle1Transform = _rifle1.GetChild(0);
        _rifle2Transform = _rifle2.GetChild(0);
        _pistolTransform = _pistol.GetChild(0);*/
        
    }


    public void SetWeaponTransform()
    {
        _rifle1Transform = _rifle1.GetChild(0);
        _rifle2Transform = _rifle2.GetChild(0);
        _pistolTransform = _pistol.GetChild(0);
    }

    // Update is called once per frame
    void Update()
    {

        
        layer0 = _animator.GetCurrentAnimatorStateInfo(0);
        layer1 = _animator.GetCurrentAnimatorStateInfo(1);
        bool rifle1Button = Input.GetKeyDown(KeyCode.Alpha1);
        bool rifle2Button = Input.GetKeyDown(KeyCode.Alpha2);
        bool pistolButton = Input.GetKeyDown(KeyCode.Alpha3);
        bool xButton = Input.GetKeyDown(KeyCode.X);


        // za puško 1 ali 2

        if (canEquip())
        {
            

            if (rifle1Button && _rifle1.childCount != 0 && !_animator.GetBool("Aim") && !canGrab && (layer1.IsName("Rifle") || layer1.IsName("State") || layer1.IsName("Pistol")))
            {
                
                toEquip(1);
            }
            if (rifle2Button && _rifle2.childCount != 0 && !_animator.GetBool("Aim") && !canGrab && (layer1.IsName("Rifle") || layer1.IsName("State") || layer1.IsName("Pistol")))
            {
                
                toEquip(2);
            }

            // za pištolo
            if (pistolButton && _pistol.childCount != 0 && !_animator.GetBool("Aim") && !canGrab && (layer1.IsName("Rifle") || layer1.IsName("State")))
            {
                
                toEquip(3);
            }
        }
        if (xButton && !_animator.GetBool("Aim") && !canGrab && (layer1.IsName("Rifle") || layer1.IsName("State") || layer1.IsName("Pistol")))
        {
            canGrab = true;
            if (_animator.GetCurrentAnimatorStateInfo(0).IsTag("Rloco"))
            {

                //
                //CmdSetTrigger("Disarm");
                networkAnimator.SetTrigger("Disarm");
                _animator.ResetTrigger("Disarm");
                
                StartCoroutine(ChangeWeaponYield());

            }
            else if (_animator.GetCurrentAnimatorStateInfo(0).IsTag("Ploco"))
            {
                networkAnimator.SetTrigger("Disarm");
                _animator.ResetTrigger("Disarm");

                StartCoroutine(ChangeWeaponYield());
                //preDisarmedWeapon = 3;
            }
            else
            {
                //preDisarmedWeapon = 1;
                //
                //CmdSetTrigger("GrabRifle");
                if (newWeapon == 3)
                {
                    networkAnimator.SetTrigger("GrabPistol");
                    _animator.ResetTrigger("GrabPistol");
                }
                else
                {
                    networkAnimator.SetTrigger("GrabRifle");
                    _animator.ResetTrigger("GrabRifle");
                }
                //CmdEquip(currentWeapon);
                toEquip(newWeapon);
            }
            StartCoroutine(ChangeWeaponYield());
        }
    }
    /*
    [Command]
    public void CmdSetTrigger(string trigger)
    {
        RpcSetTrigger(trigger);
    }

    [ClientRpc]
    public void RpcSetTrigger(string trigger)
    {
        Debug.Log(trigger+ " is set");
        _animator.SetTrigger(trigger);
    }*/
    
    public void ChangeWeapon(int oldWeapon, int newWeapon)
    {
        CmdDisarm(oldWeapon);
        CmdEquip(newWeapon);
    }

    [Command]
    public void CmdDisarm(int _currentWeapon)
    {

        RpcSyncDisarmParent(_currentWeapon);
    }

    [ClientRpc]
    public void RpcSyncDisarmParent(int _currentWeapon)
    {
        setWeaponParent(false, _currentWeapon);
    }

    [Command]
    public void CmdEquip(int _newWeapon)
    {
        RpcSyncEquipParent(_newWeapon);
    }

    [ClientRpc]
    public void RpcSyncEquipParent(int _currentWeapon)
    {
        setWeaponParent(true, _currentWeapon);
    }


    public void setWeaponParent(bool EquipOrDisarm, int _weapon)
    {
        switch (_weapon)
        {

            case 1:
                if (EquipOrDisarm)
                {
                    _rifle1Transform.parent = _equippedGun;
                    ch.target = _rifle1Transform;

                }
                else
                {
                    _rifle1Transform.transform.parent = _rifle1.transform;
                    //ch.target = null;
                }
                _rifle1Transform.localPosition = Vector3.zero;
                _rifle1Transform.localRotation = Quaternion.identity;

                break;
            case 2:
                if (EquipOrDisarm)
                {
                    _rifle2Transform.parent = _equippedGun;
                    ch.target = _rifle2Transform;
                }
                else
                {
                    _rifle2Transform.parent = _rifle2;
                    //ch.target = null;
                }
                _rifle2Transform.localPosition = Vector3.zero;
                _rifle2Transform.localRotation = Quaternion.identity;
                break;
            case 3:
                if (EquipOrDisarm)
                {
                    _pistolTransform.parent = _equippedGun;
                    ch.target = _pistolTransform;
                }
                else
                {
                    _pistolTransform.parent = _pistol;
                    //ch.target = null;
                }
                _pistolTransform.localPosition = Vector3.zero;
                _pistolTransform.localRotation = Quaternion.identity;
                break;

        }
    }


    public string WeaponToPath(string name)
    {
        if (name.Equals("Rifle1"))
        {
            return _rifle1Path;
        }
        if (name.Equals("Rifle2"))
        {
            return _rifle2Path;

        }
        return _pistolPath;
    } 
    
    private string getWeapon(int weapon)
    {
        switch (weapon)
        {
            case 1:
                return _rifle1Path;
            case 2:
                return _rifle2Path;
            case 3:
                return _pistolPath;
            default:
                return _equippedPath;

        }
    }

    IEnumerator ChangeWeaponYield()
    {
        GetComponent<IK_leftHand>().setLeftHandIK(0);
        yield return new WaitForSeconds(.9f);

        var state = _animator.GetCurrentAnimatorStateInfo(_animator.GetLayerIndex("ChangeWeapon"));
        bool leftHandIK;
        
        if (state.IsName("Grab rifle") || state.IsName("Grab pistol"))
        {
            CmdEquip(newWeapon);
            leftHandIK = true;
        }
        else if (state.IsName("Disarm Rifle") || state.IsName("Disarm Pistol"))
        {
            CmdDisarm(currentWeapon);
            leftHandIK = false;
        }
        else
        {
            ChangeWeapon(currentWeapon, newWeapon);
            leftHandIK = true;
        }
        
        
        changed = true;
        yield return new WaitForSeconds(0.020f);
        changed = false;
        
        yield return new WaitForSeconds(1.180f);
        canGrab = false;
        if (leftHandIK)
        {
            currentWeapon = newWeapon;
        }
        else
        {
            currentWeapon = 0;
        }
        
        GetComponent<IK_leftHand>().setLeftHandIK(currentWeapon);


    }
       



    public void toEquip(int weaponNumber)
    {
        canGrab = true;
        if (weaponNumber < 3)
        {
            if (layer0.IsTag("Bloco"))
            {
                networkAnimator.SetTrigger("GrabRifle");
                _animator.ResetTrigger("GrabRifle");
                

            }
            else
            {
                networkAnimator.SetTrigger("DisarmAndGrabRifle");
                _animator.ResetTrigger("DisarmAndGrabRifle");

            }

        }
        else
        {
            //pistol

            if (layer0.IsTag("Bloco"))
            {
                networkAnimator.SetTrigger("GrabPistol");
                _animator.ResetTrigger("GrabPistol");
               
            }
            else
            {
                networkAnimator.SetTrigger("DisarmAndGrabPistol");
                _animator.ResetTrigger("DisarmAndGrabPistol");
            }
        }
        newWeapon = weaponNumber;
        StartCoroutine(ChangeWeaponYield());
    }

    public bool canEquip()
    {
        if (hasChild(ref _rifle1) || hasChild(ref _rifle2 ) || hasChild(ref _pistol))
        {
            return true;
        }
        return false;
    }


    public bool hasChild(ref Transform parent)
    {
        if(parent.childCount > 0)
        {
            return true;
        }
        return false;
    }



    // path of gameObject
    public string GetGameObjectPath(GameObject obj)
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


}

