using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nsSound
{
    public class GameSceneSound : MonoBehaviour
    {
        //プレイヤーのアバターの情報
        AvatarController m_ownPlayerAvatarController = null;
        //プレイヤーのパラメータの取得
        ParamManage m_paramManager = null;

        //ゴールした時のファンファーレ
        SoundSource m_goalFanfare = null;

        //ゴールしたかどうか
        bool m_goaled = false;
        //ファンファーレが終わったかどうか。
        bool m_fanfareEnd = false;

        // Start is called before the first frame update
        void Start()
        {
            //エンジン音
            SoundSource m_startEngineSE = new GameObject("SoundSource").AddComponent<nsSound.SoundSource>();
            m_startEngineSE.SetSoundType(nsSound.EnSoundTypes.enSE);
            m_startEngineSE.Be3DSound();
            m_startEngineSE.SetLoop(false);
            m_startEngineSE.PlayStart(nsSound.SENames.m_startEngine);

            StartCoroutine("Init");


        }
        IEnumerator Init()
        {
            yield return null;

            //プレイヤーのアバターの情報
            m_ownPlayerAvatarController = GameObject.Find("OwnPlayer").GetComponent<AvatarController>();
            //ゲーム中のパラメータ保存インスタンスを取得する
            m_paramManager = GameObject.Find("ParamManager").GetComponent<ParamManage>();
        }

        // Update is called once per frame
        void Update()
        {
            

            GoalSound();
            
        }

		private void OnDestroy()
		{
            if (m_goalFanfare != null)
            {
                m_goalFanfare.FadeOutStart();
            }
		}

		private void GoalSound()
        {
            if (m_goaled == false)
            {
                //ゴールしていたら
                if (m_ownPlayerAvatarController.GetGoaled())
                {
                    //フラグをゴールしたことに
                    m_goaled = true;

                    BGM.Instance.Stop();

                    //ファンファーレの再生
                    m_goalFanfare = new GameObject("SoundSource").AddComponent<nsSound.SoundSource>();
                    m_goalFanfare.SetSoundType(nsSound.EnSoundTypes.enBGM);
                    m_goalFanfare.SetLoop(false);
                    //順位に応じてファンファーレを設定
                    switch (m_paramManager.GetPlace())
                    {
                        case 1:
                            m_goalFanfare.PlayStart(nsSound.BGMNames.m_fanfare1);
                            break;
                        default:
                            m_goalFanfare.PlayStart(nsSound.BGMNames.m_fanfare2);
                            break;
                    }
                }
            }
            else
            {
                if (m_fanfareEnd == false)
                {
                    //ファンファーレの再生が終わったら
                    if (m_goalFanfare == null)
                    {
                        //フラグを設定
                        m_fanfareEnd = true;
                        //リザルト曲の再生
                        //順位に応じてリザルト曲を設定
                        switch (m_paramManager.GetPlace())
                        {
                            case 1:
                                BGM.Instance.SetPlayBGM(nsSound.BGMNames.m_result1);
                                break;
                            default:
                                BGM.Instance.SetPlayBGM(nsSound.BGMNames.m_result2);
                                break;
                        }
                    }
                }
            }
        }

        public void PlayGetItem()
        {
        }

        //public void Play
    }
}