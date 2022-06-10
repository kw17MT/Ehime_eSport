using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Operation : MonoBehaviour
{
    private Vector3 touchStartPos = Vector3.zero;               //タッチを開始した位置
    private Vector3 touchEndPos = Vector3.zero;                 //タッチを終えた位置、もしくは現在タッチしている位置（モードによって扱いが違う）
    private string direction = "";                              //フリックしている方向
    private float touchTime = 0.0f;                             //長押しを継続している時間
    private bool isTouching = false;                            //現在タッチしているか
    private bool isLongTouch = false;                           //長押しかどうか
    private bool isDecideDirWhenLongTouch = false;              //一定時間長押ししている時、その時点での方向を確認したか
    private GameObject rotateObject = null;                     //回転させるゲームオブジェクト

    public bool isWorkEveryFrame = false;                       //毎フレームタッチの移動方向を調べるか。ゲームシーンで切り替え可

    void Start()
    {
        //回転の対象となるゲームオブジェクトを名前で検索する
        rotateObject = GameObject.Find("Cube");
    }

    //どの方向にフリックしたか判断する関数
    void DecideDirection()
	{
        //タッチを離した時の画面上の位置を取得
        touchEndPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z);
        //X,Y方向の移動量を計算
        float directionX = touchEndPos.x - touchStartPos.x;
        float directionY = touchEndPos.y - touchStartPos.y;

        if (Mathf.Abs(directionY) < Mathf.Abs(directionX))
        {
            if (30 < directionX)
            {
                //右向きにフリック
                direction = "right";
            }
            else if (-30 > directionX)
            {
                //左向きにフリック
                direction = "left";
            }
        }
        else if (Mathf.Abs(directionX) < Mathf.Abs(directionY))
        {
            if (30 < directionY)
            {
                //上向きにフリック
                direction = "up";
            }
            else if (-30 > directionY)
            {
                //下向きのフリック
                direction = "down";
            }
        }
        else
        {
            //タッチを検出
            direction = "touch";
        }
    }

    //長押しかどうかを取得するゲッター
    public bool GetIsLongTouch()
	{
        return isLongTouch;
	}

    //更新関数
    void Update()
    {
        //タップされた時（左クリック）
        //引数０...左１...右...３...中
        if (Input.GetMouseButtonDown(0))
        {
            //画面にタッチしている
            isTouching = true;
            //タッチしている画面上の座標を取得
            touchStartPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z);
        }

        if (rotateObject != null)
        {


            //毎フレームタッチの移動量を取得するモードならば
            if (isWorkEveryFrame)
            {
                //タッチしているとき、一番初めのタップ一から現在のタップ位置でフリック方向を決定する
                if (isTouching)
                {
                    //前フレームのタッチ位置と現在のフレームのタッチ位置が一緒だったらオブジェクトを回転させない
                    // if (touchEndPos.magnitude != new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z).magnitude)
                    //{
                    //最終地点（現在のタップ位置）を更新してフリック方向を決定する
                    DecideDirection();
                    //回転対象のオブジェクトにフリック方向を与える
                    rotateObject.GetComponent<ObjectRotation>().Rotate(direction);
                    // }
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
                    //どの方向にフリックしたか判断する
                    DecideDirection();
                    //オブジェクトをフリックした方向に回転させる
                    rotateObject.GetComponent<ObjectRotation>().Rotate(direction);
                }
            }

        }

        //タップが離されたとき、
        if (Input.GetMouseButtonUp(0))
        {
            //タッチのフラグや数値を初期化する
            TachDataInit();
        }

        if (isTouching)
		{
            //タッチしている時間をゲームタイムで計測
            touchTime += Time.deltaTime;

            //ある程度長押ししたらスライドしていないか確かめる
            if (touchTime >= 1.2f && !isDecideDirWhenLongTouch)
            {
                //長押し時のフリック方向確認した
                isDecideDirWhenLongTouch = true;
                //どの方向にフリックしたか判断する
                DecideDirection();
                //タッチしていたら
                if (direction == "touch")
                {
                    //長押し時間読み上げ開始
                    isLongTouch = true;
                }
            }

            if(isLongTouch)
			{
                //長押し中のデバック表記
                //Debug.Log("counting! " + touchTime);
            }
        }
    }

    //タッチのフラグや数値を初期化する関数
    void TachDataInit()
    {
        //タッチしていない
        isTouching = false;
        //長押ししていない
        isLongTouch = false;
        //一定時間以上長押ししていない
        isDecideDirWhenLongTouch = false;
        //タッチしている時間をリセット
        touchTime = 0.0f;
    }
}