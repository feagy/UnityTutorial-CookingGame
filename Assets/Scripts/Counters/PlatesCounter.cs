using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatesCounter : BaseCounter
{
    [SerializeField]
    private KitchenObjectSO plateKitchenObjectSO;
    private float spawnPlateTimer;
    private float spawnPlateTimerMax=4f;
    private int platesSpawned;
    private int platesSpawnedMax=4;

    [SerializeField]
    private Transform plateVisualPrefab;

    private List<GameObject> plateObjectsList;

    private void Awake()
    {
        plateObjectsList = new List<GameObject>();
    }

    private void Update()
    {
        spawnPlateTimer += Time.deltaTime;
        if (spawnPlateTimer>spawnPlateTimerMax)
        {
            spawnPlateTimer = 0;
            if (platesSpawned < platesSpawnedMax)
            {
                platesSpawned ++;

                Transform plateVisualTransform=Instantiate(plateVisualPrefab, GetCounterTopPoint());

                plateObjectsList.Add(plateVisualTransform.gameObject);
                float plateOffsetY = .1f;
                plateVisualTransform.localPosition = new Vector3(0,plateOffsetY*(platesSpawned-1),0);


            }
        }
    }

    public override void Interact(Player player)
    {

        if (!player.HasKitchenObject())
        {
           if(platesSpawned> 0)
            {
                platesSpawned--;
                KitchenObject.SpawnKitchenObject(plateKitchenObjectSO, player);

                GameObject plateGameObject = plateObjectsList[plateObjectsList.Count-1];
                plateObjectsList.Remove(plateGameObject);
                Destroy(plateGameObject);
            }
        }
        
    }
  }

