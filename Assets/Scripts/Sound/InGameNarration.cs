using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�g���K�[�{�b�N�X�ɕt�����Ă���^�O���ƏƂ炵���킹��萔string
public class SoundTriggerTagName
{
    public const string m_rightCurve = "RightCurve";    //�E�J�[�u�O
    public const string m_leftCurve = "LeftCurve";      //���J�[�u�O
    public const string m_item = "ItemNarration";       //�A�C�e���O

    public const string m_wall = "Wall";                //��
}

//���[�X���̃i���[�V�����̃T�E���h���Ǘ�����N���X
public class InGameNarration : MonoBehaviour
{

    //�l�X�Ȕ���Ɏg�������i���E�̕ǁA�A�C�e���̈ʒu�j
    public enum EnDirection
    {
        enNone,     //�Ȃ�
        enRight,    //�E
        enLeft      //��
    }
    private bool m_isOwnPlayer = false;                             //�v���C���[���g���H
    private nsSound.SoundSource m_soundSource = null;               //�i���[�V�����̃T�E���h�\�[�X�A�d�˂čĐ��������Ȃ��T�E���h�������Ƃ��Ɏg�p�ł��邩��
    private ObtainItemController m_obtainItemController = null;     //�v���C���[�̃A�C�e�����
    private bool m_getItemSoundPlayFlag = false;                    //�A�C�e���擾���̃i���[�V�������Đ����ꂽ�H
    private float m_hitWallTimer = 0.0f;                            //�ǂɓ������Ă��玟�ɒ��ӂ����܂ł̃^�C�}�[
    private bool m_hitWall = false;                                 //�ǂɓ��������H

    //�萔
    private float m_hitWallTimerConst = 3.0f;

    //ObtainItemController��EnItemType��private�ŃA�N�Z�X����������̂łƂ肠�����������̂�p��
    //��ł��܂���ɔ���ł�����̂�p�ӂ��邱�ƁB
    enum EnItemType
    {
        enNothing = -1,
        enOrangePeel,                                   //�I�����W�̔�
        enOrangeJet,                                    //�I�����W�W���[�X�W�F�b�g
        enSnapperCannon,                                //�^�C�C
        enTrain,                                        //�V������ԃL���[
        enStar,                                         //�X�^�[
        enItemTypeNum                                   //�A�C�e���̎�ނ̐�
    }


    //�������֐��B
    void Start()
    {
        //�v���C���[���g���ǂ�������B
        m_isOwnPlayer = (this.gameObject.name == "OwnPlayer");

        if (m_isOwnPlayer)
        {
            //�A�C�e�������Ǘ�����R���|�[�l���g���擾�B
            m_obtainItemController = this.gameObject.GetComponent<ObtainItemController>();
        }
    }

    //�X�V�֐��B
    void Update()
    {
        //�v���C���[���g����Ȃ��Ȃ瑁�����^�[���B
        if (!m_isOwnPlayer) { return; }

        //�A�C�e���l������̃i���[�V�����̊֐������s�B
        ExecuteItemNarration();

        //�ǏՓ˃^�C�}�[���X�V�B
        UpdateHitWallTimer();
    }

    //���̂��g���K�[�ɐڐG�����Ƃ��A�P�x�����Ă΂��
    private void OnTriggerEnter(Collider collision)
    {
        //�ڐG�����g���K�[�̃^�O���擾����B
        string triggerName = collision.gameObject.tag;

        //�^�O�̖��O����ǂ̉������Đ����邩���肵�čĐ��B
        PlayNarration(triggerName, EnDirection.enNone);
    }

    // ���̂��R���W�����ڐG�����Ƃ��A�P�x�����Ă΂��
    private void OnCollisionEnter(Collision collision)
    {
        //�ڐG�����R���W�����̃^�O���擾����B
        string triggerName = collision.gameObject.tag;

        //�Փ˓_
        Vector3 contactPoint = Vector3.zero;
        //�Փ˕���
        EnDirection direction = EnDirection.enNone;

        //�ǂɏՓ˂��Ă��Ȃ�����̏�ԂŕǂɏՓ˂�����
        if (!m_hitWall && triggerName == "Wall")
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

            //�O�ς���ǂ��E�ɂ���̂����ɂ���̂����f����B
            if (dot.y < 0.0)
            {
                direction = EnDirection.enRight;
            }
            else
            {
                direction = EnDirection.enLeft;
            }
        }
        //�^�O�̖��O�ƏՓ˕�������ǂ̉������Đ����邩���肵�čĐ��B
        PlayNarration(triggerName, direction);
    }

    //�^�O�̖��O���特�����Đ�����֐��B
    private void PlayNarration(string triggerName, EnDirection direction){

        nsSound.SoundSource ss = new GameObject("SoundSource").AddComponent<nsSound.SoundSource>();
        ss.SetSoundType(nsSound.EnSoundTypes.enNarration);
        ss.Be3DSound();

        //�E�J�[�u�O
        if (triggerName == SoundTriggerTagName.m_rightCurve)
        {
            ss.PlayStart(nsSound.NarInGameNames.m_MIGICURVEGAARUYO);
        }

        //���J�[�u�O
        if (triggerName == SoundTriggerTagName.m_leftCurve)
        {
            ss.PlayStart(nsSound.NarInGameNames.m_HIDARICURVEGAARUYO);
        }

        //�A�C�e���O
        if (triggerName == SoundTriggerTagName.m_item)
        {
            ss.PlayStart(nsSound.NarInGameNames.m_ITEMGAARUYO);
        }

        //��
        if (triggerName == SoundTriggerTagName.m_wall)
        {
            if (!m_hitWall)
            {
                //�E���ɓ������Ă���B
                if (direction == EnDirection.enRight)
                {
                    ss.PlayStart(nsSound.NarInGameNames.m_MIGINOKABENIATATTEIRUYO);
                }
                //�����ɓ������Ă���B
                else if(direction == EnDirection.enLeft)
                {
                    ss.PlayStart(nsSound.NarInGameNames.m_HIDARINOKABENIATATTEIRUYO);
                }
                //�ǂɏՓ˂����B
                m_hitWall = true;
                //�^�C�}�[���Z�b�g�B
                m_hitWallTimer = m_hitWallTimerConst;
            }
        }
    }

    //�A�C�e���ԍ����特�����Đ�����֐��B
    private void PlayNarration(EnItemType itemType)
    {
        //�T�E���h�\�[�X�𐶐��B
        nsSound.SoundSource itemSS = new GameObject("SoundSource").AddComponent<nsSound.SoundSource>();
        itemSS.SetSoundType(nsSound.EnSoundTypes.enNarration);
        itemSS.Be3DSound();

        //�A�C�e���ԍ�����Ή����鉹�����Đ��B
        switch (itemType) 
        {
            //�I�����W�̔�
            case EnItemType.enOrangePeel:
                itemSS.PlayStart(nsSound.NarInGameItemNames.m_MIKANNNOKAWADA);
                break;

            //�I�����W�W���[�X�W�F�b�g
            case EnItemType.enOrangeJet:
                itemSS.PlayStart(nsSound.NarInGameItemNames.m_JUICEJETDA);
                break;

            //�^�C�C
            case EnItemType.enSnapperCannon:
                itemSS.PlayStart(nsSound.NarInGameItemNames.m_TAIHOUDA);
                break;

            //�V������ԃL���[
            case EnItemType.enTrain:
                itemSS.PlayStart(nsSound.NarInGameItemNames.m_BOCCHANRESSYADA);
                break;

            //�X�^�[
            case EnItemType.enStar:
                itemSS.PlayStart(nsSound.NarInGameItemNames.m_OUGONNMIKANNDA);
                break;
        }
    }

    private void ExecuteItemNarration()
    {
        //�A�C�e������肵����
        if (m_obtainItemController.GetObtainItemType() != -1)
        {
            //�A�C�e���X���b�g�̒��I���Ȃ�(���I���I����ĂȂ��Ȃ�)
            if (m_obtainItemController.GetLotteryFinish() == false)
            {
                //�A�C�e���擾���̃i���[�V�����̃t���O�𗧂Ă�B
                m_getItemSoundPlayFlag = true;
            }
            else if (m_getItemSoundPlayFlag)
            {
                //�擾�����A�C�e�����������ׂ�B
                EnItemType itemType = (EnItemType)m_obtainItemController.GetObtainItemType();

                //�Ή����鉹�����Đ�����B
                PlayNarration(itemType);

                //���Đ������̂ŏI���B
                m_getItemSoundPlayFlag = false;
            }
        }
    }

    //�ǏՓ˃^�C�}�[���X�V����֐��B
    private void UpdateHitWallTimer()
    {
        if (m_hitWallTimer > 0.0f)
        {
            m_hitWallTimer -= Time.deltaTime;
            if (m_hitWallTimer <= 0.0f)
            {
                m_hitWallTimer = 0.0f;

                //������������B
                m_hitWall = false;
            }
        }
    }
}
