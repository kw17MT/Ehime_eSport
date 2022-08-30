using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharaReaction : MonoBehaviour
{
    [SerializeField] int m_myNumber = 0;                    //キャラ番号
    private float m_rolling = 0.0f;                         //回転度合い
    Vector3 currentScale = new Vector3(0.5f,0.5f,0.5f);                     //現在の拡大率
    private Quaternion rot = Quaternion.identity;           //キャラの回転
    private CharaSelectChange m_charaSelect = null;         //現在選択しているキャラ番号保有インスタンス

    const float ROLLING_RATE = 50.0f;

    // Start is called before the first frame update
    void Start()
    {
        m_charaSelect = GameObject.Find("SceneManager").GetComponent<CharaSelectChange>();
    }

    // Update is called once per frame
    void Update()
    {
        //ゲームタイムで回転を加算
        m_rolling += Time.deltaTime * ROLLING_RATE;
        //Y軸まわりに回転
        rot = Quaternion.Euler(0.0f, m_rolling, 0.0f);
        this.transform.rotation = rot;

        Vector3 scaleChangeRate = new Vector3(Time.deltaTime, Time.deltaTime, Time.deltaTime);
        //現在選択されているキャラが自分の番号ならば
        if (m_myNumber == m_charaSelect.GetNowSelectState())
		{

            currentScale += scaleChangeRate;
            //最大拡大率は1にする
            if (currentScale.x >= 1.0f)
			{
                currentScale = Vector3.one;
            }
		}
		else
		{
            currentScale -= scaleChangeRate;
            if(currentScale.x <= 0.5f)
			{
                currentScale = new Vector3(0.5f, 0.5f, 0.5f) ;
			}
		}
        this.transform.localScale = currentScale;
    }
}
