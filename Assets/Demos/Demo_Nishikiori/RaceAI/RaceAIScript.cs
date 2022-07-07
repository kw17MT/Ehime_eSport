using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// CPUのレースキャラ用のスクリプト
/// </summary>
public class RaceAIScript : MonoBehaviour
{
    public string m_AITag = "Player";                                           //AIにつけられたゲームオブジェクトのタグ
    public string m_playerTag = "OwnPlayer";                                    //プレイヤーにつけられたゲームオブジェクトのタグ
    private Rigidbody m_rigidbody = null;                                       //AIキャラクターの剛体
    private const float m_kMaxSpeed = 25.0f;                                    //最高速度
    private Vector3 m_rightSteeringVector = new Vector3(0.0f, 5.0f, 0.0f);      //右方向への回転用ベクトル
    private Vector3 m_leftSteeringVector = new Vector3(0.0f, -5.0f, 0.0f);      //左方向への回転用ベクトル
    private float m_shiftLength = 0.0f;
    private Vector3 m_targetOffset = new Vector3(0.0f,0.0f,0.0f);               //現在の目標のウェイポイントからずらす幅
    private int m_targetNumber = -1;                                            //現在目標にしているウェイポイントの番号
    
    
    RaycastHit m_rayCastHit;


    //ウェイポイントから目標地点をずらす幅(コースによって幅が違うためウェイポイント側への実装も検討)
    public float m_innerShiftMaxLength = 5.0f;//内側への最大のずらし幅
    public float m_outerShiftMaxLength = 3.0f;//外側への最大のずらし幅

    //AIはウェイポイントとの距離に応じてどの角度以内ならハンドルを切るかを変化させる
    //例:
    //遠い距離の場合→多少ウェイポイントへの向きと進行方向が違ってもハンドルを切る必要がない
    //近い距離の場合→ウェイポイントに向かってハンドルを切らなければいけない
    private const float m_kMinSteeringLength = 10.0f;   //AIがハンドルを切る判断をする角度の幅が最小になる距離
    private const float m_kMaxSteeringAngle = 1.0f;     //ハンドルを切る判断をする角度の幅の最大
    private const float m_kMinSteeringAngle = 0.1f;     //ハンドルを切る判断をする角度の幅の最小
    private const float m_kContactAngle = 0.3f;         //接触と判断する角度

    private bool m_canMove = false;

    private void Awake()
    {
        //剛体を取得
        m_rigidbody = this.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(m_canMove)
		{
            //ウェイポイントが変更されたかを調べる
            CheckWayPointChange();

            //ハンドルを切る向きを決定
            HandlingDecision();

            //剛体に力を加える
            m_rigidbody.AddForce(transform.forward * m_kMaxSpeed - m_rigidbody.velocity);
        }

#if UNITY_EDITOR
        //AIの目標地点を出力
        Debug.DrawRay(this.GetComponent<WayPointChecker>().GetNextWayPoint() + m_targetOffset, Vector3.up * 100.0f, Color.red);
#endif
    }

    public void SetCanMove(bool canMove)
	{
        m_canMove = canMove;
        this.GetComponent<AICommunicator>().SetMoving(canMove);
    }

    private void CheckWayPointChange()
    {
        //次のウェイポイントを取得
        int nextNumber = this.GetComponent<WayPointChecker>().GetNextWayPointNumber();

        //現在目指しているウェイポイントと番号が同じなら何もしない
        if(m_targetNumber == nextNumber)
        {
            return;
        }

        //番号が違う = 目指すウェイポイントが更新された

        //目指すウェイポイントの番号を更新
        m_targetNumber = nextNumber;

        this.GetComponent<AICommunicator>().SetNextWayPoint(m_targetNumber);

        //ウェイポイントの座標からずらす幅を乱数で決定
        m_shiftLength = Random.Range(-m_innerShiftMaxLength, m_outerShiftMaxLength);

        //目指す位置をローカル座標系で左右にずらすベクトルを計算
        m_targetOffset = this.GetComponent<WayPointChecker>().GetNextWayPointRight() * m_shiftLength;
    }

    private void HandlingDecision()
    {
        //障害物アイテムを避ける処理
        if (Physics.SphereCast(transform.position, 1.0f, transform.forward, out m_rayCastHit, 20.0f) && m_rayCastHit.collider.gameObject.name.Contains("TestObstacle"))
        {
            if (m_shiftLength > 0.0f)
            {
                RightHandling();
            }
            else
            {
                LeftHandling();
            }

            return;
        }

        //現在目指している位置へのベクトルを計算
        Vector3 toNextPoint = this.GetComponent<WayPointChecker>().GetNextWayPoint() + m_targetOffset - this.transform.position;

        //目指す方向を計算
        Vector3 newForward = toNextPoint;
        newForward.y = 0.0f;
        newForward.Normalize();

        //目指している位置へ向くための角度を計算(左右の判断もするため右向きのベクトルと計算)
        float steeringAngle = Vector3.Dot(transform.right, newForward);

        //目指している位置への距離を取得
        float toNextLength = toNextPoint.magnitude;

        //目指している位置への距離からAIがハンドルを切る判断をする角度の幅が最小になる距離に対しての割合を計算
        float lerpRate = (m_kMinSteeringLength - toNextLength) / m_kMinSteeringLength;

        //距離に対しての割合からハンドルを切る角度のしきい値を計算
        float angleThreshold = Mathf.Lerp(m_kMinSteeringAngle, m_kMaxSteeringAngle, lerpRate);

        //ハンドルを切る角度がしきい値内なら
        if (Mathf.Abs(steeringAngle) > angleThreshold)
        {
            if (steeringAngle > 0.0f)
            {
                //右にハンドルを切る(回転させる)
                RightHandling();
            }
            else
            {
                //左にハンドルを切る(回転させる)
                LeftHandling();
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawRay(transform.position, transform.forward * m_rayCastHit.distance);
        Gizmos.DrawWireSphere(transform.position + transform.forward * m_rayCastHit.distance, 1.0f);
    }

    private void OnCollisionStay(Collision collision)
    {
        //当たった先のオブジェクトがプレイヤーかAIのタグを持っていないなら何もしない。
        if(collision.gameObject.CompareTag(m_playerTag) == false && collision.gameObject.CompareTag(m_AITag) == false)
        {
            return;
        }

        //当たったゲームオブジェクトの座標へのベクトルを計算
        Vector3 toHitGameObject = collision.gameObject.transform.position - this.transform.position;

        //角度を計算
        float angle = Vector3.Dot(toHitGameObject.normalized, this.transform.right);

        //左右どちらにいるか
        if (angle > m_kContactAngle)
        {
            //右にいるので左にハンドルを切る
            LeftHandling();
        }
        else if(angle < -m_kContactAngle)
        {
            //左にいるので右にハンドルを切る
            RightHandling();
        }
    }

    private void RightHandling()
    {
        transform.Rotate(m_rightSteeringVector);
    }

    private void LeftHandling()
    {
        transform.Rotate(m_leftSteeringVector);
    }
}
