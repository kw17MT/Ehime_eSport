using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nsSound
{
	//�Q�[�����œ�����BGM�𕡐��Đ����邱�Ƃ͊�{�I�ɂȂ��̂ŁA�V���O���g���B

	public class BGM
	{
		//�C���X�^���X
		private static BGM m_instance = null;

		//BGM�̃T�E���h�\�[�X
		private nsSound.SoundSource m_bgmSS;

		//�Đ����̃T�E���h�̖��O
		private string m_plaingBGMNames = nsSound.BGMNames.m_nothing;

		static public BGM Instance
		{
			get
			{
				if (m_instance == null)
				{
					m_instance = new BGM();
				}
				return m_instance;
			}
			set
			{
			}
		}


		// Start is called before the first frame update
		void Init()
		{
			//BGM
			m_bgmSS = new GameObject("BGM").AddComponent<nsSound.SoundSource>();
			m_bgmSS.SetSoundType(nsSound.EnSoundTypes.enBGM);
			m_bgmSS.SetLoop(true);
		}

		// Update is called once per frame
		void Update()
		{

		}

		//BGM���Đ�����֐��B�����ȂȂ�΍ŏ�����ɂȂ炸�A���������Đ����܂��B
		//�����́ASoundNames�N���X��BGM���̒萔���g�p���܂��傤�B
		public void SetPlayBGM(string bgmName)
		{
			if (m_plaingBGMNames == bgmName)
			{
				return;
			}

			//����BGM�����Ă��Ȃ��Ȃ�A�T�E���h���쐬
			if (m_bgmSS == null)
			{
				Init();
			}

			m_plaingBGMNames = bgmName;

			//BGM���Đ�
			m_bgmSS.PlayStart(bgmName);
		}

		//�Đ�����BGM�̖��O���擾
		public string GetPlayingBGMNames
		{
			get
			{
				return m_plaingBGMNames;
			}
		}

		//BGM���t�F�[�h�A�E�g
		public void FadeOutStart()
		{
			m_bgmSS.FadeOutStart();
		}

		//BGM�̒�~
		public void Stop()
		{
			m_bgmSS.Stop();
		}

		public void SetVolume(float volume)
		{
			m_bgmSS.SetSourceVolume(volume);
		}

	}
}