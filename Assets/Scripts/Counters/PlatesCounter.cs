using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatesCounter : BaseCounter
{
    public event EventHandler OnPlateSpawned;
    public event EventHandler OnPlateRemoved;


    [SerializeField] KitchenObjectSO plateKitchenObjectSO;

    private float spawnPlateTimer=0f;
    private float spawnPlateTimerMax=4f;

    private int platesSpawnedAmount=0;
    private int platesSpawnedAmountMax=4;

    private void Update()
    {
        spawnPlateTimer += Time.deltaTime;
        if( spawnPlateTimer > spawnPlateTimerMax )
        {
            spawnPlateTimer = 0f;
            if(platesSpawnedAmount< platesSpawnedAmountMax)
            {
                OnPlateSpawned?.Invoke(this, EventArgs.Empty);
                platesSpawnedAmount++;
            }
            
        }
    }
    public override void Interact(Player player)
    {
        if(!player.HasKitchenObject())//玩家手中没有盘子
        {
            if(platesSpawnedAmount>0)//桌上有盘子
            {
                KitchenObject.SpawnKithcenObject(plateKitchenObjectSO, player);
                OnPlateRemoved?.Invoke(this, EventArgs.Empty);
                platesSpawnedAmount--;
            }
        }
    }

}
