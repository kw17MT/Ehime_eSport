using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ウェイポイントの基本行動クラス
public class WayPoint : MonoBehaviour
{
    private int m_myNumber = 0;             //自分のウェイポイント番号

    // Start is called before the first frame update
    void Start()
    {
        //切り抜いた番号部分
        string no;
        //ウェイポイントの番号が二桁ならば ex.)WayPoint25
        if (this.gameObject.name.Length == 10)
		{
            //オブジェクトの名前の[8]-[9]部分を取得
            no = this.gameObject.name[8..10];
        }
        //一桁ならば
		else
		{
            //オブジェクトの名前の[8]部分を取得
            no = this.gameObject.name[8..9];
        }
        //自分の番号として保存
        m_myNumber = int.Parse(no);
    }

    //ウェイポイントを通過したら
    private void OnTriggerEnter(Collider col)
	{
        //自分の座標と番号を知らせる
        col.gameObject.GetComponent<WayPointChecker>().SetNextWayPoint(this.gameObject.transform.position, m_myNumber);
	}
}
