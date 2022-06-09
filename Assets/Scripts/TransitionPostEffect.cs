using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionPostEffect : MonoBehaviour
{
    [SerializeField] private Material postEffectMaterial; //トランジションのマテリアル
    [SerializeField] private float transitionTime = 2f; //トランジションの時間
    readonly int _progressId = Shader.PropertyToID("_Progress"); //シェーダープロパティのReference名
    bool m_isTransitionIn = true;   //トランジションがInかOutかフラグ
    bool m_isPlay = false;  //トランジションが再生されているかフラグ

    /// <summary>
    /// 開始時に実行
    /// </summary>
    void Start()
    {
        if (postEffectMaterial != null)
        {
            //トランジションを開始
            StartCoroutine(InTransition());
        }

        //シーン遷移してもこのトランジションのオブジェクトは削除しない
        DontDestroyOnLoad(this);
    }

    //トランジション起動関数
    public void OnTransition()
    {
       //Inのとき、
       if (m_isTransitionIn)
       {
           //トランジションが再生中のとき、
           if (m_isPlay)
           {
               //Outトランジションを一旦終了させる。
               StopCoroutine(OutTransition());
           }
           //Inトランジションを開始
           StartCoroutine(InTransition());
       }
       //Outのとき、
       else
       {
           //トランジションが再生中のとき、
           if (m_isPlay)
           {
               //Inトランジションを一旦終了させる。
               StopCoroutine(InTransition());
           }

           //Outトランジションを開始
           StartCoroutine(OutTransition());
       }
    }

    /// <summary>
    ///Inトランジション
    /// </summary>
    IEnumerator InTransition()
    {
        //トランジションを次はOutが実行されるようにする。
        m_isTransitionIn = !m_isTransitionIn;

        //トランジション再生中にする。
        m_isPlay = true;

        float t = 0f;
        while (t < transitionTime)
        {
            float progress = t / transitionTime;

            // シェーダーの_Progressに値を設定
            postEffectMaterial.SetFloat(_progressId, progress);
            yield return null;

            t += Time.deltaTime;
        }

        postEffectMaterial.SetFloat(_progressId, 1f);

        //トランジション停止中にする。
        m_isPlay = false;
    }

    /// <summary>
    ///Outトランジション
    /// </summary>
    IEnumerator OutTransition()
    {
        //トランジションを次はInが実行されるようにする。
        m_isTransitionIn = !m_isTransitionIn;

        //トランジション再生中にする。
        m_isPlay = true;

        float t = transitionTime;
        while (t > 0.0f)
        {
            float progress = t / transitionTime;

            // シェーダーの_Progressに値を設定
            postEffectMaterial.SetFloat(_progressId, progress);
            yield return null;

            t -= Time.deltaTime;
        }

        postEffectMaterial.SetFloat(_progressId, 1f);

        //トランジション停止中にする。
        m_isPlay = false;
    }
}