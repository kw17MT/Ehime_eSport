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
		//�����Ȃ��
	    if(col.gameObject.tag == "OwnPlayer")
		{
			//�����_���ȃA�C�e�����擾�B
			col.gameObject.GetComponent<ObtainItemController>().GetRandomItem();
		}
	}

	// Update is called once per frame
	void Update()
    {
        
    }
}
