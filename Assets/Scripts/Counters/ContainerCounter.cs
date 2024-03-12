using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerCounter : BaseCounter,IKitchenObjectParent
{
    public event EventHandler OnPlayerGrabbedObject;
    [SerializeField] private KitchenObjectSO kitchenObjectSO;

    public override void Interact(Player player)
    {
        if(!player.HasKitchenObject())
        {
           KitchenObject.SpawnKithcenObject(kitchenObjectSO, player);
            OnPlayerGrabbedObject.Invoke(this, EventArgs.Empty);
        }
    }
            

}
