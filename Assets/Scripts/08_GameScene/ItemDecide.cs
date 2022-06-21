using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemDecide : MonoBehaviour
{
    //全てのアイテムスプライトの配列
    [SerializeField] Sprite[] itemSprite = null;
    //アイテム画像
    [SerializeField]Image itemImage = null;
    //ステート
    enum EnItemState
    {
        enNothingState,  //アイテムを持っていない状態
        enLotteryState,  //抽選状態
        enBlinkingState  //抽選終了した後の点滅状態
    }
    EnItemState itemState = EnItemState.enNothingState;
    //点滅の回数
    [SerializeField] int blinkingNumberOfTimes = 0;
    //点滅の表示時間
    [SerializeField] float blinkingDelayTime = 0.0f;
    //点滅処理が終了したかどうか
    bool isBlinkingFinish = false;
    //抽選回数
    [SerializeField] int lotteryNumberOfTimes = 0;
    //抽選の表示時間
    [SerializeField] float lotteryDelayTime = 0.0f;
    //抽選処理が終了したかどうか
    bool isLotteryFinish = false;
    //表示するアイテムスプライトの配列番号
    int itemSpriteActiveArrayNum = 0;
    //前回表示したアイテムスプライトの配列番号
    int itemSpriteBeforeActiveArrayNum = 0;

    void Start()
    {
        //アイテムのデータを初期化
        ItemInit();
    }

    void Update()
    {
        //状態によって処理を分岐
        switch (itemState)
        {
            //アイテムを持っていない状態
            case EnItemState.enNothingState:
                //アイテムを獲得したら、
                if (Input.GetButtonDown("Fire1"))
                {
                    //抽選状態に移行
                    itemState = EnItemState.enLotteryState;
                    //アイテム画像を表示
                    itemImage.color = new Color(itemImage.color.r, itemImage.color.g, itemImage.color.b, 1.0f);
                    //抽選処理のコルーチンを開始
                    StartCoroutine("LotteryCoroutine");
                }
                break;

            //抽選状態
            case EnItemState.enLotteryState:
                //抽選が終了したら、
                if (isLotteryFinish)
                {
                    //点滅状態に移行
                    itemState = EnItemState.enBlinkingState;
                    //点滅処理のコルーチンを開始
                    StartCoroutine("BlinkingCoroutine");
                    //抽選が終了したかどうかの判定を初期状態に戻す
                    isLotteryFinish = false;
                }
                break;

            //決定した後の点滅状態
            case EnItemState.enBlinkingState:
                //アイテムが使われたら
                if (Input.GetButtonDown("Fire1"))
                {
                    //点滅処理のコルーチンが終了していなかったら
                    if (!isBlinkingFinish)
                    {
                        //点滅処理のコルーチンを止める
                        StopCoroutine("BlinkingCoroutine");
                    }
                    //アイテムのデータを初期化
                    ItemInit();
                }
                break;
        }
    }

    //抽選処理
    IEnumerator LotteryCoroutine()
    {
        //lotteryNumberOfTimesの回数分抽選
        for (int lotteryNum = 0; lotteryNum < lotteryNumberOfTimes; lotteryNum++)
        {
            //do-while文を使って前回表示したアイテムスプライトと同じ画像を連続で表示しないようにする
            do
            {
                //今あるアイテムの中からランダムで表示
                itemSpriteActiveArrayNum = Random.Range(0, itemSprite.Length);
            }while (itemSpriteActiveArrayNum == itemSpriteBeforeActiveArrayNum);
            //ImageにSpriteを設定
            itemImage.sprite = itemSprite[itemSpriteActiveArrayNum];

            //待機
            yield return new WaitForSeconds(blinkingDelayTime);

            //前回表示したアイテムスプライトの配列番号を更新
            itemSpriteBeforeActiveArrayNum = itemSpriteActiveArrayNum;
        }

        //抽選処理のコルーチンが終了したということを返す
        isLotteryFinish = true;
    }

    //点滅処理
    IEnumerator BlinkingCoroutine()
    {
        //blinkingNumberOfTimesの回数分点滅する
        for (int blinkingNum = 0; blinkingNum < blinkingNumberOfTimes; blinkingNum++)
        {
            //表示されているとき
            if(itemImage.color.a == 1.0f)
            {
                //アイテム画像を非表示
                itemImage.color = new Color(itemImage.color.r , itemImage.color.g , itemImage.color.b ,0.0f);
            }
            //非表示されているとき
            else
            {
                //アイテム画像を表示
                itemImage.color = new Color(itemImage.color.r, itemImage.color.g, itemImage.color.b, 1.0f);
            }

            //待機
            yield return new WaitForSeconds(blinkingDelayTime);
        }

        //点滅処理のコルーチンが終了したということを返す
        isBlinkingFinish = true;
    }

    //初期化関数
    void ItemInit()
    {
        //アイテムを持っていない状態に設定
        itemState = EnItemState.enNothingState;
        //アイテム画像を非表示
        itemImage.color = new Color(itemImage.color.r, itemImage.color.g, itemImage.color.b, 0.0f);
        //点滅処理は終了していない判定に設定しておく
        isBlinkingFinish = false;
    }
}
