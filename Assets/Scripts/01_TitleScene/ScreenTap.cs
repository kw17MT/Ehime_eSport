using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenTap : MonoBehaviour
{
    //列車が前進できるかどうか
    bool canAdvance = false;
    //列車の移動速度
    [SerializeField] float advanceSpeed = 0.0f;
    //列車の移動範囲
    [SerializeField] float trainMoveRange = 0.0f;
    //トランジションシステム
    [SerializeField] TransitionPostEffect m_transitionPostEffect = null;
    //操作システム
    Operation m_operation = null;

    void Start()
    {
        Application.targetFrameRate = 30;
        //操作システムのゲームオブジェクトを検索しコンポーネントを取得
        m_operation = GameObject.Find("OperationSystem").GetComponent<Operation>();
        ////////////////////////////////////////
        //BGMの再生
        nsSound.BGM.Instance.SetPlayBGM(nsSound.BGMNames.m_title);
        ////////////////////////////////////////
    }

    void Update()
    {
        //画面がタップされたかを判定
        TapScreen();

        //列車が前進できるとき
        if (canAdvance)
        {
            //列車を前進させる
            this.transform.position += new Vector3(0.0f,0.0f,advanceSpeed);
        }

        //カメラに一定の距離まで近づいたら、
        if (this.transform.position.z < trainMoveRange)
        {
            //シーン遷移処理
            ChangeScene();
        }
    }

    //画面タップ判定関数
    void TapScreen()
    {
        //画面タップが１度しかできないようにする
        if (canAdvance)
        {
            return;
        }

        //画面がタップされたら、
        if(m_operation.GetNowOperation() == "touch")
        {
            //トランジション起動
            m_transitionPostEffect.OnTransition();

            //列車が前進できる判定にする
            canAdvance = true;

            //////////////////////////////
            //発進SEの再生
            nsSound.SoundSource dashSS = new GameObject("SoundSource").AddComponent<nsSound.SoundSource>();
            dashSS.SetSoundType(nsSound.EnSoundTypes.enSE);
            dashSS.Be3DSound();
            dashSS.PlayStart(nsSound.SENames.m_dash);
            //BGMをフェードアウト
            nsSound.BGM.Instance.FadeOutStart();
            //////////////////////////////
        }
    }

    //シーン遷移処理関数
    void ChangeScene()
    {
        //トランジション起動
        m_transitionPostEffect.OnTransition();

        //操作の判定を初期化させる
        m_operation.TachDataInit();

        //モード選択シーンに遷移
        SceneManager.LoadScene("02_ModeSelectScene");
    }
}
