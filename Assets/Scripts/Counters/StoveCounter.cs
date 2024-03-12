using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CuttingCounter;

public class StoveCounter : BaseCounter,IHasProgress
{
    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;//�ӿڵ�����

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
                    //������
                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                    {
                        progressNormalized = 0f
                    });
                    break;
                case State.Frying:
                    fryingTimer += Time.deltaTime;
                    //������
                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                    {
                        progressNormalized = fryingTimer / fryingRecipeSO.fryingProgressMax
                    });

                    if (fryingTimer >= fryingRecipeSO.fryingProgressMax)
                    {
                        //�ѿ���
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

                    //������
                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                    {
                        progressNormalized = burningTimer / burningReceipeSO.burningProgressMax
                    });

                    if (burningTimer >= burningReceipeSO.burningProgressMax)
                    {
                        //�ѿ���
                        GetKitchenObject().DestorySelf();

                        KitchenObject.SpawnKithcenObject(burningReceipeSO.output, this);
                        state = State.Burned;

                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
                        {
                            state = state
                        });

                        //������
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
        if ((!HasKitchenObject()))//�������̨��û������
        {
            if (player.HasKitchenObject())//�������������
            {
                if (HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSO()))//������е������ǿ��Կ���ʳ��
                {
                    //����̨��û������,����������������壬�������ŵ�����
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

                    //������
                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                    {
                        progressNormalized = 0f
                    });
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
                //�������û�����壬�ѹ���̨�ϵ����彻�����
                GetKitchenObject().SetKitchenObjectParent(player);
                state = State.Idle;
                OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
                {
                    state = state
                });
                //������
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
