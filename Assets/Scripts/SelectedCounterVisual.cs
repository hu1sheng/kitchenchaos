using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedCounterVisual : MonoBehaviour
{


    [SerializeField] private BaseCounter baseCounter;
    [SerializeField] private GameObject[] visualGameObjectArry;


    private void Start()
    {
        Player.Instance.OnSelectedCounterChanged += Player_OnSelectedCounterChanged;
    }

    private void Player_OnSelectedCounterChanged(object sender, Player.OnSelectedCounterChangedEventArgs e)
    {
        if (e.selectedCounter == baseCounter)
        {
            Show();
        }
        else
        {
            Hide();
        }
    }

    private void Show()
    {
        foreach (var item in visualGameObjectArry)
        {
            item.gameObject.SetActive(true);
        }
        //visualGameObject.SetActive(true);
    }

    private void Hide()
    {
        foreach(var item in visualGameObjectArry)
        { item.gameObject.SetActive(false);}
        //visualGameObject.SetActive(false);
    }

}