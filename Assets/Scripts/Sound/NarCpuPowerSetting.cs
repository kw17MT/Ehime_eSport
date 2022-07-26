using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nsSound
{
    public class NarCpuPowerSetting : OutGameNarBase
    {
        //今のシーンの情報
        private CpuPowerSettingChange m_sceneScript = null;

        //Init()でやること。
        //1.シーンの情報を取得
        //2.共通のナレーションを用意する。
        //3.ナレーションのリストの用意
        //4.ナレーションのリストのリストに、3のリストをAdd()する。(そのシーンの選択状態の列挙体の順番通りに！)
        //もし列挙体の順番が変わったら、ナレーションのリストのリストにAdd()する順番も変えること！
        protected override void Init()
        {
            //1.
            //シーンの情報を取得
            m_sceneScript = GameObject.Find("SceneManager").GetComponent<CpuPowerSettingChange>();
            m_selectStateNo = m_sceneScript.GetNowSelectState();

            //2.
            //共通のナレーションをリストに加えていく
            m_commonNarList = new List<string>();
            m_commonNarList.Add(nsSound.NarAdvanceNames.m_SAYUUHURIKKUDE);
            m_commonNarList.Add(nsSound.NarCPUSeleNames.m_COMPUTERNOTUYOSAWOERABERUYO);
            m_commonNarList.Add(nsSound.NarAdvanceNames.m_ERABINAOSHITAITOKIHAGAMENHIDARISHITAWONAGAOSHISHITENE);

            //3.
            //弱いを選択時のナレーションのリスト
            List<string> weakNarList = new List<string>();
            weakNarList.Add(nsSound.NarCPUSeleNames.m_YASASHII);

            //普通を選択時のナレーションのリスト
            List<string> normalNarList = new List<string>();
            normalNarList.Add(nsSound.NarCPUSeleNames.m_HUTSUU);

            //強いを選択時のナレーションのリスト
            List<string> strongModeNarList = new List<string>();
            strongModeNarList.Add(nsSound.NarCPUSeleNames.m_TSUYOI);

            //4.
            //ナレーションのリストを、リストに加えていく(enumの順番と同じにすること！)
            m_narList = new List<List<string>>();
            m_narList.Add(weakNarList);
            m_narList.Add(normalNarList);
            m_narList.Add(strongModeNarList);
        }

        //現在の選択状態を調べる。return:変わっているかどうか
        protected override bool CheckNowSelectState()
        {
            //モードの選択が変わっていたら、
            if (m_selectStateNo != (int)m_sceneScript.GetNowSelectState())
            {
                //モードの選択を更新
                m_selectStateNo = (int)m_sceneScript.GetNowSelectState();
                return true;
            }
            return false;
        }
    }
}