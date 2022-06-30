using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSoundPlay : MonoBehaviour
{
    [SerializeField]
    Dictionary<string, CriWare.CriAtomSource> m_atomCrces;

    nsSound.SoundSource m_ss = null;

    float m_volume = 1.0f;
    float m_pitch = 0.0f;
    float m_ratio = 1.0f;

    float x = 0.0f;
    float y = 10.0f;
    float z = 10.0f;

    Vector3 m_pos;

    // Start is called before the first frame update
    void Start()
    {
        m_pos = new Vector3(x, y, z);
    }

    // Update is called once per frame     
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            m_ss = new GameObject("SoundSource").AddComponent<nsSound.SoundSource>();
            m_ss.SetSoundType(nsSound.EnSoundTypes.enBGM);
            m_ss.SetLoop(true);

            m_ss.Be3DSound();
            m_ss.Set3DListenerPos(0.0f,0.0f,0.0f);
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            m_ss.PlayStart(nsSound.BGMNames.m_kTest);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            m_ss.FadeOutStart();
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            m_ss.Pause();
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            m_ss.SetFadeInTime(0);
            m_ss.SetPlayStartOffset(1000);
            m_ss.SetFadeOutTime(1000);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            m_ss.Resume();
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            //m_volume += 0.1f;
            //m_ss.SetSourceVolume(m_volume);
            m_pitch += 20.0f;
            m_ss.SetPitch(m_pitch);
            //m_ratio += 0.1f;
            //m_ss.SetPlaybackRatio(m_ratio);

        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            //m_volume -= 0.1f;
            //m_ss.SetSourceVolume(m_volume);
            m_pitch -= 20.0f;
            m_ss.SetPitch(m_pitch);
            //m_ratio -= 0.1f;
            //m_ss.SetPlaybackRatio(m_ratio);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            //m_ss.SetLoop(true);

            m_pos.x += 100.0f;
            m_ss.Set3DSourcePos(m_pos);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            //m_ss.SetLoop(false);

            m_pos.x -= 1.0f;
            m_ss.Set3DSourcePos(m_pos);
        }
    }
}
