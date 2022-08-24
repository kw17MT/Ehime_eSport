using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nsSound
{
    public class NarCharaSelect : OutGameNarBase
    {
        //今のシーンの情報
        private CharaSelectChange m_sceneScript = null;

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
            m_sceneScript = GameObject.Find("SceneManager").GetComponent<CharaSelectChange>();
            m_selectStateNo = m_sceneScript.GetNowSelectState();

            //2.
            //共通のナレーションをリストに加えていく
            m_commonNarList = new List<string>();
            m_commonNarList.Add(nsSound.NarAdvanceNames.m_SAYUUHURIKKUDE);
            m_commonNarList.Add(nsSound.NarCharaSeleNames.m_CHARACTERWOERABERUYO);
            m_commonNarList.Add(nsSound.NarAdvanceNames.m_ERABINAOSHITAITOKIHAGAMENHIDARISHITAWONAGAOSHISHITENE);

            //3.
            //みかんまる選択時のナレーションのリスト
            List<string> mikanmaruNarList = new List<string>();
            mikanmaruNarList.Add(nsSound.NarCharaSeleNames.m_MIKANMARU);
            mikanmaruNarList.Add(nsSound.NarCharaSeleNames.m_BARANSUGATADAYO);

            //レモレモン選択時のナレーションのリスト
            List<string> remoremonNarList = new List<string>();
            remoremonNarList.Add(nsSound.NarCharaSeleNames.m_REMOREMON);
            remoremonNarList.Add(nsSound.NarCharaSeleNames.m_SPEEDGATADAYO);

            //キウイーン選択時のナレーションのリスト
            List<string> kiwinNarList = new List<string>();
            kiwinNarList.Add(nsSound.NarCharaSeleNames.m_KIUIIN);
            kiwinNarList.Add(nsSound.NarCharaSeleNames.m_POWERFULGATADAYO);

            //Ms.ライミリィー選択時のナレーションのリスト
            List<string> msLimilyNarList = new List<string>();
            msLimilyNarList.Add(nsSound.NarCharaSeleNames.m_MISURAIMIRII);
            msLimilyNarList.Add(nsSound.NarCharaSeleNames.m_TECHNICGATADAYO);

            //4.
            //ナレーションのリストを、リストに加えていく(enumの順番と同じにすること！)
            m_narList = new List<List<string>>();
            m_narList.Add(mikanmaruNarList);
            m_narList.Add(remoremonNarList);
            m_narList.Add(kiwinNarList);
            m_narList.Add(msLimilyNarList);
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