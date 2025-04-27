using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IKitchenObjectParent
{
    public static Player Instance { get; private set; }

    public event EventHandler OnPlayerPickup;
    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;
    public class OnSelectedCounterChangedEventArgs : EventArgs
    {
        public BaseCounter selectedCounter;
    }
    
    [SerializeField]
    private float moveSpeed = 7f;
    [SerializeField]
    private Animator playerAnimator;
    [SerializeField]
    private GameInput gameInput;
    [SerializeField]
    private LayerMask countersLayerMask;
    [SerializeField]
    private Transform kitchenObjectHoldPoint;

    private BaseCounter selectedCounter;

    private Vector3 lastInteractDirection;

    private KitchenObject kitchenObject;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There Is More Than One PLayer Instance");
        }
        Instance = this;
    }
    private void Start()
    {
        playerAnimator = GetComponentInChildren<Animator>();
        gameInput.OnInteractAction += GameInput_OnInteractAction;
        gameInput.OnInteractAlternateAction += GameInput_OnInteractAlternateAction;
    }

    private void GameInput_OnInteractAlternateAction(object sender, EventArgs e)
    {
        if (selectedCounter != null)
        {
            selectedCounter.InteractAlternate(this);
        }
    }

    private void GameInput_OnInteractAction(object sender, System.EventArgs e)
    {
       if(selectedCounter != null)
        {
            selectedCounter.Interact(this);
        }
    }

    private void Update()
    {
        MovementHandler();
        InteractionHandler();
    }

    private void AnimationHandler(Vector3 inputVector) { 
        if(inputVector!=Vector3.zero)
        {
            playerAnimator.SetBool("IsWalking", true);
        }
        else
        {
            playerAnimator.SetBool("IsWalking", false);
        }
    }
    private void MovementHandler()
    {
        Vector3 inputVector = gameInput.GetMovementVectorNormalized(); 

        float moveDistance = moveSpeed * Time.deltaTime;
        float playerRadius = .7f;
        float playerHeight = 2f;
        bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, inputVector, moveDistance);

        if (!canMove)
        {
            Vector3 inputVectorX = new Vector3(inputVector.x, 0, 0).normalized;
            canMove = inputVector.x != 0 &&!Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, inputVectorX, moveDistance);
            if (canMove)
            {
                inputVector = inputVectorX;
            }
            else
            {
                Vector3 inputVectorZ = new Vector3(0, 0, inputVector.z).normalized;
                canMove = inputVector.z!=0&&!Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, inputVectorZ, moveDistance);
                if (canMove)
                {
                    inputVector = inputVectorZ;
                }
            }
        }
        if (canMove)
        {
            transform.position += inputVector * Time.deltaTime * moveSpeed;
        }
        AnimationHandler(inputVector);
        float rotateSpeed = 12f;
        transform.forward = Vector3.Slerp(transform.forward, inputVector, Time.deltaTime * rotateSpeed);
    }

    private void InteractionHandler()
    {
        Vector3 inputVector = gameInput.GetMovementVectorNormalized();
        if (inputVector != Vector3.zero)
        {
            lastInteractDirection = inputVector;
        }
        float interactDistance = 2f;
        if (Physics.Raycast(transform.position, lastInteractDirection, out RaycastHit raycastHit, interactDistance, countersLayerMask))
        {
            if (raycastHit.transform.TryGetComponent(out BaseCounter baseCounter))
            {
                //Has Clear Counter
                if (baseCounter != selectedCounter)
                {
                    SetSelectedCounter(baseCounter);
                }
            }
            else
            {
                SetSelectedCounter(null);
            }
        }
        else
        {
            SetSelectedCounter(null);
        }
          }
    private void SetSelectedCounter(BaseCounter selectedCounter)
    {
        this.selectedCounter = selectedCounter;
        OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs
        {
            selectedCounter = selectedCounter
        });
    }

    public Transform GetKitchenObjectFollowTransform()
    {
        return kitchenObjectHoldPoint;
    }

    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        this.kitchenObject = kitchenObject;

        if(kitchenObject != null)
        {
            OnPlayerPickup?.Invoke(this, EventArgs.Empty);
        }
    }
    public KitchenObject GetKitchenObject() { return kitchenObject; }
    public void ClearKitchenObject()
    {
        kitchenObject = null;
    }

    public bool HasKitchenObject()
    {
        return kitchenObject != null;
    }

    public bool IsWalking()
    {
        if(gameInput.GetMovementVectorNormalized()!=Vector3.zero)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
