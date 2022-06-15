using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPoint : MonoBehaviour
{
    private int m_myNumber = 0;
    // Start is called before the first frame update
    void Start()
    {
        string wayPointName = this.gameObject.name;
        string no;
        //ウェイポイントの番号が二桁ならば
        if (wayPointName.Length == 10)
		{
            no = this.gameObject.name[8..10];
        }
        //一桁ならば
		else
		{
            no = this.gameObject.name[8..9];
        }

        m_myNumber = int.Parse(no);
    }

    private void OnTriggerEnter(Collider col)
	{
        col.gameObject.GetComponent<WayPointChecker>().SetNextWayPoint(col.gameObject.transform.position, m_myNumber);
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
