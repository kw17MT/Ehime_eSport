using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGetPoint : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

	private void OnTriggerEnter(Collider col)
	{
		//自分ならば
	    if(col.gameObject.tag == "OwnPlayer")
		{
			//ランダムなアイテムを取得。
			col.gameObject.GetComponent<ObtainItemController>().GetRandomItem();
		}
	}

	// Update is called once per frame
	void Update()
    {
        
    }
}
