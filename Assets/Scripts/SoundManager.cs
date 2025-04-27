using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance {get; private set;}
    [SerializeField]
    private AudioClipSO audioClipSO;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        DeliveryManager.Instance.OnRecipeSuccess += Instance_OnRecipeSuccess;
        DeliveryManager.Instance.OnRecipeFailure += Instance_OnRecipeFailure;
        CuttingCounter.OnAnyCut += CuttingCounter_OnAnyCut;
        Player.Instance.OnPlayerPickup += Instance_OnPlayerPickup;
        BaseCounter.OnPlayerDrop += BaseCounter_OnPlayerDrop;
        TrashCounter.OnTrashUsed += TrashCounter_OnTrashUsed;
    }

    private void TrashCounter_OnTrashUsed(object sender, System.EventArgs e)
    {
        TrashCounter trashCounter=sender as TrashCounter;
        PlaySound(audioClipSO.trash, trashCounter.transform.position);
    }

    private void BaseCounter_OnPlayerDrop(object sender, System.EventArgs e)
    {
        BaseCounter counter =sender as BaseCounter;
        PlaySound(audioClipSO.objectDrop, counter.transform.position);
    }

    private void Instance_OnPlayerPickup(object sender, System.EventArgs e)
    {
        Player player=sender as Player;
        PlaySound(audioClipSO.objectPickup, player.transform.position);
    }

    private void CuttingCounter_OnAnyCut(object sender, System.EventArgs e)
    {
        CuttingCounter cuttingCounter= sender as CuttingCounter;
        PlaySound(audioClipSO.chop,cuttingCounter.transform.position);
    }

    private void Instance_OnRecipeFailure(object sender, System.EventArgs e)
    {
        PlaySound(audioClipSO.deliveryFail, DeliveryCounter.Inatance.transform.position);
    }

    private void Instance_OnRecipeSuccess(object sender, System.EventArgs e)
    {
        PlaySound(audioClipSO.deliverySuccess, DeliveryCounter.Inatance.transform.position);
    }

    private void PlaySound(AudioClip audioClip, Vector3 position, float volume=1f)
    {
        AudioSource.PlayClipAtPoint(audioClip, position, volume);
    }

    private void PlaySound(AudioClip[] audioClipArray, Vector3 position, float volume = 1f)
    {
        PlaySound(audioClipArray[Random.Range(0,audioClipArray.Length)], position, volume);
    }
    public void PlayFootStepSound(Vector3 position)
    {
        PlaySound(audioClipSO.footstep, position);
    }
}
