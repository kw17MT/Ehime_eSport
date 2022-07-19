using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    private Camera camera;
    private GameObject m_ownPlayer = null;        //�ǐ��Ώۂ̃Q�[���I�u�W�F�N�g�i�v���C���[�j
    private bool m_isGetOwnPlayer = false;        //�v���C���[�C���X�^���X���m�ۂł�����
    private Vector3 m_prevPlayerForward = Vector3.zero;
    private Vector3 m_prevCameraPos = Vector3.zero;

    public float BEHIND_RATE_FROM_PLAYER = 5.0f; //�J�����̈ʒu���ǂ̂��炢�v���C���[�̌��ɂ��邩
    public float UPPER_RATE_FROM_PLAYER = 5.0f;   //�J�����̈ʒu���ǂ̂��炢�v���C���[�̏�ɂ��邩

	private void Start()
	{
        //���C���J�������擾
        camera = Camera.main;
    }

	// Update is called once per frame
	void Update()
    {
        //�ʐM�̊֌W��A�����ŃC���X�^���X���m�ۂł���܂ŒT��
        if(!m_isGetOwnPlayer)
		{
            //�����̃C���X�^���X���擾
            m_ownPlayer = GameObject.Find("OwnPlayer");
            if(m_ownPlayer != null)
			{
                m_isGetOwnPlayer = true;
            }
        }

        Vector3 cameraPos;
        if (!m_ownPlayer.GetComponent<AvatarController>().GetIsAttacked())
		{
            //�J�����̈ʒu�̓v���C���[�̏������̈ʒu��
            cameraPos = m_ownPlayer.transform.position + (m_ownPlayer.transform.forward * -1.0f) * BEHIND_RATE_FROM_PLAYER;
            //���������ݒ肷��B
            cameraPos.y += UPPER_RATE_FROM_PLAYER;

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
        camera.gameObject.transform.position = Vector3.Lerp(cameraPos, m_prevCameraPos, 0.1f);
        m_prevCameraPos = camera.gameObject.transform.position;

        if (!m_ownPlayer.GetComponent<AvatarController>().GetIsAttacked())
		{
            //���ڑΏۂ̓v���C���[�ɂ���
            camera.gameObject.transform.LookAt(m_ownPlayer.transform);
        }

    }
}
