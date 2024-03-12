using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioClipRefsSO audioClipRefsSO;
    // Start is called before the first frame update
    void Start()
    {
        DeliveryManager.Instance.OnDeliverySuccess += Instance_OnDeliverySuccess;
        DeliveryManager.Instance.OnDeliveryFailed += Instance_OnDeliveryFailed;
        CuttingCounter.OnAnyCut += CuttingCounter_OnAnyCut;
        Player.OnPlayerMove += Player_OnPlayerMove;
    }

    private void Player_OnPlayerMove(object sender, System.EventArgs e)
    {
        Player player = (Player)sender;
        PlaySound(audioClipRefsSO.footstep, player.transform.position);
    }

    private void CuttingCounter_OnAnyCut(object sender, System.EventArgs e)
    {
        CuttingCounter cuttingCounter = (CuttingCounter)sender;
        PlaySound(audioClipRefsSO.chop,cuttingCounter.transform.position );
    }



    private void Instance_OnDeliveryFailed(object sender, System.EventArgs e)
    {
        DeliveryCounter deliveryCounter = DeliveryCounter.Instance;
        PlaySound(audioClipRefsSO.deliveryFail, deliveryCounter.transform.position);
    }

    private void Instance_OnDeliverySuccess(object sender, System.EventArgs e)
    {
        DeliveryCounter deliveryCounter = DeliveryCounter.Instance;
        PlaySound(audioClipRefsSO.deliverySuccess, deliveryCounter.transform.position);
    }

    private void PlaySound(AudioClip audioClip,Vector3 position,float volume=1f)
    {
        AudioSource.PlayClipAtPoint(audioClip,position,volume);
    }
    private void PlaySound(AudioClip[] audioClipArray, Vector3 position, float volume = 1f)
    {
        AudioSource.PlayClipAtPoint(audioClipArray[Random.Range(0,audioClipArray.Length)], position, volume);
    }
}
