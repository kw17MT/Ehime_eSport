using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGetPoint : MonoBehaviour
{
	private void OnTriggerEnter(Collider col)
	{
		//�����Ȃ��
	    if(col.gameObject.tag == "OwnPlayer")
		{
			//�����_���ȃA�C�e�����擾�B
			col.gameObject.GetComponent<ObtainItemController>().GetRandomItem();
		}
	}
}
