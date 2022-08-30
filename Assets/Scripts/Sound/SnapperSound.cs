using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nsSound
{
    public class SnapperSound : MonoBehaviour
    {
        private Transform m_ownPlayerTransform = null;
        private Transform m_myTransform = null;

        private SoundSource m_ss = null;
        // Start is called before the first frame update
        void Start()
        {
            m_ownPlayerTransform = GameObject.Find("OwnPlayer").transform;
            m_myTransform = this.transform;
            
            
            m_ss = new GameObject("SoundSource").AddComponent<nsSound.SoundSource>();
            m_ss.SetSoundType(nsSound.EnSoundTypes.enSE);
            m_ss.Be3DSound();
            m_ss.SetLoop(true);
            m_ss.PlayStart(nsSound.SENames.m_snapper);
        }

        // Update is called once per frame
        void Update()
        {
            m_ss.Set3DListenerPos(m_ownPlayerTransform.position);
            m_ss.Set3DListenerDir(m_ownPlayerTransform.forward);
            m_ss.Set3DSourcePos(m_myTransform.position);
        }

		private void OnDestroy()
		{
            if (m_ss != null)
            {
                m_ss.Stop();
            }
		}
	}
}