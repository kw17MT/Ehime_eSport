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
    public float period = 0.0f;

    //周れるかどうか
    bool aroundMoveOn = false;

    //周る時間カウンター
    int aroundCount = 0;

    //時計回りか反時計回りか
    int m_reverse = 1;

    //操作システム
    OperationNew m_operation = null;

    void Start()
    {
        //操作システムのゲームオブジェクトを検索しスクリプトを使用する
        m_operation = GameObject.Find("OperationSystem").GetComponent<OperationNew>();
    }

    void Update()
    {
        if (!aroundMoveOn)
        {
            //画面が右フリックされたら、
            if (m_operation.GetNowOperation() == "right")
            {
                //周れる状態にする
                aroundMoveOn = true;
                //時計回り
                m_reverse = 1;
            }
            //画面が左フリックされたら、
            if (m_operation.GetNowOperation() == "left")
            {
                //周れる状態にする
                aroundMoveOn = true;
                //時計回り
                m_reverse = -1;
            }
        }


        //周る処理を実行
        GoAround();
    }

    //周る処理関数
    void GoAround()
    {
        //周ない状態のときは処理をしない。
        if (!aroundMoveOn) return;

        // 中心点centerの周りを、軸axisで、period周期で円運動
        transform.RotateAround(
            m_center.transform.position,                     //中心点
            m_axis,                                          //軸
            360 / period * Time.deltaTime * m_reverse      //周期
        );

        //カウント計測
        aroundCount++;
        //カウントが指定した数値より大きくなったら、
        if (aroundCount > 100)
        {
            //周れない状態に戻す
            aroundMoveOn = false;
            //カウントの初期化
            aroundCount = 0;
        }
    }
}