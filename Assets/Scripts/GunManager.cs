using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Globals;

[RequireComponent(typeof(PlayerSetup))]
public class GunManager : NetworkBehaviour
{
    SpawnManager sp;

    public Animator _animator;
    [SerializeField]
    public Transform _equippedGun;

    public Gun EquippedGunParams;
    public GunChanger _gc;
    public bool ParamsSet = false;

    public Camera _pc;
    public PlayerCam _pcScript;
    private Vector3 point;

    public AudioSource audioSource;
    [SerializeField]
    private AudioClip[] ShootClips;

    [SerializeField]
    GameObject bulletPrefab;

    GameObject cameraAIM;


    // Start is called before the first frame update
    void Start()
    {

        sp = GetComponent<SpawnManager>();
        _animator = transform.root.gameObject.GetComponent<Animator>();
        _gc = GetComponent<GunChanger>();
        _pc = GetComponent<SwitchCamera>().GetActiveCamera();
        _pcScript = _pc.GetComponent<PlayerCam>();
        
    }


    
    // Update is called once per frame
    void Update()
    {


        if (_pc != GetComponent<SwitchCamera>().GetActiveCamera())
        {
            _pc= GetComponent<SwitchCamera>().GetActiveCamera();

        }

        if (_equippedGun.childCount > 0 && _gc.canGrab)
        {

            EquippedGunParams = _equippedGun.GetChild(0).gameObject.GetComponent<Gun>();
            EquippedGunParams.shootAble = true;
            ParamsSet = true;
            audioSource = EquippedGunParams.GetComponent<AudioSource>();


        }
        if (_equippedGun.childCount == 0)
        {

            ParamsSet = false;
            DisplayBullets(false);
            return;
        }
        // zgornje se mora zgodit, spodnje pa ne , ker ne moremo streljat ko je pause menu
        if (PauseMenuScript.IsOn)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            
            GetComponent<IK_leftHand>().setLeftHandIK(0);
            StartCoroutine(ReloadYield());
            _animator.SetTrigger(("Reload"));
        }

       

        
        if (Input.GetButtonDown(PlayerInput.Fire2) && (_animator.GetCurrentAnimatorStateInfo(0).IsTag("Rloco") || _animator.GetCurrentAnimatorStateInfo(0).IsTag("Ploco")) && ParamsSet)
        {
            
            // toggle crosshair
            GetComponent<PlayerSetup>().togglePlayerUI();

            GetComponent<PlayerSetup>().disableSniperUI();

            cameraAIM = _equippedGun.GetChild(0).GetChild(4).gameObject;
            _animator.SetBool("Aim", !_animator.GetBool("Aim"));
            if (_animator.GetBool("Aim")) {
                
                cameraAIM.AddComponent<Camera>();
                cameraAIM.GetComponent<Camera>().cullingMask = -1 & ~(1 << 12);
                if (EquippedGunParams.type != "Sniper")
                {
                    _pcScript.sensitivity = 2f;
                    cameraAIM.GetComponent<Camera>().fieldOfView = 40;
                    
                    
                    cameraAIM.GetComponent<Camera>().nearClipPlane = .07f;
                    
                }
                else
                {
                    _pcScript.sensitivity = .5f;
                    GetComponent<PlayerSetup>().SniperObj.SetActive(true);
                    cameraAIM.GetComponent<Camera>().fieldOfView = 10;

                }
            }
            else
            {
                _pcScript.sensitivity = 4;
                Destroy(cameraAIM.GetComponent<Camera>());
            }

        }

        if (_equippedGun.childCount > 0 && !_animator.GetCurrentAnimatorStateInfo(2).IsName("Reload") && !_gc.canGrab && (_animator.GetCurrentAnimatorStateInfo(1).IsName("Rifle") || _animator.GetCurrentAnimatorStateInfo(1).IsName("Pistol")) && ParamsSet)
        {

            DisplayBullets(true);

            if (EquippedGunParams.getBullets() == 0)
            {
                return;
            }
            if (EquippedGunParams.type.Equals("Assault Rifle"))
            {

                if (Input.GetButton(PlayerInput.Fire1) && EquippedGunParams.shootAble)
                {
                    EquippedGunParams.shootAble = false;
                    InvokeRepeating("Shoot", 0f, 1f / EquippedGunParams.fireRate);

                }
                else if (Input.GetButtonUp(PlayerInput.Fire1))
                {
                    CancelInvoke("Shoot");
                    EquippedGunParams.shootAble = true;

                }
            }
            else
            {
                if (Input.GetButtonDown(PlayerInput.Fire1) && EquippedGunParams.shootAble)
                {

                    EquippedGunParams.shootAble = false;
                    Shoot();

                    StartCoroutine(ShootingYield());
                }
            }

        }
        else
        {
            CancelInvoke("Shoot");
        }

    }

    [Client]
    void Shoot ()
    {
        if (EquippedGunParams.getBullets() == 0)
        {
            CancelInvoke("Shoot");
            return;
        }

        CmdShootSound(_gc.currentWeapon, EquippedGunParams.gunBarrel.position);
        

        EquippedGunParams.currentBullets--;

        sp.CmdspawnBullet(
            bulletPrefab.name,
            EquippedGunParams.gunBarrel.position,
            EquippedGunParams.gunBarrel.rotation,
            point,
            EquippedGunParams.bulletSpeed,
            EquippedGunParams.bulletDMGHead,
            EquippedGunParams.bulletDMGTorso,
            EquippedGunParams.bulletDMGLegsAndArms);

        sp.CmdspawnFlash(_equippedGun.GetChild(0).Find("3_flash").position, _equippedGun.GetChild(0).Find("3_flash").rotation);
    }


    public void returnToBaseCam()
    {
        if (_animator.GetBool("Aim"))
        {
            _animator.SetBool("Aim", false);
            _pcScript.sensitivity = 4;
            GetComponent<PlayerSetup>().disableSniperUI();
            if (cameraAIM != null)
            {
                Destroy(cameraAIM.GetComponent<Camera>());
            }
        }
    }
    

    private void LateUpdate()
    {
        
        if (_equippedGun.childCount > 0 && !Input.GetButton(PlayerInput.UnlockCamera) && (_animator.GetCurrentAnimatorStateInfo(0).IsTag("Rloco") || _animator.GetCurrentAnimatorStateInfo(0).IsTag("Ploco")) && (_animator.GetCurrentAnimatorStateInfo(1).IsName("Pistol") || _animator.GetCurrentAnimatorStateInfo(1).IsName("Rifle")))
        {
            
            var ray = _pc.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0));
            point = ray.GetPoint(103f);
            _equippedGun.GetChild(0).LookAt(point);
        }
    }



    [Command]
    public void CmdShootSound(int clipId, Vector3 pos)
    {
        RpcShootSound(clipId, pos);


    }


    [ClientRpc]
    public void RpcShootSound(int clipId, Vector3 pos)
    {

        AudioSource.PlayClipAtPoint(ShootClips[clipId], pos);
    }


    IEnumerator ShootRechargeYield()
    {
        yield return new WaitForSeconds(ShootClips[1].length);
        audioSource.PlayOneShot(ShootClips[0]);
        
    }

    IEnumerator ShootingYield()
    {
        if (EquippedGunParams.type.Equals("Sniper"))
        {
            StartCoroutine(ShootRechargeYield());
            yield return new WaitForSeconds(ShootClips[1].length + ShootClips[0].length);
        }
        EquippedGunParams.shootAble = true;
    }

    /*
    public float getShootTime()
    {
        switch (EquippedGunParams.type)
        {
            case "Sniper": return ShootClips[1].length+ ShootClips[0].length;
                break;
            case "Pistol": return 0.2f;
                break;
            case "Assault Rifle": return 0.2f;
                break;
            default:
                return 0.25f;
        }
    }
    */

    IEnumerator ReloadYield()
    {
        yield return new WaitForSeconds(3);
        if (EquippedGunParams.AllBullets + EquippedGunParams.getBullets() - EquippedGunParams.MaxBullets >= 0)
        {
            
            EquippedGunParams.AllBullets -= EquippedGunParams.MaxBullets - EquippedGunParams.getBullets();
            EquippedGunParams.currentBullets = EquippedGunParams.MaxBullets;
        }
        else {
            EquippedGunParams.currentBullets += EquippedGunParams.AllBullets;
            EquippedGunParams.AllBullets = 0;
        }
        GetComponent<IK_leftHand>().setLeftHandIK(_gc.currentWeapon);
    }



    public void DisplayBullets(bool disp)
    {
        // prikazat kolko metkov še imamo
        if (disp)
        {
            string color;

            if (EquippedGunParams.AllBullets > 50)
            {
                color= "lime";
            }
            else if (EquippedGunParams.AllBullets > 5)
            {
                color = "yellow";
            }
            else
            {
                color = "red";
            }
            GetComponent<PlayerSetup>().displayBullets(EquippedGunParams.getBullets() + "/<color=" + color + ">" + EquippedGunParams.AllBullets + "</color>");

        }
        else
        {
            GetComponent<PlayerSetup>().displayBullets("");
        }
    }


    [Command]
    public void CmdShoot(Vector3 point, GameObject prefab, Vector3 pos, Quaternion rot, float bulletSpeed, float dmgHead, float dmgTorso, float dmgLegsAndArms)
    {
        var bullet = Instantiate(bulletPrefab, pos, rot);
        bullet.GetComponent<bulletScript>().setDmgToBulletInstance(dmgHead, dmgTorso, dmgLegsAndArms);

        NetworkServer.Spawn(bullet);


        bullet.GetComponent<Rigidbody>().AddForce((point - pos).normalized * bulletSpeed);
    }


}
