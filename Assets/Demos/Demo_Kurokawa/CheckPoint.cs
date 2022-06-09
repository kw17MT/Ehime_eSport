using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CheckPoint : MonoBehaviourPunCallbacks
{
	//チェックポイントをくぐったら
    void OnTriggerEnter(Collider col)
	{
		//OwnPlayerとタグ付けされていたら
        if(col.gameObject.tag == "OwnPlayer")
		{
			//チェックした場所の名前を保存
            col.gameObject.GetComponent<ProgressChecker>().SetThroughPointName(this.gameObject.name);
		}
	}
}
