using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// トランジションクラス
/// </summary>
public class PostEffect : MonoBehaviour
{
    //ポストエフェクトシェーダー入りのマテリアル
    public Material m_postEffectMat;

    //画面切り替え
    const float m_kFadeMax = 128.0f;
    float m_fadeCount = 1.0f;
    [SerializeField]bool m_fadeFlag;

    //更新関数
    void Update()
    {
        //画面切り替え
        //(仮処理：左ボタンクリックをしたら切り替え)
        if(Input.GetMouseButtonDown(0))
        {
            //フェードを切り替える
            m_fadeFlag = !m_fadeFlag;
        }

        m_fadeCount = Mathf.Clamp(m_fadeCount + (m_fadeFlag ? 1.0f : -1.0f) / m_kFadeMax, 0.0f, 1.0f);

        m_postEffectMat.SetFloat("_FadeCount", m_fadeCount);
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        Graphics.Blit(src, dest, m_postEffectMat);
    }
}