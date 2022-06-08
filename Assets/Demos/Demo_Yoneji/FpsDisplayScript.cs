using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FpsDisplayScript : MonoBehaviour
{

    //// Field ////
    int m_frameCount = 0;
    float m_prevTime = 0.0f;
    float m_fps = 0.0f;

    //// Const Field ////
    float m_kFpsUpdateInterval = 0.5f;


    //// Unity Function ////

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        m_frameCount++;

        float time = Time.realtimeSinceStartup - m_prevTime;

        if (time >= m_kFpsUpdateInterval)
        {
            m_fps = m_frameCount / time;
            Debug.Log(m_fps);

            m_frameCount = 0;
            m_prevTime = Time.realtimeSinceStartup;
        }

        return;
    }

    private void OnGUI()
    {
        GUILayout.Label(m_fps.ToString());

        return;
    }
}
