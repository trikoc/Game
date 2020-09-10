using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Globals;
/*
 * work in progress pistol.........
 * 
 * 
 */
public class GunChangertemp : MonoBehaviour
{
    private Animator _animator;

    public Transform _rifle1;
    public Transform _rifle2;
    public Transform _pistol;
    public Transform _equippedGun;

    public int preDisarmedWeapon = 1;
    public bool canGrab = false;
    public bool changed = false;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        _rifle1 = GameObject.Find("Rifle1").transform;
        _rifle2 = GameObject.Find("Rifle2").transform;
        _pistol = GameObject.Find("Pistol").transform;
        _equippedGun = GameObject.Find("EquippedGun").transform;

    }



    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown(PlayerInput.Rifle) && !_animator.GetBool("Aim") && !canGrab)
        {

            if (_animator.GetCurrentAnimatorStateInfo(0).IsTag("Rloco") || _animator.GetCurrentAnimatorStateInfo(0).IsTag("Ploco"))
            {

                if (_rifle1.childCount == 1 && _rifle2.childCount == 0 && Input.GetKeyDown(KeyCode.Alpha1))
                {

                    preDisarmedWeapon = 1;
                    _animator.SetTrigger("DisarmAndGrabRifle");
                }
                else if (_rifle1.childCount == 0 && _rifle2.childCount == 1 && Input.GetKeyDown(KeyCode.Alpha2))
                {

                    preDisarmedWeapon = 2;
                    _animator.SetTrigger("DisarmAndGrabRifle");
                }
            }

            else
            {
                //canGrab = true;
                _animator.SetTrigger("GrabRifle");
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    preDisarmedWeapon = 1;
                }
                else if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    preDisarmedWeapon = 2;
                }
            }
            canGrab = true;
            StartCoroutine(ChangeWeaponYield());

        }

        if (Input.GetButtonDown(PlayerInput.Pistol) && !_animator.GetCurrentAnimatorStateInfo(0).IsTag("Ploco") && !_animator.GetBool("Aim") && !canGrab)
        {
            canGrab = true;
            if (_animator.GetCurrentAnimatorStateInfo(0).IsTag("Rloco"))
            {
                _animator.SetTrigger("DisarmAndGrabPistol");
            }
            else
            {
                _animator.SetTrigger("GrabPistol");
            }
            preDisarmedWeapon = 3;
            // za pištolo
        }
        if (Input.GetKeyDown(KeyCode.X) && !_animator.GetBool("Aim") && !canGrab)
        {
            Debug.Log("dddd");
            canGrab = true;
            if ((_animator.GetCurrentAnimatorStateInfo(1).IsName("Pistol") || _animator.GetCurrentAnimatorStateInfo(1).IsName("Rifle")))
            {

                _animator.SetTrigger("Disarm");

            }
            else if (_animator.GetCurrentAnimatorStateInfo(1).IsName("State"))
            {

                _animator.SetTrigger("GrabRifle");
            }
            StartCoroutine(ChangeWeaponYield());
        }
    }


    private void LateUpdate()
    {

    }

    public void changeWeapon(ref Transform oldRifle, ref Transform newRifle)
    {
        //_equippedGun.GetChild(_equippedGun.childCount - 1).SetParent(oldRifle, false);
        disarm(ref oldRifle);
        //newRifle.GetChild(newRifle.childCount - 1).SetParent(_equippedGun, false);
        requip(ref newRifle);

    }

    public void disarm(ref Transform weapon)
    {
        // Destroy(_equippedGun.GetChild(0).GetChild(4).GetComponent<Camera>());
        _equippedGun.GetChild(_equippedGun.childCount - 1).SetParent(weapon, false);
        weapon.GetChild(0).localRotation = Quaternion.identity;

    }
    public void requip(ref Transform weapon)
    {
        weapon.GetChild(weapon.childCount - 1).SetParent(_equippedGun, false);
        /*_equippedGun.GetChild(0).GetChild(4).gameObject.AddComponent<Camera>();
        _equippedGun.GetChild(0).GetChild(4).gameObject.GetComponent<Camera>().fieldOfView=40;*/
    }

    private ref Transform getWeapon(int weapon)
    {
        switch (weapon)
        {
            case 1:
                return ref _rifle1;
                break;
            case 2:
                return ref _rifle2;
                break;
            default:
                return ref _pistol;
                break;


        }
    }

    IEnumerator ChangeWeaponYield()
    {

        yield return new WaitForSeconds(.9f);

        var state = _animator.GetCurrentAnimatorStateInfo(_animator.GetLayerIndex("ChangeWeapon"));

        if (state.IsName("Grab rifle") || state.IsName("Grab pistol"))
        {

            requip(ref getWeapon(preDisarmedWeapon));

        }
        else if (state.IsName("Disarm"))
        {

            if (_rifle2.childCount == 0)
            {

                disarm(ref _rifle2);
                //preDisarmedWeapon = 2;

            }
            else if (_rifle1.childCount == 0)
            {

                disarm(ref _rifle1);
                //preDisarmedWeapon = 1;

            }
            else
            {
                disarm(ref _pistol);
                //preDisarmedWeapon = 3;
            }
        }
        else if (state.IsName("Change Weapon"))
        {
            if (_rifle1.childCount == 1 && _rifle2.childCount == 0)
            {

                changeWeapon(ref _rifle2, ref _rifle1);

            }
            else if (_rifle1.childCount == 0 && _rifle2.childCount == 1)
            {

                changeWeapon(ref _rifle1, ref _rifle2);

            }

        }
        else if (state.IsName("Change To Pistol"))
        {
            if (_rifle2.childCount == 0)
            {

                changeWeapon(ref _rifle2, ref _pistol);

            }
            else if (_rifle1.childCount == 0)
            {

                changeWeapon(ref _rifle1, ref _pistol);

            }

        }
        else if (state.IsName("Change To Rifle"))
        {
            changeWeapon(ref _pistol, ref getWeapon(preDisarmedWeapon));


        }

        changed = true;
        yield return new WaitForSeconds(0.020f);
        changed = false;
        yield return new WaitForSeconds(1.180f);
        canGrab = false;



    }

}