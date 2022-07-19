using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class ItemStateCommunicator : MonoBehaviourPunCallbacks
{
    public GameObject m_orangePeel;             //�I�����W�̔�v���t�@�u
    public GameObject m_snapper;                //�^�C�v���t�@�u
    private int m_orangePeelSum = 0;            //�Q�[�������������I�����W�̔�̍��v
    private int m_snapperSum = 0;               //�Q�[�������������^�C�̍��v

    //���O���g�p���ăC���X�^���X��j�󂷂�i�����ʐM�����ƂɃ��[�J���Ő����������^�C�𖼑O�ŏ����j
    [PunRPC]
    public void DestroyItemWithName(string name)
	{
        GameObject go = GameObject.Find(name);
        if(go != null)
		{
            Destroy(go);
        }
	}

    //�I�����W�̔炩����w�肳�ꂽ�ʒu�ɐ�������i��2�̈����̓^�C�̎��̂ݎg�p�j
    public void PopItem(string prefabName, Vector3 popPos, int wayPointNumber, int playerNumber)
    { 
        //�I�����W�̔炪�w�肳��Ă����ꍇ
        if(prefabName == "OrangePeel")
		{
            //���������I�����W�̔�̌��𑝂₷
            m_orangePeelSum++;
            //�I�����W�̔琶��
            GameObject peel = Instantiate(m_orangePeel, popPos, Quaternion.identity);
            peel.name = "OrangePeel" + m_orangePeelSum;
        }
        //�^�C���w�肳��Ă����ꍇ
        else if(prefabName == "Snapper")
		{
            //���������^�C�̌��𑝂₷
            m_snapperSum++;
            //�^�C����
            GameObject snapper = Instantiate(m_snapper, popPos, Quaternion.identity);
            //���ڎw���E�F�C�|�C���g��ݒ�
            snapper.GetComponent<WayPointChecker>().SetNextWayPointDirectly(popPos, wayPointNumber);
            //���˂����v���C���[��ID���L�^
            snapper.GetComponent<SnapperController>().SetOwnerID(playerNumber);
            snapper.name = "Snapper" + m_snapperSum;
        }
    }
}
