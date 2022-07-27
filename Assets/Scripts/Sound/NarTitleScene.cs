using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nsSound
{
    public class NarTitleScene : MonoBehaviour
    {
        //定数
        private const int m_firstNarPlayCount = 50;
        private const int m_maxNarPlayCount = 300;

        private SoundSource m_narSS = null;

        //ナレーションを再生するまでのカウント
        int m_narPlayCount = 0;

        // Start is called before the first frame update
        void Start()
        {
            m_narPlayCount = m_firstNarPlayCount;
        }
        private void OnDestroy()
        {
            if (m_narSS != null)
            {
                m_narSS.Stop();
            }
        }

        // Update is called once per frame
        void Update()
        {
            m_narPlayCount--;

            if (m_narPlayCount <= 0)
            {
                m_narSS = new GameObject("SoundSource").AddComponent<nsSound.SoundSource>();
                m_narSS.SetSoundType(nsSound.EnSoundTypes.enNarration);
                m_narSS.Be3DSound();
                m_narSS.PlayStart(nsSound.NarTitleNames.m_GAMENNAGAOSHIDEGAMESTART);

                m_narPlayCount = m_maxNarPlayCount;

            }
        }
    }
}