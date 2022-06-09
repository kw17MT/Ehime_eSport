using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LimitTime : MonoBehaviour
{
    //制限時間ラベル
    [SerializeField] Text limitTimeLabel;

    //制限時間の背景画像
    [SerializeField] Image limitTimeBackImage1;
    [SerializeField] Image limitTimeBackImage2;

    //制限時間値(変化させていく)
    [SerializeField] float limitTimeValue;
    //最大制限時間値
    float maxLimitTimeValue;


    void Start()
    {
        maxLimitTimeValue = limitTimeValue;
    }

    void Update()
    {
        //制限時間が0になるまで制限時間カウント実行
        if (limitTimeValue >= 0)
        {
            limitTimeValue -= Time.deltaTime;
        }

        //制限時間の背景画像のゲージを減少させていく
        limitTimeBackImage2.fillAmount += Time.deltaTime / maxLimitTimeValue;

        //制限時間が終了わずかになったら色を変化させる
        if(limitTimeValue<=6)
        {
            limitTimeBackImage1.color = Color.red;
        }

        //制限時間ラベルを更新
        limitTimeLabel.text = "" + (int)limitTimeValue;
    }
}
