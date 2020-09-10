using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Networking;
using Globals;

[RequireComponent(typeof(PlayerInfo))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(GunChanger))]
[RequireComponent(typeof(GunManager))]
[RequireComponent(typeof(IK_leftHand))]
[RequireComponent(typeof(SwitchCamera))]
[RequireComponent(typeof(CharacterController))]


public class PlayerMotor : MonoBehaviour
{
    //public GameObject _player;
    
    private CharacterController _characterController;
    private Animator _animator;

    private PlayerInfo _playerInfo;

    Vector3 prevPos;
    int L_R_step; // left = 0, right = 1;
    [SerializeField]
    private AudioClip [] footSpteps;
    private AudioSource audioSource;

    public float moveSpeed = 6.0f;
    public float gravity = 20f;
    public float jump = 5f;

    private Vector3 moveDir = Vector3.zero;



    private void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _playerInfo = GetComponent<PlayerInfo>();


        audioSource = GetComponent<AudioSource>();
        prevPos = transform.position;
        L_R_step = 0;

    }

    public void stopMove()
    {
        _animator.SetFloat("VelX", 0f);
        _animator.SetFloat("VelY", 0f);
        _characterController.Move(Vector3.down * gravity * Time.deltaTime);
        GetComponent<PlayerMotor>().enabled = false;
    }


    private void Update()
    {
        if ((Vector3.Scale(transform.position, new Vector3(1f,0f,1f)) - Vector3.Scale(prevPos, new Vector3(1f, 0f, 1f))).magnitude > (moveSpeed == 6 ?3f:1.5f))
        {
            L_R_step = L_R_step % 2;
            audioSource.PlayOneShot(footSpteps[L_R_step]);

            prevPos = transform.position;
        }


        if (PauseMenuScript.IsOn)
        {
            if (Cursor.lockState != CursorLockMode.None)
            {
                Cursor.lockState = CursorLockMode.None;
            }

            /*_animator.SetFloat("VelX", 0f, 0.1f, Time.deltaTime);
            _animator.SetFloat("VelY", 0f, 0.1f, Time.deltaTime);
            _characterController.Move(Vector3.down * gravity * Time.deltaTime);
    */
            //stopMove();

            return;
        }

        if (Cursor.lockState != CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        if (!_animator.GetBool("End Game") && GameManager.canMove)
        {
            var hor = Input.GetAxis("Horizontal");
            var ver = Input.GetAxis("Vertical");

            if (_characterController.isGrounded)
            {

                moveDir = ((transform.forward * ver) + (transform.right * hor)).normalized;
                
                if (Input.GetButton(PlayerInput.Walk) || _animator.GetBool("Aim"))
                {
                    moveSpeed = 1f;
                }
                else
                {
                    moveSpeed = 6f;
                }

                moveDir *= moveSpeed;



                if (Input.GetButton(PlayerInput.Jump))
                {
                    moveDir.y = jump;
                }


            }
            moveDir.y -= gravity * Time.deltaTime;


            _animator.SetFloat("VelX", hor * moveSpeed, 0.1f, Time.deltaTime);
            _animator.SetFloat("VelY", ver * moveSpeed, 0.1f, Time.deltaTime);

            
            _characterController.Move(moveDir * Time.deltaTime);


        }
        else
        {
            _characterController.Move(new Vector3(0f,-20f,0f) * Time.deltaTime);
        }
    }
    

}