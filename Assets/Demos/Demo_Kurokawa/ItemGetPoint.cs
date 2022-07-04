using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGetPoint : MonoBehaviour
{
	void OnTriggerEnter(Collider col)
	{
		//自分ならば
	    if(col.gameObject.tag == "OwnPlayer")
		{
			//ランダムなアイテムを取得。
			col.gameObject.GetComponent<ObtainItemController>().GetRandomItem();
		}
	}
}
