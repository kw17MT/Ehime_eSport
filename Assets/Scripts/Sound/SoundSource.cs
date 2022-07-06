using CriWare;
using UnityEngine;

namespace nsSound
{
    //�T�E���h�\�[�X�B
    public class SoundSource : MonoBehaviour
    {
        //�T�E���h�}�l�[�W���[�̃C���X�^���X�����邽�߂̕ϐ�
        private SoundManager m_soundManager;

        //�L���[�V�[�g�����[�h�����ǂ��� true:���[�h�� false:���[�h�ς�
        private bool m_cueSheetsAreLoading = true;
        //���̃T�E���h�\�[�X�ŗL�̃{�����[��
        private float m_sourceVolume = 1.0f;
        //�T�E���h�̕��ނɂ��{�����[��
        private float m_soundTypeVolume = 1.0f;

        //���̃T�E���h�\�[�X���Đ�����T�E���h�̕���
        private EnSoundTypes m_enSoundType = EnSoundTypes.enNone;

        //�Đ���~�̃t�F�[�h�A�E�g�����ǂ���
        private bool m_isDoingFadeOut = false;

        //CRIADX2�֘A
        //�L���[���̔z��
        private CriAtomEx.CueInfo[] m_cueInfoList = null;
        //�T�E���h���Đ����邽�߂̃v���C���[(�Đ��@)
        private CriAtomExPlayer m_atomExPlayer = null;
        //ACB�AAWB�t�@�C���̃L���[�����Ǘ�����N���X�B
        private CriAtomExAcb m_atomExAcb = null;
        //3D�֘A
        // 3D�T�E���h�̃��X�i�[(������)
        private CriAtomEx3dListener m_atomEx3Dlistener = null;
        // 3D�T�E���h�̃\�[�X(�鑤)
        private CriAtomEx3dSource m_atomEx3DSource = null;

        // Start is called before the first frame update
        private void Awake()
        {

        }

        private void OnDestroy()
        {
            //�v���C���[��j��
            if (m_atomExPlayer != null)
            {
                m_atomExPlayer.Dispose();
            }
            //3D���X�i�[�̔j��
            if (m_atomEx3Dlistener != null)
            {
                m_atomEx3Dlistener.Dispose();
            }
            //3D�T�E���h�\�[�X�̔j��
            if (m_atomEx3DSource != null)
            {
                m_atomEx3DSource.Dispose();
            }

        }
        // Update is called once per frame
        void Update()
        {
            Debug.Log(m_atomExPlayer.GetStatus());

            //�Đ����I�������A�����B
            if (m_atomExPlayer.GetStatus() == CriAtomExPlayer.Status.PlayEnd)
            {
                Destroy(this.gameObject.transform.root.gameObject);
            }

            //�t�F�[�h�A�E�g���Ă���Ȃ�΁A
            if (m_isDoingFadeOut == true)
            {
                DoFadeOut();
            }
        }

        //�������B�������A�L���[�V�[�g�����[�h�ς݂łȂ��ꍇ�͏��������s���Ȃ����߁A
        //������֐��Ăяo���̎n�߂ɂ��̊֐����Ăяo���ă��[�h�ς݂��ǂ����𒲂ׁA
        //���[�h�ς݂Ȃ�Έ�x�����������������s���B
        private bool Init()
        {
            //�L���[�V�[�g�����[�h�ς݂��ǂ���
            if (m_cueSheetsAreLoading == false)
            {
                // ���[�h�ς݂����珉�������������Ȃ�
                Debug.Log("�������ς݂ł��B");
                return true;
            }
            else
            {
                //�L���[�V�[�g�̃��[�h���������Ă�����A
                if (CriAtom.CueSheetsAreLoading == false)
                {
                    //���[�h�����ǂ�����false�ɁB(���[�h�ς݂�)
                    m_cueSheetsAreLoading = CriAtom.CueSheetsAreLoading;
                }
                //�܂����[�h���Ȃ�A�������Ȃ��B
                else
                {
                    Debug.Log("�܂��L���[�V�[�g�̃��[�h���ł��B");
                    return false;
                }
            }

            Debug.Log("���[�h�����������̂ŏ��������܂��B");

            //�T�E���h�}�l�[�W���[�̃C���X�^���X���擾�B
            m_soundManager = SoundManager.Instance;

            /* AtomExPlayer�̐��� */
            m_atomExPlayer = new CriAtomExPlayer();

            //�����ł��̃T�E���h�̕��ނɍ��킹���L���[�V�[�g�Ɖ��ʂ�ݒ肷��悤�ɂ���B
            switch (m_enSoundType)
            {
                case EnSoundTypes.enSE:
                    m_soundTypeVolume = m_soundManager.GetSEVolume();
                    m_atomExAcb = CriAtom.GetAcb("test02");
                    break;
                case EnSoundTypes.enBGM:
                    m_soundTypeVolume = m_soundManager.GetSEVolume();
                    m_atomExAcb = CriAtom.GetAcb("test03");
                    break;
                case EnSoundTypes.enNarration:
                    m_soundTypeVolume = m_soundManager.GetSEVolume();
                    m_atomExAcb = CriAtom.GetAcb("test03");
                    break;
                case EnSoundTypes.enCharaVoice:
                    m_soundTypeVolume = m_soundManager.GetSEVolume();
                    m_atomExAcb = CriAtom.GetAcb("test03");
                    break;
                default:
                    break;
            }
            //�L���[�̏����擾����B
            m_cueInfoList = m_atomExAcb.GetCueInfoList();

            //�t�F�[�_���A�^�b�`����B����ɂ���āA�t�F�[�h�̏������s����悤�ɂȂ�B
            m_atomExPlayer.AttachFader();

            return true;
        }

        //�T�E���h�̃^�C�v��ݒ肷��B(�K�{)
        //�T�E���h�\�[�X�𐶐�������A�܂����߂ɂ��̊֐����Ăяo���Ă��������B
        public void SetSoundType(EnSoundTypes enSoundType)
        {
            m_enSoundType = enSoundType;
        }

        //�T�E���h���Đ��J�n����B 
        //����:soundName�ɂ́A�T�E���h�̖��O��n���܂��B
        //�萔��p�ӂ���̂ŁAnsSound.������Names.m_k������ �̂悤�Ȍ`�ŗ��p���Ă��������B
        //SetFadeInTime�֐��Ńt�F�[�h�C���̎��Ԃ�ݒ肵�Ă����ꍇ�A�t�F�[�h�C�����s���܂��B
        //���̌�A�����Đ����ɂ�����xPlayStart�֐����Ăяo���ƁA�N���X�t�F�[�h���s���܂��B
        public void PlayStart(string soundName)
        {
            if (Init() != true)
            {
                return;
            }
            //�L���[�̐ݒ�
            m_atomExPlayer.SetCue(m_atomExAcb, soundName);
            //�Đ��J�n
            m_atomExPlayer.Start();
        }

        //�Đ��J�n����܂ł̎��Ԃ�ݒ肵�܂��B����:ms�ɓn���l��1000��1�b
        //���̊֐��ɒl��ݒ肵����PlayStart�֐����Ă΂��ƁA
        //�ݒ肵�����Ԃ��o���Ă��特�����Đ�����n�߂܂��B
        public void SetPlayStartOffset(int ms)
        {
            m_atomExPlayer.SetFadeInStartOffset(ms);
        }

        //���̃T�E���h�\�[�X�̉��ʂ�ݒ�(���̃T�E���h�Ƃ͊֌W�Ȃ��ŗL�̃p�����[�^)
        public void SetSourceVolume(float volume)
        {
            if (Init() != true)
            {
                return;
            }
            m_sourceVolume = volume;
            //�\�[�X�̉��ʂƁA���̃\�[�X�̕��ނ̉��ʂƁA�S�̂̉��ʂŁA�ŏI�̉��ʂ�ݒ肷��B
            m_atomExPlayer.SetVolume(m_sourceVolume * m_soundTypeVolume * m_soundManager.GetAllVolume());
            m_atomExPlayer.UpdateAll();
        }

        //�T�E���h���~����B���̃T�E���h�\�[�X�͏����܂��B
        public void Stop()
        {
            if (Init() != true)
            {
                return;
            }

            m_atomExPlayer.Stop();          
            Destroy(this.gameObject.transform.root.gameObject);
        }

        //�t�F�[�h�A�E�g���܂��B�t�F�[�h�A�E�g���I���ƁA���̃T�E���h�\�[�X�͏����܂��B
        //�������t�F�[�h�A�E�g��t�F�[�h�C�����ɂ��̊֐����Ă΂ꂽ�ꍇ�A�Ԃ؂�ɂȂ�܂��B
        //SetFadeOutTime�֐��Őݒ肵�����Ԃ͓K������܂��B
        public void FadeOutStart()
        {
            if (Init() != true)
            {
                return;
            }

            //�t�F�[�h�A�E�g���ɐݒ�B
            m_isDoingFadeOut = true;
            m_atomExPlayer.Stop(false);
        }

        //�t�F�[�h�A�E�g���̏���
        private void DoFadeOut()
        {
            //�t�F�[�h�A�E�g���I�������A�����B
            if (m_atomExPlayer.GetStatus() == CriAtomExPlayer.Status.Stop)
            {
                Destroy(this.gameObject.transform.root.gameObject);
            }
        }

        //�|�[�Y�B�������ꎞ��~���܂��B
        //Resume�֐����ĂԂ��ƂŃ|�[�Y����������A�ꎞ��~�����Ƃ��납��Đ�����܂��B
        public void Pause()
        {
            if (Init() != true)
            {
                return;
            }
            m_atomExPlayer.Pause();
        }

        //�|�[�Y��Ԃ��������āA�������ꎞ��~����Ă����Ƃ��납��Đ����܂��B
        public void Resume()
        {
            if (Init() != true)
            {
                return;
            }
            m_atomExPlayer.Resume(CriAtomEx.ResumeMode.AllPlayback);
        }

        //���[�v�Đ����邩�ǂ�����ݒ肵�܂��Btrue:���[�v����B false:���[�v���Ȃ��B
        public void SetLoop(bool sw)
        {
            if (Init() != true)
            {
                return;
            }
            m_atomExPlayer.Loop(sw);
        }

        //�s�b�`��ݒ肵�܂��B����:pitch�́A0.0f���ʏ�B100.0f�Ŕ����オ��A-100.0f�Ŕ���������܂��B
        public void SetPitch(float pitch)
        {
            if (Init() != true)
            {
                return;
            }
            m_atomExPlayer.SetPitch(pitch);
            m_atomExPlayer.UpdateAll();
        }

        //�V�[�P���X�Đ����V�I�̐ݒ�炵���ł��B
        public void SetPlaybackRatio(float ratio)
        {
            if (Init() != true)
            {
                return;
            }
            m_atomExPlayer.SetPlaybackRatio(ratio);
            m_atomExPlayer.UpdateAll();
        }

        ////////////////////////////////////////////////////////////////////////
        
        //�t�F�[�h�C���E�t�F�[�h�A�E�g�֌W�B
        //������SetPlayStartOffset�֐��𗘗p���邱�ƂŁA�N���X�t�F�[�h���s���܂��B

        //�t�F�[�h�C���̎��Ԃ�ݒ肵�܂��B����:ms�ɓn���l��1000��1�b
        //�t�F�[�h�C���̎��Ԃ�ݒ肷��ƁAPlayStart�֐����Ăяo���ꂽ���A�����Ńt�F�[�h�C�����s���܂��B
        public void SetFadeInTime(int ms)
        {
            m_atomExPlayer.SetFadeInTime(ms);
        }    
        //�t�F�[�h�A�E�g�̎��Ԃ�ݒ肵�܂��B����:ms�ɓn���l��1000��1�b
        public void SetFadeOutTime(int ms)
        {
            m_atomExPlayer.SetFadeOutTime(ms);
        }

        /////////////////////////////////////////////////////////////

        //3D�T�E���h�֌W�BBe3DSound()���Ăяo���ƁA3D�T�E���h�ɂȂ�܂��B

        //3D�T�E���h�ɂ���B
        public void Be3DSound()
        {
            if (Init() != true)
            {
                return;
            }
            //3D�T�E���h�̃��X�i�̍쐬
            m_atomEx3Dlistener = new CriAtomEx3dListener();
            //3D�T�E���h�̃\�[�X�̍쐬
            m_atomEx3DSource = new CriAtomEx3dSource();
            // �\�[�X�A���X�i���v���[���ɐݒ�
            m_atomExPlayer.Set3dListener(m_atomEx3Dlistener);
            m_atomExPlayer.Set3dSource(m_atomEx3DSource);
        }

        //3D�T�E���h�p�B���X�i�[�̈ʒu��ݒ肷��B��{�I�ɃQ�[���J�����̈ʒu��ݒ肷��B
        public void Set3DListenerPos(Vector3 pos)
        {
            if (Init() != true)
            {
                return;
            }
            m_atomEx3Dlistener.SetPosition(pos.x, pos.y, pos.z);
            m_atomEx3Dlistener.Update();
        }
        public void Set3DListenerPos(float x, float y, float z)
        {
            if (Init() != true)
            {
                return;
            }
            m_atomEx3Dlistener.SetPosition(x, y, z);
            m_atomEx3Dlistener.Update();
        }
        //3D�T�E���h�p�B�T�E���h����ʒu��ݒ肷��B
        public void Set3DSourcePos(Vector3 pos)
        {
            if (Init() != true)
            {
                return;
            }
            m_atomEx3DSource.SetPosition(pos.x, pos.y, pos.z);
            m_atomEx3DSource.Update();
        }
        public void Set3DSourcePos(float x, float y, float z)
        {
            if (Init() != true)
            {
                return;
            }
            m_atomEx3DSource.SetPosition(x, y, z);
            m_atomEx3DSource.Update();
        }

    }
}