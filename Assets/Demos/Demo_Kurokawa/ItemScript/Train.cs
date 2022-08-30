using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Train : MonoBehaviourPunCallbacks
{

    [SerializeField] float LIFETIME = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(this.gameObject, LIFETIME);   
    }

    // Update is called once per frame
    void Update()
    {
    }
}
