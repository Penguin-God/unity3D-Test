using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManger : MonoBehaviour
{
    public GameObject menuCamera;
    public GameObject gameCamera;
    public GameObject menuPanel;
    public GameObject gamePanel;

    public Player player;
    public Boss boss;

    public int stage;
    public float playTime;
    public int currnt_NomalEnemy;
    public int currnt_ChargeEnemy;
    public int currnt_ADEnemy;
    public bool isBattle;

    public Text bestScoreTxt;
    public Text scoreTxt;
    public Text stageTxt;
    public Text playTimeTxt;
    public Text playerHpTxt;
    public Text playerAmmoTxt;
    public Text playerCoinTxt;
    public Text currnt_NomalEnemyTxt;
    public Text currnt_ChargeEnemyTxt;
    public Text currnt_ADEnemyTxt;

    public Image Weapon1_Image;
    public Image Weapon2_Image;
    public Image Weapon3_Image;
    public Image grenade_Image;

    public RectTransform bossHpGroup;
    public RectTransform bossHpBar;

}
