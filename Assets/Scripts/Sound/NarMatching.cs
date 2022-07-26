using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nsSound
{
    public class NarMatching : MonoBehaviour
    {
        //定数
        private const int m_maxNarPlayCount = 200;

        //ナレーションを再生するまでのカウント
        int m_narPlayCount = 0;

        private SoundSource m_narSS = null;

        // Start is called before the first frame update
        void Start()
        {
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
                m_narSS.PlayStart(nsSound.NarMatchNames.m_match_MATCHINGCHUUDAYO);

                m_narPlayCount = m_maxNarPlayCount;

            }
        }
    }
}