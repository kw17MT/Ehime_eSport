using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 壁沿いに移動するクラスの検証用ゲームオブジェクト
/// </summary>
public class TestMoveObject : MonoBehaviour
{
    private Rigidbody m_rigidbody = null;                                   //ゲームオブジェクトの剛体
    private Vector3 m_moveDirection = new Vector3(-1.0f, 0.0f, -1.0f);      //ゲームオブジェクトの移動方向
    private float m_moveSpeed = 100.0f;                                     //ゲームオブジェクトの移動速度
    private AlongWall m_alongWall = null;                                   //壁沿いの移動処理の計算クラス

    // Start is called before the first frame update
    void Start()
    {
        //剛体と壁沿い移動計算クラスを初期化
        m_rigidbody = this.GetComponent<Rigidbody>();
        m_alongWall = new AlongWall();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //剛体に力を伝える
        m_rigidbody.AddForce(m_moveDirection * m_moveSpeed);
    }

    void Update()
    {
        //デバッグ用、移動方向のベクトルを線で描画
        Debug.DrawRay(m_rigidbody.position, m_moveDirection, Color.green);
    }


    private void OnCollisionEnter(Collision collision)
    {
        //壁にぶつかった時用の処理
        m_alongWall.CollisionEnter(collision,m_rigidbody,ref m_moveDirection);
    }

}
