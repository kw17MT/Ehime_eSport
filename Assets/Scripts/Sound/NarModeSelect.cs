using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nsSound
{
    public class NarModeSelect : OutGameNarBase
    {       
        //���̃V�[���̏��
        private ModeChange m_sceneScript = null;

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
            m_sceneScript = GameObject.Find("SceneManager").GetComponent<ModeChange>();
            m_selectStateNo = m_sceneScript.GetNowSelectState();

            //2.
            //���ʂ̃i���[�V���������X�g�ɉ����Ă���
            m_commonNarList = new List<string>();
            m_commonNarList.Add(nsSound.NarAdvanceNames.m_SAYUUHURIKKUDE);
            m_commonNarList.Add(nsSound.NarModeNames.m_ASOBIKATAWOERABERUYO);
            m_commonNarList.Add(nsSound.NarAdvanceNames.m_ERABINAOSHITAITOKIHAGAMENHIDARIUEWOTAPSHITENE);

            //3.
            //�I�����C�����[�h�I�����̃i���[�V�����̃��X�g
            List<string> onlineModeNarList = new List<string>();
            onlineModeNarList.Add(nsSound.NarModeNames.m_ONLINETAISEN);
            onlineModeNarList.Add(nsSound.NarModeNames.m_HOKANOPLAYERTOTAISENDEKIRUYO);

            //CPU���[�h�I�����̃i���[�V�����̃��X�g
            List<string> cpuModeNarList = new List<string>();
            cpuModeNarList.Add(nsSound.NarModeNames.m_CPUTAISEN);
            cpuModeNarList.Add(nsSound.NarModeNames.m_COMPUTERTOTAISENDEKIRUYO);

            //�^�C���A�^�b�N���[�h�I�����̃i���[�V�����̃��X�g
            //List<string> timeAttackModeNarList = new List<string>();
            //timeAttackModeNarList.Add(nsSound.NarModeNames.m_TIMEATTACK);
            //timeAttackModeNarList.Add(nsSound.NarModeNames.m_SHINKIROKUWOMEZASOU);

            //�ݒ胂�[�h�I�����̃i���[�V�����̃��X�g
            List<string> settingModeNarList = new List<string>();
            settingModeNarList.Add(nsSound.NarModeNames.m_SETTEI);
            settingModeNarList.Add(nsSound.NarModeNames.m_OTOGAMENNOSETTEIGADEKIRUYO);

            //4.
            //�i���[�V�����̃��X�g���A���X�g�ɉ����Ă���(enum�̏��ԂƓ����ɂ��邱�ƁI)
            m_narList = new List<List<string>>();
            m_narList.Add(onlineModeNarList);
            m_narList.Add(cpuModeNarList);
            //m_narList.Add(timeAttackModeNarList);
            m_narList.Add(settingModeNarList);
        }

		//���݂̑I����Ԃ𒲂ׂ�Breturn:�ς���Ă��邩�ǂ���
		protected override bool CheckNowSelectState()
        {
            //���[�h�̑I�����ς���Ă�����A
            if (m_selectStateNo != (int)m_sceneScript.GetNowSelectState() && m_narList.Count > (int)m_sceneScript.GetNowSelectState())
            {
                //���[�h�̑I�����X�V
                m_selectStateNo = (int)m_sceneScript.GetNowSelectState();
                return true;
            }
            return false;
        }
    }
}