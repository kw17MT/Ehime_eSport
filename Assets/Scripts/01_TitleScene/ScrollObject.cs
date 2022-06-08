using System.Collections;
using UnityEngine;

public class ScrollObject : MonoBehaviour
{
    public float speed;      //速度
    public float startPosition;     //開始位置
    public float endPosition;       //終了位置

    //更新関数
    void Update()
    {
        //毎フレームxポジションを少しずつ移動させる
        transform.Translate(speed * Time.deltaTime, 0, 0);

        //スクロールが目標ポイントまで到達したかをチェック
        if(transform.position.x >= endPosition)ScrollEnd();
    }

    void ScrollEnd()
    {
        //スクロールする距離分を戻す
        transform.Translate(-1 * (endPosition - startPosition), 0, 0);

        //同じゲームオブジェクトにアタッチされているコンポーネントにメッセージを送る
        SendMessage("OnScrollEnd", SendMessageOptions.DontRequireReceiver);
    }
}