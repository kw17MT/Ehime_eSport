using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class DebugColoredLine : MonoBehaviour
{
    public Color m_drawColor;

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
        Debug.DrawRay(gameObject.transform.position, Vector3.up * 3.0f, m_drawColor);
#endif
    }
}
