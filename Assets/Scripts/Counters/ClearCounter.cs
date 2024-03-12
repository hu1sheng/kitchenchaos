using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ClearCounter : BaseCounter
{


    [SerializeField] private KitchenObjectSO kitchenObjectSO;

    public override void Interact(Player player)
    {
        if((!HasKitchenObject()))
        {
            //�������̨��û������
            if(player.HasKitchenObject())
            {
                //����̨��û������,����������������壬�������ŵ�����
                player.GetKitchenObject().SetKitchenObjectParent(this); 
            }
            else
            {
                //����̨��û�����壬�������Ҳû������,��ʲôҲ������
            }
        }
        else //����̨��������
        {
            if (player.HasKitchenObject())//�������������
            {
                
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))//�������
                {      
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO()))//���ʳ��
                    {
                        GetKitchenObject().DestorySelf();//�ݻ����ϵĶ���
                    }
                }
                else//����̨���ж������������û������
                {
                    if (GetKitchenObject().TryGetPlate(out  plateKitchenObject))//����̨�������� 
                    {
                        if(plateKitchenObject.TryAddIngredient(player.GetKitchenObject().GetKitchenObjectSO()))//������е�ʳ�ĳ��Լ�������
                        {
                            player.GetKitchenObject().DestorySelf();
                        }
                    }
                        
                }
            }
            else 
            {
                //�������û�����壬�ѹ���̨�ϵ����彻�����
                GetKitchenObject().SetKitchenObjectParent(player);
            }
        }
    }

}