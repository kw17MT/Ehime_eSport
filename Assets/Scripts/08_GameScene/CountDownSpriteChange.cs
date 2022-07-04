using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �Q�[����ʂ̃J�E���g�_�E���̕\�����X�V����N���X
/// </summary>
public class CountDownSpriteChange : MonoBehaviour
{
    //�J�E���g�_�E���摜
    [SerializeField] Sprite[] m_countDownSprite = null;

    public void ChangeCountDownSprite(int countDownNum)
    {
        //���ʂɉ����ĉ摜��ύX
        this.GetComponent<Image>().sprite = m_countDownSprite[countDownNum];
    }
}