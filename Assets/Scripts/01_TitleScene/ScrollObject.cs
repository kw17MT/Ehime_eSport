using System.Collections;
using UnityEngine;

public class ScrollObject : MonoBehaviour
{
    [SerializeField] float speed = 0.0f;             //速度
    [SerializeField] float startPosition = 0.0f;     //開始位置
    [SerializeField] float endPosition = 0.0f;       //終了位置

    //更新関数
    void Update()
    {
        //毎フレームxポジションを少しずつ移動させる
        transform.Translate(speed * Time.deltaTime, 0, 0);

        //スクロールが目標ポイントまで到達したかをチェック
        if (transform.position.x >= endPosition)
        {
            ScrollEnd();
        }
    }

    void ScrollEnd()
    {
        //スクロールする距離分を戻す
        transform.Translate(-1 * (endPosition - startPosition), 0, 0);

        //同じゲームオブジェクトにアタッチされているコンポーネントにメッセージを送る
        SendMessage("OnScrollEnd", SendMessageOptions.DontRequireReceiver);
    }
}