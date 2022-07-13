using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nsSound
{
    public class PlayerSound : MonoBehaviour
    {
        //定数
        //アクセル音のピッチの高さ
        private const int m_accelePitchHeight = 50;
        //3Dサウンドが聞こえる最小距離
        private const float m_3DSoundMinDistance = 1.0f;
        //3Dサウンドが聞こえる最大距離
        private const float m_3DSoundMaxDistance = 20.0f;

        //3Dサウンド用
        private Vector3 m_ssPos;
        private Vector3 m_listenerPos;

        private SoundSource m_engineSE = null;
        private SoundSource m_acceleSE = null;
        private float m_acceleSEPitch = 0.0f;

        //このゲーム画面のプレイヤーの情報
        private RaceAIScript m_ownPlayerRaceAI = null;

        //このゲーム画面のプレイヤーのアイテム情報
        private ObtainItemController m_obtainItemController = null;

        //アイテムスロット回転中のサウンド
        private SoundSource m_itemSlotLotterySE = null;
        //アイテムスロットのサウンドを鳴らすフラグ
        private bool m_itemSlotSoundPlayFlag = false;

        // Start is called before the first frame update
        void Start()
        {
            //エンジン音
            m_engineSE = new GameObject("SoundSource").AddComponent<nsSound.SoundSource>();
            m_engineSE.SetSoundType(nsSound.EnSoundTypes.enSE);
            m_engineSE.Be3DSound();
            m_engineSE.SetLoop(true);
            m_engineSE.Set3DMinMaxDistance(m_3DSoundMinDistance, m_3DSoundMaxDistance);
            m_engineSE.PlayStart(nsSound.SENames.m_engine);


            //アクセル音
            m_acceleSE = new GameObject("SoundSource").AddComponent<nsSound.SoundSource>();
            m_acceleSE.SetSoundType(nsSound.EnSoundTypes.enSE);
            m_acceleSE.Be3DSound();
            m_acceleSE.SetLoop(true);
            m_acceleSE.Set3DMinMaxDistance(m_3DSoundMinDistance, m_3DSoundMaxDistance);
            m_acceleSE.PlayStart(nsSound.SENames.m_accele);

            StartCoroutine("Init");

        }

        IEnumerator Init()
        {
            yield return null;

            //このゲームオブジェクトがプレイヤー自信ならば
            if (this.gameObject.name == "OwnPlayer")
            {
                m_obtainItemController = this.gameObject.GetComponent<ObtainItemController>();

            }
            //プレイヤー以外ならば
            else
            {
                m_ownPlayerRaceAI = GameObject.Find("OwnPlayer").GetComponent<RaceAIScript>();
            }
        }

        // Update is called once per frame
        void Update()
        {

            if (this.gameObject.name != "OwnPlayer")
            {
                m_ssPos = this.GetComponent<RaceAIScript>().GetRigidBody.position;
                m_listenerPos = m_ownPlayerRaceAI.GetRigidBody.position;

                m_engineSE.Set3DSourcePos(m_ssPos);
                m_engineSE.Set3DListenerPos(m_listenerPos);
            }

            AcceleSound();

            if (this.gameObject.name == "OwnPlayer")
            {
                ItemGetSound();
            }
        }

        private void OnDestroy()
        {
            if (m_engineSE != null)
            {
                m_engineSE.Stop();
            }
            if (m_acceleSE != null)
            {
                m_acceleSE.Stop();
            }
            if (m_itemSlotLotterySE != null)
            {
                m_itemSlotLotterySE.Stop();
            }
        }

        //アクセル音の再生。移動速度に合わせて、ピッチを変える。
        private void AcceleSound()
        {
            //車の速さ
            Vector3 kartVelocity = this.GetComponent<RaceAIScript>().GetRigidBody.velocity;
            kartVelocity.y = 0.0f;
            float kartSpeed = kartVelocity.magnitude;

            //車がほぼ動いていなければ、音を消す。
            if (kartSpeed <= 1.0f)
            {
                m_acceleSE.SetSourceVolume(0.0f);
            }
            //車が動いていれば、音を流す。
            else
            {
                m_acceleSE.SetSourceVolume(1.0f);
            }

            //車の速さに合わせてピッチを設定
            m_acceleSEPitch = kartSpeed * m_accelePitchHeight;
            m_acceleSE.SetPitch(m_acceleSEPitch);

            //自分以外の車の音は、3Dサウンドに。
            if (this.gameObject.name != "OwnPlayer")
            {
                m_acceleSE.Set3DSourcePos(m_ssPos);
                m_acceleSE.Set3DListenerPos(m_listenerPos);
            }
        }

        private void ItemGetSound()
        {
            //アイテムを入手したら
            if (m_obtainItemController.GetObtainItemType() != -1)
            {
                //アイテムスロットの抽選中なら(抽選が終わってないなら)
                if (m_obtainItemController.GetLotteryFinish() == false)
                {
                    if (m_itemSlotSoundPlayFlag == false)
                    {
                        //アイテムスロットのサウンドを鳴らすフラグを音にする。
                        m_itemSlotSoundPlayFlag = true;

                        //アイテム抽選サウンドの開始
                        m_itemSlotLotterySE = new GameObject("SoundSource").AddComponent<nsSound.SoundSource>();
                        m_itemSlotLotterySE.SetSoundType(nsSound.EnSoundTypes.enSE);
                        m_itemSlotLotterySE.Be3DSound();
                        m_itemSlotLotterySE.SetLoop(true);
                        m_itemSlotLotterySE.PlayStart(nsSound.SENames.m_itemSlotLottery);
                    }
                }
                //抽選が終わったら
                else
                {
                    if (m_itemSlotSoundPlayFlag)
                    {
                        //アイテム抽選音の終了 
                        m_itemSlotLotterySE.Stop();
                        SoundSource itemSlotLotteryEndSS = new GameObject("SoundSource").AddComponent<nsSound.SoundSource>();
                        itemSlotLotteryEndSS.SetSoundType(nsSound.EnSoundTypes.enSE);
                        itemSlotLotteryEndSS.Be3DSound();
                        itemSlotLotteryEndSS.SetLoop(false);
                        itemSlotLotteryEndSS.PlayStart(nsSound.SENames.m_itemSlotLotteryEnd);

                        //アイテムスロットの音を再生し終わったのでフラグをfalseに。
                        m_itemSlotSoundPlayFlag = false;    
                    }
                }
            }
        }





    }
}