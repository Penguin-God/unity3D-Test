using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject menuCamera;
    public GameObject gameCamera;
    public GameObject menuPanel;
    public GameObject gamePanel;

    public Player player;
    public Boss boss;

    public int stage;
    public float playTime;
    public int current_NomalEnemy;
    public int current_ChargeEnemy;
    public int current_ADEnemy;
    public int current_Boss;
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

    public GameObject itemShop;
    public GameObject weaponShop;
    public GameObject stageStartZone;

    public Transform[] enemyRespawnZone;
    public GameObject[] enemies;
    private List<int> respawnEnemyList;

    private void Awake()
    {
        bestScoreTxt.text = string.Format("{0:n0}", PlayerPrefs.GetInt("MaxScore"));
        respawnEnemyList = new List<int>();
    }

    public void GameStart()
    {
        menuCamera.SetActive(false);
        gameCamera.SetActive(true);

        menuPanel.SetActive(false);
        gamePanel.SetActive(true);

        player.gameObject.SetActive(true);
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

    public void StageEnd()
    {
        player.transform.position = Vector3.up * 2;
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
        scoreTxt.text = string.Format("{0:n0}", player.score);
        stageTxt.text = "STAGE " + stage;
        int playHour = (int)(playTime / 3600);
        int playMin = (int)((playTime - playHour * 3600) / 60) ;
        int playSceond = (int)(playTime % 60);
        playTimeTxt.text = string.Format("{0:00}", playHour) + ":" + string.Format("{0:00}", playMin) + ":" + string.Format("{0:00}", playSceond);

        // 플레이어 상태 UI
        playerHpTxt.text = player.playerhp + " / " + player.maxHp;
        playerCoinTxt.text = string.Format("{0:n0}", player.coin);
        if (player.Weapons == null)
            playerAmmoTxt.text = "- / " + player.Max총알;
        else if(player.Weapons.type == Weapons.Type.Melee)
            playerAmmoTxt.text = "- / " + player.Max총알;
        else if (player.Weapons.type == Weapons.Type.Range)
            playerAmmoTxt.text = player.보유총알 + " / " + player.Max총알;

        // 무기 유무 UI
        Weapon1_Image.color = new Color(1, 1, 1, player.무기보유[0] ? 1 : 0);
        Weapon2_Image.color = new Color(1, 1, 1, player.무기보유[1] ? 1 : 0);
        Weapon3_Image.color = new Color(1, 1, 1, player.무기보유[2] ? 1 : 0);
        grenade_Image.color = new Color(1, 1, 1, player.수류탄 > 0 ? 1 : 0);

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
