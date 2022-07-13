using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nsSound
{
    public class GameSceneSound : MonoBehaviour
    {
        //�v���C���[�̃A�o�^�[�̏��
        AvatarController m_ownPlayerAvatarController = null;
        //�v���C���[�̃p�����[�^�̎擾
        ParamManage m_paramManager = null;

        //�S�[���������̃t�@���t�@�[��
        SoundSource m_goalFanfare = null;

        //�S�[���������ǂ���
        bool m_goaled = false;
        //�t�@���t�@�[�����I��������ǂ����B
        bool m_fanfareEnd = false;

        // Start is called before the first frame update
        void Start()
        {
            //�G���W����
            SoundSource m_startEngineSE = new GameObject("SoundSource").AddComponent<nsSound.SoundSource>();
            m_startEngineSE.SetSoundType(nsSound.EnSoundTypes.enSE);
            m_startEngineSE.Be3DSound();
            m_startEngineSE.SetLoop(false);
            m_startEngineSE.PlayStart(nsSound.SENames.m_startEngine);

            StartCoroutine("Init");


        }
        IEnumerator Init()
        {
            yield return null;

            //�v���C���[�̃A�o�^�[�̏��
            m_ownPlayerAvatarController = GameObject.Find("OwnPlayer").GetComponent<AvatarController>();
            //�Q�[�����̃p�����[�^�ۑ��C���X�^���X���擾����
            m_paramManager = GameObject.Find("ParamManager").GetComponent<ParamManage>();
        }

        // Update is called once per frame
        void Update()
        {
            

            GoalSound();
            
        }

		private void OnDestroy()
		{
            if (m_goalFanfare != null)
            {
                m_goalFanfare.FadeOutStart();
            }
		}

		private void GoalSound()
        {
            if (m_goaled == false)
            {
                //�S�[�����Ă�����
                if (m_ownPlayerAvatarController.GetGoaled())
                {
                    //�t���O���S�[���������Ƃ�
                    m_goaled = true;

                    BGM.Instance.Stop();

                    //�t�@���t�@�[���̍Đ�
                    m_goalFanfare = new GameObject("SoundSource").AddComponent<nsSound.SoundSource>();
                    m_goalFanfare.SetSoundType(nsSound.EnSoundTypes.enBGM);
                    m_goalFanfare.SetLoop(false);
                    //���ʂɉ����ăt�@���t�@�[����ݒ�
                    switch (m_paramManager.GetPlace())
                    {
                        case 1:
                            m_goalFanfare.PlayStart(nsSound.BGMNames.m_fanfare1);
                            break;
                        default:
                            m_goalFanfare.PlayStart(nsSound.BGMNames.m_fanfare2);
                            break;
                    }
                }
            }
            else
            {
                if (m_fanfareEnd == false)
                {
                    //�t�@���t�@�[���̍Đ����I�������
                    if (m_goalFanfare == null)
                    {
                        //�t���O��ݒ�
                        m_fanfareEnd = true;
                        //���U���g�Ȃ̍Đ�
                        //���ʂɉ����ă��U���g�Ȃ�ݒ�
                        switch (m_paramManager.GetPlace())
                        {
                            case 1:
                                BGM.Instance.SetPlayBGM(nsSound.BGMNames.m_result1);
                                break;
                            default:
                                BGM.Instance.SetPlayBGM(nsSound.BGMNames.m_result2);
                                break;
                        }
                    }
                }
            }
        }

        public void PlayGetItem()
        {
        }

        //public void Play
    }
}