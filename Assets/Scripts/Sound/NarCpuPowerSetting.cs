using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nsSound
{
    public class NarCpuPowerSetting : OutGameNarBase
    {
        //���̃V�[���̏��
        private CpuPowerSettingChange m_sceneScript = null;

        //Init()�ł�邱�ƁB
        //1.�V�[���̏����擾
        //2.���ʂ̃i���[�V������p�ӂ���B
        //3.�i���[�V�����̃��X�g�̗p��
        //4.�i���[�V�����̃��X�g�̃��X�g�ɁA3�̃��X�g��Add()����B(���̃V�[���̑I����Ԃ̗񋓑̂̏��Ԓʂ�ɁI)
        //�����񋓑̂̏��Ԃ��ς������A�i���[�V�����̃��X�g�̃��X�g��Add()���鏇�Ԃ��ς��邱�ƁI
        protected override void Init()
        {
            //1.
            //�V�[���̏����擾
            m_sceneScript = GameObject.Find("SceneManager").GetComponent<CpuPowerSettingChange>();
            m_selectStateNo = m_sceneScript.GetNowSelectState();

            //2.
            //���ʂ̃i���[�V���������X�g�ɉ����Ă���
            m_commonNarList = new List<string>();
            m_commonNarList.Add(nsSound.NarAdvanceNames.m_SAYUUHURIKKUDE);
            m_commonNarList.Add(nsSound.NarCPUSeleNames.m_COMPUTERNOTUYOSAWOERABERUYO);
            m_commonNarList.Add(nsSound.NarAdvanceNames.m_ERABINAOSHITAITOKIHAGAMENHIDARISHITAWONAGAOSHISHITENE);

            //3.
            //�ア��I�����̃i���[�V�����̃��X�g
            List<string> weakNarList = new List<string>();
            weakNarList.Add(nsSound.NarCPUSeleNames.m_YASASHII);

            //���ʂ�I�����̃i���[�V�����̃��X�g
            List<string> normalNarList = new List<string>();
            normalNarList.Add(nsSound.NarCPUSeleNames.m_HUTSUU);

            //������I�����̃i���[�V�����̃��X�g
            List<string> strongModeNarList = new List<string>();
            strongModeNarList.Add(nsSound.NarCPUSeleNames.m_TSUYOI);

            //4.
            //�i���[�V�����̃��X�g���A���X�g�ɉ����Ă���(enum�̏��ԂƓ����ɂ��邱�ƁI)
            m_narList = new List<List<string>>();
            m_narList.Add(weakNarList);
            m_narList.Add(normalNarList);
            m_narList.Add(strongModeNarList);
        }

        //���݂̑I����Ԃ𒲂ׂ�Breturn:�ς���Ă��邩�ǂ���
        protected override bool CheckNowSelectState()
        {
            //���[�h�̑I�����ς���Ă�����A
            if (m_selectStateNo != (int)m_sceneScript.GetNowSelectState())
            {
                //���[�h�̑I�����X�V
                m_selectStateNo = (int)m_sceneScript.GetNowSelectState();
                return true;
            }
            return false;
        }
    }
}