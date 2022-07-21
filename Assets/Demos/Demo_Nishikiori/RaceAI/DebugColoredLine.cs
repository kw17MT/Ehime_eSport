using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class DebugColoredLine : MonoBehaviour
{
#if UNITY_EDITOR

    //���̎��(�����A�O��)
    private enum LineType
    {
        Inner,
        Outer,
    }

    [SerializeField]
    private LineType m_lineType;                                            //�����̐����A�O���̐���

    private Color[] m_drawColors = new Color[] { Color.blue,Color.red};     //�`��F(��:�A�O:��)
    
    private GameObject m_parent = null;                                     //�e�̃Q�[���I�u�W�F�N�g(�E�F�C�|�C���g)
#endif

    public void Awake()
    {
#if UNITY_EDITOR
        //�e�̃I�u�W�F�N�g(�E�F�C�|�C���g)���擾
        m_parent = gameObject.transform.parent.gameObject;
#endif
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
        //�e���Ȃ���Ή������Ȃ��B
        if(m_parent == null)
        {
            return;
        }

        //���S����`��ʒu�܂ł̋���(BoxCollider��X�����̃T�C�Y�̔���)���v�Z
        float lineDistance = m_parent.GetComponent<BoxCollider>().size.x / 2;

        //�����̐��Ȃ狗���̕����𔽓]
        if(m_lineType == LineType.Inner)
        {
            lineDistance = -lineDistance;
        }

        //����������̕`��ʒu���Z�b�g
        gameObject.transform.localPosition = new Vector3(lineDistance, 0.0f, 0.0f);

        //�`��
        Debug.DrawRay(gameObject.transform.position, Vector3.up * 3.0f, m_drawColors[(int)m_lineType]);
#endif
    }
}
