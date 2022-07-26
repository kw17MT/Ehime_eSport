using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nsSound
{
    public class OutGameNarBase : MonoBehaviour
    {
        //�萔
        protected const int m_maxNarInterval = 10;

        //��]�̏��
        protected CircleCenterRotateAround m_circleCenterRotateAround = null;

        //���݂̑I�����
        protected int m_selectStateNo = 0;

        //�i���[�V�����̊Ԋu
        protected int m_narInterval = 0;

        //�i���[�V�����̃T�E���h�\�[�X
        protected SoundSource m_narSS = null;

        //���ʂ̃i���[�V����
        protected List<string> m_commonNarList;

        //�I�����Ă��郂�[�h���L�̃i���[�V�����̃��X�g�̃��X�g
        protected List<List<string>> m_narList;

        //�Đ�����i���[�V�����̔ԍ�
        protected int m_playNarNo = 1;

        //�Đ����Ă���i���[�V������m_commonNarList���ǂ��� 
        protected bool m_playCommonNar = true;

        // Start is called before the first frame update
        void Start()
        {
            //�ŏ��̃i���[�V��������܂ł̊Ԋu��ݒ�
            m_narInterval = m_maxNarInterval;
            //�Đ�����i���[�V�����̖��O�̃��X�g�̃��X�g
            m_narList = new List<List<string>>();
            //�d�Ԃ̉�]�̏����擾
            m_circleCenterRotateAround = GameObject.Find("Train").GetComponent<CircleCenterRotateAround>();


            Init();
        }

		private void OnDestroy()
		{
            if (m_narSS != null)
            {
                m_narSS.Stop();
            }

            m_narList.Clear();
            m_commonNarList.Clear();
        }

		// Update is called once per frame
		void Update()
        {
            //�d�Ԃ���]���Ȃ��
            if (m_circleCenterRotateAround.GetAroundMoveOn())
            {
                return;
            }

            //�T�E���h���Đ�����Ă��Ȃ���΁A�C���^�[�o���𐔂���
            if (m_narSS == null)
            {
                m_narInterval--;
            }
            //�C���^�[�o�����I�������
            if (m_narInterval <= 0)
            {
                //���̑I�����Ă��郂�[�h���L�̃i���[�V�����̍Đ�
                if (m_playCommonNar == false)
                {
                    //�i���[�V�������p�ӂ���Ă��Ȃ���΁AcommonNar�ֈڍs
                    if (m_narList.Count == 0)
                    {
                        m_playCommonNar = true;
                        return;
                    }
                    //�i���[�V�����̍Đ�
                    m_narSS = new GameObject("SoundSource").AddComponent<nsSound.SoundSource>();
                    m_narSS.SetSoundType(nsSound.EnSoundTypes.enNarration);
                    m_narSS.PlayStart(m_narList[m_selectStateNo][m_playNarNo]);

                    //���Ƀv���C����i���[�V�����̔ԍ���
                    m_playNarNo++;
                    Debug.Log(m_selectStateNo);
                    //�Ō�܂ōĐ����ꂽ��
                    if (m_playNarNo >= m_narList[m_selectStateNo].Count)
                    {
                        //���ʂ̃i���[�V�����Ɉڍs
                        m_playCommonNar = true;
                        //�ԍ������Z�b�g
                        m_playNarNo = 0;
                    }
                }
                else
                {
                    //commonNar���p�ӂ���Ă��Ȃ���΁A�߂�B
                    if (m_commonNarList.Count == 0)
                    {
                        m_playCommonNar = false;
                        return;
                    }

                    //�i���[�V�����̍Đ�
                    m_narSS = new GameObject("SoundSource").AddComponent<nsSound.SoundSource>();
                    m_narSS.SetSoundType(nsSound.EnSoundTypes.enNarration);
                    m_narSS.PlayStart(m_commonNarList[m_playNarNo]);

                    //���Ƀv���C����i���[�V�����̔ԍ���
                    m_playNarNo++;
                    //�Ō�܂ōĐ����ꂽ��
                    if (m_playNarNo >= m_commonNarList.Count)
                    {
                        //�I�����Ă��郂�[�h�̃i���[�V�����ɖ߂�
                        m_playCommonNar = false;
                        //�ԍ������Z�b�g
                        m_playNarNo = 0;
                    }
                }

                //�C���^�[�o�����Đݒ�
                m_narInterval = m_maxNarInterval;
            }

            //���̑I����Ԃ��`�F�b�N����B
            if (CheckNowSelectState())
            {
                ResetNarration();
            }

        }

        private void ResetNarration()
        {
            //�i���[�V�������Đ����Ȃ�A�~�߂�
            if (m_narSS != null)
            {
                m_narSS.Stop();
            }
            //�C���^�[�o�������Z�b�g
            m_narInterval = 0;
            //(���ʂł͂Ȃ�)���̃��[�h�̃i���[�V������
            m_playCommonNar = false;
            //�ԍ������Z�b�g
            m_playNarNo = 0;
        }


        //Init()�ł�邱�ƁB
        //1.�V�[���̏����擾
        //2.���ʂ̃i���[�V������p�ӂ���B
        //3.�i���[�V�����̃��X�g�̗p��
        //4.�i���[�V�����̃��X�g�̃��X�g�ɁA3�̃��X�g��Add()����B(���̃V�[���̑I����Ԃ̗񋓑̂̏��Ԓʂ�ɁI)
        //�����񋓑̂̏��Ԃ��ς������A�i���[�V�����̃��X�g�̃��X�g��Add()���鏇�Ԃ��ς��邱�ƁI
        protected virtual void Init()
        {
        }

        //���݂̑I����Ԃ𒲂ׂ�Breturn:�ς���Ă��邩�ǂ���
        protected virtual bool CheckNowSelectState()
        {           
            return false;
        }
    }
}