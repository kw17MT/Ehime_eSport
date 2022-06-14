using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnPoint : MonoBehaviour
{
    private bool m_isSpawnPlayer = false;
    
    public void SetPlayerSpawned()
	{
        m_isSpawnPlayer = true;
    }

    public bool GetPlayerSpawned()
	{
        return m_isSpawnPlayer;
	}
}
