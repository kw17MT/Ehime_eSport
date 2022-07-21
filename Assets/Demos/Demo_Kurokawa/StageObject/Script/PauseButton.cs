using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PauseButton : MonoBehaviourPunCallbacks
{
    public GameObject m_pausePanel;                     //ポーズ画面
    private bool m_isDisplayPausePanel = false;         //ポーズ画面を出しているかどうか

    // Start is called before the first frame update
    void Start()
    {
        //ポーズ画面をオフにする
        m_pausePanel.SetActive(false);
    }

    //インゲーム側でポーズ画面をオフにする
    public void OffPausePanel()
	{
        m_isDisplayPausePanel = false;
        m_pausePanel.SetActive(false);
        Time.timeScale = 1.0f;
    }

    //現在ポーズしているかどうか
    public bool GetIsPause()
	{
        return m_isDisplayPausePanel;
	}

    public void OnPauseButtonClicked()
	{
        //ポーズの状態を反転させる
        m_isDisplayPausePanel = !m_isDisplayPausePanel;

        //ダブルタップの状態をリセットする
        GameObject.Find("OperationSystem").GetComponent<Operation>().ResetDoubleTapParam();

        //ポーズ画面を出しているならば
        if(m_isDisplayPausePanel)
		{
            //ポーズ画面をオンにする
            m_pausePanel.SetActive(true);
            if (PhotonNetwork.OfflineMode)
			{
                Time.timeScale = 0.0f;
            }
        }
		else
		{
            m_pausePanel.SetActive(false);
            Time.timeScale = 1.0f;
		}
	}
}
