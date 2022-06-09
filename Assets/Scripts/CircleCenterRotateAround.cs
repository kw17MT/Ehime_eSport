using UnityEngine;

/// <summary>
/// Transform.RotateAroundを用いた円運動
/// </summary>
public class CircleCenterRotateAround : MonoBehaviour
{
    // 中心点
    [SerializeField]GameObject center;

    // 回転軸(Y軸)
    private Vector3 axis = Vector3.up;

    // 円運動周期
    public float period;

    //周れるかどうか
    bool aroundMoveOn = false;

    //周る時間カウンター
    int aroundCount = 0;

    void Update()
    {
        //画面がタップされたら、
        if (Input.GetButtonDown("Fire1")&&!aroundMoveOn)
        {
            //周れる状態にする
            aroundMoveOn = true;
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
            center.transform.position,          //中心点
            axis,                               //軸
            360 / period * Time.deltaTime       //周期
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