using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GoalScript : MonoBehaviourPunCallbacks
{
    //�S�[�������������Ƃ�
    void OnTriggerEnter(Collider col)
	{
        //�S�[�������������̂������̃v���C���[�C���X�^���X��������
        if(col.gameObject.tag == "OwnPlayer")
        {
            //�`�F�b�N�|�C���g��S���ʂ��Ă�����S�[������
            col.gameObject.GetComponent<ProgressChecker>().CheckCanGoal();

            //�R�[�X��3���S�[�����Ă�����
            if(col.gameObject.GetComponent<ProgressChecker>().IsFinishRacing())
			{
                //�v���C���[���S�[����������ɂ���B
                col.gameObject.GetComponent<AvatarController>().SetGoaled();
            }
        }

        if(col.gameObject.tag == "Player")
		{
            col.gameObject.GetComponent<AIProgressChecker>().CheckCanGoal();
		}
    }
}