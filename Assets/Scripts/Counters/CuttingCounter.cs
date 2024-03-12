using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CuttingCounter : BaseCounter,IHasProgress
{
    public static event EventHandler OnAnyCut;
    public event EventHandler OnCut;//切菜事件
    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;


    [SerializeField] private CuttingRecipeSO[] cuttingRecipeSOArray;
    private int cuttingProgress;
    public override void Interact(Player player)
    {
        if ((!HasKitchenObject()))//如果工作台上没有物体
        {   
            if (player.HasKitchenObject())//玩家手中有物体
            {
                if (HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSO()))//玩家手中的物体是可以切的食材
                {
                    

                    //工作台上没有物体,玩家手里是拿着物体，则把物体放到桌上
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
                //工作台上没有物体，玩家手中也没有物体,就什么也不做。
            }
        }
        else //工作台上有物体
        {
            if (player.HasKitchenObject())
            {
                //玩家手中有物体
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))//添加盘子
                {
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO()))//添加食材
                    {
                        GetKitchenObject().DestorySelf();//摧毁桌上的东西
                    }
                }
            }
            else
            {
                //玩家手中没有物体，把工作台上的物体交给玩家
                GetKitchenObject().SetKitchenObjectParent(player);
            }
        }
    }

    public override void InteractAlternate(Player player)
    {
        if(HasKitchenObject()&&HasRecipeWithInput(GetKitchenObject().GetKitchenObjectSO()))//切割台上有物体且可以被切
        {
            cuttingProgress++;
            OnCut?.Invoke(this, EventArgs.Empty);//触发切割动画
            OnAnyCut?.Invoke(this, EventArgs.Empty);
            CuttingRecipeSO cuttingRecipeSO = GetCuttingReceipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());

            OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
            {
                progressNormalized = (float)cuttingProgress / cuttingRecipeSO.cuttingProgressMax

            }); ;

            if(cuttingProgress>=cuttingRecipeSO.cuttingProgressMax)//切到足够刀数后才会变成片
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

