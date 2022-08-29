using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nsSound
{
    public class NarStageSelect : OutGameNarBase
    {
        //���̃V�[���̏��
        private StageSelectChange m_sceneScript = null;

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
            m_sceneScript = GameObject.Find("SceneManager").GetComponent<StageSelectChange>();
            m_selectStateNo = m_sceneScript.GetNowSelectState();

            //2.
            //���ʂ̃i���[�V���������X�g�ɉ����Ă���
            m_commonNarList = new List<string>();
            m_commonNarList.Add(nsSound.NarAdvanceNames.m_SAYUUHURIKKUDE);
            m_commonNarList.Add(nsSound.NarStageSeleNames.m_STAGEWOERABERUYO);
            m_commonNarList.Add(nsSound.NarAdvanceNames.m_ERABINAOSHITAITOKIHAGAMENHIDARIUEWOTAPSHITENE);

            //3.
            //�����X�e�[�W�I�����̃i���[�V�����̃��X�g
            List<string> straightStageNarList = new List<string>();
            straightStageNarList.Add(nsSound.NarStageSeleNames.m_CHOKUSENNNOSTAGEDAYO);

            //�ȉ~�`�X�e�[�W�I�����̃i���[�V�����̃��X�g
            List<string> ellipseStageNarList = new List<string>();
            ellipseStageNarList.Add(nsSound.NarStageSeleNames.m_DAENKEINOSTAGEDAYO);

            //4.
            //�i���[�V�����̃��X�g���A���X�g�ɉ����Ă���(enum�̏��ԂƓ����ɂ��邱�ƁI)
            m_narList = new List<List<string>>();
            m_narList.Add(straightStageNarList);
            m_narList.Add(ellipseStageNarList);
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