using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// トグル
/// </summary>
public class BlindToggle : MonoBehaviour
{
    [SerializeField]Image m_backgroundImage = null;
    [SerializeField]RectTransform m_handle = null;
    [SerializeField]bool m_onAwake = false;

    /// <summary>
    /// トグルの値
    /// </summary>
    [NonSerialized] public bool m_value = false;

    float m_handlePosX = 0.0f;
    Sequence m_sequence = null;

    static readonly Color OFF_BG_COLOR = new Color(0.92f, 0.92f, 0.92f);
    static readonly Color ON_BG_COLOR = new Color(0.2f, 0.84f, 0.3f);

    const float SWITCH_DURATION = 0.36f;

    void Start()
    {
        m_handlePosX = Mathf.Abs(m_handle.anchoredPosition.x);
        m_value = m_onAwake;
        UpdateToggle(0);
    }

    /// <summary>
    /// トグルのボタンアクションに設定しておく
    /// </summary>
    public void SwitchToggle()
    {
        m_value = !m_value;
        UpdateToggle(SWITCH_DURATION);
    }

    /// <summary>
    /// 状態を反映させる
    /// </summary>
    void UpdateToggle(float duration)
    {
        var bgColor = m_value ? ON_BG_COLOR : OFF_BG_COLOR;
        var handleDestX = m_value ? m_handlePosX : -m_handlePosX;

        m_sequence?.Complete();
        m_sequence = DOTween.Sequence();
        m_sequence.Append(m_backgroundImage.DOColor(bgColor, duration))
            .Join(m_handle.DOAnchorPosX(handleDestX, duration / 2));
    }
}