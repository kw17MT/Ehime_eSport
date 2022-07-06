using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���[�U�[����(���E�t���b�N��^�b�`)
/// </summary>
public class Operation : MonoBehaviour
{
    Vector3 m_touchStartPos = Vector3.zero;               //�^�b�`���J�n�����ʒu
    Vector3 m_touchEndPos = Vector3.zero;                 //�^�b�`���I�����ʒu�A�������͌��݃^�b�`���Ă���ʒu�i���[�h�ɂ���Ĉ������Ⴄ�j
    float m_touchTime = 0.0f;                             //���������p�����Ă��鎞��
    bool m_isTouching = false;                            //���݃^�b�`���Ă��邩
    bool m_isLongTouch = false;                           //���������ǂ���
    bool m_isDecideDirWhenLongTouch = false;              //��莞�Ԓ��������Ă��鎞�A���̎��_�ł̕������m�F������
    [SerializeField] bool m_isWorkEveryFrame = true;      //���t���[���^�b�`�̈ړ������𒲂ׂ邩�B�Q�[���V�[���Ő؂�ւ���
    //���������肪�N�����鎞��
    [SerializeField] float m_longTachJudgmentActivationTime = 1.2f;
    //�t���b�N�����܂��̓^�b�v�̏���n�����Ԃ��ǂ���
    bool m_canDataThrow = false;

    void Start()
    {
        //�V�[���J�ڂ��Ă����̑���I�u�W�F�N�g�͍폜���Ȃ�
        DontDestroyOnLoad(this);
    }

    //�X�V�֐�
    void Update()
    {
        //�^�b�v���ꂽ�Ƃ��̏���
        FireMouseButtonDown();

       �@//���t���[���^�b�`�̈ړ��ʂ��擾���郂�[�h�Ȃ��
        if (m_isWorkEveryFrame)
        {
       �@    //�^�b�`���Ă���Ƃ��A��ԏ��߂̃^�b�v�ꂩ�猻�݂̃^�b�v�ʒu�Ńt���b�N���������肷��
       �@   if (m_isTouching)
            {
                NowOperation();
       �@        //�t���b�N�����܂��̓^�b�v�̏���n�����Ԃɂ���
                m_canDataThrow = true;
            }
            else
            {
                //�t���b�N�����܂��̓^�b�v�̏���n���Ȃ���Ԃɂ���
                m_canDataThrow = false;
            }
        }
       �@//�^�b�v�J�n�ʒu����^�b�v�𗣂����Ƃ���܂ł��t���b�N����Ƃ݂Ȃ����[�h�Ȃ��
        else
        {
       �@    //�^�b�v�������ꂽ��
       �@   if (Input.GetMouseButtonUp(0))
            {
       �@       //�^�b�`�̃t���O�␔�l������������
       �@       TachDataInit();

       �@       //�t���b�N�����܂��̓^�b�v�̏���n�����Ԃɂ���
                m_canDataThrow = true;
            }
           else
           {
               //�t���b�N�����܂��̓^�b�v�̏���n���Ȃ���Ԃɂ���
               m_canDataThrow = false;
           }
        }

        //�^�b�v�������ꂽ�Ƃ��A
        if (Input.GetMouseButtonUp(0))
        {
            //�^�b�`�̃t���O�␔�l������������
            TachDataInit();
        }

        //�������֐�
        LongTach();
    }

    //�^�b�v���ꂽ�Ƃ��̏����֐�
    void FireMouseButtonDown()
    {
        //�^�b�v���ꂽ���i���N���b�N�j
        //�����O...���P...�E...�R...��
        if (!Input.GetMouseButtonDown(0))
        {
            return;
        }
        //��ʂɃ^�b�`���Ă���
        m_isTouching = true;
        //�^�b�`���Ă����ʏ�̍��W���擾
        m_touchStartPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z);
    }

    //���݂ǂ�ȑ�������Ă��邩���f����֐�
    string NowOperation()
    {
        //�^�b�`�𗣂������̉�ʏ�̈ʒu���擾
        m_touchEndPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z);
        //X,Y�����̈ړ��ʂ��v�Z
        float directionX = m_touchEndPos.x - m_touchStartPos.x;
        float directionY = m_touchEndPos.y - m_touchStartPos.y;

        if (Mathf.Abs(directionY) < Mathf.Abs(directionX))
        {
            if (30 < directionX)
            {
                //�E�����Ƀt���b�N
                return "right";
            }
            else if (-30 > directionX)
            {
                //�������Ƀt���b�N
                return "left";
            }
        }
        else if (Mathf.Abs(directionX) < Mathf.Abs(directionY))
        {
            if (30 < directionY)
            {
                //������Ƀt���b�N
                return "up";
            }
            else if (-30 > directionY)
            {
                //�������̃t���b�N
                return "down";
            }
        }
        else
        {
            //�^�b�`�����o
            return "touch";
        }

        return "";
    }

    //���݂ǂ�ȑ��삪����Ă��邩���擾����֐�
    public string GetNowOperation()
    {
        //�t���b�N�����܂��̓^�b�v�̏���n������
        //����Ȃ��Ƃ��̓f�[�^��n���Ȃ��B
        if (!m_canDataThrow)
        {
            return "";
        }

        return NowOperation();
    }

    //�����������ǂ������擾����v���p�e�B
    public bool GetIsLongTouch
    {
        get
        {
            return m_isLongTouch;
        }
    }

    //�^�b�`�̃t���O�␔�l������������֐�
    public void TachDataInit()
    {
        //�^�b�`���Ă��Ȃ�
        m_isTouching = false;
        //���������Ă��Ȃ�
        m_isLongTouch = false;
        //��莞�Ԉȏ㒷�������Ă��Ȃ�
        m_isDecideDirWhenLongTouch = false;
        //�^�b�`���Ă��鎞�Ԃ����Z�b�g
        m_touchTime = 0.0f;
    }

    //�������֐�
    void LongTach()
    {
        //�^�b�`���Ă���Ƃ��̂ݏ������s���B
        if (!m_isTouching)
        {
            return;
        }

        //�^�b�`���Ă��鎞�Ԃ��Q�[���^�C���Ōv��
        m_touchTime += Time.deltaTime;

        //������x����������܂ł͒������`�F�b�N���s��Ȃ��B
        if (m_touchTime <= m_longTachJudgmentActivationTime)
        {
            return;
        }

        //���������̃t���b�N���������łɊm�F���Ă����璷�����`�F�b�N���s��Ȃ��B
        if(m_isDecideDirWhenLongTouch)
        {
            return;
        }

        //���������̃t���b�N�����m�F����
        m_isDecideDirWhenLongTouch = true;
        //�X���C�h���Ă��Ȃ����m���߂�B
        //�X���C�h���Ă��Ȃ�������A
        //���������ԓǂݏグ�J�n
        m_isLongTouch = NowOperation() == "touch" ? true : false;
    }

    public string GetTouchedScreenDirection()
    {
        if (m_isTouching)
        {
            if (Input.mousePosition.x >= Screen.width / 2.0f)
            {
                return "right";
            }
            else
            {
                return "left";
            }
        }
        else
        {
            return "nothing";
        }
    }
}