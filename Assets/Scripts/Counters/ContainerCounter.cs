using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerCounter : BaseCounter { 
    [SerializeField]
    private KitchenObjectSO kitchenObjectSO;
    [SerializeField]
    private Animator animator;

    public void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }
    public override void Interact(Player player)
    {
        if (!HasKitchenObject())
        {
            if (!player.HasKitchenObject())
            {
                KitchenObject.SpawnKitchenObject(kitchenObjectSO, player);
                animator.SetTrigger("OpenClose");
            }
        }
       
 
    }
    
}
