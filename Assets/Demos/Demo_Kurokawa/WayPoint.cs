using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�E�F�C�|�C���g�̊�{�s���N���X
public class WayPoint : MonoBehaviour
{
    private int m_myNumber = 0;             //�����̃E�F�C�|�C���g�ԍ�

    // Start is called before the first frame update
    void Start()
    {
        //�؂蔲�����ԍ�����
        string no;
        //�E�F�C�|�C���g�̔ԍ����񌅂Ȃ�� ex.)WayPoint25
        if (this.gameObject.name.Length == 10)
		{
            //�I�u�W�F�N�g�̖��O��[8]-[9]�������擾
            no = this.gameObject.name[8..10];
        }
        //�ꌅ�Ȃ��
		else
		{
            //�I�u�W�F�N�g�̖��O��[8]�������擾
            no = this.gameObject.name[8..9];
        }
        //�����̔ԍ��Ƃ��ĕۑ�
        m_myNumber = int.Parse(no);
    }

    //�E�F�C�|�C���g��ʉ߂�����
    private void OnTriggerEnter(Collider col)
	{
        //�����̍��W�Ɣԍ���m�点��
        col.gameObject.GetComponent<WayPointChecker>().SetNextWayPoint(col.gameObject.transform, m_myNumber);
	}
}
