using UnityEngine;

/// <summary>
/// Transform.RotateAround��p�����~�^��
/// </summary>
public class CircleCenterRotateAround : MonoBehaviour
{
    // ���S�_
    [SerializeField]GameObject m_center = null;

    // ��]��(Y��)
    Vector3 m_axis = Vector3.up;

    // �~�^������
    public float m_period = 0.0f;

    //����邩�ǂ���
    bool m_aroundMoveOn = false;

    //���鎞�ԃJ�E���^�[
    int m_aroundCount = 0;

    //���v��肩�����v��肩
    int m_reverse = 1;

    //����V�X�e��
    Operation m_operation = null;

    //�^�C�}�[�̉ғ�����
    [SerializeField]int m_countTime = 0;

    void Start()
    {
        //����V�X�e���̃Q�[���I�u�W�F�N�g���������X�N���v�g���g�p����
        m_operation = GameObject.Find("OperationSystem").GetComponent<Operation>();
    }

    void Update()
    {
        if (!m_aroundMoveOn)
        {
            //��ʂ��E�t���b�N���ꂽ��A
            if (m_operation.GetNowOperation() == "right")
            {
                //������Ԃɂ���
                m_aroundMoveOn = true;
                //���v���
                m_reverse = 1;
            }
            //��ʂ����t���b�N���ꂽ��A
            if (m_operation.GetNowOperation() == "left")
            {
                //������Ԃɂ���
                m_aroundMoveOn = true;
                //���v���
                m_reverse = -1;
            }
        }


        //���鏈�������s
        GoAround();
    }

    //���鏈���֐�
    void GoAround()
    {
        //���Ȃ���Ԃ̂Ƃ��͏��������Ȃ��B
        if (!m_aroundMoveOn) return;

        // ���S�_center�̎�����A��axis�ŁAperiod�����ŉ~�^��
        transform.RotateAround(
            m_center.transform.position,                     //���S�_
            m_axis,                                          //��
            360 / m_period * Time.deltaTime * m_reverse      //����
        );

        //�J�E���g�v��
        m_aroundCount++;

        //�J�E���g���w�肵�����l���傫���Ȃ�����A
        if (m_aroundCount > m_countTime)
        {
            //����Ȃ���Ԃɖ߂�
            m_aroundMoveOn = false;
            //�J�E���g�̏�����
            m_aroundCount = 0;
        }
    }

    //�^�C�}�[�̉ғ����Ԃ��擾����Q�b�^�[
    public int GetCountTime()
    {
        return m_countTime;
    }
}