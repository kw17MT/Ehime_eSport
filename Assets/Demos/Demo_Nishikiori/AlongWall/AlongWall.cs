using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �ǂɐڐG�����ہA�ǉ����Ɉړ�����x�N�g�����v�Z�����̂ɓK������N���X�B
/// </summary>
public class AlongWall
{
    //�ǉ����Ɉړ����鑬�x�B
    public float m_alongWallSpeed { get; set; } = 25.0f;

    //�Փ˔���Ƃ���Q�[���I�u�W�F�N�g�̃^�O
    public string m_collideTag { get; set; } = "Wall";

    /// <summary>
    /// �I�u�W�F�N�g���ǂɐڐG�����u�ԁA�ǉ����Ɉړ�����x�N�g�����v�Z�����̂ɓK������B
    /// </summary>
    /// <param name="collision">�I�u�W�F�N�g���ڐG�����R���W����</param>
    /// <param name="rigidbody">�I�u�W�F�N�g�̍���</param>
    /// <param name="moveDirection">(�Q��)�I�u�W�F�N�g�̈ړ�����</param>
    public void CollisionEnter(Collision collision, Rigidbody rigidbody, ref Vector3 moveDirection)
    {
        //�R���W���������Q�[���I�u�W�F�N�g���Փ˔�����s���^�O�������Ă��Ȃ��ꍇ�͉������Ȃ��B
        if (collision.gameObject.CompareTag(m_collideTag) == false)
        {
            return;
        }

        //���̂̈ړ��������擾
        Vector3 velocity = rigidbody.velocity;
        velocity.Normalize();
        Debug.Log("Velocity " + velocity);

        //�ڐG���������蔻��̖@�����擾
        Vector3 normal = collision.contacts[0].normal;

        Debug.Log("Normal " + normal);

        if (Vector3.Dot(velocity, normal) > 0.0f)
        {



            //�ǂɉ����ē����x�N�g�����v�Z���A�ړ������Ƃ��Ċi�[
            moveDirection = velocity - (Vector3.Dot(velocity, normal) * normal);
            Debug.Log(moveDirection);
            //moveDirection.Normalize();

            //�����������̌��������Z�b�g���A�X�s�[�h���Z�b�g
            //rigidbody.velocity = m_alongWallSpeed * moveDirection;

        }

#if UNITY_EDITOR
        //���������ǂ̖@��������(Debug)
        DrawNormal(collision.contacts[0].point,normal);
#endif
    }

    /// <summary>
    /// ���������ꏊ�̖@����`��@�g�p����ꍇ�V�[����NormalDebugManager��u�����ƁB
    /// </summary>
    private void DrawNormal(Vector3 contactPoint,Vector3 normal)
    {
        //�@���f�o�b�O�}�l�[�W��������
        GameObject normalDebugManager = GameObject.Find("NormalDebugManager");

        if (normalDebugManager != null)
        {
            //�������ꍇ�@���f�o�b�O�p�I�u�W�F�N�g�𐶐�����悤�ɖ���
            normalDebugManager.GetComponent<NormalDebugManager>().Spawn(contactPoint, normal);
        }
    }
}
