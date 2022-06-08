using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class OrangePeel : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    void OnCollisionEnter(Collision col)
	{
        photonView.RPC(nameof(DestroyHittedOrangePeel), RpcTarget.All, this.gameObject.name);
        Debug.Log("collisionEnter " + this.gameObject.name);
    }

    [PunRPC]
    public void DestroyHittedOrangePeel(string hittedOrangePeelName)
    {
        GameObject orange = GameObject.Find(hittedOrangePeelName);
        
        if (orange != null)
        {
            Debug.Log("destroy " + hittedOrangePeelName);
            Destroy(orange.gameObject);
        }
    }
}
