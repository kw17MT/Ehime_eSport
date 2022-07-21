using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LimitTime : MonoBehaviour
{
    //制限時間ラベル
    [SerializeField] Text limitTimeLabel = null;
    //制限時間の背景画像
    [SerializeField] Image limitTimeBackImage1 = null;
    [SerializeField] Image limitTimeBackImage2 = null;
    //制限時間値(変化させていく)
    [SerializeField] float limitTimeValue = 0.0f;
    //最大制限時間値
    float maxLimitTimeValue = 0.0f;


    void Start()
    {
        //制限時間の計測の開始秒数を設定
        maxLimitTimeValue = limitTimeValue;
    }

    public void SetLimitTime(int time)
	{
        limitTimeValue = time;
	}

    void Update()
    {
        //制限時間が0になるまで制限時間カウント実行
        if (limitTimeValue >= 0)
        {
            //limitTimeValue -= Time.deltaTime;
        }
        //制限時間が来たら、
        else
        {
            //ゲームシーンに遷移
            //SceneManager.LoadScene("08_GameScene");
        }

        //制限時間の背景画像のゲージを減少させていく
        limitTimeBackImage2.fillAmount += Time.deltaTime / maxLimitTimeValue;

        //制限時間が終了わずかになったら色を変化させる
        if (limitTimeValue<=6)
        {
            //赤色
            limitTimeBackImage1.color = Color.red;
        }

        //制限時間ラベルを更新
        //limitTimeLabel.text = "" + (int)limitTimeValue;
    }
}
