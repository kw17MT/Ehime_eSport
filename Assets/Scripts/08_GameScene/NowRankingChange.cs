using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �Q�[����ʂ̌��݂̏��ʂ̕\�����X�V����N���X
/// </summary>
public class NowRankingChange : MonoBehaviour
{
    [SerializeField] Sprite[] m_rankingImage = { null };

    public void ChangeRanking(int rank)
	{
        this.GetComponent<Image>().sprite = m_rankingImage[rank];
	}
}
