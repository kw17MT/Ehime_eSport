using UnityEngine;
using Photon.Pun;

//�I�����W�̔�N���X�B�l�b�g���[�N�I�u�W�F�N�g
public class OrangePeel : MonoBehaviourPunCallbacks
{

    [PunRPC]
    private void DestroyItemWithName(string name)
    {
        //�V�[���ɏo�Ă���Q�[���I�u�W�F�N�g�𖼑O�������ăf���[�g����
        GameObject.Find("SceneDirector").GetComponent<ItemStateCommunicator>().DestroyItemWithName(name);
    }

    //�I�����W�̔�Əd�Ȃ�����
    void OnTriggerEnter(Collider col)
	{
        //�����������̂����������삷��v���C���[�ł�������
        if(col.gameObject.tag == "OwnPlayer")
		{
            //���������v���C���[���U�����ꂽ����ɂ���B
            col.gameObject.GetComponent<AvatarController>().SetIsAttacked();
            photonView.RPC(nameof(DestroyItemWithName), RpcTarget.All, this.gameObject.name);
        }
        if(col.gameObject.tag == "Player")
		{
            //��������AI���U�����ꂽ����ɂ���B
            if (col.gameObject.GetComponent<AICommunicator>() != null)
            {
                col.gameObject.GetComponent<AICommunicator>().SetIsAttacked(true);
                photonView.RPC(nameof(DestroyItemWithName), RpcTarget.All, this.gameObject.name);

            }
            else if (col.gameObject.GetComponent<AvatarController>() != null)
			{
                //���������v���C���[���U�����ꂽ����ɂ���B
                col.gameObject.GetComponent<AvatarController>().SetIsAttacked();
                photonView.RPC(nameof(DestroyItemWithName), RpcTarget.All, this.gameObject.name);

            }
        }

        if(col.gameObject.name.Length >= 7 && col.gameObject.name[0..7] == "Snapper")
		{
            if((col.gameObject.transform.position - this.gameObject.transform.position).magnitude < 2.0f)
			{
                //�I�����W�̔�̃C���X�^���X�������ʐM���s��
                photonView.RPC(nameof(DestroyItemWithName), RpcTarget.All, this.gameObject.name);

                //�^�C�̃C���X�^���X�������ʐM���s��
                photonView.RPC(nameof(DestroyItemWithName), RpcTarget.All, col.gameObject.name);
            }
        }
	}
}
