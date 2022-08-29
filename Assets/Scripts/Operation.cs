using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// ���[�U�[����(���E�t���b�N��^�b�`)
/// </summary>
public class Operation : MonoBehaviour
{
    Vector3 m_touchStartPos = Vector3.zero;               //�^�b�`���J�n�����ʒu
    Vector3 m_touchEndPos = Vector3.zero;                 //�^�b�`���I�����ʒu�A�������͌��݃^�b�`���Ă���ʒu�i���[�h�ɂ���Ĉ������Ⴄ�j
    float m_touchTime = 0.0f;                             //���������p�����Ă��鎞��
    private float m_timeFromFirstTouch = 0.0f;            //�ŏ��Ƀ^�b�`���Ă���̎���
    bool m_isTouching = false;                            //���݃^�b�`���Ă��邩
    bool m_isLongTouch = false;                           //���������ǂ���
    bool m_isDecideDirWhenLongTouch = false;              //��莞�Ԓ��������Ă��鎞�A���̎��_�ł̕������m�F������
    private bool m_isSingleTouch = false;
    [SerializeField] bool m_isWorkEveryFrame = true;      //���t���[���^�b�`�̈ړ������𒲂ׂ邩�B�Q�[���V�[���Ő؂�ւ���
    //���������肪�N�����鎞��
    [SerializeField] float m_longTachJudgmentActivationTime = 1.2f;
    //�t���b�N�����܂��̓^�b�v�̏���n�����Ԃ��ǂ���
    bool m_canDataThrow = false;

    const float TOUCH_ACTIVE_TIME = 1.0f;                 //�^�b�`�̌��͎���
    const float SWIPE_AMOUNT = 100;
    private bool m_isDoubleTouch = false;
    private float m_prevDeltaTime = 0.0f;

	public struct InputInfo
	{
        public string dir;
        public float power;
	};

    private InputInfo m_iputInfo;

	void Start()
    {
        m_iputInfo.dir = "";
        m_iputInfo.power = 0.0f;

    //�V�[���J�ڂ��Ă����̑���I�u�W�F�N�g�͍폜���Ȃ�
        DontDestroyOnLoad(this);
    }

    //�X�V�֐�
    void Update()
    {
        //�^�b�v���ꂽ�Ƃ��̏���
        FireMouseButtonDown();

       �@//���t���[���^�b�`�̈ړ��ʂ��擾���郂�[�h�Ȃ��
        //if (m_isWorkEveryFrame)
        //{
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
       // }
       �@//�^�b�v�J�n�ʒu����^�b�v�𗣂����Ƃ���܂ł��t���b�N����Ƃ݂Ȃ����[�h�Ȃ��
        //else
        //{
       �@//    //�^�b�v�������ꂽ��
       �@//   if (Input.GetMouseButtonUp(0))
        //    {
       �@//       //�^�b�`�̃t���O�␔�l������������
       �@//       TachDataInit();

       �@//       //�t���b�N�����܂��̓^�b�v�̏���n�����Ԃɂ���
        //        m_canDataThrow = true;
        //        //���͕����Ɖ�]�̗ʂ����Z�b�g
        //        m_iputInfo.dir = "";
        //        m_iputInfo.power = 0.0f;
        //    }
        //   else
        //   {
        //       //�t���b�N�����܂��̓^�b�v�̏���n���Ȃ���Ԃɂ���
        //       m_canDataThrow = false;
        //   }
        //}

        //�^�b�v�������ꂽ�Ƃ��A
        if (Input.GetMouseButtonUp(0))
        {
            //�^�b�`�̃t���O�␔�l������������
            TachDataInit();

            //���͕����Ɖ�]�̗ʂ����Z�b�g
            m_iputInfo.dir = "";
            m_iputInfo.power = 0.0f;
        }

        //�������֐�
        LongTach();

        //W�^�b�v�������ǂ����𔻒f����
        JudgeIsDoubleTouch();

        if (SceneManager.GetActiveScene().name[0..2] == "08" && !GameObject.Find("PauseButton").GetComponent<PauseButton>().GetIsPause())
		{
            m_prevDeltaTime = Time.deltaTime;
		}
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
        //��ʉ�����������A
        if (Screen.height / 1.2 <= Input.mousePosition.y)
        {
            return;
        }
            //��ʂɃ^�b�`���Ă���
            m_isTouching = true;
        //�^�b�`���Ă����ʏ�̍��W���擾
        m_touchStartPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z);
    }

    private void JudgeIsDoubleTouch()
	{
        if(m_isDoubleTouch)
		{
            //m_isSingleTouch = false;
            m_isDoubleTouch = false;
		}

        //�{�^���𗣂�����
        if (Input.GetMouseButtonUp(0))
        {
            //����̓^�b�v���삩
            if (NowOperation() == "touch")
            {
                //�ߋ�1�b�ȓ���1��^�b�`���Ă�����
                if (m_isSingleTouch)
                {
                    //W�^�b�`����
                    m_isDoubleTouch = true;
                    Debug.Log("Double Touch");
                    //�V���O���^�b�`�t���O������
                    m_isSingleTouch = false;
                }
                //���Ă��Ȃ����
                else
                {
                    //1��ڂ̃^�b�`
                    m_isSingleTouch = true;
                }
            }
        }
        //1��ڂ̃^�b�`�͂��Ă�����
        if (m_isSingleTouch)
        {
            if (SceneManager.GetActiveScene().name[0..2] == "08")
            {
                if (GameObject.Find("PauseButton").GetComponent<PauseButton>().GetIsPause())
                {
                    m_timeFromFirstTouch += m_prevDeltaTime;
                }
                else
                {
                    //�^�b�`���Ă���̌o�ߎ��Ԃ𑪒�
                    m_timeFromFirstTouch += Time.deltaTime;
                }
            }
			else
			{
                //�^�b�`���Ă���̌o�ߎ��Ԃ𑪒�
                m_timeFromFirstTouch += Time.deltaTime;
            }


            //�K��ȏソ������
            if (m_timeFromFirstTouch > TOUCH_ACTIVE_TIME)
            {
                //�o�ߎ��Ԃ����Z�b�g
                m_timeFromFirstTouch = 0.0f;
                //�V���O���^�b�`����𖳂���
                m_isSingleTouch = false;
            }
        }
    }

    public bool GetIsDoubleTouch()
    {
        if(m_isDoubleTouch)
		{
            m_isDoubleTouch = false;
            return true;
		}
		else
		{
            return m_isDoubleTouch;
        }
    }
    
    //���݂ǂ�ȑ�������Ă��邩���f����֐�
    string NowOperation()
    {
        //�^�b�`�𗣂������̉�ʏ�̈ʒu���擾
        m_touchEndPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z);
        //X,Y�����̈ړ��ʂ��v�Z
        float directionX = m_touchEndPos.x - m_touchStartPos.x;
        float directionY = m_touchEndPos.y - m_touchStartPos.y;
        string ope = "";

        if (Mathf.Abs(directionY) >= 3 && Mathf.Abs(directionX) >= 3)
        {
            if (Mathf.Abs(directionY) < Mathf.Abs(directionX))
            {
                if (30 < directionX)
                {
                    //�E�����Ƀt���b�N
                    ope = "right";
                }
                else if (-30 > directionX)
                {
                    //�������Ƀt���b�N
                    ope = "left";
                }
            }
            else if (Mathf.Abs(directionX) < Mathf.Abs(directionY))
            {
                if (30 < directionY)
                {
                    //������Ƀt���b�N
                    ope = "up";
                }
                else if (-30 > directionY)
                {
                    //�������̃t���b�N
                    ope = "down";
                }
            }
        }
        else
        {
            //�^�b�`�����o
            ope = "touch";
        }

        return ope;
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

    public InputInfo GetNowOperationAndPower()
	{
        if (!m_canDataThrow)
        {
            m_iputInfo.dir = "";
            m_iputInfo.power = 0.0f;
            return m_iputInfo;
        }

        m_iputInfo.dir = NowOperation();

        if(m_iputInfo.dir == "right"
            || m_iputInfo.dir == "left")
		{
            float directionX = m_touchEndPos.x - m_touchStartPos.x;
            float power = directionX / (Screen.width / 2.0f);
            m_iputInfo.power = power;
        }
		else
		{
            m_iputInfo.power = 0.0f;
		}

        return m_iputInfo;
	}

    //�����������ǂ������擾����v���p�e�B
    public bool GetIsLongTouch
    {
        get
        {
            if (m_isLongTouch)
            {
                //�V�[���J�ڂ̍ہA��������������ƃV�[�����X�L�b�v����Ă��܂����߃��Z�b�g
                m_isLongTouch = false;
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    /////////////////////////////////////////////////////////////////////////////////
    //�����������ǂ�����ݒ肷��v���p�e�B
    public void SetIsLongTouch(bool isLongTouch)
    {
        m_isLongTouch = isLongTouch;
    }
    /////////////////////////////////////////////////////////////////////////////////

    //�^�b�`�̃t���O�␔�l������������֐�
    public void TachDataInit()
    {
        //�^�b�`���Ă��Ȃ�
        m_isTouching = false;
        //
        m_isDoubleTouch = false;
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

        if (SceneManager.GetActiveScene().name[0..2] == "08")
        {
            if (GameObject.Find("PauseButton").GetComponent<PauseButton>().GetIsPause())
            {
                m_touchTime += m_prevDeltaTime;
            }
            else
            {
                //�^�b�`���Ă��鎞�Ԃ��Q�[���^�C���Ōv��
                m_touchTime += Time.deltaTime;
            }
        }
		else
		{
            //�^�b�`���Ă��鎞�Ԃ��Q�[���^�C���Ōv��
            m_touchTime += Time.deltaTime;
        }

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

    public void ResetDoubleTapParam()
	{
        m_isSingleTouch = false;
        m_isDoubleTouch = false;
        m_timeFromFirstTouch = TOUCH_ACTIVE_TIME;
    }

    public void SetIsDoubleTouch(bool isDoubleTouch)
    {
        m_isDoubleTouch = isDoubleTouch;
    }
}