using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// CPUのレースキャラ用のスクリプト
/// </summary>
public class RaceAIScript : MonoBehaviour
{
    private Rigidbody m_rigidbody = null;                                      //AIキャラクターの剛体
    private const float m_kMaxSpeed = 25.0f;                                   //最高速度
    private Vector3 m_RightSteeringVector = new Vector3(0.0f, 5.0f, 0.0f);     //右方向への回転用ベクトル
    private Vector3 m_LeftSteeringVector = new Vector3(0.0f, -5.0f, 0.0f);     //左方向への回転用ベクトル

    //AIはウェイポイントとの距離に応じてどの角度以内ならハンドルを切るかを変化させる
    //例:
    //遠い距離の場合→多少ウェイポイントへの向きと進行方向が違ってもハンドルを切る必要がない
    //近い距離の場合→ウェイポイントに向かってハンドルを切らなければいけない
    private const float m_kMinSteeringLength = 10.0f;   //AIがハンドルを切る判断をする角度の幅が最小になる距離
    private const float m_kMaxSteeringAngle = 1.0f;     //ハンドルを切る判断をする角度の幅の最大
    private const float m_kMinSteeringAngle = 0.1f;     //ハンドルを切る判断をする角度の幅の最小

    private void Awake()
    {
        //剛体を取得
        m_rigidbody = this.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //現在目指しているウェイポイントへのベクトルを計算
        Vector3 toNextPoint = this.GetComponent<WayPointChecker>().GetNextWayPoint() - this.transform.position;

        //目指す方向を計算
        Vector3 newForward = toNextPoint;
        newForward.y = 0.0f;
        newForward.Normalize() ;

        //ウェイポイントへ向くための角度を計算(左右の判断もするため右向きのベクトルと計算)
        float steeringAngle = Vector3.Dot(transform.right, newForward);

        //次のウェイポイントへの距離を取得
        float toNextLength = toNextPoint.magnitude;

        //ウェイポイントへの距離からAIがハンドルを切る判断をする角度の幅が最小になる距離に対しての割合を計算
        float lerpRate = (m_kMinSteeringLength - toNextLength) / m_kMinSteeringLength;

        //距離からハンドルを切る角度のしきい値を計算
        float angleThresold = Mathf.Lerp(m_kMinSteeringAngle, m_kMaxSteeringAngle, lerpRate);

        //ハンドルを切る角度がしきい値内なら
        if(Mathf.Abs(steeringAngle) > angleThresold)
        {
            if (steeringAngle > 0.0f)
            {
                //右にハンドルを切る(回転させる)
                transform.Rotate(m_RightSteeringVector);
            }
            else
            {
                //左にハンドルを切る(回転させる)
                transform.Rotate(m_LeftSteeringVector);
            }
        }

        //剛体に力を加える
        m_rigidbody.AddForce(transform.forward * m_kMaxSpeed - m_rigidbody.velocity);

        //次に目指すウェイポイントの位置を出力
        Debug.Log(GetComponent<WayPointChecker>().GetNextWayPoint());
    }

}
