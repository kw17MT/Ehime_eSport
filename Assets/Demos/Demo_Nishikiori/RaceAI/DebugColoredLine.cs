using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class DebugColoredLine : MonoBehaviour
{
#if UNITY_EDITOR

    //線の種類(内側、外側)
    private enum LineType
    {
        Inner,
        Outer,
    }

    [SerializeField]
    private LineType m_lineType;                                            //内側の線か、外側の線か

    private Color[] m_drawColors = new Color[] { Color.blue,Color.red};     //描画色(内:青、外:赤)
    
    private GameObject m_parent = null;                                     //親のゲームオブジェクト(ウェイポイント)
#endif

    public void Awake()
    {
#if UNITY_EDITOR
        //親のオブジェクト(ウェイポイント)を取得
        m_parent = gameObject.transform.parent.gameObject;
#endif
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
        //親がなければ何もしない。
        if(m_parent == null)
        {
            return;
        }

        //中心から描画位置までの距離(BoxColliderのX方向のサイズの半分)を計算
        float lineDistance = m_parent.GetComponent<BoxCollider>().size.x / 2;

        //内側の線なら距離の符号を反転
        if(m_lineType == LineType.Inner)
        {
            lineDistance = -lineDistance;
        }

        //距離から線の描画位置をセット
        gameObject.transform.localPosition = new Vector3(lineDistance, 0.0f, 0.0f);

        //描画
        Debug.DrawRay(gameObject.transform.position, Vector3.up * 3.0f, m_drawColors[(int)m_lineType]);
#endif
    }
}
