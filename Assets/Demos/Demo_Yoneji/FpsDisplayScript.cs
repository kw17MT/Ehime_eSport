using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FpsDisplayScript : MonoBehaviour
{
    // SerializeField
    [SerializeField]
    int m_fontSize = 25;

    // Field
    int m_frameCount = 0;
    float m_prevTime = 0.0f;
    float m_fps = 0.0f;
    GUIStyle m_guiStyle = new GUIStyle();

    // Const Field
    float m_kFpsUpdateInterval = 0.5f;


    // Unity Function

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
    }

    // Update is called once per frame
    void Update()
    {
        FpsDisplay();

        return;
    }

    private void OnGUI()
    {
        var styleState = new GUIStyleState();
        styleState.textColor = Color.white;

        var style = new GUIStyle();
        style.fontSize = m_fontSize;
        style.normal = styleState;



        GUILayout.BeginArea(new Rect(50, 50, 400, 150));

        string text = string.Format("FPS = {0:F2}", m_fps);
        GUILayout.Label(text, style);

        string useAPIName = string.Format("API = {0}", SystemInfo.graphicsDeviceType.ToString());
        GUILayout.Label(useAPIName, style);

        GUILayout.EndArea();

        return;
    }


    // Function
    void FpsDisplay()
    {
        m_frameCount++;

        float time = Time.realtimeSinceStartup - m_prevTime;

        if (time >= m_kFpsUpdateInterval)
        {
            m_fps = m_frameCount / time;
            //Debug.Log(m_fps);

            m_frameCount = 0;
            m_prevTime = Time.realtimeSinceStartup;
        }

        return;
    }
}
