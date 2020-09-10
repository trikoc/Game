using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

using Globals;

public class PlayerCam : MonoBehaviour {




    public Transform _playerTransform;

    public Transform _XForm_Camera;
    public Transform _XForm_Parent;

    private Vector3 _localRotation;
    private Vector3 _localRotationSTD;
    private float _cameraDist = 3f;

    public float sensitivity = 4f;
    public float orbitSpeed = 10f;

    public float sensitivityScroll = 2f;
    public float scrollSpeed = 6f;

    

    // Use this for initialization
    void Start () {

        this._XForm_Camera = this.transform;
        this._XForm_Parent = this.transform.parent;

    }



    // Update is called once per frame
    void LateUpdate () {
        if (PauseMenuScript.IsOn)
        {
            return;
        }
        if (this.enabled)
        {
            if(Input.GetAxis(PlayerInput.MouseX) != 0 || Input.GetAxis(PlayerInput.MouseY) != 0)
            {
                _localRotation.x += Input.GetAxis(PlayerInput.MouseX) * sensitivity;
                _localRotation.y += -Input.GetAxis(PlayerInput.MouseY) * sensitivity;

                // preprečimo da kamera se obrne čez vrh
                _localRotation.y = Mathf.Clamp(_localRotation.y, -60f, 30f);



            }
            // ČE ŽELIMO MOŽNOST ZOOMIRANJA - s koleščkom lahko zomiramo
            /*
            if(Input.GetAxis(PlayerInput.Scroll) != 0){
                float amount = Input.GetAxis(PlayerInput.Scroll) * sensitivityScroll;

                // omogoča hitrejše zoomiranje ko je dlje
                amount *= (this._cameraDist * .3f);

                this._cameraDist += amount * -1f;


                //min in max razdalja
                this._cameraDist = Mathf.Clamp(this._cameraDist, 1.5f, 100f);

            }*/

        }

        // camera transform
        Quaternion qt = Quaternion.Euler(_localRotation.y, _localRotation.x, 0f);
        this._XForm_Parent.rotation = Quaternion.Lerp(this._XForm_Parent.rotation, qt, Time.deltaTime * orbitSpeed);

        if(this._XForm_Camera.localPosition.z != this._cameraDist * -1f)
        {
            
            this._XForm_Camera.localPosition = new Vector3(0f, 0f, Mathf.Lerp(this._XForm_Camera.localPosition.z, this._cameraDist * -1f, Time.deltaTime * scrollSpeed));
        }

        if (Input.GetButtonUp(PlayerInput.UnlockCamera))
        {
            _localRotation = _localRotationSTD;
            _playerTransform.localRotation = Quaternion.Euler(Vector3.up * _localRotation.x);

        }
        if (!Input.GetButton(PlayerInput.UnlockCamera))
        {
            _localRotationSTD = _localRotation;
            if (!_playerTransform.GetComponent<PlayerInfo>().IsDead)
            {
                _playerTransform.localRotation = Quaternion.Euler(Vector3.up * _localRotation.x);
            }
        }

	}

}
