using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// ステージ選択画面クラス
/// </summary>
public class StageSelectChange : MonoBehaviour
{
    //ステージ名
    [SerializeField] string[] m_stageName = null;
    //ステージ名ラベル
    [SerializeField] Text m_stageNameLabel = null;
    //難易度スターゲームオブジェクト
    [SerializeField] GameObject[] m_difficlutyStar = null;
    //ステージごとの難易度(0,簡単。1,普通。2,難しい)
    [SerializeField]int[] m_stageDifficluty = { 0, 1, 2 };
    //ステージ説明文
    [SerializeField] string[] m_stageExplanationSentence = null;
    //ステージ説明文ラベル
    [SerializeField] Text m_stageExplanationLabel = null;

    enum EnStageType
    {
        enStage1,       //ステージ1
        enStage2,       //ステージ2
        enStage3,       //ステージ3
        enMaxStageNum   //最大ステージ数
    }
    //現在選択されているステージ
    EnStageType m_nowSelectStage = EnStageType.enStage1;
    //操作システム
    Operation m_operation = null;
    //選択移動をしているか
    bool m_selectMove = false;
    //移動時間カウンター
    int m_selectMoveCount = 0;

    CircleCenterRotateAround m_circleCenterRotateAround = null;

    //ユーザーが設定した情報を格納して置く保管場所
    UserSettingData m_userSettingData = null;

    void Start()
    {
        //操作システムのゲームオブジェクトを検索しゲームコンポーネントを取得する
        m_operation = GameObject.Find("OperationSystem").GetComponent<Operation>();
        //円の中心を電車が回転する機能付きのゲームオブジェクトを検索しゲームコンポーネントを取得する
        m_circleCenterRotateAround = GameObject.Find("Train").GetComponent<CircleCenterRotateAround>();
    }

    //アップデート関数
    void Update()
    {
        //選択するステージを移動する
        ChangeSelectStage();

        //画面が長押しされたら、
        if (m_operation.GetIsLongTouch)
        {
            //次のシーンに遷移させる
            GoNextScene();
        }

        //電車の移動に合わせて選択しているデータを合わせるカウンター
        Count();

        //ステージ選択シーンのテキストなどのデータを更新
        StageSceneDataUpdate();
    }

    //選択するステージを移動する関数
    void ChangeSelectStage()
    {
        if (m_selectMove)
        {
            return;
        }
        //画面が右フリックされたら、
        if (m_operation.GetNowOperation() == "right")
        {
            //次のステージに選択を移動
            GoNextStage();
        }
        //画面が左フリックされたら、
        if (m_operation.GetNowOperation() == "left")
        {
            //前のステージに選択を移動
            GoBackStage();
        }
    }

    //次のステージに選択を移動する関数
    void GoNextStage()
    {
        //選択移動状態にする
        m_selectMove = true;
        //選択されているステージを次のステージにする
        m_nowSelectStage++;
        if (m_nowSelectStage >= EnStageType.enMaxStageNum)
        {
            m_nowSelectStage = EnStageType.enStage1;
        }
    }
    //前のステージに選択を移動する関数
    void GoBackStage()
    {
        //選択移動状態にする
        m_selectMove = true;
        //選択されているステージを前のステージにする
        m_nowSelectStage--;
        if (m_nowSelectStage < EnStageType.enStage1)
        {
            m_nowSelectStage = EnStageType.enMaxStageNum - 1;
        }
    }

    //ステージ選択シーンのテキストなどのデータを更新させる関数
    void StageSceneDataUpdate()
    {
        //難易度画像を更新
        for (int starNum = 0; starNum < m_stageDifficluty.Length; starNum++)
        {
            if (starNum <= m_stageDifficluty[(int)m_nowSelectStage])
            {
                //表示
                m_difficlutyStar[starNum].GetComponent<Image>().enabled = true;
            }
            else
            {
                //非表示
                m_difficlutyStar[starNum].GetComponent<Image>().enabled = false;
            }
        }
        //ステージ名ラベルを更新
        m_stageNameLabel.text = m_stageName[(int)m_nowSelectStage];
        //ステージ説明文を更新
        m_stageExplanationLabel.text = m_stageExplanationSentence[(int)m_nowSelectStage];
    }

    //次のシーンに遷移させる関数
    void GoNextScene()
    {
        //操作の判定を初期化させる
        m_operation.TachDataInit();

        //ユーザー設定データのゲームオブジェクトを検索し、
        //ゲームコンポーネントを取得する
        m_userSettingData = GameObject.Find("UserSettingDataStorageSystem").GetComponent<UserSettingData>();
        //選択されたステージデータを保存
        m_userSettingData.GetSetStageType = (int)m_nowSelectStage;

        //CPU強さ設定選択シーンに遷移
        SceneManager.LoadScene("06_CpuPowerSettingScene");
    }

    //電車の移動に合わせて選択しているデータを合わせるカウンター
    void Count()
    {
        //選択移動状態じゃないときは処理をしない。
        if (!m_selectMove) return;

        //カウント計測
        m_selectMoveCount++;

        //カウントが指定した数値より大きくなったら、
        if (m_selectMoveCount > m_circleCenterRotateAround.GetCountTime)
        {
            //選択移動していない状態に戻す
            m_selectMove = false;
            //カウントの初期化
            m_selectMoveCount = 0;
        }
    }
}