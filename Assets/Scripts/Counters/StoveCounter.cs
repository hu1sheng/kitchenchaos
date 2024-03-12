using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CuttingCounter;

public class StoveCounter : BaseCounter,IHasProgress
{
    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;//接口的声明

    [SerializeField] private FryingRecipeSO[] fryingRecipeSOArray;
    [SerializeField] private BurningReceipeSO[] burningReceipeSOArray;

    private float fryingTimer;
    private FryingRecipeSO fryingRecipeSO;
    private BurningReceipeSO burningReceipeSO;
    private float burningTimer;

    public event EventHandler<OnStateChangedEventArgs> OnStateChanged;
    public class OnStateChangedEventArgs:EventArgs
    {
        public State state;
    }
    public enum State
    {
        Idle,
        Frying,
        Fried,
        Burned,
    }
    private State state;
    private void Start()
    {
        state = State.Idle;
    }



    private void Update()
    {


        if(HasKitchenObject())
        {
            switch (state)
            {
                case State.Idle:
                    //进度条
                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                    {
                        progressNormalized = 0f
                    });
                    break;
                case State.Frying:
                    fryingTimer += Time.deltaTime;
                    //进度条
                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                    {
                        progressNormalized = fryingTimer / fryingRecipeSO.fryingProgressMax
                    });

                    if (fryingTimer >= fryingRecipeSO.fryingProgressMax)
                    {
                        //已烤熟
                        GetKitchenObject().DestorySelf();

                        KitchenObject.SpawnKithcenObject(fryingRecipeSO.output, this);
                        burningTimer = 0f;
                        burningReceipeSO=GetBurningReceipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());
                        state = State.Fried;

                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
                        {
                            state = state
                        }); 
                    }
                    break;
                case State.Fried:
                    burningTimer += Time.deltaTime;

                    //进度条
                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                    {
                        progressNormalized = burningTimer / burningReceipeSO.burningProgressMax
                    });

                    if (burningTimer >= burningReceipeSO.burningProgressMax)
                    {
                        //已烤焦
                        GetKitchenObject().DestorySelf();

                        KitchenObject.SpawnKithcenObject(burningReceipeSO.output, this);
                        state = State.Burned;

                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
                        {
                            state = state
                        });

                        //进度条
                        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                        {
                            progressNormalized = 0f
                        }) ;
                    }
                    break;
                case State.Burned:
                    break;

            }
        }

    }
    public override void Interact(Player player)
    {
        if ((!HasKitchenObject()))//如果工作台上没有物体
        {
            if (player.HasKitchenObject())//玩家手中有物体
            {
                if (HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSO()))//玩家手中的物体是可以烤的食材
                {
                    //工作台上没有物体,玩家手里是拿着物体，则把物体放到桌上
                    player.GetKitchenObject().SetKitchenObjectParent(this);

                    fryingRecipeSO = GetFryingReceipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());
                    state= State.Frying;
                    fryingTimer = 0f;

                    OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
                    {
                        state = state
                    });

                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                    {
                        progressNormalized = fryingTimer / fryingRecipeSO.fryingProgressMax
                    });

                    //进度条
                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                    {
                        progressNormalized = 0f
                    });
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

                        state = State.Idle;
                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
                        {
                            state = state
                        });
                    }
                }
            }
            else
            {
                //玩家手中没有物体，把工作台上的物体交给玩家
                GetKitchenObject().SetKitchenObjectParent(player);
                state = State.Idle;
                OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
                {
                    state = state
                });
                //进度条
                OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                {
                    progressNormalized = 0f
                });

            }
        }
    }

    private bool HasRecipeWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        FryingRecipeSO fryingReceipeSO = GetFryingReceipeSOWithInput(inputKitchenObjectSO);
        if (fryingReceipeSO != null)
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
        FryingRecipeSO fryingReceipeSO = GetFryingReceipeSOWithInput(inputKitchenObjectSO);
        if (inputKitchenObjectSO != null)
        {
            return fryingReceipeSO.output;
        }
        return null;
    }

    private FryingRecipeSO GetFryingReceipeSOWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (FryingRecipeSO fryingReceipeSO in fryingRecipeSOArray)
        {
            if (fryingReceipeSO.input == inputKitchenObjectSO)
            {
                return fryingReceipeSO;
            }
        }
        return null;
    }

    private BurningReceipeSO GetBurningReceipeSOWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (BurningReceipeSO burningReceipeSO in burningReceipeSOArray)
        {
            if (burningReceipeSO.input == inputKitchenObjectSO)
            {
                return burningReceipeSO;
            }
        }
        return null;
    }

}
