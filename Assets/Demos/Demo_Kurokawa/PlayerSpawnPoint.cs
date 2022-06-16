using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//プレイヤーのスポーンポイントクラス
public class PlayerSpawnPoint : MonoBehaviour
{
    private bool m_isSpawnPlayer = false;           //自分はプレイヤーをスポーンさせたか
    
    //プレイヤーをスポーンさせたことを保存
    public void SetPlayerSpawned()
	{
        m_isSpawnPlayer = true;
    }

    //プレイヤーをスポーンさせたかを取得
    public bool GetPlayerSpawned()
	{
        return m_isSpawnPlayer;
	}
}
