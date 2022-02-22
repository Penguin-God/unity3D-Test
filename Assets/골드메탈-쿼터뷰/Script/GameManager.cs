using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject menuCamera;
    public GameObject gameCamera;
    public GameObject menuPanel;
    public GameObject gamePanel;
    public GameObject gameOverPanel;

    [SerializeField] PlayerData playerData;
    //public Player player;
    public Boss boss;

    public int stage;
    public int score;
    public float playTime;
    public int current_NomalEnemy;
    public int current_ChargeEnemy;
    public int current_ADEnemy;
    public int current_Boss;
    public bool isBattle;

    public Text menuBestScoreTxt;
    public Text inGameScoreTxt;
    public Text stageTxt;
    public Text playTimeTxt;
    public Text playerHpTxt;
    public Text playerAmmoTxt;
    public Text playerCoinTxt;
    public Text currnt_NomalEnemyTxt;
    public Text currnt_ChargeEnemyTxt;
    public Text currnt_ADEnemyTxt;
    public Text currentScoreTxt;
    public Text bestTxt;

    public Image Weapon1_Image;
    public Image Weapon2_Image;
    public Image Weapon3_Image;
    public Image grenade_Image;

    public Text Weapon2_CurrentAmmo;
    public Text Weapon3_CurrentAmmo;

    public RectTransform bossHpGroup;
    public RectTransform bossHpBar;

    public GameObject itemShop;
    public GameObject weaponShop;
    public GameObject stageStartZone;

    public Transform[] enemyRespawnZone;
    public GameObject[] enemies;
    private List<int> respawnEnemyList;

    private void Awake()
    {
        playerData.Death.AddListener(GameOver);
        respawnEnemyList = new List<int>();

        if (!PlayerPrefs.HasKey("MaxScore")) PlayerPrefs.SetInt("MaxScore", 0);
        menuBestScoreTxt.text = string.Format("{0:n0}", PlayerPrefs.GetInt("MaxScore")); 
    }

    public void GameStart()
    {
        menuCamera.SetActive(false);
        gameCamera.SetActive(true);

        menuPanel.SetActive(false);
        gamePanel.SetActive(true);

        //player.gameObject.SetActive(true);
    }

    public void StageStart()
    {
        itemShop.SetActive(false);
        weaponShop.SetActive(false);
        stageStartZone.SetActive(false);
        enemyRespawnZone[0].parent.gameObject.SetActive(true);

        isBattle = true;
        StartCoroutine(InBattle());
    }

    public void GameOver()
    {
        gamePanel.SetActive(false);
        gameOverPanel.SetActive(true);
        currentScoreTxt.text = inGameScoreTxt.text;

        int maxScore = PlayerPrefs.GetInt("MaxScore"); 
        if(score > maxScore) // 최고점수 갱신
        {
            bestTxt.gameObject.SetActive(true);
            PlayerPrefs.SetInt("MaxScore", score);
        }
    }

    public void ReStart()
    {
        SceneManager.LoadScene(0);
    }

    public void StageEnd()
    {
        //player.transform.position = Vector3.up * 2;
        isBattle = false;
        enemyRespawnZone[0].parent.gameObject.SetActive(false);

        itemShop.SetActive(true);
        weaponShop.SetActive(true);
        stageStartZone.SetActive(true);
        stage++;
    }

    IEnumerator InBattle()
    {
        if (stage % 5 == 0) // 5의 배수인 스테이지에서 보스 생성
        {
            int ran = Random.Range(0, 4);
            GameObject instantEnemy = Instantiate(enemies[3], enemyRespawnZone[ran].position, enemyRespawnZone[ran].rotation);
            Boss bossScript = instantEnemy.GetComponent<Boss>();
            boss = bossScript;
            boss.gameManager = this;
            current_Boss++;
        }
        else // 몬스터 랜덤하게 생성
        {
            for (int i = 0; i < stage; i++)
            {
                int ran = Random.Range(0, 3);
                respawnEnemyList.Add(ran);
                switch (ran)
                {
                    case 0:
                        current_NomalEnemy++;
                        break;
                    case 1:
                        current_ChargeEnemy++;
                        break;
                    case 2:
                        current_ADEnemy++;
                        break;
                }
            }

            while (respawnEnemyList.Count > 0)
            {
                int ran = Random.Range(0, 4);
                GameObject instantEnemy = Instantiate(enemies[respawnEnemyList[0]], enemyRespawnZone[ran].position, enemyRespawnZone[ran].rotation);
                Enemy enemyScript = instantEnemy.GetComponent<Enemy>();
                enemyScript.gameManager = this;
                yield return new WaitForSeconds(2.5f);
                respawnEnemyList.RemoveAt(0);
            }
        }

        while (current_NomalEnemy + current_ChargeEnemy + current_ADEnemy + current_Boss > 0) // 몬스터가 다 죽을 때까지 돌아감
        {
            yield return null; // 이것이 Update문이랑 비슷함 
        }

        yield return new WaitForSeconds(2.5f);
        if (boss != null) boss = null;
        StageEnd(); // 몬스터가 다 죽으면 실행
    }

    private void Update()
    {
        if (isBattle) playTime += Time.deltaTime;
    }

    private void LateUpdate() // Update()가 끝난 후 호출되는 생명주기 (UI작업할 때 유용)
    {
        // 인게임 UI
        inGameScoreTxt.text = string.Format("{0:n0}", score);
        stageTxt.text = "STAGE " + stage;
        int playHour = (int)(playTime / 3600);
        int playMin = (int)((playTime - playHour * 3600) / 60) ;
        int playSceond = (int)(playTime % 60);
        playTimeTxt.text = string.Format("{0:00}", playHour) + ":" + string.Format("{0:00}", playMin) + ":" + string.Format("{0:00}", playSceond);

        // 플레이어 상태 UI
        playerHpTxt.text = playerData.CurrentHp + " / " + playerData.MaxHp;
        //playerCoinTxt.text = string.Format("{0:n0}", player.currentCoin);

        //if (player.Weapons == null)
        //{
        //    playerAmmoTxt.text = "- / " + player.maxAmmo;
        //    Weapon2_CurrentAmmo.gameObject.SetActive(false);
        //    Weapon3_CurrentAmmo.gameObject.SetActive(false);
        //}
        //else if(player.Weapons.weaponsType == Weapons.WeaponsType.Melee)
        //{
        //    playerAmmoTxt.text = "- / " + player.maxAmmo;
        //    Weapon2_CurrentAmmo.gameObject.SetActive(false);
        //    Weapon3_CurrentAmmo.gameObject.SetActive(false);
        //}
        //else if (player.Weapons.weaponsType == Weapons.WeaponsType.Range)
        //{
        //    playerAmmoTxt.text = player.currentAmmo + " / " + player.maxAmmo;
            
        //    // 현재 총알 장전 수 UI
        //    Weapons weapons = player.무기[player.EquipObjcetIndex].GetComponent<Weapons>();
        //    if (player.EquipObjcetIndex == 1)
        //    {
        //        Weapon2_CurrentAmmo.gameObject.SetActive(true);
        //        Weapon2_CurrentAmmo.text = weapons.inBullet + " / " + weapons.maxBullet;
        //        Weapon3_CurrentAmmo.gameObject.SetActive(false);
        //    }
        //    else
        //    {
        //        Weapon3_CurrentAmmo.gameObject.SetActive(true);
        //        Weapon3_CurrentAmmo.text = weapons.inBullet + " / " + weapons.maxBullet;
        //        Weapon2_CurrentAmmo.gameObject.SetActive(false);
        //    }
        //}

        //// 무기 유무 UI
        //Weapon1_Image.color = new Color(1, 1, 1, player.무기보유[0] ? 1 : 0);
        //Weapon2_Image.color = new Color(1, 1, 1, player.무기보유[1] ? 1 : 0);
        //Weapon3_Image.color = new Color(1, 1, 1, player.무기보유[2] ? 1 : 0);
        //grenade_Image.color = new Color(1, 1, 1, player.currentGrenade > 0 ? 1 : 0);

        // 몬스터 수 UI
        currnt_NomalEnemyTxt.text = current_NomalEnemy.ToString();
        currnt_ChargeEnemyTxt.text = current_ChargeEnemy.ToString();
        currnt_ADEnemyTxt.text = current_ADEnemy.ToString();

        // 보스 체력 UI
        if (boss != null)
        {
            bossHpBar.localScale = new Vector3((float)boss.CurrentHp / boss.MaxHP, 1, 1);
            bossHpGroup.anchoredPosition = Vector3.down * 30;
        }
        else
            bossHpGroup.anchoredPosition = Vector3.up * 500;
    }
}
