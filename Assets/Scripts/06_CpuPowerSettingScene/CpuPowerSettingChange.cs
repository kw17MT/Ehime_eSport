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
    string[] m_strengthName = { "よわい", "ふつう", "つよい" };
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
            //次のCPUの強さに選択を移動
            GoNextCpuPower();
        }
        //画面が左フリックされたら、
        if (m_operation.GetNowOperation() == "left")
        {
            //前のCPUの強さに選択を移動
            GoBackStage();
        }

        //画面が長押しされたら、
        if (m_operation.GetIsLongTouch())
        {
            //次のシーンに遷移させる
            GoNextScene();
        }

        //CPUの強さラベルを更新
        m_strengthLabel.text = m_strengthName[(int)m_nowSelectStrength];
    }

    //次のCPUの強さに選択を移動する関数
    void GoNextCpuPower()
    {
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

        //マッチングシーンに遷移
        SceneManager.LoadScene("07_MatchingScene");
    }
}
