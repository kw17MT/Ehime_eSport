using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �g�O��
/// </summary>
public class ToggleOnOff : MonoBehaviour
{
    [SerializeField] Image m_backgroundImage = null;
    [SerializeField] RectTransform m_handle = null;
    [SerializeField] bool m_onAwake = false;
    //�g�O���̒l
    [NonSerialized] public bool m_value = false;

    float m_handlePosX = 0.0f;
    Sequence m_sequence = null;

    static readonly Color m_OFF_BG_COLOR = new Color(0.92f, 0.92f, 0.92f);
    static readonly Color m_ON_BG_COLOR = new Color(0.2f, 0.84f, 0.3f);

    const float m_kSwitchDuration = 0.36f;

    void Start()
    {
        m_handlePosX = Mathf.Abs(m_handle.anchoredPosition.x);
        m_value = m_onAwake;
        UpdateToggle(0);
    }

    /// <summary>
    /// �g�O���̃{�^���A�N�V�����ɐݒ肵�Ă���
    /// </summary>
    public void SwitchToggle()
    {
        m_value = !m_value;
        UpdateToggle(m_kSwitchDuration);
    }

    /// <summary>
    /// ��Ԃ𔽉f������
    /// </summary>
    void UpdateToggle(float duration)
    {
        var bgColor = m_value ? m_ON_BG_COLOR : m_OFF_BG_COLOR;
        var handleDestX = m_value ? m_handlePosX : -m_handlePosX;

        m_sequence?.Complete();
        m_sequence = DOTween.Sequence();
        m_sequence.Append(m_backgroundImage.DOColor(bgColor, duration))
            .Join(m_handle.DOAnchorPosX(handleDestX, duration / 2));
    }

    //�g�O���̒l���擾����v���p�e�B
    public bool GetSetToggleValue
    {
        //�Q�b�^�[
        get
        {
            return m_value;
        }
        //�Z�b�^�[
        set
        {
            m_value = value;
        }
    }
}