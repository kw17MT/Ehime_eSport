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
        //操作システムのゲームオブジェクトを検索しスクリプトを使用する
        m_operation = GameObject.Find("OperationSystem").GetComponent<Operation>();
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
            //トランジション起動
            m_transitionPostEffect.OnTransition();

            //操作の判定を初期化させる
            m_operation.TachDataInit();

            //モード選択シーンに遷移
            SceneManager.LoadScene("02_ModeSelectScene");
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
        }
    }
}
