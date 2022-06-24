using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nsSound
{
    //サウンドの分類
    public enum EnSoundTypes
    {
        enSE,           //SE
        enBGM,          //BGM
        enNarration,    //ナレーションや実況
        enCharaVoice,   //キャラクターの声
        enNone
    }   

    //サウンドマネージャークラス。シングルトン。
    public class SoundManager : MonoBehaviour
    {
        //サウンドマネージャーのインスタンス
        private static SoundManager m_instance = null;

        //サウンド全体のボリューム。オプション画面で設定する。
        private float m_allVolume = 1.0f;

        //SE、BGM、ナレーション、キャラクターの声、それぞれの分類で設定するボリューム。オプション画面で設定する。
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

        //インスタンス
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

        //全体の音量を入手と設定。
        public float GetAllVolume()
        {
            return m_allVolume;       
        }
        public void SetAllVolume(float volume)
        {
            m_allVolume = volume;
        }

        //SEの音量を入手と設定。
        public float GetSEVolume()
        {
            return m_seVolume;
        }
        public void SetSEVolume(float volume)
        {
            m_seVolume = volume;
        }
        //BGMの音量を入手と設定。
        public float GetBGMVolume()
        {
            return m_bgmVolume;
        }
        public void SetBGMVolume(float volume)
        {
            m_bgmVolume = volume;
        }
        //ナレーションの音量を入手と設定。
        public float GetNarrationVolume()
        {
            return m_narrationVolume;
        }
        public void SetNarrationVolume(float volume)
        {
            m_narrationVolume = volume;
        }
        //キャラクターの声の音量を入手と設定。
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