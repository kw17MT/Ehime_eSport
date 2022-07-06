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
    private const float m_kMaxSpeed = 25.0f;                                    //�ō����x
    private Vector3 m_rightSteeringVector = new Vector3(0.0f, 5.0f, 0.0f);      //�E�����ւ̉�]�p�x�N�g��
    private Vector3 m_leftSteeringVector = new Vector3(0.0f, -5.0f, 0.0f);      //�������ւ̉�]�p�x�N�g��
    private Vector3 m_targetOffset = new Vector3(0.0f,0.0f,0.0f);               //���݂̖ڕW�̃E�F�C�|�C���g���炸�炷��
    private int m_targetNumber = -1;                                            //���ݖڕW�ɂ��Ă���E�F�C�|�C���g�̔ԍ�


    //�E�F�C�|�C���g����ڕW�n�_�����炷��(�R�[�X�ɂ���ĕ����Ⴄ���߃E�F�C�|�C���g���ւ̎���������)
    public float m_innerShiftMaxLength = 5.0f;//�����ւ̍ő�̂��炵��
    public float m_outerShiftMaxLength = 3.0f;//�O���ւ̍ő�̂��炵��

    //AI�̓E�F�C�|�C���g�Ƃ̋����ɉ����Ăǂ̊p�x�ȓ��Ȃ�n���h����؂邩��ω�������
    //��:
    //���������̏ꍇ�������E�F�C�|�C���g�ւ̌����Ɛi�s����������Ă��n���h����؂�K�v���Ȃ�
    //�߂������̏ꍇ���E�F�C�|�C���g�Ɍ������ăn���h����؂�Ȃ���΂����Ȃ�
    private const float m_kMinSteeringLength = 10.0f;   //AI���n���h����؂锻�f������p�x�̕����ŏ��ɂȂ鋗��
    private const float m_kMaxSteeringAngle = 1.0f;     //�n���h����؂锻�f������p�x�̕��̍ő�
    private const float m_kMinSteeringAngle = 0.1f;     //�n���h����؂锻�f������p�x�̕��̍ŏ�
    private const float m_kContactAngle = 0.3f;         //�ڐG�Ɣ��f����p�x

    private void Awake()
    {
        //���̂��擾
        m_rigidbody = this.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //�E�F�C�|�C���g���ύX���ꂽ���𒲂ׂ�
        CheckWayPointChange();

        //�n���h����؂����������
        HandlingDecision();

        //���̂ɗ͂�������
        m_rigidbody.AddForce(transform.forward * m_kMaxSpeed - m_rigidbody.velocity);

#if UNITY_EDITOR
        //AI�̖ڕW�n�_���o��
        Debug.DrawRay(this.GetComponent<WayPointChecker>().GetNextWayPoint() + m_targetOffset, Vector3.up * 100.0f, Color.red);
#endif
    }

    private void CheckWayPointChange()
    {
        //���̃E�F�C�|�C���g���擾
        int nextNumber = this.GetComponent<WayPointChecker>().GetNextWayPointNumber();

        //���ݖڎw���Ă���E�F�C�|�C���g�Ɣԍ��������Ȃ牽�����Ȃ�
        if(m_targetNumber == nextNumber)
        {
            return;
        }

        //�ԍ����Ⴄ = �ڎw���E�F�C�|�C���g���X�V���ꂽ

        //�ڎw���E�F�C�|�C���g�̔ԍ����X�V
        m_targetNumber = nextNumber;

        //�E�F�C�|�C���g�̍��W���炸�炷���𗐐��Ō���
        float shiftLength = Random.Range(-m_innerShiftMaxLength, m_outerShiftMaxLength);

        //�ڎw���ʒu�����[�J�����W�n�ō��E�ɂ��炷�x�N�g�����v�Z
        m_targetOffset = this.GetComponent<WayPointChecker>().GetNextWayPointRight() * shiftLength;
    }

    private void HandlingDecision()
    {
        //���ݖڎw���Ă���ʒu�ւ̃x�N�g�����v�Z
        Vector3 toNextPoint = this.GetComponent<WayPointChecker>().GetNextWayPoint() + m_targetOffset - this.transform.position;

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
                transform.Rotate(m_rightSteeringVector);
            }
            else
            {
                //���Ƀn���h����؂�(��]������)
                transform.Rotate(m_leftSteeringVector);
            }
        }
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
            transform.Rotate(m_leftSteeringVector);
        }
        else if(angle < -m_kContactAngle)
        {
            //���ɂ���̂ŉE�Ƀn���h����؂�
            transform.Rotate(m_rightSteeringVector);
        }
    }

}
