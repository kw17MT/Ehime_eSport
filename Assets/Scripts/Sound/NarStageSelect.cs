using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nsSound
{
    public class NarStageSelect : OutGameNarBase
    {
        //今のシーンの情報
        private StageSelectChange m_sceneScript = null;

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
            m_sceneScript = GameObject.Find("SceneManager").GetComponent<StageSelectChange>();
            m_selectStateNo = m_sceneScript.GetNowSelectState();

            //2.
            //共通のナレーションをリストに加えていく
            m_commonNarList = new List<string>();
            m_commonNarList.Add(nsSound.NarAdvanceNames.m_SAYUUHURIKKUDE);
            m_commonNarList.Add(nsSound.NarStageSeleNames.m_STAGEWOERABERUYO);
            m_commonNarList.Add(nsSound.NarAdvanceNames.m_ERABINAOSHITAITOKIHAGAMENHIDARIUEWOTAPSHITENE);

            //3.
            //直線ステージ選択時のナレーションのリスト
            List<string> straightStageNarList = new List<string>();
            straightStageNarList.Add(nsSound.NarStageSeleNames.m_CHOKUSENNNOSTAGEDAYO);

            //楕円形ステージ選択時のナレーションのリスト
            List<string> ellipseStageNarList = new List<string>();
            ellipseStageNarList.Add(nsSound.NarStageSeleNames.m_DAENKEINOSTAGEDAYO);

            //4.
            //ナレーションのリストを、リストに加えていく(enumの順番と同じにすること！)
            m_narList = new List<List<string>>();
            m_narList.Add(straightStageNarList);
            m_narList.Add(ellipseStageNarList);
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