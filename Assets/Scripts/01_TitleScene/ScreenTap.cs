using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenTap : MonoBehaviour
{
    //��Ԃ��O�i�ł��邩�ǂ���
    bool canAdvance = false;
    //��Ԃ̈ړ����x
    [SerializeField] float advanceSpeed = 0.0f;
    //��Ԃ̈ړ��͈�
    [SerializeField] float trainMoveRange = 0.0f;

    //�g�����W�V�����V�X�e��
    [SerializeField] TransitionPostEffect m_transitionPostEffect = null;

    //����V�X�e��
    Operation m_operation = null;

    void Start()
    {
        //����V�X�e���̃Q�[���I�u�W�F�N�g���������X�N���v�g���g�p����
        m_operation = GameObject.Find("OperationSystem").GetComponent<Operation>();
    }

    void Update()
    {
        //��ʂ��^�b�v���ꂽ���𔻒�
        TapScreen();

        //��Ԃ��O�i�ł���Ƃ�
        if (canAdvance)
        {
            //��Ԃ�O�i������
            this.transform.position += new Vector3(0.0f,0.0f,advanceSpeed);
        }

        //�J�����Ɉ��̋����܂ŋ߂Â�����A
        if (this.transform.position.z < trainMoveRange)
        {
            //�g�����W�V�����N��
            m_transitionPostEffect.OnTransition();

            //����̔����������������
            m_operation.TachDataInit();

            //���[�h�I���V�[���ɑJ��
            SceneManager.LoadScene("02_ModeSelectScene");
        }
    }

    //��ʃ^�b�v����֐�
    void TapScreen()
    {
        //��ʃ^�b�v���P�x�����ł��Ȃ��悤�ɂ���
        if (canAdvance)
        {
            return;
        }

        //��ʂ��^�b�v���ꂽ��A
        if(m_operation.GetNowOperation() == "touch")
        {
            //�g�����W�V�����N��
            m_transitionPostEffect.OnTransition();

            //��Ԃ��O�i�ł��锻��ɂ���
            canAdvance = true;
        }
    }
}