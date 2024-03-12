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
            //如果工作台上没有物体
            if(player.HasKitchenObject())
            {
                //工作台上没有物体,玩家手里是拿着物体，则把物体放到桌上
                player.GetKitchenObject().SetKitchenObjectParent(this); 
            }
            else
            {
                //工作台上没有物体，玩家手中也没有物体,就什么也不做。
            }
        }
        else //工作台上有物体
        {
            if (player.HasKitchenObject())//玩家手中有物体
            {
                
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))//添加盘子
                {      
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO()))//添加食材
                    {
                        GetKitchenObject().DestorySelf();//摧毁桌上的东西
                    }
                }
                else//工作台上有东西，玩家手中没有盘子
                {
                    if (GetKitchenObject().TryGetPlate(out  plateKitchenObject))//工作台上有盘子 
                    {
                        if(plateKitchenObject.TryAddIngredient(player.GetKitchenObject().GetKitchenObjectSO()))//玩家手中的食材尝试加入盘子
                        {
                            player.GetKitchenObject().DestorySelf();
                        }
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

}