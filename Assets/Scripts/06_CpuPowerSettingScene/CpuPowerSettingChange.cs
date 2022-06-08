using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// CPUのつよさ選択画面クラス
/// </summary>
public class CpuPowerSettingChange : MonoBehaviour
{
    //CPUの強さ名
    string[] m_strengthName = { "よわい", "ふつう", "つよい" };
    //CPUの強さラベル
    [SerializeField] Text m_strengthLabel;

    enum EnCpuPowerType
    {
        enWeak,             //弱い
        enNormal,           //普通
        enStrong,           //強い
        enMaxCpuPowserNum   //最大CPU強さ数
    }
    //現在選択されている強さ
    EnCpuPowerType m_nowSelectStrength = EnCpuPowerType.enWeak;

    //アップデート関数
    void Update()
    {
        //画面がタップされたら、
        if (Input.GetButtonDown("Fire1"))
        {
            //次のCPUの強さに選択を移動
            GoNextCpuPower();
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
}
