using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharaReaction : MonoBehaviour
{
    [SerializeField] int m_myNumber = 0;                    //�L�����ԍ�
    private float m_rolling = 0.0f;                         //��]�x����
    Vector3 currentScale = new Vector3(0.5f,0.5f,0.5f);                     //���݂̊g�嗦
    private Quaternion rot = Quaternion.identity;           //�L�����̉�]
    private CharaSelectChange m_charaSelect = null;         //���ݑI�����Ă���L�����ԍ��ۗL�C���X�^���X

    const float ROLLING_RATE = 50.0f;

    // Start is called before the first frame update
    void Start()
    {
        m_charaSelect = GameObject.Find("SceneManager").GetComponent<CharaSelectChange>();
    }

    // Update is called once per frame
    void Update()
    {
        //�Q�[���^�C���ŉ�]�����Z
        m_rolling += Time.deltaTime * ROLLING_RATE;
        //Y���܂��ɉ�]
        rot = Quaternion.Euler(0.0f, m_rolling, 0.0f);
        this.transform.rotation = rot;

        Vector3 scaleChangeRate = new Vector3(Time.deltaTime, Time.deltaTime, Time.deltaTime);
        //���ݑI������Ă���L�����������̔ԍ��Ȃ��
        if (m_myNumber == m_charaSelect.GetNowSelectState())
		{

            currentScale += scaleChangeRate;
            //�ő�g�嗦��1�ɂ���
            if (currentScale.x >= 1.0f)
			{
                currentScale = Vector3.one;
            }
		}
		else
		{
            currentScale -= scaleChangeRate;
            if(currentScale.x <= 0.5f)
			{
                currentScale = new Vector3(0.5f, 0.5f, 0.5f) ;
			}
		}
        this.transform.localScale = currentScale;
    }
}
