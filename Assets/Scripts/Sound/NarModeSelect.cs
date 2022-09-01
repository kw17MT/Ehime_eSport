using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nsSound
{
    public class NarModeSelect : OutGameNarBase
    {       
        //今のシーンの情報
        private ModeChange m_sceneScript = null;

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
            m_sceneScript = GameObject.Find("SceneManager").GetComponent<ModeChange>();
            m_selectStateNo = m_sceneScript.GetNowSelectState();

            //2.
            //共通のナレーションをリストに加えていく
            m_commonNarList = new List<string>();
            m_commonNarList.Add(nsSound.NarAdvanceNames.m_SAYUUHURIKKUDE);
            m_commonNarList.Add(nsSound.NarModeNames.m_ASOBIKATAWOERABERUYO);
            m_commonNarList.Add(nsSound.NarAdvanceNames.m_ERABINAOSHITAITOKIHAGAMENHIDARIUEWOTAPSHITENE);

            //3.
            //オンラインモード選択時のナレーションのリスト
            List<string> onlineModeNarList = new List<string>();
            onlineModeNarList.Add(nsSound.NarModeNames.m_ONLINETAISEN);
            onlineModeNarList.Add(nsSound.NarModeNames.m_HOKANOPLAYERTOTAISENDEKIRUYO);

            //CPUモード選択時のナレーションのリスト
            List<string> cpuModeNarList = new List<string>();
            cpuModeNarList.Add(nsSound.NarModeNames.m_CPUTAISEN);
            cpuModeNarList.Add(nsSound.NarModeNames.m_COMPUTERTOTAISENDEKIRUYO);

            //タイムアタックモード選択時のナレーションのリスト
            //List<string> timeAttackModeNarList = new List<string>();
            //timeAttackModeNarList.Add(nsSound.NarModeNames.m_TIMEATTACK);
            //timeAttackModeNarList.Add(nsSound.NarModeNames.m_SHINKIROKUWOMEZASOU);

            //設定モード選択時のナレーションのリスト
            List<string> settingModeNarList = new List<string>();
            settingModeNarList.Add(nsSound.NarModeNames.m_SETTEI);
            settingModeNarList.Add(nsSound.NarModeNames.m_OTOGAMENNOSETTEIGADEKIRUYO);

            //4.
            //ナレーションのリストを、リストに加えていく(enumの順番と同じにすること！)
            m_narList = new List<List<string>>();
            m_narList.Add(onlineModeNarList);
            m_narList.Add(cpuModeNarList);
            //m_narList.Add(timeAttackModeNarList);
            m_narList.Add(settingModeNarList);
        }

		//現在の選択状態を調べる。return:変わっているかどうか
		protected override bool CheckNowSelectState()
        {
            //モードの選択が変わっていたら、
            if (m_selectStateNo != (int)m_sceneScript.GetNowSelectState() && m_narList.Count > (int)m_sceneScript.GetNowSelectState())
            {
                //モードの選択を更新
                m_selectStateNo = (int)m_sceneScript.GetNowSelectState();
                return true;
            }
            return false;
        }
    }
}