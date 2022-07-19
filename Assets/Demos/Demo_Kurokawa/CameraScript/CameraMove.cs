using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    private Camera m_camera;                                                  //�g�p���郁�C���J����
    private GameObject m_ownPlayer = null;                                  //�ǐ��Ώۂ̃Q�[���I�u�W�F�N�g�i�v���C���[�j
    private bool m_isGetOwnPlayer = false;                                  //�v���C���[�C���X�^���X���m�ۂł�����
    private Vector3 m_prevPlayerForward = Vector3.zero;                     //�O�t���[���̃v���C���[�̑O����
    private Vector3 m_prevCameraPos = Vector3.zero;                         //�O�t���[���̃J�����̈ʒu
    [SerializeField] float BEHIND_RATE_FROM_PLAYER = 5.0f;                  //�J�����̈ʒu���ǂ̂��炢�v���C���[�̌��ɂ��邩
    [SerializeField] float UPPER_RATE_FROM_PLAYER = 5.0f;                   //�J�����̈ʒu���ǂ̂��炢�v���C���[�̏�ɂ��邩

	private void Start()
	{
        //���C���J�������擾
        m_camera = Camera.main;
    }

    private void FindOwnPlayer()
	{
        //�ʐM�̊֌W��A�����ŃC���X�^���X���m�ۂł���܂ŒT��
        if (!m_isGetOwnPlayer)
        {
            //�����̃C���X�^���X���擾
            m_ownPlayer = GameObject.Find("OwnPlayer");
            if (m_ownPlayer != null)
            {
                m_isGetOwnPlayer = true;
            }
        }
    }

    private void FollowPlayer()
	{
        Vector3 cameraPos;
        //�v���C���[���U������Ă��Ȃ����
        if (!m_ownPlayer.GetComponent<AvatarController>().GetIsAttacked())
        {
            //�J�����̈ʒu�̓v���C���[�̏������̈ʒu��
            cameraPos = m_ownPlayer.transform.position + (m_ownPlayer.transform.forward * -1.0f) * BEHIND_RATE_FROM_PLAYER;
            //���������ݒ肷��B
            cameraPos.y += UPPER_RATE_FROM_PLAYER;
            //�v���C���[�̑O�������L�^
            m_prevPlayerForward = m_ownPlayer.transform.forward;
        }
        else
        {
            //�J�����̈ʒu�̓v���C���[�̏������̈ʒu��
            cameraPos = m_ownPlayer.transform.position + (m_prevPlayerForward * -1.0f) * BEHIND_RATE_FROM_PLAYER;
            //���������ݒ肷��B
            cameraPos.y += UPPER_RATE_FROM_PLAYER;
        }

        //�ʒu��ݒ肵
        m_camera.gameObject.transform.position = Vector3.Lerp(cameraPos, m_prevCameraPos, 0.1f);
        m_prevCameraPos = m_camera.gameObject.transform.position;

        if (!m_ownPlayer.GetComponent<AvatarController>().GetIsAttacked())
        {
            //���ڑΏۂ̓v���C���[�ɂ���
            m_camera.gameObject.transform.LookAt(m_ownPlayer.transform);
        }

    }

    // Update is called once per frame
    void Update()
    {
        //�����̃v���C���[�C���X�^���X��T��
        FindOwnPlayer();

        //�Q�[�����I��������A�O�̃V�[���ɖ߂�Ƃ��Ƀk�����Q�Ƃ��Ȃ��悤�ɂ��邽��
        if (m_ownPlayer == null)
		{
            return;
		}

        FollowPlayer();
    }
}
