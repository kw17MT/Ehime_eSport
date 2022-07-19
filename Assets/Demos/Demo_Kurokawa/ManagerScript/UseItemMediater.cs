using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseItemMediater : MonoBehaviour
{
	//キャンバス上のアイテム使用位置が押されたら、自分のプレイヤーにアイテムを使うように伝える
    public void LetPlayerUseItem()
	{
		//OwnPlayerはネットワークオブジェクトとしてInstantiateするため、生成されるまではOwnPlayerを参照できないため、この関数が必要
        GameObject.Find("OwnPlayer").GetComponent<ObtainItemController>().SetUseItem();
	}
}
