using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PauseButton : MonoBehaviourPunCallbacks
{
    public GameObject m_pauseButtonPrefab;
    private GameObject m_pausePanel = null;
    private bool m_isDisplayPausePanel = false;

    // Start is called before the first frame update
    void Start()
    {
        m_pauseButtonPrefab.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OffPausePanel()
	{
        m_isDisplayPausePanel = false;
        m_pauseButtonPrefab.SetActive(false);
        Time.timeScale = 1.0f;
    }

    public bool GetIsPause()
	{
        return m_isDisplayPausePanel;
	}

    public void OnPauseButtonClicked()
	{
        m_isDisplayPausePanel = !m_isDisplayPausePanel;

        GameObject.Find("OperationSystem").GetComponent<Operation>().ResetDoubleTapParam();

        if(m_isDisplayPausePanel)
		{
            //m_pausePanel = Instantiate(m_pauseButtonPrefab, Vector3.one, Quaternion.identity);
            m_pauseButtonPrefab.SetActive(true);
            if (PhotonNetwork.OfflineMode)
			{
                Time.timeScale = 0.0f;
            }
        }
		else
		{
            m_pauseButtonPrefab.SetActive(false);
            Time.timeScale = 1.0f;
		}
	}
}
