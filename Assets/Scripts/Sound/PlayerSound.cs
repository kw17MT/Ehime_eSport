using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nsSound
{
    public class PlayerSound : MonoBehaviour
    {
        //�萔
        //�A�N�Z�����̃s�b�`�̍���
        private const int m_accelePitchHeight = 50;
        //3D�T�E���h����������ŏ�����
        private const float m_3DSoundMinDistance = 1.0f;
        //3D�T�E���h����������ő勗��
        private const float m_3DSoundMaxDistance = 20.0f;

        //3D�T�E���h�p
        private Vector3 m_ssPos;
        private Vector3 m_listenerPos;
        private Vector3 m_listenerDir;

        //���̃Q�[����ʂ̃v���C���[�̏��
        private Transform m_ownPlayerTransform = null;
        private AvatarController m_ownPlayerAvatarController = null;

        //���̃Q�[����ʂ̃v���C���[�̃A�C�e�����
        private ObtainItemController m_obtainItemController = null;

        //�J�����̈ʒu�̏��
        private Transform m_mainCameraTransform = null;

        private AvatarController m_myAvatarContoroller = null;

        private SoundSource m_engineSE = null;
        private SoundSource m_acceleSE = null;
        private float m_acceleSEPitch = 0.0f;

        //�A�C�e���X���b�g��]���̃T�E���h
        private SoundSource m_itemSlotLotterySE = null;
        //�A�C�e���X���b�g�̃T�E���h��炷�t���O
        private bool m_itemSlotSoundPlayFlag = false;

        //�L���[��SE�̃\�[�X
        nsSound.SoundSource m_trainWhistleSS = null;
        nsSound.SoundSource m_trainSSRun = null;
        //�X�^�[��BGM�̃\�[�X
        private SoundSource m_starSS = null;

        //�A�C�e���������Ă��邩�ǂ���
        private bool m_isHavingItem = false;
        //�����Ă���A�C�e��
        private int m_haveItem = -1;

        //�U�����ꂽ���ǂ���
        private bool m_isAttacked = false;

        // Start is called before the first frame update
        void Start()
        {
            //�G���W����
            m_engineSE = new GameObject("SoundSource").AddComponent<nsSound.SoundSource>();
            m_engineSE.SetSoundType(nsSound.EnSoundTypes.enSE);
            m_engineSE.Be3DSound();
            m_engineSE.SetLoop(true);
            m_engineSE.Set3DMinMaxDistance(m_3DSoundMinDistance, m_3DSoundMaxDistance);
            m_engineSE.PlayStart(nsSound.SENames.m_engine);


            //�A�N�Z����
            m_acceleSE = new GameObject("SoundSource").AddComponent<nsSound.SoundSource>();
            m_acceleSE.SetSoundType(nsSound.EnSoundTypes.enSE);
            m_acceleSE.Be3DSound();
            m_acceleSE.SetLoop(true);
            m_acceleSE.Set3DMinMaxDistance(m_3DSoundMinDistance, m_3DSoundMaxDistance);
            m_acceleSE.PlayStart(nsSound.SENames.m_accele);

            Init();

        }

        void Init()
        {
            //���̃Q�[���I�u�W�F�N�g���v���C���[���M�Ȃ��
            if (this.gameObject.name == "OwnPlayer")
            {
                m_obtainItemController = this.gameObject.GetComponent<ObtainItemController>();
                m_ownPlayerTransform = this.gameObject.transform;
                m_ownPlayerAvatarController = this.gameObject.GetComponent<AvatarController>();
                m_myAvatarContoroller = this.gameObject.GetComponent<AvatarController>();
            }
            //�v���C���[�ȊO�Ȃ��
            else
            {
                m_ownPlayerTransform = GameObject.Find("OwnPlayer").transform;
                m_ownPlayerAvatarController = GameObject.Find("OwnPlayer").gameObject.GetComponent<AvatarController>();
                m_myAvatarContoroller = this.gameObject.GetComponent<AvatarController>();
            }
            //���C���J�����̏��
            m_mainCameraTransform = GameObject.Find("Main Camera").transform;
        }

        // Update is called once per frame
        void Update()
        {
            //�v���C���[���S�[�����Ă�����A�I��
            if (m_ownPlayerAvatarController.GetGoaled())
            {
                Destroy(this);
            }

            //3D�T�E���h�̏��̍X�V
            Update3DSoundStatus();

            //�A�N�Z����
            AcceleSound();

            TrainSound();
            StarSound();

            //�v���C���[���M�̂Ƃ��̂ݖ炷��
            if (this.gameObject.name == "OwnPlayer")
            {
                ItemGetSound();
                ItemUseSound();
                //�U�����ꂽ��
                Attacked();

            }
        }

        //3D�T�E���h�̏��(�ʒu�A������)���X�V
        private void Update3DSoundStatus()
        {
            //�v���C���[���M�̂Ƃ�
            if (this.gameObject.name == "OwnPlayer")
            {
                m_ssPos = m_ownPlayerTransform.position;
                m_listenerPos = m_mainCameraTransform.position + GameObject.Find("Main Camera").transform.forward * 4.5f;
                m_listenerDir = m_mainCameraTransform.forward;
            }
            //���̑��̃v���C���[�̎�
            else
            {
                m_ssPos = this.GetComponent<RaceAIScript>().GetRigidBody.position;
                m_listenerPos = m_ownPlayerTransform.position;
                m_listenerDir = m_mainCameraTransform.forward;
            }
        }

        private void OnDestroy()
        {
            if (m_engineSE != null)
            {
                m_engineSE.Stop();
            }
            if (m_acceleSE != null)
            {
                m_acceleSE.Stop();
            }
            if (m_itemSlotLotterySE != null)
            {
                m_itemSlotLotterySE.Stop();
            }
            if (m_starSS != null)
            {
                m_starSS.Stop();
            }
            BGM.Instance.SetVolume(1.0f);
        }

        //�A�N�Z�����̍Đ��B�ړ����x�ɍ��킹�āA�s�b�`��ς���B
        private void AcceleSound()
        {
            //�Ԃ̑���
            Vector3 kartVelocity = this.GetComponent<RaceAIScript>().GetRigidBody.velocity;
            kartVelocity.y = 0.0f;
            float kartSpeed = kartVelocity.magnitude;

            //�Ԃ��قړ����Ă��Ȃ���΁A���������B
            if (kartSpeed <= 1.0f)
            {
                m_acceleSE.SetSourceVolume(0.0f);
            }
            //�Ԃ������Ă���΁A���𗬂��B
            else
            {
                m_acceleSE.SetSourceVolume(1.0f);
            }

            //�Ԃ̑����ɍ��킹�ăs�b�`��ݒ�
            m_acceleSEPitch = kartSpeed * m_accelePitchHeight;
            m_acceleSE.SetPitch(m_acceleSEPitch);

            //3D�T�E���h�ɁB
            m_acceleSE.Set3DSourcePos(m_ssPos);
            m_acceleSE.Set3DListenerPos(m_listenerPos);
            m_acceleSE.Set3DListenerDir(m_listenerDir);

            //�G���W�������B
            m_engineSE.Set3DSourcePos(m_ssPos);
            m_engineSE.Set3DListenerPos(m_listenerPos);
            m_engineSE.Set3DListenerDir(m_listenerDir);
        }

        //�U�����ꂽ����SE
        private void Attacked()
        {
            if (m_isAttacked == false)
            {
                if (m_ownPlayerAvatarController.GetIsAttacked())
                {
                    m_isAttacked = true;

                    SoundSource attackedSS = new GameObject("SoundSource").AddComponent<nsSound.SoundSource>();
                    attackedSS.SetSoundType(nsSound.EnSoundTypes.enSE);
                    attackedSS.Be3DSound();
                    attackedSS.PlayStart(nsSound.SENames.m_attacked);

                    //attackedSS.Set3DSourcePos(m_ssPos);
                    //attackedSS.Set3DListenerPos(m_listenerPos);
                    //attackedSS.Set3DListenerDir(m_listenerDir);                 
                }
            }
            else
            {
                m_isAttacked = m_ownPlayerAvatarController.GetIsAttacked();
            }

        }

        private void ItemGetSound()
        {
            //�A�C�e������肵����
            if (m_obtainItemController.GetObtainItemType() != -1)
            {
                //�A�C�e���X���b�g�̒��I���Ȃ�(���I���I����ĂȂ��Ȃ�)
                if (m_obtainItemController.GetLotteryFinish() == false)
                {
                    if (m_itemSlotSoundPlayFlag == false)
                    {
                        //�A�C�e���X���b�g�̃T�E���h��炷�t���O�����ɂ���B
                        m_itemSlotSoundPlayFlag = true;

                        //�A�C�e�����I�T�E���h�̊J�n
                        m_itemSlotLotterySE = new GameObject("SoundSource").AddComponent<nsSound.SoundSource>();
                        m_itemSlotLotterySE.SetSoundType(nsSound.EnSoundTypes.enSE);
                        m_itemSlotLotterySE.Be3DSound();
                        m_itemSlotLotterySE.SetLoop(true);
                        m_itemSlotLotterySE.PlayStart(nsSound.SENames.m_itemSlotLottery);
                    }
                }
                //���I���I�������
                else
                {
                    if (m_itemSlotSoundPlayFlag)
                    {
                        //�A�C�e�����I���̏I�� 
                        m_itemSlotLotterySE.Stop();
                        SoundSource itemSlotLotteryEndSS = new GameObject("SoundSource").AddComponent<nsSound.SoundSource>();
                        itemSlotLotteryEndSS.SetSoundType(nsSound.EnSoundTypes.enSE);
                        itemSlotLotteryEndSS.Be3DSound();
                        itemSlotLotteryEndSS.SetLoop(false);
                        itemSlotLotteryEndSS.PlayStart(nsSound.SENames.m_itemSlotLotteryEnd);

                        //�A�C�e���X���b�g�̉����Đ����I������̂Ńt���O��false�ɁB
                        m_itemSlotSoundPlayFlag = false;
                    }
                }
            }
        }

        //�A�C�e�����g���Ƃ��̃T�E���h
        private void ItemUseSound()
        {
            //���������Ă��Ȃ����
            if (m_isHavingItem == false)
            {
                //�������Ă���A�C�e���̎�ނ��擾
                m_haveItem = m_obtainItemController.GetObtainItemType();

                if (m_haveItem != -1)
                {
                    //�A�C�e���������Ă���ɐݒ�
                    m_isHavingItem = true;
                }
            }
            //�A�C�e���������Ă���Ƃ��A
            else
            {
                //�A�C�e���������Ȃ��Ă�����A�g�p�����Ɣ���
                if (m_obtainItemController.GetObtainItemType() == -1)
                {
                    //�g�����A�C�e���ɍ��킹�ăT�E���h���Đ�
                    switch (m_haveItem)
                    {
                        case 0:
                            //��𗎂Ƃ����̍Đ�
                            nsSound.SoundSource orangePeelSS = new GameObject("SoundSource").AddComponent<nsSound.SoundSource>();
                            orangePeelSS.SetSoundType(nsSound.EnSoundTypes.enSE);
                            orangePeelSS.Be3DSound();
                            orangePeelSS.PlayStart(nsSound.SENames.m_dropOrangePeel);
                            break;
                        case 1:
                            //����SE�̍Đ�
                            nsSound.SoundSource orangeJetSS = new GameObject("SoundSource").AddComponent<nsSound.SoundSource>();
                            orangeJetSS.SetSoundType(nsSound.EnSoundTypes.enSE);
                            orangeJetSS.Be3DSound();
                            orangeJetSS.PlayStart(nsSound.SENames.m_dash);
                            break;
                        case 2:
                            //��C�̉��̍Đ�
                            nsSound.SoundSource snapperCannonSS = new GameObject("SoundSource").AddComponent<nsSound.SoundSource>();
                            snapperCannonSS.SetSoundType(nsSound.EnSoundTypes.enSE);
                            snapperCannonSS.Be3DSound();
                            snapperCannonSS.PlayStart(nsSound.SENames.m_cannon);
                            break;
                        case 3:
                            break;
                        case 4:                           
                            break;
                        default:
                            break;

                    }

                    //�A�C�e���������Ă��Ȃ���Ԃ�
                    m_isHavingItem = false;
                    m_haveItem = -1;
                }
            }
        }

        private void TrainSound()
        {
            //�L���[�̎��͓��ꔻ��B�g���Ă������ɂ̓A�C�e���������Ȃ����߁B
            if (m_myAvatarContoroller != null && m_myAvatarContoroller.GetIsUsingKiller() == true)
            {
                if (m_trainWhistleSS == null)
                {
                    //��Ԃ̉��̍Đ�
                    m_trainWhistleSS = new GameObject("SoundSource").AddComponent<nsSound.SoundSource>();
                    m_trainWhistleSS.SetSoundType(nsSound.EnSoundTypes.enSE);
                    m_trainWhistleSS.Be3DSound();
                    m_trainWhistleSS.PlayStart(nsSound.SENames.m_trainWhistle);
                }
                else
                {
                    m_trainWhistleSS.Set3DSourcePos(m_ssPos);
                    m_trainWhistleSS.Set3DListenerPos(m_listenerPos);
                    m_trainWhistleSS.Set3DListenerDir(m_listenerDir);       
                }
                if (m_trainSSRun == null)
                {
                    m_trainSSRun = new GameObject("SoundSource").AddComponent<nsSound.SoundSource>();
                    m_trainSSRun.SetSoundType(nsSound.EnSoundTypes.enSE);
                    m_trainSSRun.Be3DSound();
                    m_trainSSRun.PlayStart(nsSound.SENames.m_trainRun);
                }
                else
                {
                    m_trainSSRun.Set3DSourcePos(m_ssPos);
                    m_trainSSRun.Set3DListenerPos(m_listenerPos);
                    m_trainSSRun.Set3DListenerDir(m_listenerDir);
                }
            }
        }
        private void StarSound()
        {
            //�X�^�[�̎��͓��ꔻ��
            if (m_myAvatarContoroller != null && m_myAvatarContoroller.GetIsUsingStar() == true)
            {
                if (m_starSS == null)
                {
                    if (this.gameObject.name == "OwnPlayer")
                    {
                        BGM.Instance.SetVolume(0.0f);
                    }

                    m_starSS = new GameObject("SoundSource").AddComponent<nsSound.SoundSource>();
                    m_starSS.SetSoundType(nsSound.EnSoundTypes.enBGM);
                    m_starSS.SetLoop(true);
                    m_starSS.PlayStart(nsSound.BGMNames.m_star);
                    m_starSS.Set3DSourcePos(m_ssPos);
                    m_starSS.Set3DListenerPos(m_listenerPos);
                    m_starSS.Set3DListenerDir(m_listenerDir);
                }
                else
                {
                    m_starSS.Set3DSourcePos(m_ssPos);
                    m_starSS.Set3DListenerPos(m_listenerPos);
                    m_starSS.Set3DListenerDir(m_listenerDir);
                }
            }
            else
            {        
                if (m_starSS != null)
                {
                    m_starSS.Stop();
                    if (this.gameObject.name == "OwnPlayer")
                    {
                        BGM.Instance.SetVolume(1.0f);
                    }
                }
            }


		}

        // ���̂��R���W�����ڐG�����Ƃ��A�P�x�����Ă΂��
        private void OnCollisionEnter(Collision collision)
        {
            if (this.gameObject.name != "OwnPlayer")
            {
                return;
            }
            //�ڐG�����R���W�����̃^�O���擾����B
            string triggerName = collision.gameObject.tag;

            //�Փ˓_
            Vector3 contactPoint = Vector3.zero;

            //�ǂɏՓ˂��Ă��Ȃ�����̏�ԂŕǂɏՓ˂�����
            if (/*!m_hitWall && */triggerName == "Wall")
            {
                //�Փ˓_���擾����B
                foreach (ContactPoint contact in collision.contacts)
                {
                    contactPoint = new Vector3(contact.point.x, contact.point.y, contact.point.z);
                }

                //���g����Փ˓_�Ɍ������ĐL�т�x�N�g�����v�Z�B
                Vector3 contactVec = contactPoint - this.gameObject.transform.position;

                //y�������폜�B
                contactVec.y = 0.0f;

                //���K���B
                contactVec.Normalize();

                //�O�ς����߂�B
                Vector3 dot = Vector3.Cross(contactVec, this.gameObject.transform.forward);

                //�Փˉ�
                SoundSource hitWallSS = new GameObject("SoundSource").AddComponent<nsSound.SoundSource>();
                hitWallSS.SetSoundType(nsSound.EnSoundTypes.enSE);
                hitWallSS.Be3DSound();
                hitWallSS.PlayStart(nsSound.SENames.m_hitWall);
                hitWallSS.Set3DListenerPos(m_listenerPos);
                hitWallSS.Set3DListenerDir(m_listenerDir);

                Vector3 axisY = new Vector3(0.0f, 1.0f, 0.0f);
                Vector3 rightDir = Vector3.Cross(m_mainCameraTransform.forward, axisY);

                //�O�ς���ǂ��E�ɂ���̂����ɂ���̂����f����B
                //�E
                if (dot.y < 0.0)
                {
                    hitWallSS.Set3DSourcePos(m_listenerPos + rightDir * 2.0f);
                }
                //��
                else
                {
                    hitWallSS.Set3DSourcePos(m_listenerPos + rightDir * -2.0f);
                }
            }
        }
    }
}