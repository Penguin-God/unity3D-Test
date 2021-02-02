using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    public RectTransform uiGroup;
    public Animator animator;

    public GameObject[] itemObject;
    public int[] itemPrice;
    public Transform[] itemPos;
    public Text talkText;
    public string[] talkData;
        
    Player enterPlayer;

    public void Enter(Player player)
    {
        enterPlayer = player;
        uiGroup.anchoredPosition = Vector3.zero;
    }

    public void Exit()
    {
        animator.SetTrigger("doHello");
        uiGroup.anchoredPosition = Vector3.down * 1000;
    }

    public void Buy(int index)
    {
        int Price = itemPrice[index];
        if (Price > enterPlayer.currentCoin)
        {
            StopCoroutine(DontBuy());
            StartCoroutine(DontBuy());
        }
        else
        {
            enterPlayer.currentCoin -= Price;
            Vector3 RandomVec = Vector3.right * Random.Range(-3, 3) + Vector3.forward * Random.Range(-3, 3);
            Instantiate(itemObject[index], itemPos[index].position + RandomVec, itemPos[index].rotation);
        }

    }

    IEnumerator DontBuy()
    {
        talkText.text = talkData[0];
        yield return new WaitForSeconds(2f);
        talkText.text = talkData[1];
    }
}
