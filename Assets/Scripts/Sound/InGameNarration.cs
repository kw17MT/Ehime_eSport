using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//トリガーボックスに付随しているタグ名と照らし合わせる定数string
public class SoundTriggerTagName
{
    public const string m_rightCurve = "RightCurve";    //右カーブ前
    public const string m_leftCurve = "LeftCurve";      //左カーブ前
    public const string m_item = "ItemNarration";       //アイテム前

    public const string m_wall = "Wall";                //壁
}

//レース中のナレーションのサウンドを管理するクラス
public class InGameNarration : MonoBehaviour
{

    //様々な判定に使う方向（左右の壁、アイテムの位置）
    public enum EnDirection
    {
        enNone,     //なし
        enRight,    //右
        enLeft      //左
    }
    private bool m_isOwnPlayer = false;                             //プレイヤー自身か？
    private nsSound.SoundSource m_soundSource = null;               //ナレーションのサウンドソース、重ねて再生したくないサウンドを扱うときに使用できるかも
    private ObtainItemController m_obtainItemController = null;     //プレイヤーのアイテム情報
    private bool m_getItemSoundPlayFlag = false;                    //アイテム取得時のナレーションが再生された？
    private float m_hitWallTimer = 0.0f;                            //壁に当たってから次に注意されるまでのタイマー
    private bool m_hitWall = false;                                 //壁に当たった？

    //定数
    private float m_hitWallTimerConst = 3.0f;

    //ObtainItemControllerのEnItemTypeがprivateでアクセスが難しかったのでとりあえず同じものを用意
    //後でうまい具合に判定できるものを用意すること。
    enum EnItemType
    {
        enNothing = -1,
        enOrangePeel,                                   //オレンジの皮
        enOrangeJet,                                    //オレンジジュースジェット
        enSnapperCannon,                                //タイ砲
        enTrain,                                        //坊ちゃん列車キラー
        enStar,                                         //スター
        enItemTypeNum                                   //アイテムの種類の数
    }


    //初期化関数。
    void Start()
    {
        //プレイヤー自身かどうか判定。
        m_isOwnPlayer = (this.gameObject.name == "OwnPlayer");

        if (m_isOwnPlayer)
        {
            //アイテム情報を管理するコンポーネントを取得。
            m_obtainItemController = this.gameObject.GetComponent<ObtainItemController>();
        }
    }

    //更新関数。
    void Update()
    {
        //プレイヤー自身じゃないなら早期リターン。
        if (!m_isOwnPlayer) { return; }

        //アイテム獲得周りのナレーションの関数を実行。
        ExecuteItemNarration();

        //壁衝突タイマーを更新。
        UpdateHitWallTimer();
    }

    //物体がトリガーに接触したとき、１度だけ呼ばれる
    private void OnTriggerEnter(Collider collision)
    {
        //接触したトリガーのタグを取得する。
        string triggerName = collision.gameObject.tag;

        //タグの名前からどの音源を再生するか判定して再生。
        PlayNarration(triggerName, EnDirection.enNone);
    }

    // 物体がコリジョン接触したとき、１度だけ呼ばれる
    private void OnCollisionEnter(Collision collision)
    {
        //接触したコリジョンのタグを取得する。
        string triggerName = collision.gameObject.tag;

        //衝突点
        Vector3 contactPoint = Vector3.zero;
        //衝突方向
        EnDirection direction = EnDirection.enNone;

        //壁に衝突していない判定の状態で壁に衝突したら
        if (!m_hitWall && triggerName == "Wall")
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

            //外積から壁が右にあるのか左にあるのか判断する。
            if (dot.y < 0.0)
            {
                direction = EnDirection.enRight;
            }
            else
            {
                direction = EnDirection.enLeft;
            }
        }
        //タグの名前と衝突方向からどの音源を再生するか判定して再生。
        PlayNarration(triggerName, direction);
    }

    //タグの名前から音源を再生する関数。
    private void PlayNarration(string triggerName, EnDirection direction){

        nsSound.SoundSource ss = new GameObject("SoundSource").AddComponent<nsSound.SoundSource>();
        ss.SetSoundType(nsSound.EnSoundTypes.enNarration);
        ss.Be3DSound();

        //右カーブ前
        if (triggerName == SoundTriggerTagName.m_rightCurve)
        {
            ss.PlayStart(nsSound.NarInGameNames.m_MIGICURVEGAARUYO);
        }

        //左カーブ前
        if (triggerName == SoundTriggerTagName.m_leftCurve)
        {
            ss.PlayStart(nsSound.NarInGameNames.m_HIDARICURVEGAARUYO);
        }

        //アイテム前
        if (triggerName == SoundTriggerTagName.m_item)
        {
            ss.PlayStart(nsSound.NarInGameNames.m_ITEMGAARUYO);
        }

        //壁
        if (triggerName == SoundTriggerTagName.m_wall)
        {
            if (!m_hitWall)
            {
                //右側に当たっている。
                if (direction == EnDirection.enRight)
                {
                    ss.PlayStart(nsSound.NarInGameNames.m_MIGINOKABENIATATTEIRUYO);
                }
                //左側に当たっている。
                else if(direction == EnDirection.enLeft)
                {
                    ss.PlayStart(nsSound.NarInGameNames.m_HIDARINOKABENIATATTEIRUYO);
                }
                //壁に衝突した。
                m_hitWall = true;
                //タイマーをセット。
                m_hitWallTimer = m_hitWallTimerConst;
            }
        }
    }

    //アイテム番号から音源を再生する関数。
    private void PlayNarration(EnItemType itemType)
    {
        //サウンドソースを生成。
        nsSound.SoundSource itemSS = new GameObject("SoundSource").AddComponent<nsSound.SoundSource>();
        itemSS.SetSoundType(nsSound.EnSoundTypes.enNarration);
        itemSS.Be3DSound();

        //アイテム番号から対応する音源を再生。
        switch (itemType) 
        {
            //オレンジの皮
            case EnItemType.enOrangePeel:
                itemSS.PlayStart(nsSound.NarInGameItemNames.m_MIKANNNOKAWADA);
                break;

            //オレンジジュースジェット
            case EnItemType.enOrangeJet:
                itemSS.PlayStart(nsSound.NarInGameItemNames.m_JUICEJETDA);
                break;

            //タイ砲
            case EnItemType.enSnapperCannon:
                itemSS.PlayStart(nsSound.NarInGameItemNames.m_TAIHOUDA);
                break;

            //坊ちゃん列車キラー
            case EnItemType.enTrain:
                itemSS.PlayStart(nsSound.NarInGameItemNames.m_BOCCHANRESSYADA);
                break;

            //スター
            case EnItemType.enStar:
                itemSS.PlayStart(nsSound.NarInGameItemNames.m_OUGONNMIKANNDA);
                break;
        }
    }

    private void ExecuteItemNarration()
    {
        //アイテムを入手したら
        if (m_obtainItemController.GetObtainItemType() != -1)
        {
            //アイテムスロットの抽選中なら(抽選が終わってないなら)
            if (m_obtainItemController.GetLotteryFinish() == false)
            {
                //アイテム取得時のナレーションのフラグを立てる。
                m_getItemSoundPlayFlag = true;
            }
            else if (m_getItemSoundPlayFlag)
            {
                //取得したアイテムが何か調べる。
                EnItemType itemType = (EnItemType)m_obtainItemController.GetObtainItemType();

                //対応する音源を再生する。
                PlayNarration(itemType);

                //一回再生したので終了。
                m_getItemSoundPlayFlag = false;
            }
        }
    }

    //壁衝突タイマーを更新する関数。
    private void UpdateHitWallTimer()
    {
        if (m_hitWallTimer > 0.0f)
        {
            m_hitWallTimer -= Time.deltaTime;
            if (m_hitWallTimer <= 0.0f)
            {
                m_hitWallTimer = 0.0f;

                //判定を初期化。
                m_hitWall = false;
            }
        }
    }
}
