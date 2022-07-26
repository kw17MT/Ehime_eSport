using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nsSound
{
    public class OutGameNarBase : MonoBehaviour
    {
        //定数
        protected const int m_maxNarInterval = 10;

        //回転の情報
        protected CircleCenterRotateAround m_circleCenterRotateAround = null;

        //現在の選択状態
        protected int m_selectStateNo = 0;

        //ナレーションの間隔
        protected int m_narInterval = 0;

        //ナレーションのサウンドソース
        protected SoundSource m_narSS = null;

        //共通のナレーション
        protected List<string> m_commonNarList;

        //選択しているモード特有のナレーションのリストのリスト
        protected List<List<string>> m_narList;

        //再生するナレーションの番号
        protected int m_playNarNo = 1;

        //再生しているナレーションがm_commonNarListかどうか 
        protected bool m_playCommonNar = true;

        // Start is called before the first frame update
        void Start()
        {
            //最初のナレーションが鳴るまでの間隔を設定
            m_narInterval = m_maxNarInterval;
            //再生するナレーションの名前のリストのリスト
            m_narList = new List<List<string>>();
            //電車の回転の情報を取得
            m_circleCenterRotateAround = GameObject.Find("Train").GetComponent<CircleCenterRotateAround>();


            Init();
        }

		private void OnDestroy()
		{
            if (m_narSS != null)
            {
                m_narSS.Stop();
            }

            m_narList.Clear();
            m_commonNarList.Clear();
        }

		// Update is called once per frame
		void Update()
        {
            //電車が回転中ならば
            if (m_circleCenterRotateAround.GetAroundMoveOn())
            {
                return;
            }

            //サウンドが再生されていなければ、インターバルを数える
            if (m_narSS == null)
            {
                m_narInterval--;
            }
            //インターバルが終わったら
            if (m_narInterval <= 0)
            {
                //その選択しているモード特有のナレーションの再生
                if (m_playCommonNar == false)
                {
                    //ナレーションが用意されていなければ、commonNarへ移行
                    if (m_narList.Count == 0)
                    {
                        m_playCommonNar = true;
                        return;
                    }
                    //ナレーションの再生
                    m_narSS = new GameObject("SoundSource").AddComponent<nsSound.SoundSource>();
                    m_narSS.SetSoundType(nsSound.EnSoundTypes.enNarration);
                    m_narSS.PlayStart(m_narList[m_selectStateNo][m_playNarNo]);

                    //次にプレイするナレーションの番号に
                    m_playNarNo++;
                    Debug.Log(m_selectStateNo);
                    //最後まで再生されたら
                    if (m_playNarNo >= m_narList[m_selectStateNo].Count)
                    {
                        //共通のナレーションに移行
                        m_playCommonNar = true;
                        //番号をリセット
                        m_playNarNo = 0;
                    }
                }
                else
                {
                    //commonNarが用意されていなければ、戻る。
                    if (m_commonNarList.Count == 0)
                    {
                        m_playCommonNar = false;
                        return;
                    }

                    //ナレーションの再生
                    m_narSS = new GameObject("SoundSource").AddComponent<nsSound.SoundSource>();
                    m_narSS.SetSoundType(nsSound.EnSoundTypes.enNarration);
                    m_narSS.PlayStart(m_commonNarList[m_playNarNo]);

                    //次にプレイするナレーションの番号に
                    m_playNarNo++;
                    //最後まで再生されたら
                    if (m_playNarNo >= m_commonNarList.Count)
                    {
                        //選択しているモードのナレーションに戻す
                        m_playCommonNar = false;
                        //番号をリセット
                        m_playNarNo = 0;
                    }
                }

                //インターバルを再設定
                m_narInterval = m_maxNarInterval;
            }

            //今の選択状態をチェックする。
            if (CheckNowSelectState())
            {
                ResetNarration();
            }

        }

        private void ResetNarration()
        {
            //ナレーションが再生中なら、止める
            if (m_narSS != null)
            {
                m_narSS.Stop();
            }
            //インターバルをリセット
            m_narInterval = 0;
            //(共通ではなく)そのモードのナレーションに
            m_playCommonNar = false;
            //番号をリセット
            m_playNarNo = 0;
        }


        //Init()でやること。
        //1.シーンの情報を取得
        //2.共通のナレーションを用意する。
        //3.ナレーションのリストの用意
        //4.ナレーションのリストのリストに、3のリストをAdd()する。(そのシーンの選択状態の列挙体の順番通りに！)
        //もし列挙体の順番が変わったら、ナレーションのリストのリストにAdd()する順番も変えること！
        protected virtual void Init()
        {
        }

        //現在の選択状態を調べる。return:変わっているかどうか
        protected virtual bool CheckNowSelectState()
        {           
            return false;
        }
    }
}