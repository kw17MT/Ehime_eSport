using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestObstacleScript : MonoBehaviour
{
    public string m_AITag = "Player";                                           //AIにつけられたゲームオブジェクトのタグ
    public string m_playerTag = "OwnPlayer";                                    //プレイヤーにつけられたゲームオブジェクトのタグ

    private void OnTriggerEnter(Collider collision)
    {
        //当たった先のオブジェクトがプレイヤーかAIのタグを持っていないなら何もしない。
        if (collision.gameObject.CompareTag(m_playerTag) == false && collision.gameObject.CompareTag(m_AITag) == false)
        {
            return;
        }

        Debug.Log("ObstacleHit:" + collision.gameObject.name);

        Destroy(gameObject);
    }
}