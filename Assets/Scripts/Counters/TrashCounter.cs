using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashCounter : BaseCounter
{
    public static event EventHandler OnTrashUsed;   
    public override void Interact(Player player)
    {
        
            //No Kitchen Object
            if (player.HasKitchenObject())
            {
                //player is carrying something
                player.GetKitchenObject().DestroySelf();

            OnTrashUsed?.Invoke(this, new EventArgs());
            }
    }
}
