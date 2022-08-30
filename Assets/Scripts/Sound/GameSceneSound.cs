using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace nsSound
{
    public class GameSceneSound : MonoBehaviour
    {
        //ゴールの情報
        GoalScript m_goalScript = null;
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

            //StartCoroutine("Init");
            Init();

        }
        //コルーチン辞めろ
        void Init()
        {
            //yield return null;

            //ゴールの情報
            m_goalScript = GameObject.Find("Goal").GetComponent<GoalScript>();
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
                if (m_goalScript.GetOwnPlayerGoaled())
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

                        //モードに応じて音声案内を生成。
                        //サウンドソースを生成。
                        nsSound.SoundSource nextSS = new GameObject("SoundSource").AddComponent<nsSound.SoundSource>();
                        nextSS.SetSoundType(nsSound.EnSoundTypes.enNarration);
                        nextSS.Be3DSound();
                        //オフライン時
                        if (PhotonNetwork.OfflineMode)
                        {
                            nextSS.PlayStart(nsSound.NarResultNames.m_DOUBLETAPPUDEMOUICHIDORACEWOSURUYO);
                        }
                        //オンライン時
                        else
                        {
                            nextSS.PlayStart(nsSound.NarResultNames.m_DOUBLETAPPUDEMATCHINSCENENIMODORUYO);
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