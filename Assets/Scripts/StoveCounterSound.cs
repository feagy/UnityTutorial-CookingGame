using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveCounterSound : MonoBehaviour
{
    [SerializeField]
    private StoveCounter stoveCounter;
    private AudioSource audioSource;
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        stoveCounter.OnStateChanged += StoveCounter_OnStateChanged;
    }

    private void StoveCounter_OnStateChanged(object sender, StoveCounter.OnStateChangedEventArgs e)
    {
        if(e.state==StoveCounter.State.Frying|| e.state == StoveCounter.State.Fried)
        {
            audioSource.Play();
        }
        else
        {
            audioSource.Pause();
        }
    }
}
