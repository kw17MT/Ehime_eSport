using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �Q�[����ʂ̌��݂̏��ʂ̕\�����X�V����N���X
/// </summary>
public class NowRankingChange : MonoBehaviour
{
    //���ʉ摜
    [SerializeField] Sprite[] m_rankingSprite = null;

    public void ChangeRanking(int rank)
	{
        //���ʂɉ����ĉ摜��ύX
        this.GetComponent<Image>().sprite = m_rankingSprite[rank];
    }
}