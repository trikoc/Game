using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Networking;

public class HitParticleScript : NetworkBehaviour
{
    private ParticleSystem ps;
    // Start is called before the first frame update
    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        //ps.Play();
    }

    // Update is called once per frame
    void Update()
    {
            if (!ps.isPlaying)
            {
                NetworkServer.Destroy(gameObject);
            }

    }
}
