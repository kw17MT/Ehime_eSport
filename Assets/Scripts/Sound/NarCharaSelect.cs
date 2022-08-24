using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nsSound
{
    public class NarCharaSelect : OutGameNarBase
    {
        //���̃V�[���̏��
        private CharaSelectChange m_sceneScript = null;

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
            m_sceneScript = GameObject.Find("SceneManager").GetComponent<CharaSelectChange>();
            m_selectStateNo = m_sceneScript.GetNowSelectState();

            //2.
            //���ʂ̃i���[�V���������X�g�ɉ����Ă���
            m_commonNarList = new List<string>();
            m_commonNarList.Add(nsSound.NarAdvanceNames.m_SAYUUHURIKKUDE);
            m_commonNarList.Add(nsSound.NarCharaSeleNames.m_CHARACTERWOERABERUYO);
            m_commonNarList.Add(nsSound.NarAdvanceNames.m_ERABINAOSHITAITOKIHAGAMENHIDARISHITAWONAGAOSHISHITENE);

            //3.
            //�݂���܂�I�����̃i���[�V�����̃��X�g
            List<string> mikanmaruNarList = new List<string>();
            mikanmaruNarList.Add(nsSound.NarCharaSeleNames.m_MIKANMARU);
            mikanmaruNarList.Add(nsSound.NarCharaSeleNames.m_BARANSUGATADAYO);

            //�����������I�����̃i���[�V�����̃��X�g
            List<string> remoremonNarList = new List<string>();
            remoremonNarList.Add(nsSound.NarCharaSeleNames.m_REMOREMON);
            remoremonNarList.Add(nsSound.NarCharaSeleNames.m_SPEEDGATADAYO);

            //�L�E�C�[���I�����̃i���[�V�����̃��X�g
            List<string> kiwinNarList = new List<string>();
            kiwinNarList.Add(nsSound.NarCharaSeleNames.m_KIUIIN);
            kiwinNarList.Add(nsSound.NarCharaSeleNames.m_POWERFULGATADAYO);

            //Ms.���C�~���B�[�I�����̃i���[�V�����̃��X�g
            List<string> msLimilyNarList = new List<string>();
            msLimilyNarList.Add(nsSound.NarCharaSeleNames.m_MISURAIMIRII);
            msLimilyNarList.Add(nsSound.NarCharaSeleNames.m_TECHNICGATADAYO);

            //4.
            //�i���[�V�����̃��X�g���A���X�g�ɉ����Ă���(enum�̏��ԂƓ����ɂ��邱�ƁI)
            m_narList = new List<List<string>>();
            m_narList.Add(mikanmaruNarList);
            m_narList.Add(remoremonNarList);
            m_narList.Add(kiwinNarList);
            m_narList.Add(msLimilyNarList);
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