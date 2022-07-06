using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�����̃v���C���[�̂��ߋL�^�ꏊ�ŁA�ʐM����͎Ւf�����N���X
public class ParamManage : MonoBehaviour
{
    private int m_playerID = 0;                         //�v���C���[��ID�i�������������̏��j
    private int m_place = 0;                            //���݂̃v���C���[�̏���
    private bool m_isOfflineMode = false;               //�I�t���C�����[�h�Ńv���C���邩

    // Start is called before the first frame update
    void Start()
    {
        //�V�[���ړ��Ŕj�����Ȃ�
        DontDestroyOnLoad(this);
    }

    //�����̃v���C���[ID���L�^
    public void SetPlayerID(int id)
	{
        m_playerID = id;
	}

    //�����̃v���C���[ID���擾
    public int GetPlayerID()
	{
        return m_playerID;
	}

    //�I�t���C�����[�h�ŊJ�n����悤��
    public void SetOfflineMode()
	{
        m_isOfflineMode = true;
	}

    //�I�t���C�����[�h��
    public bool GetIsOfflineMode()
	{
        return m_isOfflineMode;
	}

    //�����̏��ʂ��L�^
    public void SetPlace(int place)
    {
        m_place = place;
    }

    //�����̏��ʂ��擾
    public int GetPlace()
    {
        return m_place;
    }

    public void  TouchItemUI()
	{
        GameObject.Find("OwnPlayer").GetComponent<ObtainItemController>().SetUseItem();
	}
}
