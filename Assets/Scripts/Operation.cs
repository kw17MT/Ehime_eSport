using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// ユーザー操作(左右フリックやタッチ)
/// </summary>
public class Operation : MonoBehaviour
{
    Vector3 m_touchStartPos = Vector3.zero;               //タッチを開始した位置
    Vector3 m_touchEndPos = Vector3.zero;                 //タッチを終えた位置、もしくは現在タッチしている位置（モードによって扱いが違う）
    float m_touchTime = 0.0f;                             //長押しを継続している時間
    private float m_timeFromFirstTouch = 0.0f;            //最初にタッチしてからの時間
    bool m_isTouching = false;                            //現在タッチしているか
    bool m_isLongTouch = false;                           //長押しかどうか
    bool m_isDecideDirWhenLongTouch = false;              //一定時間長押ししている時、その時点での方向を確認したか
    private bool m_isSingleTouch = false;
    [SerializeField] bool m_isWorkEveryFrame = true;      //毎フレームタッチの移動方向を調べるか。ゲームシーンで切り替え可
    //長押し判定が起動する時間
    [SerializeField] float m_longTachJudgmentActivationTime = 1.2f;
    //フリック方向またはタップの情報を渡せる状態かどうか
    bool m_canDataThrow = false;

    const float TOUCH_ACTIVE_TIME = 1.0f;                 //タッチの効力時間
    const float SWIPE_AMOUNT = 100;
    private bool m_isDoubleTouch = false;
    private float m_prevDeltaTime = 0.0f;

	public struct InputInfo
	{
        public string dir;
        public float power;
	};

    private InputInfo m_iputInfo;

	void Start()
    {
        m_iputInfo.dir = "";
        m_iputInfo.power = 0.0f;

    //シーン遷移してもこの操作オブジェクトは削除しない
        DontDestroyOnLoad(this);
    }

    //更新関数
    void Update()
    {
        //タップされたときの処理
        FireMouseButtonDown();

       　//毎フレームタッチの移動量を取得するモードならば
        //if (m_isWorkEveryFrame)
        //{
       　    //タッチしているとき、一番初めのタップ一から現在のタップ位置でフリック方向を決定する
       　   if (m_isTouching)
            {
                NowOperation();
       　        //フリック方向またはタップの情報を渡せる状態にする
                m_canDataThrow = true;
            }
            else
            {
                //フリック方向またはタップの情報を渡せない状態にする
                m_canDataThrow = false;
            }
       // }
       　//タップ開始位置からタップを離したところまでをフリック操作とみなすモードならば
        //else
        //{
       　//    //タップが離された時
       　//   if (Input.GetMouseButtonUp(0))
        //    {
       　//       //タッチのフラグや数値を初期化する
       　//       TachDataInit();

       　//       //フリック方向またはタップの情報を渡せる状態にする
        //        m_canDataThrow = true;
        //        //入力方向と回転の量をリセット
        //        m_iputInfo.dir = "";
        //        m_iputInfo.power = 0.0f;
        //    }
        //   else
        //   {
        //       //フリック方向またはタップの情報を渡せない状態にする
        //       m_canDataThrow = false;
        //   }
        //}

        //タップが離されたとき、
        if (Input.GetMouseButtonUp(0))
        {
            //タッチのフラグや数値を初期化する
            TachDataInit();

            //入力方向と回転の量をリセット
            m_iputInfo.dir = "";
            m_iputInfo.power = 0.0f;
        }

        //長押し関数
        LongTach();

        //Wタップを下かどうかを判断する
        JudgeIsDoubleTouch();

        if (SceneManager.GetActiveScene().name[0..2] == "08" && !GameObject.Find("PauseButton").GetComponent<PauseButton>().GetIsPause())
		{
            m_prevDeltaTime = Time.deltaTime;
		}
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
        //画面下部だったら、
        if (Screen.height / 1.2 <= Input.mousePosition.y)
        {
            return;
        }
            //画面にタッチしている
            m_isTouching = true;
        //タッチしている画面上の座標を取得
        m_touchStartPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z);
    }

    private void JudgeIsDoubleTouch()
	{
        if(m_isDoubleTouch)
		{
            //m_isSingleTouch = false;
            m_isDoubleTouch = false;
		}

        //ボタンを離したら
        if (Input.GetMouseButtonUp(0))
        {
            //操作はタップ操作か
            if (NowOperation() == "touch")
            {
                //過去1秒以内に1回タッチしていたら
                if (m_isSingleTouch)
                {
                    //Wタッチした
                    m_isDoubleTouch = true;
                    Debug.Log("Double Touch");
                    //シングルタッチフラグを消す
                    m_isSingleTouch = false;
                }
                //していなければ
                else
                {
                    //1回目のタッチ
                    m_isSingleTouch = true;
                }
            }
        }
        //1回目のタッチはしていたら
        if (m_isSingleTouch)
        {
            if (SceneManager.GetActiveScene().name[0..2] == "08")
            {
                if (GameObject.Find("PauseButton").GetComponent<PauseButton>().GetIsPause())
                {
                    m_timeFromFirstTouch += m_prevDeltaTime;
                }
                else
                {
                    //タッチしてからの経過時間を測定
                    m_timeFromFirstTouch += Time.deltaTime;
                }
            }
			else
			{
                //タッチしてからの経過時間を測定
                m_timeFromFirstTouch += Time.deltaTime;
            }


            //規定以上たったら
            if (m_timeFromFirstTouch > TOUCH_ACTIVE_TIME)
            {
                //経過時間をリセット
                m_timeFromFirstTouch = 0.0f;
                //シングルタッチ判定を無くす
                m_isSingleTouch = false;
            }
        }
    }

    public bool GetIsDoubleTouch()
    {
        if(m_isDoubleTouch)
		{
            m_isDoubleTouch = false;
            return true;
		}
		else
		{
            return m_isDoubleTouch;
        }
    }
    
    //現在どんな操作をしているか判断する関数
    string NowOperation()
    {
        //タッチを離した時の画面上の位置を取得
        m_touchEndPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z);
        //X,Y方向の移動量を計算
        float directionX = m_touchEndPos.x - m_touchStartPos.x;
        float directionY = m_touchEndPos.y - m_touchStartPos.y;
        string ope = "";

        if (Mathf.Abs(directionY) >= 3 && Mathf.Abs(directionX) >= 3)
        {
            if (Mathf.Abs(directionY) < Mathf.Abs(directionX))
            {
                if (30 < directionX)
                {
                    //右向きにフリック
                    ope = "right";
                }
                else if (-30 > directionX)
                {
                    //左向きにフリック
                    ope = "left";
                }
            }
            else if (Mathf.Abs(directionX) < Mathf.Abs(directionY))
            {
                if (30 < directionY)
                {
                    //上向きにフリック
                    ope = "up";
                }
                else if (-30 > directionY)
                {
                    //下向きのフリック
                    ope = "down";
                }
            }
        }
        else
        {
            //タッチを検出
            ope = "touch";
        }

        return ope;
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

    public InputInfo GetNowOperationAndPower()
	{
        if (!m_canDataThrow)
        {
            m_iputInfo.dir = "";
            m_iputInfo.power = 0.0f;
            return m_iputInfo;
        }

        m_iputInfo.dir = NowOperation();

        if(m_iputInfo.dir == "right"
            || m_iputInfo.dir == "left")
		{
            float directionX = m_touchEndPos.x - m_touchStartPos.x;
            float power = directionX / (Screen.width / 2.0f);
            m_iputInfo.power = power;
        }
		else
		{
            m_iputInfo.power = 0.0f;
		}

        return m_iputInfo;
	}

    //長押し中かどうかを取得するプロパティ
    public bool GetIsLongTouch
    {
        get
        {
            if (m_isLongTouch)
            {
                //シーン遷移の際、長押しし続けるとシーンがスキップされてしまうためリセット
                m_isLongTouch = false;
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    /////////////////////////////////////////////////////////////////////////////////
    //長押し中かどうかを設定するプロパティ
    public void SetIsLongTouch(bool isLongTouch)
    {
        m_isLongTouch = isLongTouch;
    }
    /////////////////////////////////////////////////////////////////////////////////

    //タッチのフラグや数値を初期化する関数
    public void TachDataInit()
    {
        //タッチしていない
        m_isTouching = false;
        //
        m_isDoubleTouch = false;
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

        if (SceneManager.GetActiveScene().name[0..2] == "08")
        {
            if (GameObject.Find("PauseButton").GetComponent<PauseButton>().GetIsPause())
            {
                m_touchTime += m_prevDeltaTime;
            }
            else
            {
                //タッチしている時間をゲームタイムで計測
                m_touchTime += Time.deltaTime;
            }
        }
		else
		{
            //タッチしている時間をゲームタイムで計測
            m_touchTime += Time.deltaTime;
        }

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

    public string GetTouchedScreenDirection()
    {
        if (m_isTouching)
        {
            if (Input.mousePosition.x >= Screen.width / 2.0f)
            {
                return "right";
            }
            else
            {
                return "left";
            }
        }
        else
        {
            return "nothing";
        }
    }

    public void ResetDoubleTapParam()
	{
        m_isSingleTouch = false;
        m_isDoubleTouch = false;
        m_timeFromFirstTouch = TOUCH_ACTIVE_TIME;
    }

    public void SetIsDoubleTouch(bool isDoubleTouch)
    {
        m_isDoubleTouch = isDoubleTouch;
    }
}