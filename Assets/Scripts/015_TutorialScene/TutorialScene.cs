using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialScene : MonoBehaviour
{
    //����V�X�e��
    Operation m_operation = null;

    // Start is called before the first frame update
    void Start()
    {
        //����V�X�e���̃Q�[���I�u�W�F�N�g���������R���|�[�l���g���擾
        m_operation = GameObject.Find("OperationSystem").GetComponent<Operation>();
    }

    // Update is called once per frame
    void Update()
    {
        //��ʂ��^�b�v���ꂽ��A
        if (m_operation.GetNowOperation() == "touch")
        {
            //���[�h�V�[���ɑJ��
            SceneManager.LoadScene("02_ModeSelectScene");
        }
    }
}