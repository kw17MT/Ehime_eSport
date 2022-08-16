using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// レンズフレアの移動用スクリプト
/// </summary>
public class LansFlareMove : MonoBehaviour
{
    // Serialize Field
    [SerializeField]
    Vector3 m_toLansFlareVec = new Vector3(-30.0f, 15.0f, 200.0f);
    [SerializeField]
    bool m_enableMove = false;

    // Field
    Camera m_mainCam = null;

    // Start is called before the first frame update
    void Start()
    {
        m_mainCam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_enableMove != true)
        {
            return;
        }


        this.transform.position = m_mainCam.transform.position + m_toLansFlareVec;
    }
}
