using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class ItemStateCommunicator : MonoBehaviourPunCallbacks
{
    public GameObject m_orangePeel;
    public GameObject m_snapper;

    private int m_orangePeelSum = 0;            //ゲーム中生成したオレンジの皮の合計
    private int m_snapperSum = 0;               //ゲーム中生成したタイの合計

    // Start is called before the first frame update
    void Start()
    {
        
    }

    [PunRPC]
    public void DestroyItemWithName(string name)
	{
        Debug.Log("in destroy fnc");
        GameObject go = GameObject.Find(name);
        if(go != null)
		{
            Debug.Log(go.name);
            Destroy(go);
        }
		else
		{
            Debug.Log("NULL");
		}
	}

    public void PopItem(string prefabName, Vector3 popPos, int wayPointNumber, int playerNumber)
	{
        if(prefabName == "OrangePeel")
		{
            m_orangePeelSum++;
            GameObject peel = Instantiate(m_orangePeel, popPos, Quaternion.identity);
            peel.name = "OrangePeel" + m_orangePeelSum;
        }
        else if(prefabName == "Snapper")
		{
            m_snapperSum++;
            GameObject snapper = Instantiate(m_snapper, popPos, Quaternion.identity);
            snapper.GetComponent<WayPointChecker>().SetCurrentWayPointDirectly(popPos, wayPointNumber);
            snapper.GetComponent<SnapperController>().SetOwnerID(playerNumber);
            snapper.name = "Snapper" + m_snapperSum;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
