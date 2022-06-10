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
    string[] m_stageName = { "ステージ1", "ステージ2", "ステージ3" };
    //ステージ名ラベル
    [SerializeField] Text m_stageNameLabel = null;

    //難易度スプライト
    [SerializeField] Sprite[] m_difficlutySprite = null;
    //難易度イメージ
    [SerializeField] Image m_difficlutyImage = null;
    //ステージごとの難易度(0,簡単。1,普通。2,難しい)
    int[] m_stageDifficluty = { 0, 1, 2 };

    //ステージ説明文
    string[] m_stageExplanationSentence =
    {
        //ステージ1
        "ここはステージ1。えへへでえへへなえへへである。えへへでえへへ。えへへへへへへへ。",
        //ステージ2
        "ここはステージ2。えへへでえへへなえへへである。えへへでえへへ。えへへへへへへへ。",
        //ステージ3
        "ここはステージ3えへへでえへへなえへへである。えへへでえへへ。えへへへへへへへ。"
    };
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
    OperationNew m_operation = null;

    void Start()
    {
        //操作システムのゲームオブジェクトを検索しスクリプトを使用する
        m_operation = GameObject.Find("OperationSystem").GetComponent<OperationNew>();
    }

    //アップデート関数
    void Update()
    {
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

        //選択されているステージによって分岐
        switch (m_nowSelectStage)
        {
            //ステージ1
            case EnStageType.enStage1:
                break;
            //ステージ2
            case EnStageType.enStage2:
                break;
            //ステージ3
            case EnStageType.enStage3:
                break;
        }

        //画面が長押しされたら、
        if (m_operation.GetIsLongTouch())
        {
            //次のシーンに遷移させる
            GoNextScene();
        }

        //ステージ選択シーンのテキストなどのデータを更新
        StageSceneDataUpdate();
    }

    //次のステージに選択を移動する関数
    void GoNextStage()
    {
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
        m_difficlutyImage.sprite = m_difficlutySprite[m_stageDifficluty[(int)m_nowSelectStage]];
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

        //CPU強さ設定選択シーンに遷移
        SceneManager.LoadScene("06_CpuPowerSettingScene");
    }
}