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
    private WayPointChecker m_wayPointChecker = null;                           //AIキャラクターのウェイポイントチェッカー
    public AIDifficulty m_AIDifficulty = null;

    //ステータス//
    //スピード
    private float m_maxSpeed {get;set;} = 25.0f;                                //最高速度

    //操作性
    private Vector3 m_rightSteeringVector { get; set; }
        = new Vector3(0.0f, 5.0f, 0.0f);                                        //右方向への回転用ベクトル
    private Vector3 m_leftSteeringVector { get; set; }
        = new Vector3(0.0f, -5.0f, 0.0f);                                       //左方向への回転用ベクトル

    //パワフル?

    //運の良さ?


    private float m_shiftLength = 0.0f;                                         //目指す地点がウェイポイントから右方向にどれだけ離れているか
    private Vector3 m_targetOffset = new Vector3(0.0f,0.0f,0.0f);               //現在の目標のウェイポイントからずらす幅
    private int m_targetNumber = -1;                                            //現在目標にしているウェイポイントの番号


    //障害物避け用変数
    public LayerMask m_obstacleLayerMask;                                       //障害物のレイヤーマスク
    RaycastHit m_sphereCastHit;                                                 //SphereCastの結果を格納する変数
    float m_sphereCastRadius = 1.0f;                                            //SphereCastの半径
    float m_sphereCastMaxDistance = 20.0f;                                      //SphereCastの最大距離
    float m_onLineLength = 1.0f;                                                //障害物がライン上にあると判断する距離


    //ウェイポイントから目標地点をずらす幅
    public float m_maxMoveRatio = 0.2f;
    private float m_currentShiftRatio = 0.5f;//現在のずらし幅の割合(内側:0.0f～外側:1.0f)

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
        m_rigidbody = GetComponent<Rigidbody>();

        //ウェイポイントチェッカーを取得
        m_wayPointChecker = GetComponent<WayPointChecker>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //カウントダウンが終了して動ける状態で、攻撃されていなければ
        if(m_canMove)
		{
            if(this.gameObject.tag == m_playerTag || !this.gameObject.GetComponent<AICommunicator>().GetIsAttacked())
			{
                //ウェイポイントが変更されたかを調べる
                CheckWayPointChange();

                //ハンドルを切る向きを決定
                HandlingDecision();

                //剛体に力を加える
                m_rigidbody.AddForce(transform.forward * m_maxSpeed - m_rigidbody.velocity);
            }

#if UNITY_EDITOR
            //デバッグ用　AIの目標地点を出力
            Debug.DrawRay(m_wayPointChecker.GetNextWayPoint() + m_targetOffset, Vector3.up * 100.0f, Color.green);

            //デバッグ用　最後に通ったウェイポイントから次のウェイポイントまでを線でつなぐ
            Vector3 prevWayPointtoCurrentWayPoint = m_wayPointChecker.GetNextWayPoint() - m_wayPointChecker.GetCurrentWayPoint();
            Debug.DrawRay(m_wayPointChecker.GetCurrentWayPoint(), prevWayPointtoCurrentWayPoint, Color.yellow);
#endif

        }
    }

    public void SetCanMove(bool canMove)
	{
        m_canMove = canMove;
        if(this.gameObject.tag == m_AITag)
		{
            this.GetComponent<AICommunicator>().SetMoving(canMove);
        }

        this.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionY;
        this.gameObject.GetComponent<Rigidbody>().freezeRotation = true;
    }

    public int GetNextWayPoint()
	{
        return m_targetNumber;
	}

    private void CheckWayPointChange()
    {
        //次のウェイポイントを取得
        int nextNumber = m_wayPointChecker.GetNextWayPointNumber();

        //現在目指しているウェイポイントと番号が同じなら何もしない
        if(m_targetNumber == nextNumber)
        {
            return;
        }

        //番号が違う = 目指すウェイポイントが更新された

        //目指すウェイポイントの番号を更新
        m_targetNumber = nextNumber;

        //このスクリプトをもつオブジェクトがAIだったら
        if(this.gameObject.tag == m_AITag)
		{
            //次のウェイポイントをAIの情報を別オブジェクトに通信するスクリプトに保存
            this.GetComponent<AICommunicator>().SetNextWayPoint(m_targetNumber);
        }

        //次のウェイポイントの幅の半分を取得
        float nextHalfWidth = m_wayPointChecker.GetNextWayPointHalfWidth();
        
        //ウェイポイントの幅をそのまま使うとコースアウトギリギリまで行ってしまうので少しだけ値を減らす
        nextHalfWidth -= 2.0f;

        //内外に取りうる値の最大値をウェイポイントの幅と難易度から設定
        float innerShiftMaxLength = nextHalfWidth * m_AIDifficulty.innerShiftMaxRatio;
        float outerShiftMaxLength = nextHalfWidth * m_AIDifficulty.outerShiftMaxRatio;



        //現在のウェイポイントの座標からずらす割合に変化を与える値を乱数で決定
        float moveRatio = Random.Range(-m_maxMoveRatio, m_maxMoveRatio);

        //現在の割合に加算
        m_currentShiftRatio += moveRatio;

        //0~1に整える(規定値より内・外にいかないように)
        m_currentShiftRatio = Mathf.Clamp01(m_currentShiftRatio);

        //割合から実際にずらす長さを計算
        m_shiftLength = Mathf.Lerp(-innerShiftMaxLength, outerShiftMaxLength, m_currentShiftRatio);

        //目指す位置をローカル座標系で左右にずらすベクトルを計算
        m_targetOffset = m_wayPointChecker.GetNextWayPointRight() * m_shiftLength;
    }

    private void HandlingDecision()
    {
        //障害物アイテムを避ける処理
        if (Physics.SphereCast(transform.position, m_sphereCastRadius, transform.forward, out m_sphereCastHit, m_sphereCastMaxDistance,m_obstacleLayerMask))
        {
            //前に通ったウェイポイントから次のウェイポイントへのベクトルを計算
            Vector3 currentWayPointToNextWayPoint = m_wayPointChecker.GetNextWayPoint() - m_wayPointChecker.GetCurrentWayPoint();

            //そのベクトルに対する右方向を計算
            Vector3 right = Vector3.Cross(Vector3.up, currentWayPointToNextWayPoint.normalized);

            //前に通ったウェイポイントから当たった障害物へのベクトルを計算
            Vector3 prevWayPointToHitObject = m_sphereCastHit.collider.gameObject.transform.position - m_wayPointChecker.GetCurrentWayPoint();

            //障害物の位置をライン上に投影
            float dot = Vector3.Dot(currentWayPointToNextWayPoint.normalized, prevWayPointToHitObject);

            //障害物の位置から最も近いライン上の座標を求める
            Vector3 nearestLinePos = m_wayPointChecker.GetCurrentWayPoint() + dot * currentWayPointToNextWayPoint.normalized;

            //障害物の位置からライン上の位置のベクトルを求める
            Vector3 obstacleToNearestLinePos = nearestLinePos - m_sphereCastHit.collider.gameObject.transform.position;

            //ベクトルの長さが短ければ(障害物がラインに近ければ)
            if(obstacleToNearestLinePos.magnitude < m_onLineLength)
            {
#if UNITY_EDITOR
                //デバッグ用　ライン上の位置を出力
                Debug.DrawRay(nearestLinePos, Vector3.up * 100.0f, Color.red);
#endif

                //ウェイポイントより右側を目指しているか左側を目指しているかで分岐させる
                if (m_shiftLength < 0.0f)
                {
                    LeftHandling();
                }
                else
                {
                    RightHandling();
                }
                return;
            }

#if UNITY_EDITOR
            //デバッグ用　ライン上の位置を出力
            Debug.DrawRay(nearestLinePos, Vector3.up * 100, Color.blue);
#endif

            //内積を計算する
            float angle = Vector3.Dot(right, prevWayPointToHitObject);

            //内積が0より大きければ障害物はウェイポイントをつなぐ線(センターライン)より右にある
            //＝ハンドルを左に切る
            if (angle > 0.0f)
            {
                LeftHandling();
            }
            else
            {
                RightHandling();
            }

            return;
        }

        //現在目指している位置へのベクトルを計算
        Vector3 toNextPoint = m_wayPointChecker.GetNextWayPoint() + m_targetOffset - this.transform.position;

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
#if UNITY_EDITOR
        //デバッグ用　AIが障害物を検知した場合描画
        if (m_sphereCastHit.collider != null && ((1 << m_sphereCastHit.collider.gameObject.layer) & m_obstacleLayerMask) != 0)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(transform.position, transform.forward * m_sphereCastHit.distance);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position + transform.forward * m_sphereCastHit.distance, m_sphereCastRadius);
        }
#endif
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

    ////////////////////////////////////////////////////
    public Rigidbody GetRigidBody
    {
        get 
        {
            return m_rigidbody;
        }
    }
    ////////////////////////////////////////////////////
}
