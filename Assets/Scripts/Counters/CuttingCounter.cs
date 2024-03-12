using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CuttingCounter : BaseCounter,IHasProgress
{
    public static event EventHandler OnAnyCut;
    public event EventHandler OnCut;//�в��¼�
    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;


    [SerializeField] private CuttingRecipeSO[] cuttingRecipeSOArray;
    private int cuttingProgress;
    public override void Interact(Player player)
    {
        if ((!HasKitchenObject()))//�������̨��û������
        {   
            if (player.HasKitchenObject())//�������������
            {
                if (HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSO()))//������е������ǿ����е�ʳ��
                {
                    

                    //����̨��û������,����������������壬�������ŵ�����
                    player.GetKitchenObject().SetKitchenObjectParent(this);
                    cuttingProgress = 0;

                    CuttingRecipeSO cuttingRecipeSO = GetCuttingReceipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());

                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                    {
                        progressNormalized = (float)cuttingProgress / cuttingRecipeSO.cuttingProgressMax

                    }); ;
                }

            }
            else
            {
                //����̨��û�����壬�������Ҳû������,��ʲôҲ������
            }
        }
        else //����̨��������
        {
            if (player.HasKitchenObject())
            {
                //�������������
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))//�������
                {
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO()))//���ʳ��
                    {
                        GetKitchenObject().DestorySelf();//�ݻ����ϵĶ���
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

    public override void InteractAlternate(Player player)
    {
        if(HasKitchenObject()&&HasRecipeWithInput(GetKitchenObject().GetKitchenObjectSO()))//�и�̨���������ҿ��Ա���
        {
            cuttingProgress++;
            OnCut?.Invoke(this, EventArgs.Empty);//�����и��
            OnAnyCut?.Invoke(this, EventArgs.Empty);
            CuttingRecipeSO cuttingRecipeSO = GetCuttingReceipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());

            OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
            {
                progressNormalized = (float)cuttingProgress / cuttingRecipeSO.cuttingProgressMax

            }); ;

            if(cuttingProgress>=cuttingRecipeSO.cuttingProgressMax)//�е��㹻������Ż���Ƭ
            {
                KitchenObjectSO outputKitchenObjectSO = GetOutputForInput(GetKitchenObject().GetKitchenObjectSO());
                GetKitchenObject().DestorySelf();
                KitchenObject.SpawnKithcenObject(outputKitchenObjectSO, this);
            }

        }
    }

    private bool HasRecipeWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        CuttingRecipeSO cuttingRecipeSO = GetCuttingReceipeSOWithInput(inputKitchenObjectSO);
        if (cuttingRecipeSO != null)
        { 
            return true; 
        }
        else
        {
            return false;
        }
    }
    private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectSO)
    {
        CuttingRecipeSO cuttingRecipeSO =GetCuttingReceipeSOWithInput(inputKitchenObjectSO);
        if(inputKitchenObjectSO != null)
        {
            return cuttingRecipeSO.output;
        }
        return null;
    }

    private CuttingRecipeSO GetCuttingReceipeSOWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (CuttingRecipeSO cuttingRecipeSO in cuttingRecipeSOArray)
        {
            if (cuttingRecipeSO.input == inputKitchenObjectSO)
            {
                return cuttingRecipeSO;
            }
        }
        return null;
    }
}

