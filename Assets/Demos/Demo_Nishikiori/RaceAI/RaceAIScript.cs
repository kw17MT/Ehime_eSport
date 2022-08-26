using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// CPU�̃��[�X�L�����p�̃X�N���v�g
/// </summary>
public class RaceAIScript : MonoBehaviour
{
    public string m_AITag = "Player";                                           //AI�ɂ���ꂽ�Q�[���I�u�W�F�N�g�̃^�O
    public string m_playerTag = "OwnPlayer";                                    //�v���C���[�ɂ���ꂽ�Q�[���I�u�W�F�N�g�̃^�O
    private Rigidbody m_rigidbody = null;                                       //AI�L�����N�^�[�̍���
    private WayPointChecker m_wayPointChecker = null;                           //AI�L�����N�^�[�̃E�F�C�|�C���g�`�F�b�J�[
    public AIDifficulty m_AIDifficulty = null;

    //�X�e�[�^�X//
    //�X�s�[�h
    private float m_maxSpeed {get;set;} = 25.0f;                                //�ō����x

    //���쐫
    private Vector3 m_rightSteeringVector { get; set; }
        = new Vector3(0.0f, 5.0f, 0.0f);                                        //�E�����ւ̉�]�p�x�N�g��
    private Vector3 m_leftSteeringVector { get; set; }
        = new Vector3(0.0f, -5.0f, 0.0f);                                       //�������ւ̉�]�p�x�N�g��

    //�p���t��?

    //�^�̗ǂ�?


    private float m_shiftLength = 0.0f;                                         //�ڎw���n�_���E�F�C�|�C���g����E�����ɂǂꂾ������Ă��邩
    private Vector3 m_targetOffset = new Vector3(0.0f,0.0f,0.0f);               //���݂̖ڕW�̃E�F�C�|�C���g���炸�炷��
    private int m_targetNumber = -1;                                            //���ݖڕW�ɂ��Ă���E�F�C�|�C���g�̔ԍ�


    //��Q�������p�ϐ�
    public LayerMask m_obstacleLayerMask;                                       //��Q���̃��C���[�}�X�N
    RaycastHit m_sphereCastHit;                                                 //SphereCast�̌��ʂ��i�[����ϐ�
    float m_sphereCastRadius = 1.0f;                                            //SphereCast�̔��a
    float m_sphereCastMaxDistance = 20.0f;                                      //SphereCast�̍ő勗��
    float m_onLineLength = 1.0f;                                                //��Q�������C����ɂ���Ɣ��f���鋗��


    //�E�F�C�|�C���g����ڕW�n�_�����炷��
    public float m_maxMoveRatio = 0.2f;
    private float m_currentShiftRatio = 0.5f;//���݂̂��炵���̊���(����:0.0f�`�O��:1.0f)

    //AI�̓E�F�C�|�C���g�Ƃ̋����ɉ����Ăǂ̊p�x�ȓ��Ȃ�n���h����؂邩��ω�������
    //��:
    //���������̏ꍇ�������E�F�C�|�C���g�ւ̌����Ɛi�s����������Ă��n���h����؂�K�v���Ȃ�
    //�߂������̏ꍇ���E�F�C�|�C���g�Ɍ������ăn���h����؂�Ȃ���΂����Ȃ�
    private const float m_kMinSteeringLength = 10.0f;   //AI���n���h����؂锻�f������p�x�̕����ŏ��ɂȂ鋗��
    private const float m_kMaxSteeringAngle = 1.0f;     //�n���h����؂锻�f������p�x�̕��̍ő�
    private const float m_kMinSteeringAngle = 0.1f;     //�n���h����؂锻�f������p�x�̕��̍ŏ�
    private const float m_kContactAngle = 0.3f;         //�ڐG�Ɣ��f����p�x

    private bool m_canMove = false;

    private void Awake()
    {
        //���̂��擾
        m_rigidbody = GetComponent<Rigidbody>();

        //�E�F�C�|�C���g�`�F�b�J�[���擾
        m_wayPointChecker = GetComponent<WayPointChecker>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //�J�E���g�_�E�����I�����ē������ԂŁA�U������Ă��Ȃ����
        if(m_canMove)
		{
            if(this.gameObject.tag == m_playerTag || !this.gameObject.GetComponent<AICommunicator>().GetIsAttacked())
			{
                //�E�F�C�|�C���g���ύX���ꂽ���𒲂ׂ�
                CheckWayPointChange();

                //�n���h����؂����������
                HandlingDecision();

                //���̂ɗ͂�������
                m_rigidbody.AddForce(transform.forward * m_maxSpeed - m_rigidbody.velocity);
            }

#if UNITY_EDITOR
            //�f�o�b�O�p�@AI�̖ڕW�n�_���o��
            Debug.DrawRay(m_wayPointChecker.GetNextWayPoint() + m_targetOffset, Vector3.up * 100.0f, Color.green);

            //�f�o�b�O�p�@�Ō�ɒʂ����E�F�C�|�C���g���玟�̃E�F�C�|�C���g�܂ł���łȂ�
            Vector3 prevWayPointtoCurrentWayPoint = m_wayPointChecker.GetNextWayPoint() - m_wayPointChecker.GetCurrentWayPoint();
            Debug.DrawRay(m_wayPointChecker.GetCurrentWayPoint(), prevWayPointtoCurrentWayPoint, Color.yellow);
#endif

        }
    }

    public void SetCanMove(bool canMove)
	{
        m_canMove = canMove;
        if(this.gameObject.tag == m_AITag)
		{
            this.GetComponent<AICommunicator>().SetMoving(canMove);
        }

        this.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionY;
        this.gameObject.GetComponent<Rigidbody>().freezeRotation = true;
    }

    public int GetNextWayPoint()
	{
        return m_targetNumber;
	}

    private void CheckWayPointChange()
    {
        //���̃E�F�C�|�C���g���擾
        int nextNumber = m_wayPointChecker.GetNextWayPointNumber();

        //���ݖڎw���Ă���E�F�C�|�C���g�Ɣԍ��������Ȃ牽�����Ȃ�
        if(m_targetNumber == nextNumber)
        {
            return;
        }

        //�ԍ����Ⴄ = �ڎw���E�F�C�|�C���g���X�V���ꂽ

        //�ڎw���E�F�C�|�C���g�̔ԍ����X�V
        m_targetNumber = nextNumber;

        //���̃X�N���v�g�����I�u�W�F�N�g��AI��������
        if(this.gameObject.tag == m_AITag)
		{
            //���̃E�F�C�|�C���g��AI�̏���ʃI�u�W�F�N�g�ɒʐM����X�N���v�g�ɕۑ�
            this.GetComponent<AICommunicator>().SetNextWayPoint(m_targetNumber);
        }

        //���̃E�F�C�|�C���g�̕��̔������擾
        float nextHalfWidth = m_wayPointChecker.GetNextWayPointHalfWidth();
        
        //�E�F�C�|�C���g�̕������̂܂܎g���ƃR�[�X�A�E�g�M���M���܂ōs���Ă��܂��̂ŏ��������l�����炷
        nextHalfWidth -= 2.0f;

        //���O�Ɏ�肤��l�̍ő�l���E�F�C�|�C���g�̕��Ɠ�Փx����ݒ�
        float innerShiftMaxLength = nextHalfWidth * m_AIDifficulty.innerShiftMaxRatio;
        float outerShiftMaxLength = nextHalfWidth * m_AIDifficulty.outerShiftMaxRatio;



        //���݂̃E�F�C�|�C���g�̍��W���炸�炷�����ɕω���^����l�𗐐��Ō���
        float moveRatio = Random.Range(-m_maxMoveRatio, m_maxMoveRatio);

        //���݂̊����ɉ��Z
        m_currentShiftRatio += moveRatio;

        //0~1�ɐ�����(�K��l�����E�O�ɂ����Ȃ��悤��)
        m_currentShiftRatio = Mathf.Clamp01(m_currentShiftRatio);

        //����������ۂɂ��炷�������v�Z
        m_shiftLength = Mathf.Lerp(-innerShiftMaxLength, outerShiftMaxLength, m_currentShiftRatio);

        //�ڎw���ʒu�����[�J�����W�n�ō��E�ɂ��炷�x�N�g�����v�Z
        m_targetOffset = m_wayPointChecker.GetNextWayPointRight() * m_shiftLength;
    }

    private void HandlingDecision()
    {
        //��Q���A�C�e��������鏈��
        if (Physics.SphereCast(transform.position, m_sphereCastRadius, transform.forward, out m_sphereCastHit, m_sphereCastMaxDistance,m_obstacleLayerMask))
        {
            //�O�ɒʂ����E�F�C�|�C���g���玟�̃E�F�C�|�C���g�ւ̃x�N�g�����v�Z
            Vector3 currentWayPointToNextWayPoint = m_wayPointChecker.GetNextWayPoint() - m_wayPointChecker.GetCurrentWayPoint();

            //���̃x�N�g���ɑ΂���E�������v�Z
            Vector3 right = Vector3.Cross(Vector3.up, currentWayPointToNextWayPoint.normalized);

            //�O�ɒʂ����E�F�C�|�C���g���瓖��������Q���ւ̃x�N�g�����v�Z
            Vector3 prevWayPointToHitObject = m_sphereCastHit.collider.gameObject.transform.position - m_wayPointChecker.GetCurrentWayPoint();

            //��Q���̈ʒu�����C����ɓ��e
            float dot = Vector3.Dot(currentWayPointToNextWayPoint.normalized, prevWayPointToHitObject);

            //��Q���̈ʒu����ł��߂����C����̍��W�����߂�
            Vector3 nearestLinePos = m_wayPointChecker.GetCurrentWayPoint() + dot * currentWayPointToNextWayPoint.normalized;

            //��Q���̈ʒu���烉�C����̈ʒu�̃x�N�g�������߂�
            Vector3 obstacleToNearestLinePos = nearestLinePos - m_sphereCastHit.collider.gameObject.transform.position;

            //�x�N�g���̒������Z�����(��Q�������C���ɋ߂����)
            if(obstacleToNearestLinePos.magnitude < m_onLineLength)
            {
#if UNITY_EDITOR
                //�f�o�b�O�p�@���C����̈ʒu���o��
                Debug.DrawRay(nearestLinePos, Vector3.up * 100.0f, Color.red);
#endif

                //�E�F�C�|�C���g���E����ڎw���Ă��邩������ڎw���Ă��邩�ŕ��򂳂���
                if (m_shiftLength < 0.0f)
                {
                    LeftHandling();
                }
                else
                {
                    RightHandling();
                }
                return;
            }

#if UNITY_EDITOR
            //�f�o�b�O�p�@���C����̈ʒu���o��
            Debug.DrawRay(nearestLinePos, Vector3.up * 100, Color.blue);
#endif

            //���ς��v�Z����
            float angle = Vector3.Dot(right, prevWayPointToHitObject);

            //���ς�0���傫����Ώ�Q���̓E�F�C�|�C���g���Ȃ���(�Z���^�[���C��)���E�ɂ���
            //���n���h�������ɐ؂�
            if (angle > 0.0f)
            {
                LeftHandling();
            }
            else
            {
                RightHandling();
            }

            return;
        }

        //���ݖڎw���Ă���ʒu�ւ̃x�N�g�����v�Z
        Vector3 toNextPoint = m_wayPointChecker.GetNextWayPoint() + m_targetOffset - this.transform.position;

        //�ڎw���������v�Z
        Vector3 newForward = toNextPoint;
        newForward.y = 0.0f;
        newForward.Normalize();

        //�ڎw���Ă���ʒu�֌������߂̊p�x���v�Z(���E�̔��f�����邽�߉E�����̃x�N�g���ƌv�Z)
        float steeringAngle = Vector3.Dot(transform.right, newForward);

        //�ڎw���Ă���ʒu�ւ̋������擾
        float toNextLength = toNextPoint.magnitude;

        //�ڎw���Ă���ʒu�ւ̋�������AI���n���h����؂锻�f������p�x�̕����ŏ��ɂȂ鋗���ɑ΂��Ă̊������v�Z
        float lerpRate = (m_kMinSteeringLength - toNextLength) / m_kMinSteeringLength;

        //�����ɑ΂��Ă̊�������n���h����؂�p�x�̂������l���v�Z
        float angleThreshold = Mathf.Lerp(m_kMinSteeringAngle, m_kMaxSteeringAngle, lerpRate);

        //�n���h����؂�p�x���������l���Ȃ�
        if (Mathf.Abs(steeringAngle) > angleThreshold)
        {
            if (steeringAngle > 0.0f)
            {
                //�E�Ƀn���h����؂�(��]������)
                RightHandling();
            }
            else
            {
                //���Ƀn���h����؂�(��]������)
                LeftHandling();
            }
        }
    }

    void OnDrawGizmos()
    {
#if UNITY_EDITOR
        //�f�o�b�O�p�@AI����Q�������m�����ꍇ�`��
        if (m_sphereCastHit.collider != null && ((1 << m_sphereCastHit.collider.gameObject.layer) & m_obstacleLayerMask) != 0)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(transform.position, transform.forward * m_sphereCastHit.distance);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position + transform.forward * m_sphereCastHit.distance, m_sphereCastRadius);
        }
#endif
    }

    private void OnCollisionStay(Collision collision)
    {
        //����������̃I�u�W�F�N�g���v���C���[��AI�̃^�O�������Ă��Ȃ��Ȃ牽�����Ȃ��B
        if(collision.gameObject.CompareTag(m_playerTag) == false && collision.gameObject.CompareTag(m_AITag) == false)
        {
            return;
        }

        //���������Q�[���I�u�W�F�N�g�̍��W�ւ̃x�N�g�����v�Z
        Vector3 toHitGameObject = collision.gameObject.transform.position - this.transform.position;

        //�p�x���v�Z
        float angle = Vector3.Dot(toHitGameObject.normalized, this.transform.right);

        //���E�ǂ���ɂ��邩
        if (angle > m_kContactAngle)
        {
            //�E�ɂ���̂ō��Ƀn���h����؂�
            LeftHandling();
        }
        else if(angle < -m_kContactAngle)
        {
            //���ɂ���̂ŉE�Ƀn���h����؂�
            RightHandling();
        }
    }

    private void RightHandling()
    {
        transform.Rotate(m_rightSteeringVector);
    }

    private void LeftHandling()
    {
        transform.Rotate(m_leftSteeringVector);
    }

    ////////////////////////////////////////////////////
    public Rigidbody GetRigidBody
    {
        get 
        {
            return m_rigidbody;
        }
    }
    ////////////////////////////////////////////////////
}
