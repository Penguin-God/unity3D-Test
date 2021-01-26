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

    public GameObject itemShop;
    public GameObject weaponShop;
    public GameObject stageStartZone;

    private void Awake()
    {
        bestScoreTxt.text = string.Format("{0:n0}", PlayerPrefs.GetInt("MaxScore"));
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

        isBattle = true;
        StartCoroutine(Battle());
    }

    public void StageEnd()
    {
        player.transform.position = Vector3.up * 2;
        isBattle = false;

        itemShop.SetActive(true);
        weaponShop.SetActive(true);
        stageStartZone.SetActive(true);
        stage++;
    }

    IEnumerator Battle()
    {
        yield return new WaitForSeconds(5f);
        StageEnd();
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
        currnt_NomalEnemyTxt.text = currnt_NomalEnemy.ToString();
        currnt_ChargeEnemyTxt.text = currnt_ChargeEnemy.ToString();
        currnt_ADEnemyTxt.text = currnt_ADEnemy.ToString();

        // 보스 체력 UI
        bossHpBar.localScale = new Vector3((float)boss.CurrentHp / boss.MaxHP, 1, 1);
    }
}
