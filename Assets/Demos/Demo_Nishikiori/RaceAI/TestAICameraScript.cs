using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// AI�̃e�X�g�p�ɍ�����J�����X�N���v�g
/// </summary>
public class TestAICameraScript : MonoBehaviour
{
    public GameObject m_AIPlayer;

    void Update()
    {
        this.gameObject.transform.position = m_AIPlayer.transform.position + m_AIPlayer.transform.forward * -10.0f + new Vector3(0.0f, 10.0f, 0.0f);
        this.gameObject.transform.LookAt(m_AIPlayer.transform);
    }
}
