using UnityEngine;

/// <summary>
/// Transform.RotateAroundを用いた円運動
/// </summary>
public class CircleCenterRotateAround : MonoBehaviour
{
    // 中心点
    [SerializeField]GameObject m_center = null;
    // 回転軸(Y軸)
    Vector3 m_axis = Vector3.up;
    // 円運動周期
    public float m_period = 0.0f;
    //周れるかどうか
    bool m_aroundMoveOn = false;
    //周る時間カウンター
    int m_aroundCount = 0;
    //時計回りか反時計回りか
    int m_reverse = 1;
    //操作システム
    Operation m_operation = null;
    //タイマーの稼働時間
    [SerializeField]int m_countTime = 0;

    void Start()
    {
        //操作システムのゲームオブジェクトを検索しゲームコンポーネントを取得する
        m_operation = GameObject.Find("OperationSystem").GetComponent<Operation>();
    }

    void Update()
    {
        //ユーザー操作によって処理を実行
        UserOperation();

        //周る処理を実行
        GoAround();
    }

    //ユーザー操作によって処理を実行する関数
    void UserOperation()
    {
        if (m_aroundMoveOn)
        {
            return;
        }
        //画面が右フリックされたら、
        if (m_operation.GetNowOperation() == "right")
        {
            //周れる状態にする
            m_aroundMoveOn = true;
            //時計回り
            m_reverse = 1;
        }
        //画面が左フリックされたら、
        if (m_operation.GetNowOperation() == "left")
        {
            //周れる状態にする
            m_aroundMoveOn = true;
            //時計回り
            m_reverse = -1;
        }
    }

    //周る処理関数
    void GoAround()
    {
        //周ない状態のときは処理をしない。
        if (!m_aroundMoveOn) return;

        // 中心点centerの周りを、軸axisで、period周期で円運動
        transform.RotateAround(
            m_center.transform.position,                     //中心点
            m_axis,                                          //軸
            360 / m_period * Time.deltaTime * m_reverse      //周期
        );

        //カウント計測
        m_aroundCount++;

        //カウントが指定した数値より大きくなったら、
        if (m_aroundCount > m_countTime)
        {
            //周れない状態に戻す
            m_aroundMoveOn = false;
            //カウントの初期化
            m_aroundCount = 0;
        }
    }

    //タイマーの稼働時間を取得するプロパティ
    public int GetCountTime
    {
        //ゲッター
        get
        {
            return m_countTime;
        }
    }
}