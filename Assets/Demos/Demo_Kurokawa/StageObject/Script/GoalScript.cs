using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GoalScript : MonoBehaviourPunCallbacks
{
    ////////////////////////////////////////////////////////////////////////
    private bool m_ownPlayerGoaled = false;
    ////////////////////////////////////////////////////////////////////////

    //ゴールをくぐったとき
    void OnTriggerEnter(Collider col)
    {
        //ゴールをくぐったのが自分のプレイヤーインスタンスだったら
        if (col.gameObject.tag == "OwnPlayer")
        {
            //チェックポイントを全部通っていたらゴール判定
            col.gameObject.GetComponent<ProgressChecker>().CheckCanGoal();

            //コースを3周ゴールしていたら
            if (col.gameObject.GetComponent<ProgressChecker>().IsFinishRacing())
            {
                //プレイヤーがゴールした判定にする。
                col.gameObject.GetComponent<AvatarController>().SetGoaled();

                ////////////////////////////////////////////////////////////////////////
                m_ownPlayerGoaled = true;
                ////////////////////////////////////////////////////////////////////////
            }
        }

        if (col.gameObject.tag == "Player")
        {
            col.gameObject.GetComponent<AIProgressChecker>().CheckCanGoal();
        }
    }

    ////////////////////////////////////////////////////////////////////////
    public bool GetOwnPlayerGoaled()
    {
        return m_ownPlayerGoaled;
    }
    ////////////////////////////////////////////////////////////////////////
}