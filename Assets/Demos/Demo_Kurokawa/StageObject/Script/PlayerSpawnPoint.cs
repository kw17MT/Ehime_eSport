using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�v���C���[�̃X�|�[���|�C���g�N���X
public class PlayerSpawnPoint : MonoBehaviour
{
    private bool m_isSpawnPlayer = false;           //�����̓v���C���[���X�|�[����������
    
    //�v���C���[���X�|�[�����������Ƃ�ۑ�
    public void SetPlayerSpawned()
	{
        m_isSpawnPlayer = true;
    }

    //�v���C���[���X�|�[�������������擾
    public bool GetPlayerSpawned()
	{
        return m_isSpawnPlayer;
	}
}
