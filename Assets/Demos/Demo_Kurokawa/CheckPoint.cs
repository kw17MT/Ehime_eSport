using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CheckPoint : MonoBehaviourPunCallbacks
{
	//�`�F�b�N�|�C���g������������
    void OnTriggerEnter(Collider col)
	{
		//�����̃v���C���[��AI�Ȃ��
        if(col.gameObject.tag == "OwnPlayer")
		{
			//�`�F�b�N�����ꏊ�̖��O��ۑ�
            col.gameObject.GetComponent<ProgressChecker>().SetThroughPointName(this.gameObject.name);
		}

		if(col.gameObject.tag == "Player")
		{
			col.gameObject.GetComponent<AIProgressChecker>().SetCanGoaled();
		}
	}
}
