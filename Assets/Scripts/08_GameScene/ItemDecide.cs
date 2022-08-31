using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemDecide : MonoBehaviour
{
    //�S�ẴA�C�e���X�v���C�g�̔z��
    [SerializeField] Sprite[] itemSprite = null;
    //�A�C�e���摜
    [SerializeField]Image itemImage = null;

    //�X�e�[�g
    enum EnItemState
    {
        enNothingState,  //�A�C�e���������Ă��Ȃ����
        enLotteryState,  //���I���
        enBlinkingState  //���I�I��������̓_�ŏ��
    }

    EnItemState itemState = EnItemState.enNothingState;
    //�_�ł̉�
    [SerializeField] int blinkingNumberOfTimes = 0;
    //�_�ł̕\������
    [SerializeField] float blinkingDelayTime = 0.0f;
    //�_�ŏ������I���������ǂ���
    bool isBlinkingFinish = false;
    //���I��
    [SerializeField] int lotteryNumberOfTimes = 0;
    //���I�̕\������
    [SerializeField] float lotteryDelayTime = 0.0f;
    //���I�������I���������ǂ���
    bool isLotteryFinish = false;
    //�\������A�C�e���X�v���C�g�̔z��ԍ�
    int itemSpriteActiveArrayNum = 0;
    //�O��\�������A�C�e���X�v���C�g�̔z��ԍ�
    int itemSpriteBeforeActiveArrayNum = 0;

    ObtainItemController obtainItemController = null;

    void Start()
    {
        //�A�C�e���̃f�[�^��������
        ItemInit();
    }

    void Update()
    {
        //��Ԃɂ���ď����𕪊�
        switch (itemState)
        {
            //�A�C�e���������Ă��Ȃ����
            case EnItemState.enNothingState:
                obtainItemController = GameObject.Find("OwnPlayer").GetComponent<ObtainItemController>();
                if(obtainItemController == null)
				{
                    break;
				}

                //�A�C�e�����l��������A
                if (obtainItemController.GetObtainItemType() != -1)
                {
                    //���I��ԂɈڍs
                    itemState = EnItemState.enLotteryState;
                    //�A�C�e���摜��\��
                    itemImage.color = new Color(itemImage.color.r, itemImage.color.g, itemImage.color.b, 1.0f);
                    //���I�����̃R���[�`�����J�n
                    StartCoroutine("LotteryCoroutine");
                }
                break;

            //���I���
            case EnItemState.enLotteryState:
                //���I���I��������A
                if (isLotteryFinish)
                {
                    //
                    obtainItemController.SetIsLotteryFinish(true);

                    //���肵���A�C�e���摜���ŏI�\���摜�ɂ���
                    itemImage.sprite = itemSprite[obtainItemController.GetObtainItemType()];
                    //�_�ŏ�ԂɈڍs
                    itemState = EnItemState.enBlinkingState;
                    //�_�ŏ����̃R���[�`�����J�n
                    StartCoroutine("BlinkingCoroutine");
                    //���I���I���������ǂ����̔����������Ԃɖ߂�
                    isLotteryFinish = false;
                }
                break;

            //���肵����̓_�ŏ��
            case EnItemState.enBlinkingState:
                //�A�C�e�����g��ꂽ��
                if (obtainItemController.GetObtainItemType() == -1)
                {
                    //�_�ŏ����̃R���[�`�����I�����Ă��Ȃ�������
                    if (!isBlinkingFinish)
                    {
                        //�_�ŏ����̃R���[�`�����~�߂�
                        StopCoroutine("BlinkingCoroutine");
                    }
                    //�A�C�e���̃f�[�^��������
                    ItemInit();
                }
                break;
        }
    }

    //���I����
    IEnumerator LotteryCoroutine()
    {
        //lotteryNumberOfTimes�̉񐔕����I
        for (int lotteryNum = 0; lotteryNum < lotteryNumberOfTimes; lotteryNum++)
        {
            //do-while�����g���đO��\�������A�C�e���X�v���C�g�Ɠ����摜��A���ŕ\�����Ȃ��悤�ɂ���
            do
            {
                //������A�C�e���̒����烉���_���ŕ\��
                itemSpriteActiveArrayNum = Random.Range(0, itemSprite.Length);
            }while (itemSpriteActiveArrayNum == itemSpriteBeforeActiveArrayNum);
            //Image��Sprite��ݒ�
            itemImage.sprite = itemSprite[itemSpriteActiveArrayNum];

            //�ҋ@
            yield return new WaitForSeconds(blinkingDelayTime);

            //�O��\�������A�C�e���X�v���C�g�̔z��ԍ����X�V
            itemSpriteBeforeActiveArrayNum = itemSpriteActiveArrayNum;
        }

        //���I�����̃R���[�`�����I�������Ƃ������Ƃ�Ԃ�
        isLotteryFinish = true;
    }

    //�_�ŏ���
    IEnumerator BlinkingCoroutine()
    {
        //blinkingNumberOfTimes�̉񐔕��_�ł���
        for (int blinkingNum = 0; blinkingNum < blinkingNumberOfTimes; blinkingNum++)
        {
            //�\������Ă���Ƃ�
            if(itemImage.color.a == 1.0f)
            {
                //�A�C�e���摜���\��
                itemImage.color = new Color(itemImage.color.r , itemImage.color.g , itemImage.color.b ,0.0f);
            }
            //��\������Ă���Ƃ�
            else
            {
                //�A�C�e���摜��\��
                itemImage.color = new Color(itemImage.color.r, itemImage.color.g, itemImage.color.b, 1.0f);
            }

            //�ҋ@
            yield return new WaitForSeconds(blinkingDelayTime);
        }

        //�_�ŏ����̃R���[�`�����I�������Ƃ������Ƃ�Ԃ�
        isBlinkingFinish = true;
    }

    //�������֐�
    void ItemInit()
    {
        //�A�C�e���������Ă��Ȃ���Ԃɐݒ�
        itemState = EnItemState.enNothingState;
        //�A�C�e���摜���\��
        itemImage.color = new Color(itemImage.color.r, itemImage.color.g, itemImage.color.b, 0.0f);
        //�_�ŏ����͏I�����Ă��Ȃ�����ɐݒ肵�Ă���
        isBlinkingFinish = false;

        if(obtainItemController != null)
		{
            obtainItemController.SetIsLotteryFinish(false);
        }
    }
}
