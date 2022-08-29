using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nsSound
{
    //サウンドキューの名前を定数として宣言する。

    //SE
    public static class SENames
    {
        public const string m_cancel = "SE_Cancel";
        public const string m_dash = "SE_Dash";
        public const string m_enter = "SE_Enter";
        public const string m_longPress = "SE_LongPress";
        public const string m_swipeAndTableTurns = "SE_SwipeAndTableTurns";
        public const string m_accele = "SE_Accele";
        public const string m_danger = "SE_Danger";
        public const string m_dropOrangePeel = "SE_DropOrangePeel";
        public const string m_engine = "SE_Engine";
        public const string m_getItem = "SE_GetItem";
        public const string m_itemSlotLottery = "SE_ItemSlotLottery";
        public const string m_itemSlotLotteryEnd = "SE_ItemSlotLotteryEnd";
        public const string m_startEngine = "SE_StartEngine";
        public const string m_trainRun = "SE_TrainRun";
        public const string m_trainWhistle = "SE_TrainWhistle";
        public const string m_cannon = "SE_Cannon";
        public const string m_attacked = "SE_Attacked";
        public const string m_hitWall = "SE_HitWall";
        public const string m_snapper = "SE_Snapper";
    }

    //BGM
    public static class BGMNames
    {
        public const string m_race1 = "BGM_Race1";
        public const string m_title = "BGM_Title";
        public const string m_menu = "BGM_Menu";
        public const string m_result1 = "BGM_Result1";
        public const string m_result2 = "BGM_Result2";
        public const string m_fanfare1 = "BGM_Fanfare1";
        public const string m_fanfare2 = "BGM_Fanfare2";
        public const string m_star = "BGM_Star";
        public const string m_lastLap = "BGM_LastLap";

        //名前なし
        public const string m_nothing = "nothing";
    }

    //キャラクターの声
    public static class CharaVoiceNames
    {

    }

    //ナレーション
    public static class NarAdvanceNames
    {
        public const string m_ERABINAOSHITAITOKIHAGAMENHIDARISHITAWONAGAOSHISHITENE = "Nar_Advance_ERABINAOSHITAITOKIHAGAMENHIDARISHITAWONAGAOSHISHITEN";
        public const string m_GAMENNAGAOSHIDETUGIHE = "Nar_Advance_GAMENNAGAOSHIDETUGIHE";
        public const string m_NAGAOSHIDESENTAKUSHITENE = "Nar_Advance_NAGAOSHIDESENTAKUSHITENE";
        public const string m_NIKETTEI = "Nar_Advance_NIKETTEI";
        public const string m_SAYUUHURIKKUDE = "Nar_Advance_SAYUUHURIKKUDE";
        public const string m_WOERABERUYO = "Nar_Advance_WOERABERUYO";
        public const string m_ERABINAOSHITAITOKIHAGAMENHIDARIUEWONAGAOSHISHITENE = "Nar_Advance_ERABINAOSHITAITOKIHAGAMENHIDARIUEWONAGAOSHISHITENE";
        public const string m_ERABINAOSHITAITOKIHAGAMENHIDARIUEWOTAPSHITENE = "Nar_Advance_ERABINAOSHITAITOKIHAGAMENHIDARIUEWOTAPSHITENE";

    }
    public static class NarTitleNames
    {
        public const string m_GAMENNAGAOSHIDEGAMESTART = "Nar_Title_GAMENNAGAOSHIDEGAMESTART";
        public const string m_GAMENTAPPUDEGAMESTART = "Nar_Title_GAMENTAPPUDEGAMESTART";
    }
    public static class NarModeNames
    {
        public const string m_ASOBIKATAWOERABERUYO = "Nar_Mode_ASOBIKATAWOERABERUYO";
        public const string m_COMPUTERTOTAISENDEKIRUYO = "Nar_Mode_COMPUTERTOTAISENDEKIRUYO";
        public const string m_CPUTAISEN = "Nar_Mode_CPUTAISEN";
        public const string m_HOKANOPLAYERTOTAISENDEKIRUYO = "Nar_Mode_HOKANOPLAYERTOTAISENDEKIRUYO";
        public const string m_ONLINETAISEN = "Nar_Mode_ONLINETAISEN";
        public const string m_OTOGAMENNOSETTEIGADEKIRUYO = "Nar_Mode_OTOGAMENNOSETTEIGADEKIRUYO";
        public const string m_SETTEI = "Nar_Mode_SETTEI";
        public const string m_SHINKIROKUWOMEZASOU = "Nar_Mode_SHINKIROKUWOMEZASOU";
        public const string m_TIMEATTACK = "Nar_Mode_TIMEATTACK";
    }
    public static class NarCharaSeleNames
    {
        public const string m_BARANSUGATADAYO = "Nar_CharaSele_BARANSUGATADAYO";
        public const string m_CHARACTERWOERABERUYO = "Nar_CharaSele_CHARACTERWOERABERUYO";
        public const string m_KINGMONTBLANC = "Nar_CharaSele_KINGMONTBLANC";
        public const string m_KIUIIN = "Nar_CharaSele_KIUIIN";
        public const string m_MIKANMARU = "Nar_CharaSele_MIKANMARU";
        public const string m_MISURAIMIRII = "Nar_CharaSele_MISURAIMIRII";
        public const string m_OREBLOOD = "Nar_CharaSele_OREBLOOD";
        public const string m_PEARLJOU = "Nar_CharaSele_PEARLJOU";
        public const string m_POWERFULGATADAYO = "Nar_CharaSele_POWERFULGATADAYO";
        public const string m_POWERFULSPEEDGATADAYO = "Nar_CharaSele_POWERFULSPEEDGATADAYO";
        public const string m_REMOREMON = "Nar_CharaSele_REMOREMON";
        public const string m_SPEEDGATADAYO = "Nar_CharaSele_SPEEDGATADAYO";
        public const string m_TECHNICGATADAYO = "Nar_CharaSele_TECHNICGATADAYO";
        public const string m_TECHNICSPEEDGATADAYO = "Nar_CharaSele_TECHNICSPEEDGATADAYO";
        public const string m_UNGAIIYO = "Nar_CharaSele_UNGAIIYO";
    }

    public static class NarCPUSeleNames
    {
        public const string m_COMPUTERNOTUYOSAWOERABERUYO = "Nar_CPUSele_COMPUTERNOTUYOSAWOERABERUYO";
        public const string m_HUTSUU = "Nar_CPUSele_HUTSUU";
        public const string m_TSUYOI = "Nar_CPUSele_TSUYOI";
        public const string m_YASASHII = "Nar_CPUSele_YASASHII";
    }
    public static class NarMatchNames
    {
        public const string m_match_GASANKASHITAYO = "Nar_Match_GASANKASHITAYO";
        public const string m_match_MATCHINGGAKANRYOSHITAYO = "Nar_Match_MATCHINGGAKANRYOSHITAYO";
        public const string m_match_MATCHINGCHUUDAYO = "Nar_Match_MATCHINGCHUUDAYO";
    }
    public static class NarStageSeleNames
    {
        public const string m_NANYOSTAGE = "Nar_StageSele_NANYOSTAGE";
        public const string m_OCHITEIRUMIKANNICHUUISHIYOU = "Nar_StageSele_OCHITEIRUMIKANNICHUUISHIYOU";
        public const string m_STAGEWOERABERUYO = "Nar_StageSele_STAGEWOERABERUYO";
        public const string m_CHOKUSENNNOSTAGEDAYO = "Nar_StageSele_CHOKUSENNNOSTAGEDAYO";
        public const string m_DAENKEINOSTAGEDAYO = "Nar_StageSele_DAENKEINOSTAGEDAYO";

    }
    public static class NarInGameNames
    {
        public const string m_HIDARICURVEGAARUYO = "Nar_InGame_HIDARICURVEGAARUYO";
        public const string m_HIDARIGAWANI = "Nar_InGame_HIDARIGAWANI";
        public const string m_HIDARINIIKOU = "Nar_InGame_HIDARINIIKOU";
        public const string m_ITEMGAARUYO = "Nar_InGame_ITEMGAARUYO";
        public const string m_KABENIATATTEIRUYO = "Nar_InGame_KABENIATATTEIRUYO";
        public const string m_KYUUNA = "Nar_InGame_KYUUNA";
        public const string m_MAENIMIKANNNOKAWADA = "Nar_InGame_MAENIMIKANNNOKAWADA";
        public const string m_MIGICURVEGAARUYO = "Nar_InGame_MIGICURVEGAARUYO";
        public const string m_MIGIGAWANI = "Nar_InGame_MIGIGAWANI";
        public const string m_MIGINIIKOU = "Nar_InGame_MIGINIIKOU";
        public const string m_SUKOSHI = "Nar_InGame_SUKOSHI";
        public const string m_YURUYAKANA = "Nar_InGame_YURUYAKANA";
        public const string m_HIDARINOKABENIATATTEIRUYO = "Nar_InGame_HIDARINOKABENIATATTEIRUYO";
        public const string m_MIGINOKABENIATATTEIRUYO = "Nar_InGame_MIGINOKABENIATATTEIRUYO";
        public const string m_RAP1 = "Nar_InGame_RAP1";
        public const string m_RAP2 = "Nar_InGame_RAP2";
        public const string m_RAP3 = "Nar_InGame_RAP3";
        public const string m_GOAAAL = "Nar_InGame_GOAAAL";

    }
    public static class NarInGameItemNames
    {
        public const string m_BOCCHANRESSYADA = "Nar_InGameItem_BOCCHANRESSYADA";
        public const string m_IKKINIKASOKUSURUZO = "Nar_InGameItem_IKKINIKASOKUSURUZO";
        public const string m_JUICEJETDA = "Nar_InGameItem_JUICEJETDA";
        public const string m_MAENOKURUMANIATERO = "Nar_InGameItem_MAENOKURUMANIATERO";
        public const string m_MIKANNNOKAWADA = "Nar_InGameItem_MIKANNNOKAWADA";
        public const string m_MUTEKININARERUZO = "Nar_InGameItem_MUTEKININARERUZO";
        public const string m_OUGONNMIKANNDA = "Nar_InGameItem_OUGONNMIKANNDA";
        public const string m_RIVALWOOKIZARINISHIRO = "Nar_InGameItem_RIVALWOOKIZARINISHIRO";
        public const string m_SECCHISHITEAITENOJAMAGADEKIRUZO = "Nar_InGameItem_SECCHISHITEAITENOJAMAGADEKIRUZO";
        public const string m_TAIHOUDA = "Nar_InGameItem_TAIHOUDA";
    }

    public static class NarInGameLiveNames
    {
        public const string m_1 = "Nar_InGameLive_1";
        public const string m_2 = "Nar_InGameLive_2";
        public const string m_3 = "Nar_InGameLive_3";
        public const string m_ATATTA = "Nar_InGameLive_ATATTA";
        public const string m_GENZAI = "Nar_InGameLive_GENZAI";
        public const string m_GOAL = "Nar_InGameLive_GOAL";
        public const string m_GOBOUNUKIDA = "Nar_InGameLive_GOBOUNUKIDA";
        public const string m_ICHII = "Nar_InGameLive_ICHII";
        public const string m_ICHIIGATAISAWOTSUKETEIRU = "Nar_InGameLive_ICHIIGATAISAWOTSUKETEIRU";
        public const string m_ITEMWOTOTTA = "Nar_InGameLive_ITEMWOTOTTA";
        public const string m_KINGMONTBLANCSANSEI = "Nar_InGameLive_KINGMONTBLANCSANSEI";
        public const string m_KIUIIN = "Nar_InGameLive_KIUIIN";
        public const string m_MIKANMARU = "Nar_InGameLive_MIKANMARU";
        public const string m_MISURAIMIRII = "Nar_InGameLive_MISURAIMIRII";
        public const string m_NII = "Nar_InGameLive_NII";
        public const string m_NIIGAOITSUITA = "Nar_InGameLive_NIIGAOITSUITA";
        public const string m_NOTAIGAATATTESHIMATTA = "Nar_InGameLive_NOTAIGAATATTESHIMATTA";
        public const string m_OOKIKUMAWATTESHIMATTA = "Nar_InGameLive_OOKIKUMAWATTESHIMATTA";
        public const string m_OREBLOOD = "Nar_InGameLive_OREBLOOD";
        public const string m_PEARLJOU = "Nar_InGameLive_PEARLJOU";
        public const string m_REMOREMON = "Nar_InGameLive_REMOREMON";
        public const string m_SAIGONOCHOKUSEN = "Nar_InGameLive_SAIGONOCHOKUSEN";
        public const string m_SAISYONISHIKAKERUNOHADAREDA = "Nar_InGameLive_SAISYONISHIKAKERUNOHADAREDA";
        public const string m_SANNI = "Nar_InGameLive_SANNI";
        public const string m_START = "Nar_InGameLive_START";
        public const string m_TONOSESSENDA = "Nar_InGameLive_TONOSESSENDA";
        public const string m_UMAKUINWOTSUITA = "Nar_InGameLive_UMAKUINWOTSUITA";
        public const string m_YONNi = "Nar_InGameLive_YONNi";
    }
    public static class NarResultNames
    {
        public const string m_GAMENNAGAOSHIDETITLENIMODOROU = "Nar_Result_GAMENNAGAOSHIDETITLENIMODOROU";
        public const string m_GOALSHIMASHITA = "Nar_Result_GOALSHIMASHITA";
        public const string m_ICHII = "Nar_Result_ICHII";
        public const string m_KEKKAHAPPYOU = "Nar_Result_KEKKAHAPPYOU";
        public const string m_NII = "Nar_Result_NII";
        public const string m_SANNi = "Nar_Result_SANNi";
        public const string m_YONNi = "Nar_Result_YONNi";
    }
}