using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 壁に接触した際、壁沿いに移動するベクトルを計算し剛体に適応するクラス。
/// </summary>
public class AlongWall
{
    //壁沿いに移動する速度。
    public float m_alongWallSpeed { get; set; } = 25.0f;

    //衝突判定とするゲームオブジェクトのタグ
    public string m_collideTag { get; set; } = "Wall";

    /// <summary>
    /// オブジェクトが壁に接触した瞬間、壁沿いに移動するベクトルを計算し剛体に適応する。
    /// </summary>
    /// <param name="collision">オブジェクトが接触したコリジョン</param>
    /// <param name="rigidbody">オブジェクトの剛体</param>
    /// <param name="moveDirection">(参照)オブジェクトの移動方向</param>
    public void CollisionEnter(Collision collision, Rigidbody rigidbody, ref Vector3 moveDirection)
    {
        //コリジョンを持つゲームオブジェクトが衝突判定を行うタグを持っていない場合は何もしない。
        if (collision.gameObject.CompareTag(m_collideTag) == false)
        {
            return;
        }

        //剛体の移動方向を取得
        Vector3 velocity = rigidbody.velocity;
        velocity.Normalize();
        Debug.Log("Velocity " + velocity);

        //接触した当たり判定の法線を取得
        Vector3 normal = collision.contacts[0].normal;

        Debug.Log("Normal " + normal);

        if (Vector3.Dot(velocity, normal) > 0.0f)
        {



            //壁に沿って動くベクトルを計算し、移動方向として格納
            moveDirection = velocity - (Vector3.Dot(velocity, normal) * normal);
            Debug.Log(moveDirection);
            //moveDirection.Normalize();

            //当たった時の向きをリセットし、スピードをセット
            //rigidbody.velocity = m_alongWallSpeed * moveDirection;

        }

#if UNITY_EDITOR
        //当たった壁の法線を可視化(Debug)
        DrawNormal(collision.contacts[0].point,normal);
#endif
    }

    /// <summary>
    /// 当たった場所の法線を描画　使用する場合シーンにNormalDebugManagerを置くこと。
    /// </summary>
    private void DrawNormal(Vector3 contactPoint,Vector3 normal)
    {
        //法線デバッグマネージャを検索
        GameObject normalDebugManager = GameObject.Find("NormalDebugManager");

        if (normalDebugManager != null)
        {
            //あった場合法線デバッグ用オブジェクトを生成するように命令
            normalDebugManager.GetComponent<NormalDebugManager>().Spawn(contactPoint, normal);
        }
    }
}
