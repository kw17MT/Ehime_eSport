using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nsSound
{
    //�T�E���h�̕���
    public enum EnSoundTypes
    {
        enSE,           //SE
        enBGM,          //BGM
        enNarration,    //�i���[�V���������
        enCharaVoice,   //�L�����N�^�[�̐�
        enNone
    }   

    //�T�E���h�}�l�[�W���[�N���X�B�V���O���g���B
    public class SoundManager : MonoBehaviour
    {
        //�T�E���h�}�l�[�W���[�̃C���X�^���X
        private static SoundManager m_instance = null;

        //�T�E���h�S�̂̃{�����[���B�I�v�V������ʂŐݒ肷��B
        private float m_allVolume = 1.0f;

        //SE�ABGM�A�i���[�V�����A�L�����N�^�[�̐��A���ꂼ��̕��ނŐݒ肷��{�����[���B�I�v�V������ʂŐݒ肷��B
        private float m_seVolume = 1.0f;
        private float m_bgmVolume = 1.0f;
        private float m_narrationVolume = 1.0f;
        private float m_charaVoiceVolume = 1.0f;

        // Start is called before the first frame update
        void Start()
        {

        }
        private void OnDestroy()
        {
        }
        // Update is called once per frame
        void Update()
        {

        }

        //�C���X�^���X
        static public SoundManager Instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = new GameObject("SoundManager").AddComponent<SoundManager>();
                }
                return m_instance;
            }
            set
            {
            }
        }

        //�S�̂̉��ʂ����Ɛݒ�B
        public float GetAllVolume()
        {
            return m_allVolume;       
        }
        public void SetAllVolume(float volume)
        {
            m_allVolume = volume;
        }

        //SE�̉��ʂ����Ɛݒ�B
        public float GetSEVolume()
        {
            return m_seVolume;
        }
        public void SetSEVolume(float volume)
        {
            m_seVolume = volume;
        }
        //BGM�̉��ʂ����Ɛݒ�B
        public float GetBGMVolume()
        {
            return m_bgmVolume;
        }
        public void SetBGMVolume(float volume)
        {
            m_bgmVolume = volume;
        }
        //�i���[�V�����̉��ʂ����Ɛݒ�B
        public float GetNarrationVolume()
        {
            return m_narrationVolume;
        }
        public void SetNarrationVolume(float volume)
        {
            m_narrationVolume = volume;
        }
        //�L�����N�^�[�̐��̉��ʂ����Ɛݒ�B
        public float GetCharaVoiceVolume()
        {
            return m_charaVoiceVolume;
        }
        public void SetCharaVoiceVolume(float volume)
        {
            m_charaVoiceVolume = volume;
        }
    }
}