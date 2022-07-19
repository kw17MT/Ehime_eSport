using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseItemMediater : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LetPlayerUseItem()
	{
        GameObject.Find("OwnPlayer").GetComponent<ObtainItemController>().SetUseItem();
        //Debug.Log("Item Touched");
	}
}
