using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ユーザー操作(左右フリックやタッチ)
/// </summary>
public class OperationNew : MonoBehaviour
{
    Vector3 m_touchStartPos = Vector3.zero;               //タッチを開始した位置
    Vector3 m_touchEndPos = Vector3.zero;                 //タッチを終えた位置、もしくは現在タッチしている位置（モードによって扱いが違う）
    float m_touchTime = 0.0f;                             //長押しを継続している時間
    bool m_isTouching = false;                            //現在タッチしているか
    bool m_isLongTouch = false;                           //長押しかどうか
    bool m_isDecideDirWhenLongTouch = false;              //一定時間長押ししている時、その時点での方向を確認したか
    [SerializeField] bool m_isWorkEveryFrame = true;      //毎フレームタッチの移動方向を調べるか。ゲームシーンで切り替え可
    //長押し判定が起動する時間
    [SerializeField] float m_longTachJudgmentActivationTime = 1.2f;
    //フリック方向またはタップの情報を渡せる状態かどうか
    bool m_canDataThrow = false;

    void Start()
    {
        //シーン遷移してもこの操作オブジェクトは削除しない
        DontDestroyOnLoad(this);
    }

    //更新関数
    void Update()
    {
        //タップされたときの処理
        FireMouseButtonDown();

       　//毎フレームタッチの移動量を取得するモードならば
        if (m_isWorkEveryFrame)
        {
       　    //タッチしているとき、一番初めのタップ一から現在のタップ位置でフリック方向を決定する
       　   if (m_isTouching)
            {
       　        //フリック方向またはタップの情報を渡せる状態にする
                m_canDataThrow = true;
            }
            else
            {
                //フリック方向またはタップの情報を渡せない状態にする
                m_canDataThrow = false;
            }
        }
       　//タップ開始位置からタップを離したところまでをフリック操作とみなすモードならば
        else
        {
       　    //タップが離された時
       　   if (Input.GetMouseButtonUp(0))
            {
       　       //タッチのフラグや数値を初期化する
       　       TachDataInit();

       　       //フリック方向またはタップの情報を渡せる状態にする
                m_canDataThrow = true;
            }
           else
            {
                //フリック方向またはタップの情報を渡せない状態にする
                m_canDataThrow = false;
            }
        }

        //タップが離されたとき、
        if (Input.GetMouseButtonUp(0))
        {
            //タッチのフラグや数値を初期化する
            TachDataInit();
        }

        //長押し関数
        LongTach();
    }

    //タップされたときの処理関数
    void FireMouseButtonDown()
    {
        //タップされた時（左クリック）
        //引数０...左１...右...３...中
        if (!Input.GetMouseButtonDown(0))
        {
            return;
        }
        //画面にタッチしている
        m_isTouching = true;
        //タッチしている画面上の座標を取得
        m_touchStartPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z);
    }

    //現在どんな操作をしているか判断する関数
    string NowOperation()
    {
        //タッチを離した時の画面上の位置を取得
        m_touchEndPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z);
        //X,Y方向の移動量を計算
        float directionX = m_touchEndPos.x - m_touchStartPos.x;
        float directionY = m_touchEndPos.y - m_touchStartPos.y;

        if (Mathf.Abs(directionY) < Mathf.Abs(directionX))
        {
            if (30 < directionX)
            {
                //右向きにフリック
                return "right";
            }
            else if (-30 > directionX)
            {
                //左向きにフリック
                return "left";
            }
        }
        else if (Mathf.Abs(directionX) < Mathf.Abs(directionY))
        {
            if (30 < directionY)
            {
                //上向きにフリック
                return "up";
            }
            else if (-30 > directionY)
            {
                //下向きのフリック
                return "down";
            }
        }
        else
        {
            //タッチを検出
            return "touch";
        }

        return "";
    }

    //現在どんな操作がされているかを取得する関数
    public string GetNowOperation()
    {
        //フリック方向またはタップの情報を渡せる状態
        //じゃないときはデータを渡さない。
        if (!m_canDataThrow)
        {
            return "";
        }

        return NowOperation();
    }

    //長押し中かどうかを取得するゲッター
    public bool GetIsLongTouch()
    {
        return m_isLongTouch;
    }

    //タッチのフラグや数値を初期化する関数
    public void TachDataInit()
    {
        //タッチしていない
        m_isTouching = false;
        //長押ししていない
        m_isLongTouch = false;
        //一定時間以上長押ししていない
        m_isDecideDirWhenLongTouch = false;
        //タッチしている時間をリセット
        m_touchTime = 0.0f;
    }

    //長押し関数
    void LongTach()
    {
        //タッチしているときのみ処理を行う。
        if (!m_isTouching)
        {
            return;
        }

        //タッチしている時間をゲームタイムで計測
        m_touchTime += Time.deltaTime;

        //ある程度長押しするまでは長押しチェックを行わない。
        if (m_touchTime <= m_longTachJudgmentActivationTime)
        {
            return;
        }

        //長押し時のフリック方向をすでに確認していたら長押しチェックを行わない。
        if(m_isDecideDirWhenLongTouch)
        {
            return;
        }

        //長押し時のフリック方向確認した
        m_isDecideDirWhenLongTouch = true;
        //スライドしていないか確かめる。
        //スライドしていなかったら、
        //長押し時間読み上げ開始
        m_isLongTouch = NowOperation() == "touch" ? true : false;
    }
}