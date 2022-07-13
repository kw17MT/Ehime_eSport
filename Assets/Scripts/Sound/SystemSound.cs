using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nsSound
{
    public class SystemSound : MonoBehaviour
    {
        //�C���X�^���X
        //private static SystemSound m_instance = null;

        //����V�X�e��
        Operation m_operation = null;
        //������SE�̃T�E���h�\�[�X
        private nsSound.SoundSource m_longTouchSS;
        //������SE���Đ������ǂ���
        private bool m_isLongTouchSSPlaying = false;

        //���݃Q�[�������j���[�n��ʂ��ǂ���
        private bool m_gameIsMenu = true;

        // Start is called before the first frame update
        void Start()
        {
            //����V�X�e���̃Q�[���I�u�W�F�N�g���������Q�[���R���|�[�l���g���擾����
            m_operation = GameObject.Find("OperationSystem").GetComponent<Operation>();

            DontDestroyOnLoad(gameObject);
        }

        // Update is called once per frame
        void Update()
        {
            if (m_gameIsMenu == false)
            {
                return;
            }

            //������SE
            LongTouchSound();

            PlayEnterSound();
        }

        //���݃��j���[�n�̉�ʂ��ǂ�����ݒ�B
        public void SetGameIsMenu(bool gameIsMenu)
        {
            m_gameIsMenu = gameIsMenu;
        }

        //������SE�̍Đ�
        private void LongTouchSound()
        {
            if (m_operation.GetNowOperation() == "touch")
            {
                if (m_isLongTouchSSPlaying == false)
                {
                    m_longTouchSS = new GameObject("SoundSource").AddComponent<nsSound.SoundSource>();
                    m_longTouchSS.SetSoundType(nsSound.EnSoundTypes.enSE);
                    m_longTouchSS.Be3DSound();
                    m_longTouchSS.PlayStart(nsSound.SENames.m_longPress);

                    m_isLongTouchSSPlaying = true;
                }
            }

            if (m_operation.GetNowOperation() != "touch")
            {
                if (m_isLongTouchSSPlaying)
                {
                    m_longTouchSS.Stop();
                    m_isLongTouchSSPlaying = false;
                }
            }
        }

        public void PlayEnterSound()
        {
            if (m_operation.GetIsLongTouch)
            {
                //���������Ă��邩�̔��肪���Z�b�g����Ă͍���̂ŁA�Đݒ�B
                m_operation.SetIsLongTouch(true);

                //���艹�̍Đ��ƁA������SE�̒�~
                nsSound.SoundSource enterSS = new GameObject("SoundSource").AddComponent<nsSound.SoundSource>();
                enterSS.SetSoundType(nsSound.EnSoundTypes.enSE);
                enterSS.Be3DSound();
                enterSS.PlayStart(nsSound.SENames.m_enter);

                if (m_isLongTouchSSPlaying)
                {
                    m_longTouchSS.Stop();
                    m_isLongTouchSSPlaying = false;
                    Destroy(this.gameObject.transform.root.gameObject);
                }
            }
        }
    }
}
