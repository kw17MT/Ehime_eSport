using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//�����̃v���C���[�̂��ߋL�^�ꏊ�ŁA�ʐM����͎Ւf�����N���X
public class ParamManage : MonoBehaviour
{
    private int m_playerID = 0;                         //�v���C���[��ID�i�������������̏��j
    private int m_place = 0;                            //���݂̃v���C���[�̏���

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

	private void Update()
	{
		//if(SceneManager.GetActiveScene().name == "01_TitleScene")
		//{
  //          Destroy(this.gameObject);
		//}
	}
}
