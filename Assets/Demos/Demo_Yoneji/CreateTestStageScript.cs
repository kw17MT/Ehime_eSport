using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateTestStageScript : MonoBehaviour
{
    // Serialize Field
    [SerializeField]
    GameObject m_cylinderObj = null;
    [SerializeField]
    GameObject m_pointLight = null;
    [SerializeField]
    int m_cylinderNumOfVertical = 10;
    [SerializeField]
    int m_cylinderNumOfHorizontal = 10;
    [SerializeField]
    int m_lightNum = 20;
    [SerializeField]
    float m_lightMoveRange = 50.0f;
    [SerializeField]
    float m_lightMoveSpeed = 200.0f;
    [SerializeField]
    float m_lightIntensity = 10.0f;
    [SerializeField]
    float m_lightRange = 20.0f;


    // Field
    List<GameObject> m_pointLightList = new List<GameObject>();
    List<Vector3> m_toLightPosList = new List<Vector3>();
    const int m_kClinderSpapce = 3;
    const int m_kObjectHeight = 1;


    // Unity Function

    // Start is called before the first frame update
    void Start()
    {
        CreateCylinders();

        CreatePointLight();
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePointLightPosition();
    }

    // Function

    /// <summary>
    /// 指定された数だけシリンダーを生成する
    /// </summary>
    void CreateCylinders()
    {
        if (m_cylinderObj == null)
        {
            return;
        }


        Vector3 offset = 
            new Vector3(
                m_kClinderSpapce * m_cylinderNumOfHorizontal / 2.0f,
                0,
                m_kClinderSpapce * m_cylinderNumOfVertical / 2.0f
                );

        for (int x = 0; x < m_cylinderNumOfHorizontal; x++)
        {
            for (int y = 0; y < m_cylinderNumOfVertical; y++)
            {
                Vector3 pos = new Vector3(m_kClinderSpapce * x, m_kObjectHeight, m_kClinderSpapce * y);
                pos -= offset;

                Instantiate(m_cylinderObj, pos, Quaternion.identity);
            }
        }
    }

    /// <summary>
    /// 指定された数だけポイントライトを生成する
    /// </summary>
    void CreatePointLight()
    {

        if (m_pointLight == null)
        {
            return;
        }

        for (int i = 0; i < m_lightNum; i++)
        {
            // 原点を中心にランダムな角度、距離のベクトルの先に配置する

            Quaternion qRot = Quaternion.AngleAxis(Random.Range(0.0f, 360.0f), Vector3.up);
            float range = Random.Range(1.0f, m_lightMoveRange);
            Vector3 toLightPos = new Vector3(0.0f, m_kObjectHeight, range);
            toLightPos = qRot * toLightPos;

            GameObject pointLight = Instantiate(m_pointLight, toLightPos, Quaternion.identity);

            // 生成したライトの設定
            Color color = Color.white;
            pointLight.GetComponent<Light>().color = color;
            pointLight.GetComponent<Light>().intensity = m_lightIntensity;
            pointLight.GetComponent<Light>().range = m_lightRange;

            m_pointLightList.Add(pointLight);
            m_toLightPosList.Add(toLightPos);
        }
    }

    /// <summary>
    /// ポイントライトの座標を更新
    /// </summary>
    void UpdatePointLightPosition()
    {
        if (m_pointLight == null)
        {
            return;
        }

        for (int i = 0; i < m_lightNum; i++)
        {
            Quaternion qRot = Quaternion.AngleAxis(m_lightMoveSpeed * Time.deltaTime, Vector3.up);

            m_toLightPosList[i] = qRot * m_toLightPosList[i];
            m_pointLightList[i].transform.position = m_toLightPosList[i];
        }
    }

}
