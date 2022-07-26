using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// CPUのつよさ選択画面クラス
/// </summary>
public class CpuPowerSettingChange : MonoBehaviour
{
    //CPUの強さ名
    [SerializeField] string[] m_strengthName = null;
    //CPUの強さラベル
    [SerializeField] Text m_strengthLabel = null;

    enum EnCpuPowerType
    {
        enWeak,             //弱い
        enNormal,           //普通
        enStrong,           //強い
        enMaxCpuPowserNum   //最大CPU強さ数
    }
    //現在選択されている強さ
    EnCpuPowerType m_nowSelectStrength = EnCpuPowerType.enWeak;
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
        //選択するCPUの強さを移動する
        ChangeSelectCpuPower();

        //画面が長押しされたら、
        if (m_operation.GetIsDoubleTouch())
        {
            //次のシーンに遷移させる
            GoNextScene();
        }

        //電車の移動に合わせて選択しているデータを合わせるカウンター
        Count();

        //CPUの強さラベルを更新
        m_strengthLabel.text = m_strengthName[(int)m_nowSelectStrength];
    }

    //選択するCPUの強さを移動する
    void ChangeSelectCpuPower()
    {
        if (m_selectMove)
        {
            return;
        }
        //画面が右フリックされたら、
        if (m_operation.GetNowOperation() == "right")
        {
            //次のCPUの強さに選択を移動
            GoNextCpuPower();
        }
        //画面が左フリックされたら、
        if (m_operation.GetNowOperation() == "left")
        {
            //前のCPUの強さに選択を移動
            GoBackStage();
        }
    }

    //次のCPUの強さに選択を移動する関数
    void GoNextCpuPower()
    {
        //選択移動状態にする
        m_selectMove = true;
        //選択されているCPUの強さを次の強さにする
        m_nowSelectStrength++;
        if (m_nowSelectStrength >= EnCpuPowerType.enMaxCpuPowserNum)
        {
            m_nowSelectStrength = EnCpuPowerType.enWeak;
        }
    }
    //前のCPUの強さに選択を移動する関数
    void GoBackStage()
    {
        //選択移動状態にする
        m_selectMove = true;
        //選択されているCPUの強さを前のCPUの強さにする
        m_nowSelectStrength--;
        if (m_nowSelectStrength < EnCpuPowerType.enWeak)
        {
            m_nowSelectStrength = EnCpuPowerType.enMaxCpuPowserNum - 1;
        }
    }

    //次のシーンに遷移させる関数
    void GoNextScene()
    {
        //操作の判定を初期化させる
        m_operation.TachDataInit();

        //ユーザー設定データのゲームオブジェクトを検索し、
        //ゲームコンポーネントを取得する
        m_userSettingData = GameObject.Find("UserSettingDataStorageSystem").GetComponent<UserSettingData>();
        //選択されたCPU強さデータを保存
        m_userSettingData.GetSetCpuPower = (int)m_nowSelectStrength;

        if(m_userSettingData.GetSetModeType == 0)
		{
            //マッチングシーンに遷移
            SceneManager.LoadScene("07_MatchingScene");
        }
		else if(m_userSettingData.GetSetModeType == 1)
		{
            SceneManager.LoadScene("08_GameScene");
        }
    }

    //電車の移動に合わせて選択しているデータを合わせるカウンター
    void Count()
    {
		////選択移動状態じゃないときは処理をしない。
		//if (!m_selectMove) return;

		////カウント計測
		//m_selectMoveCount++;

		////カウントが指定した数値より大きくなったら、
		//if (m_selectMoveCount > m_circleCenterRotateAround.GetCountTime)
		//{
		//	//選択移動していない状態に戻す
		//	m_selectMove = false;
		//	//カウントの初期化
		//	m_selectMoveCount = 0;
		//}
	}

    ///////////////////////////////////////////////////////////////////////////////
    //現在の選択している状態を取得
    public int GetNowSelectState()
    {
        return (int)m_nowSelectStrength;
    }
    ///////////////////////////////////////////////////////////////////////////////
}
