using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GoalScript : MonoBehaviourPunCallbacks
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
        //Debug.Log("goaled");
        if(col.gameObject.tag == "OwnPlayer")
        {
            //Debug.Log("Found Own");
            if(col.gameObject.GetComponent<ProgressChecker>().CheckCanGoal())
			{
                col.gameObject.GetComponent<AvatarController>().SetGoaled();
			}
        }
    }
}
