using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryManager : MonoBehaviour
{
    public event EventHandler OnDeliverySuccess;
    public event EventHandler OnDeliveryFailed;


    public event EventHandler OnRecipeSpawned;
    public event EventHandler OnRecipeCompleted;

    [SerializeField] private RecipeListSO recipeListSO;
    public List<RecipeSO> waitingReceipeSOList;
    public string recipeName;

    private float spawnRecipeTimer;
    private float spawnRecipeTimerMax=4f;
    private int waitingRecipesMax=4;

    public static DeliveryManager Instance
    {
        get;
        private set;
    }
    private void Awake()
    {
        waitingReceipeSOList = new List<RecipeSO>();
        Instance = this;
    }



    private void Update()
    {
        spawnRecipeTimer -= Time.deltaTime;
        if (spawnRecipeTimer<=0f)
        {
            spawnRecipeTimer = spawnRecipeTimerMax;
            RecipeSO waitngRecipeSO = recipeListSO.recipeSOList[UnityEngine.Random.Range(0,recipeListSO.recipeSOList.Count)];
            Debug.Log(waitngRecipeSO.recipeName);
            waitingReceipeSOList.Add(waitngRecipeSO);
            OnRecipeSpawned?.Invoke(this, EventArgs.Empty);
        }
    }

    public void DeliverRecipe(PlateKitchenObject plateKitchenObject)
    {
        
        for (int i = 0; i < waitingReceipeSOList.Count; i++)
        {
            RecipeSO waitingRecipeSO= waitingReceipeSOList[i];
            if(waitingRecipeSO.kitchenObjectSOList.Count==plateKitchenObject.kitchenObjectSOList.Count)//盘子配料数量匹配上了
            {
                bool plateContentsMatchesRecipe = true;
                foreach(KitchenObjectSO recipeKitchenObjectSO in waitingRecipeSO.kitchenObjectSOList)
                {
                    bool ingredientFound = false;
                    foreach (KitchenObjectSO plateKitchenObjectSO in plateKitchenObject.GetKitchenObjectSOList())
                    {
                        if(recipeKitchenObjectSO==plateKitchenObjectSO)//匹配成功
                        {
                            ingredientFound = true;      
                            break;
                        }
                    }
                    if(!ingredientFound)//匹配失败
                    {
                        plateContentsMatchesRecipe= false;
                    }
                }
                if(plateContentsMatchesRecipe)//匹配成功
                {
                    Debug.Log("匹配成功");
                    waitingReceipeSOList.RemoveAt(i);
                    OnRecipeCompleted?.Invoke(this, EventArgs.Empty);
                    OnDeliverySuccess?.Invoke(this, EventArgs.Empty);
                    return;
                }
            }
        }
        //找不到可以匹配的食材，玩家没有提供正确的配方。
        OnDeliveryFailed?.Invoke(this, EventArgs.Empty);
        Debug.Log("玩家没有给出正确的配方");
    }

    public List<RecipeSO> GetwaitingReceipeSOList()
    {
        return waitingReceipeSOList;
    }
}
