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
        private Vector3 m_listenerDir;

        //このゲーム画面のプレイヤーの情報
        private Transform m_ownPlayerTransform = null;
        private AvatarController m_ownPlayerAvatarController = null;

        //このゲーム画面のプレイヤーのアイテム情報
        private ObtainItemController m_obtainItemController = null;

        //カメラの位置の情報
        private Transform m_mainCameraTransform = null;

        private AvatarController m_myAvatarContoroller = null;

        private SoundSource m_engineSE = null;
        private SoundSource m_acceleSE = null;
        private float m_acceleSEPitch = 0.0f;

        //アイテムスロット回転中のサウンド
        private SoundSource m_itemSlotLotterySE = null;
        //アイテムスロットのサウンドを鳴らすフラグ
        private bool m_itemSlotSoundPlayFlag = false;

        //キラーのSEのソース
        nsSound.SoundSource m_trainWhistleSS = null;
        nsSound.SoundSource m_trainSSRun = null;
        //スターのBGMのソース
        private SoundSource m_starSS = null;

        //アイテムを持っているかどうか
        private bool m_isHavingItem = false;
        //持っているアイテム
        private int m_haveItem = -1;

        //攻撃されたかどうか
        private bool m_isAttacked = false;

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

            Init();

        }

        void Init()
        {
            //このゲームオブジェクトがプレイヤー自信ならば
            if (this.gameObject.name == "OwnPlayer")
            {
                m_obtainItemController = this.gameObject.GetComponent<ObtainItemController>();
                m_ownPlayerTransform = this.gameObject.transform;
                m_ownPlayerAvatarController = this.gameObject.GetComponent<AvatarController>();
                m_myAvatarContoroller = this.gameObject.GetComponent<AvatarController>();
            }
            //プレイヤー以外ならば
            else
            {
                m_ownPlayerTransform = GameObject.Find("OwnPlayer").transform;
                m_ownPlayerAvatarController = GameObject.Find("OwnPlayer").gameObject.GetComponent<AvatarController>();
                m_myAvatarContoroller = this.gameObject.GetComponent<AvatarController>();
            }
            //メインカメラの情報
            m_mainCameraTransform = GameObject.Find("Main Camera").transform;
        }

        // Update is called once per frame
        void Update()
        {
            //プレイヤーがゴールしていたら、終了
            if (m_ownPlayerAvatarController.GetGoaled())
            {
                Destroy(this);
            }

            //3Dサウンドの情報の更新
            Update3DSoundStatus();

            //アクセル音
            AcceleSound();

            TrainSound();
            StarSound();

            //プレイヤー自信のときのみ鳴らす音
            if (this.gameObject.name == "OwnPlayer")
            {
                ItemGetSound();
                ItemUseSound();
                //攻撃された時
                Attacked();

            }
        }

        //3Dサウンドの情報(位置、向き等)を更新
        private void Update3DSoundStatus()
        {
            //プレイヤー自信のとき
            if (this.gameObject.name == "OwnPlayer")
            {
                m_ssPos = m_ownPlayerTransform.position;
                m_listenerPos = m_mainCameraTransform.position + GameObject.Find("Main Camera").transform.forward * 4.5f;
                m_listenerDir = m_mainCameraTransform.forward;
            }
            //その他のプレイヤーの時
            else
            {
                m_ssPos = this.GetComponent<RaceAIScript>().GetRigidBody.position;
                m_listenerPos = m_ownPlayerTransform.position;
                m_listenerDir = m_mainCameraTransform.forward;
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
            if (m_starSS != null)
            {
                m_starSS.Stop();
            }
            BGM.Instance.SetVolume(1.0f);
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

            //3Dサウンドに。
            m_acceleSE.Set3DSourcePos(m_ssPos);
            m_acceleSE.Set3DListenerPos(m_listenerPos);
            m_acceleSE.Set3DListenerDir(m_listenerDir);

            //エンジン音も。
            m_engineSE.Set3DSourcePos(m_ssPos);
            m_engineSE.Set3DListenerPos(m_listenerPos);
            m_engineSE.Set3DListenerDir(m_listenerDir);
        }

        //攻撃された時のSE
        private void Attacked()
        {
            if (m_isAttacked == false)
            {
                if (m_ownPlayerAvatarController.GetIsAttacked())
                {
                    m_isAttacked = true;

                    SoundSource attackedSS = new GameObject("SoundSource").AddComponent<nsSound.SoundSource>();
                    attackedSS.SetSoundType(nsSound.EnSoundTypes.enSE);
                    attackedSS.Be3DSound();
                    attackedSS.PlayStart(nsSound.SENames.m_attacked);

                    //attackedSS.Set3DSourcePos(m_ssPos);
                    //attackedSS.Set3DListenerPos(m_listenerPos);
                    //attackedSS.Set3DListenerDir(m_listenerDir);                 
                }
            }
            else
            {
                m_isAttacked = m_ownPlayerAvatarController.GetIsAttacked();
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

        //アイテムを使うときのサウンド
        private void ItemUseSound()
        {
            //何か持っていなければ
            if (m_isHavingItem == false)
            {
                //今持っているアイテムの種類を取得
                m_haveItem = m_obtainItemController.GetObtainItemType();

                if (m_haveItem != -1)
                {
                    //アイテムを持っているに設定
                    m_isHavingItem = true;
                }
            }
            //アイテムを持っているとき、
            else
            {
                //アイテムが無くなっていたら、使用したと判定
                if (m_obtainItemController.GetObtainItemType() == -1)
                {
                    //使ったアイテムに合わせてサウンドを再生
                    switch (m_haveItem)
                    {
                        case 0:
                            //皮を落とす音の再生
                            nsSound.SoundSource orangePeelSS = new GameObject("SoundSource").AddComponent<nsSound.SoundSource>();
                            orangePeelSS.SetSoundType(nsSound.EnSoundTypes.enSE);
                            orangePeelSS.Be3DSound();
                            orangePeelSS.PlayStart(nsSound.SENames.m_dropOrangePeel);
                            break;
                        case 1:
                            //加速SEの再生
                            nsSound.SoundSource orangeJetSS = new GameObject("SoundSource").AddComponent<nsSound.SoundSource>();
                            orangeJetSS.SetSoundType(nsSound.EnSoundTypes.enSE);
                            orangeJetSS.Be3DSound();
                            orangeJetSS.PlayStart(nsSound.SENames.m_dash);
                            break;
                        case 2:
                            //大砲の音の再生
                            nsSound.SoundSource snapperCannonSS = new GameObject("SoundSource").AddComponent<nsSound.SoundSource>();
                            snapperCannonSS.SetSoundType(nsSound.EnSoundTypes.enSE);
                            snapperCannonSS.Be3DSound();
                            snapperCannonSS.PlayStart(nsSound.SENames.m_cannon);
                            break;
                        case 3:
                            break;
                        case 4:                           
                            break;
                        default:
                            break;

                    }

                    //アイテムを持っていない状態に
                    m_isHavingItem = false;
                    m_haveItem = -1;
                }
            }
        }

        private void TrainSound()
        {
            //キラーの時は特殊判定。使ってもすぐにはアイテムが消えないため。
            if (m_myAvatarContoroller != null && m_myAvatarContoroller.GetIsUsingKiller() == true)
            {
                if (m_trainWhistleSS == null)
                {
                    //列車の音の再生
                    m_trainWhistleSS = new GameObject("SoundSource").AddComponent<nsSound.SoundSource>();
                    m_trainWhistleSS.SetSoundType(nsSound.EnSoundTypes.enSE);
                    m_trainWhistleSS.Be3DSound();
                    m_trainWhistleSS.PlayStart(nsSound.SENames.m_trainWhistle);
                }
                else
                {
                    m_trainWhistleSS.Set3DSourcePos(m_ssPos);
                    m_trainWhistleSS.Set3DListenerPos(m_listenerPos);
                    m_trainWhistleSS.Set3DListenerDir(m_listenerDir);       
                }
                if (m_trainSSRun == null)
                {
                    m_trainSSRun = new GameObject("SoundSource").AddComponent<nsSound.SoundSource>();
                    m_trainSSRun.SetSoundType(nsSound.EnSoundTypes.enSE);
                    m_trainSSRun.Be3DSound();
                    m_trainSSRun.PlayStart(nsSound.SENames.m_trainRun);
                }
                else
                {
                    m_trainSSRun.Set3DSourcePos(m_ssPos);
                    m_trainSSRun.Set3DListenerPos(m_listenerPos);
                    m_trainSSRun.Set3DListenerDir(m_listenerDir);
                }
            }
        }
        private void StarSound()
        {
            //スターの時は特殊判定
            if (m_myAvatarContoroller != null && m_myAvatarContoroller.GetIsUsingStar() == true)
            {
                if (m_starSS == null)
                {
                    if (this.gameObject.name == "OwnPlayer")
                    {
                        BGM.Instance.SetVolume(0.0f);
                    }

                    m_starSS = new GameObject("SoundSource").AddComponent<nsSound.SoundSource>();
                    m_starSS.SetSoundType(nsSound.EnSoundTypes.enBGM);
                    m_starSS.SetLoop(true);
                    m_starSS.PlayStart(nsSound.BGMNames.m_star);
                    m_starSS.Set3DSourcePos(m_ssPos);
                    m_starSS.Set3DListenerPos(m_listenerPos);
                    m_starSS.Set3DListenerDir(m_listenerDir);
                }
                else
                {
                    m_starSS.Set3DSourcePos(m_ssPos);
                    m_starSS.Set3DListenerPos(m_listenerPos);
                    m_starSS.Set3DListenerDir(m_listenerDir);
                }
            }
            else
            {        
                if (m_starSS != null)
                {
                    m_starSS.Stop();
                    if (this.gameObject.name == "OwnPlayer")
                    {
                        BGM.Instance.SetVolume(1.0f);
                    }
                }
            }


		}

        // 物体がコリジョン接触したとき、１度だけ呼ばれる
        private void OnCollisionEnter(Collision collision)
        {
            if (this.gameObject.name != "OwnPlayer")
            {
                return;
            }
            //接触したコリジョンのタグを取得する。
            string triggerName = collision.gameObject.tag;

            //衝突点
            Vector3 contactPoint = Vector3.zero;

            //壁に衝突していない判定の状態で壁に衝突したら
            if (/*!m_hitWall && */triggerName == "Wall")
            {
                //衝突点を取得する。
                foreach (ContactPoint contact in collision.contacts)
                {
                    contactPoint = new Vector3(contact.point.x, contact.point.y, contact.point.z);
                }

                //自身から衝突点に向かって伸びるベクトルを計算。
                Vector3 contactVec = contactPoint - this.gameObject.transform.position;

                //y成分を削除。
                contactVec.y = 0.0f;

                //正規化。
                contactVec.Normalize();

                //外積を求める。
                Vector3 dot = Vector3.Cross(contactVec, this.gameObject.transform.forward);

                //衝突音
                SoundSource hitWallSS = new GameObject("SoundSource").AddComponent<nsSound.SoundSource>();
                hitWallSS.SetSoundType(nsSound.EnSoundTypes.enSE);
                hitWallSS.Be3DSound();
                hitWallSS.PlayStart(nsSound.SENames.m_hitWall);
                hitWallSS.Set3DListenerPos(m_listenerPos);
                hitWallSS.Set3DListenerDir(m_listenerDir);

                Vector3 axisY = new Vector3(0.0f, 1.0f, 0.0f);
                Vector3 rightDir = Vector3.Cross(m_mainCameraTransform.forward, axisY);

                //外積から壁が右にあるのか左にあるのか判断する。
                //右
                if (dot.y < 0.0)
                {
                    hitWallSS.Set3DSourcePos(m_listenerPos + rightDir * 2.0f);
                }
                //左
                else
                {
                    hitWallSS.Set3DSourcePos(m_listenerPos + rightDir * -2.0f);
                }
            }
        }
    }
}