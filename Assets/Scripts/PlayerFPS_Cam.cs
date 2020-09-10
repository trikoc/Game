using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Globals;

public class PlayerFPS_Cam : MonoBehaviour {
    public GameObject _player;
    private Vector3 _cameraOffset;
    private Animator _animator;

    // Use this for initialization
    void Start () {
        _animator = _player.GetComponent<Animator>();
        _cameraOffset = new Vector3(0f, 1.776f, 0.13f);
            
        transform.position =_cameraOffset + _player.transform.position;
    }
	
	// Update is called once per frame
	void LateUpdate () {
        if (_animator.GetLayerWeight(AnimatorLayerIndex.UpperLayer) > 0)
        {
            transform.position = _cameraOffset + _player.transform.position + new Vector3(0f,0f,0.2f);
        }
        else
        {
            transform.position = _cameraOffset + _player.transform.position;
        }
 
    }
}
