using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CuttingCounter;

public class StoveCounter : BaseCounter,IHasProgress
{
    public event EventHandler<OnStateChangedEventArgs> OnStateChanged;
    public class OnStateChangedEventArgs : EventArgs
    {
        public State state;
    }
    [SerializeField]
    private Transform[] fryingEffects;
    public enum State
    {
        Idle,
        Frying,
        Fried,
        Burned,

    }
    [SerializeField]
    private FryingRecipeSO[] fryingRecipeSOArray;
    [SerializeField]
    private BurningRecipeSO[] burningRecipeSOArray;

    private State state;

    private float fryingTimer;

    private float burningTimer;
    
    private FryingRecipeSO fryingRecipeSO;

    private BurningRecipeSO burningRecipeSO;

    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;

    private void Start()
    {
        state = State.Idle;
        OnStateChanged?.Invoke(this,new OnStateChangedEventArgs{
        
        state=State.Idle
        });
    }
    private void Update()
    {
        if (HasKitchenObject())
        {
            switch (state)
            {
                case State.Idle:

                    break;
                case State.Frying:

                    fryingTimer += Time.deltaTime;

                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                    {
                        progressNormalized = fryingTimer / fryingRecipeSO.fryingTimerMax
                    });

                    if (fryingTimer > fryingRecipeSO.fryingTimerMax)
                        {

                            //fried
                            GetKitchenObject().DestroySelf();
                            KitchenObject.SpawnKitchenObject(fryingRecipeSO.output, this);



                            state = State.Fried;
                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
                        {

                            state = State.Fried
                        });
                        burningTimer = 0f;
                            burningRecipeSO = GetBurningRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());
                        }
                    
                    break;
                case State.Fried:

                    burningTimer += Time.deltaTime;

                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                    {
                        progressNormalized = burningTimer / burningRecipeSO.burningTimerMax
                    });

                    if (burningTimer > burningRecipeSO.burningTimerMax)
                    {

                        //fried
                        GetKitchenObject().DestroySelf();
                        KitchenObject.SpawnKitchenObject(burningRecipeSO.output, this);

                        state = State.Burned;
                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
                        {

                            state = State.Burned
                        });

                        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                        {
                            progressNormalized = 0f
                        });
                    }
                    
                    break;
                case State.Burned:

                    break;
            }
        }
        EffectsHandler();
    }

    public override void Interact(Player player)
    {

        if (!HasKitchenObject())
        {
            //No Kitchen Object
            if (player.HasKitchenObject())
            {
                //player is carrying something that can be fryed
                if (HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSO()))
                {
                    player.GetKitchenObject().SetKitchenObjectParent(this);

                    fryingRecipeSO = GetFryingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());
                    state = State.Frying;
                    OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
                    {

                        state = State.Frying
                    }) ;
                    fryingTimer = 0f;

                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                    {
                        progressNormalized = fryingTimer / fryingRecipeSO.fryingTimerMax
                    }) ;  
                }
            }
            else
            {
                //player has nothing
            }
        }
        else
        {
            if (player.HasKitchenObject())
            {
                //player is carrying something
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    //Checing if it is a type pf plate kitchen object
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO()))
                    {
                        GetKitchenObject().DestroySelf();
                        state = State.Idle;
                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
                        {

                            state = State.Idle
                        });

                        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                        {
                            progressNormalized = 0f
                        });
                    }
                }

            }
            else
            {
                //Has Kitchen Object
                this.GetKitchenObject().SetKitchenObjectParent(player);
                state=State.Idle;
                OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
                {

                    state = State.Idle
                });

                OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                {
                    progressNormalized = 0f
                });
            }
        }


    }

    private bool HasRecipeWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (FryingRecipeSO fryingRecipeSO in fryingRecipeSOArray)
        {
            if (fryingRecipeSO.input == inputKitchenObjectSO)
            {
                return true;
            }
        }
        return false;
    }

    private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (FryingRecipeSO fryingRecipeSO in fryingRecipeSOArray)
        {
            if (fryingRecipeSO.input == inputKitchenObjectSO)
            {
                return fryingRecipeSO.output;
            }
        }
        return null;
    }

    private FryingRecipeSO GetFryingRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (FryingRecipeSO fryingRecipeSO in fryingRecipeSOArray)
        {
            if (fryingRecipeSO.input == inputKitchenObjectSO)
            {
                return fryingRecipeSO;
            }
        }
        return null;

    }

    private BurningRecipeSO GetBurningRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (BurningRecipeSO burningRecipeSO in burningRecipeSOArray)
        {
            if (burningRecipeSO.input == inputKitchenObjectSO)
            {
                return burningRecipeSO;
            }
        }
        return null;

    }

    private void EffectsHandler()
    {
        if(state==State.Idle || state==State.Burned)
        {
            HideEffects();
        }
        else
        {
            ShowEffects();
        }
    }


    private void ShowEffects()
    {
        foreach(Transform effect in fryingEffects)
        {
            effect.gameObject.SetActive(true);
        }
    }
    private void HideEffects()
    {
        foreach (Transform effect in fryingEffects)
        {
            effect.gameObject.SetActive(false);
        }
    }

    
}
