using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestObstacleScript : MonoBehaviour
{
    public string m_AITag = "Player";                                           //AI�ɂ���ꂽ�Q�[���I�u�W�F�N�g�̃^�O
    public string m_playerTag = "OwnPlayer";                                    //�v���C���[�ɂ���ꂽ�Q�[���I�u�W�F�N�g�̃^�O

    private void OnTriggerEnter(Collider collision)
    {
        //����������̃I�u�W�F�N�g���v���C���[��AI�̃^�O�������Ă��Ȃ��Ȃ牽�����Ȃ��B
        if (collision.gameObject.CompareTag(m_playerTag) == false && collision.gameObject.CompareTag(m_AITag) == false)
        {
            return;
        }

        Debug.Log("ObstacleHit:" + collision.gameObject.name);

        Destroy(gameObject);
    }
}