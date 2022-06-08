using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CheckPoint : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider col)
	{
        if(col.gameObject.tag == "OwnPlayer")
		{
            col.gameObject.GetComponent<ProgressChecker>().SetThroughPointName(this.gameObject.name);
            Debug.Log(this.gameObject.name);
		}
	}
}
