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

        private SoundSource m_engineSE = null;
        private SoundSource m_acceleSE = null;
        private float m_acceleSEPitch = 0.0f;

        //���̃Q�[����ʂ̃v���C���[�̏��
        private RaceAIScript m_ownPlayerRaceAI = null;

        //���̃Q�[����ʂ̃v���C���[�̃A�C�e�����
        private ObtainItemController m_obtainItemController = null;

        //�A�C�e���X���b�g��]���̃T�E���h
        private SoundSource m_itemSlotLotterySE = null;
        //�A�C�e���X���b�g�̃T�E���h��炷�t���O
        private bool m_itemSlotSoundPlayFlag = false;

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

            StartCoroutine("Init");

        }

        IEnumerator Init()
        {
            yield return null;

            //���̃Q�[���I�u�W�F�N�g���v���C���[���M�Ȃ��
            if (this.gameObject.name == "OwnPlayer")
            {
                m_obtainItemController = this.gameObject.GetComponent<ObtainItemController>();

            }
            //�v���C���[�ȊO�Ȃ��
            else
            {
                m_ownPlayerRaceAI = GameObject.Find("OwnPlayer").GetComponent<RaceAIScript>();
            }
        }

        // Update is called once per frame
        void Update()
        {

            if (this.gameObject.name != "OwnPlayer")
            {
                m_ssPos = this.GetComponent<RaceAIScript>().GetRigidBody.position;
                m_listenerPos = m_ownPlayerRaceAI.GetRigidBody.position;

                m_engineSE.Set3DSourcePos(m_ssPos);
                m_engineSE.Set3DListenerPos(m_listenerPos);
            }

            AcceleSound();

            if (this.gameObject.name == "OwnPlayer")
            {
                ItemGetSound();
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

            //�����ȊO�̎Ԃ̉��́A3D�T�E���h�ɁB
            if (this.gameObject.name != "OwnPlayer")
            {
                m_acceleSE.Set3DSourcePos(m_ssPos);
                m_acceleSE.Set3DListenerPos(m_listenerPos);
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





    }
}