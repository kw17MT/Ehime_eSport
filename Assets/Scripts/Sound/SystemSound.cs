using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nsSound
{
    public class SystemSound : MonoBehaviour
    {
        //インスタンス
        //private static SystemSound m_instance = null;

        //操作システム
        Operation m_operation = null;
        //長押しSEのサウンドソース
        private nsSound.SoundSource m_longTouchSS;
        //長押しSEが再生中かどうか
        private bool m_isLongTouchSSPlaying = false;

        //現在ゲームがメニュー系画面かどうか
        private bool m_gameIsMenu = true;

        // Start is called before the first frame update
        void Start()
        {
            //操作システムのゲームオブジェクトを検索しゲームコンポーネントを取得する
            m_operation = GameObject.Find("OperationSystem").GetComponent<Operation>();

            DontDestroyOnLoad(gameObject);
        }

        // Update is called once per frame
        void Update()
        {
            if (m_gameIsMenu == false)
            {
                return;
            }

            //長押しSE
            LongTouchSound();

            PlayEnterSound();
        }

        //現在メニュー系の画面かどうかを設定。
        public void SetGameIsMenu(bool gameIsMenu)
        {
            m_gameIsMenu = gameIsMenu;
        }

        //長押しSEの再生
        private void LongTouchSound()
        {
            if (m_operation.GetNowOperation() == "touch")
            {
                if (m_isLongTouchSSPlaying == false)
                {
                    m_longTouchSS = new GameObject("SoundSource").AddComponent<nsSound.SoundSource>();
                    m_longTouchSS.SetSoundType(nsSound.EnSoundTypes.enSE);
                    m_longTouchSS.Be3DSound();
                    m_longTouchSS.PlayStart(nsSound.SENames.m_longPress);

                    m_isLongTouchSSPlaying = true;
                }
            }

            if (m_operation.GetNowOperation() != "touch")
            {
                if (m_isLongTouchSSPlaying)
                {
                    m_longTouchSS.Stop();
                    m_isLongTouchSSPlaying = false;
                }
            }
        }

        public void PlayEnterSound()
        {
            if (m_operation.GetIsLongTouch)
            {
                //長押ししているかの判定がリセットされては困るので、再設定。
                m_operation.SetIsLongTouch(true);

                //決定音の再生と、長押しSEの停止
                nsSound.SoundSource enterSS = new GameObject("SoundSource").AddComponent<nsSound.SoundSource>();
                enterSS.SetSoundType(nsSound.EnSoundTypes.enSE);
                enterSS.Be3DSound();
                enterSS.PlayStart(nsSound.SENames.m_enter);

                if (m_isLongTouchSSPlaying)
                {
                    m_longTouchSS.Stop();
                    m_isLongTouchSSPlaying = false;
                    Destroy(this.gameObject.transform.root.gameObject);
                }
            }
        }
    }
}
