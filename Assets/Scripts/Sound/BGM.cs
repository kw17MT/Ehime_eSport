using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nsSound
{
	//ゲーム内で同時にBGMを複数再生することは基本的にないので、シングルトン。

	public class BGM
	{
		//インスタンス
		private static BGM m_instance = null;

		//BGMのサウンドソース
		private nsSound.SoundSource m_bgmSS;

		//再生中のサウンドの名前
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

		//BGMを再生する関数。同じ曲ならば最初からにならず、引き続き再生します。
		//引数は、SoundNamesクラスのBGM名の定数を使用しましょう。
		public void SetPlayBGM(string bgmName)
		{
			if (m_plaingBGMNames == bgmName)
			{
				return;
			}

			//もしBGMが鳴っていないなら、サウンドを作成
			if (m_bgmSS == null)
			{
				Init();
			}

			m_plaingBGMNames = bgmName;

			//BGMを再生
			m_bgmSS.PlayStart(bgmName);
		}

		//再生中のBGMの名前を取得
		public string GetPlayingBGMNames
		{
			get
			{
				return m_plaingBGMNames;
			}
		}

		//BGMをフェードアウト
		public void FadeOutStart()
		{
			m_bgmSS.FadeOutStart();
		}

		//BGMの停止
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