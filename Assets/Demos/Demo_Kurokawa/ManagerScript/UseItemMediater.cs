using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseItemMediater : MonoBehaviour
{
	//�L�����o�X��̃A�C�e���g�p�ʒu�������ꂽ��A�����̃v���C���[�ɃA�C�e�����g���悤�ɓ`����
    public void LetPlayerUseItem()
	{
		//OwnPlayer�̓l�b�g���[�N�I�u�W�F�N�g�Ƃ���Instantiate���邽�߁A���������܂ł�OwnPlayer���Q�Ƃł��Ȃ����߁A���̊֐����K�v
        GameObject.Find("OwnPlayer").GetComponent<ObtainItemController>().SetUseItem();
	}
}
