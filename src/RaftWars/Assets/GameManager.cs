using System.Collections.Generic;
using DefaultNamespace.Common;
using InputSystem;
using Interface;
using RaftWars.Infrastructure;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;
    private bool _started;
    
    private int platformsCountOnStart = 1;
    private int warriorsCountOnStart = 2;
    private int powerCountOnStart = 10;
    private int healthCountOnStart = 10;
    
    [Header("Managers")]
    [SerializeField] private MapGenerator map;
    private readonly List<Enemy> _enemies = new List<Enemy>();

    private int _stage = 1;
    public Enemy boss;

    private float buyableDamagePercent = 0.2f, buyableHealthPercent = 0.2f;
    private int damageCost = 10, healthAmplificationCost = 10;
    private AdvertisingService _advertising;
    private Hud hud;
    private InputService _input;

    private StateMachine _stateMachine;

    public void Construct(MapGenerator mapGenerator, StateMachine stateMachine)
    {
        map = mapGenerator;
        hud = Game.Hud;

        hud.Replay.onClick.AddListener(RestartLevel);
        hud.Continue.onClick.AddListener(Continue);
        hud.BuyHealth.onClick.AddListener(IncreaseHealth);
        hud.BuyDamage.onClick.AddListener(IncreaseDamage);
        _stateMachine = stateMachine;
        _input = Game.InputService;
        _input.Disable();
        _advertising = Game.AdverisingService;
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(instance);
            instance = this;
        }

        map.Generate(_stage);
        map.GenerateBoss();
    }

    private void Update()
    {
        if (!_started)
        {
            StartGameOnClick();
        }
        else if(_stage == 5 && boss == null && !hud.stagePanel.activeSelf)
        {
            WinTheGame();
        }
        else if(_stage != 5 && boss == null && !hud.stagePanel.activeSelf)
        {
            WarmupStage();
        }

        for(var i = 0; i < _enemies.Count; i++)
        {
            if (_enemies[i] == null)
                _enemies.RemoveAt(i);
        }
    }

    private void WarmupStage()
    {
        Player.instance.canPlay = false;
        hud.blackBG.SetActive(true);
        WarmupUiStats();
        WarmupStats();
        hud.stagePanel.SetActive(true);
        hud.progressFill.fillAmount = _stage / 5f;
        _stage++;
    }

    private void WarmupStats()
    {
        hud.platformsCountAdd.text = "+" + (Player.instance.platformCount - platformsCountOnStart);
        if (Player.instance.warriorsCount - warriorsCountOnStart >= 0)
        {
            hud.warriorsCountAdd.text = "+" + (Player.instance.warriorsCount - warriorsCountOnStart);
        }
        else
        {
            hud.warriorsCountAdd.text = (Player.instance.warriorsCount - warriorsCountOnStart).ToString();
        }

        if (Player.instance.damage - powerCountOnStart >= 0)
        {
            hud.powerCountAdd.text = "+" + (Player.instance.damage - powerCountOnStart);
        }
        else
        {
            hud.powerCountAdd.text = (Player.instance.damage - powerCountOnStart).ToString();
        }

        if (Player.instance.hp - healthCountOnStart >= 0)
        {
            hud.healthCountAdd.text = "+" + (Player.instance.hp - healthCountOnStart);
        }
        else
        {
            hud.healthCountAdd.text = (Player.instance.hp - healthCountOnStart).ToString();
        }

        warriorsCountOnStart = Player.instance.warriorsCount;
        platformsCountOnStart = Player.instance.platformCount;
        powerCountOnStart = (int)Player.instance.damage;
        healthCountOnStart = (int)Player.instance.hp;
    }

    private void WarmupUiStats()
    {
        hud.platformsCountPrev.text = platformsCountOnStart.ToString();
        hud.warriorsCountPrev.text = warriorsCountOnStart.ToString();
        hud.powerCountPrev.text = powerCountOnStart.ToString();
        hud.healthCountPrev.text = healthCountOnStart.ToString();
    }

    private void WinTheGame()
    {
        Player.instance.canPlay = false;
        hud.blackBG.SetActive(true);
        hud.winPanel.SetActive(true);
        IncrementLevel();
        _advertising.ShowInterstitial();
    }

    private static void IncrementLevel()
    {
        void Increment()
        {
            PlayerPrefs.SetInt("Level", PlayerPrefs.GetInt("Level") + 1);
        }

        //Increment();
        LoopLevelsOnOverflow();
    }

    private static void LoopLevelsOnOverflow()
    {
        if (PlayerPrefs.GetInt("Level") == 6)
        {
            PlayerPrefs.SetInt("Level", 1);
        }
    }

    private void StartGameOnClick()
    {
        _input.Enable();
        if (!Input.GetMouseButtonUp(0)) return;
        _started = true;
        hud.tapToPlay.SetActive(false);
        Player.instance.canPlay = true;
    }

    public void AddEnemy(Enemy enemy)
    {
        _enemies.Add(enemy);
    }

    public void NextStage()
    {
        hud.progressText.text = _stage + "/5";
        map.Generate(_stage);
        Player.instance.canPlay = true;
        hud.blackBG.SetActive(false);
        hud.stagePanel.SetActive(false);
        map.GenerateBoss();
        _advertising.ShowInterstitial();
    }

    public void Continue()
    {
        SceneManager.LoadScene("Level" + PlayerPrefs.GetInt("Level", 1));
    }

    public void RestartLevel()
    {
        _advertising.ShowInterstitial();
        _stateMachine.Enter<LoadLevelState, int>(PlayerPrefs.GetInt("Level", 1));
    }

    public void PlayerLost()
    {
        hud.blackBG.SetActive(true);
        hud.failedPanel.SetActive(true);
        _input.Disable();
    }

    public void IncreaseHealth()
    {
        bool IsNotEnoughMoneyToIncreaseHealth()
        {
            return Player.instance.coins < healthAmplificationCost;
        }

        if (hud._healthUpgradeVideoIcon.gameObject.activeInHierarchy)
        {
            _advertising.ShowRewarded(() =>
            {
                Player.instance.IncreaseHealth((float)buyableHealthPercent);
                UpdateNextHealthUpgrade();
            });
            return;
        }
        if (IsNotEnoughMoneyToIncreaseHealth()) return;

        Player.instance.SpendCoins(healthAmplificationCost);
        Player.instance.IncreaseHealth((float)buyableHealthPercent);
        UpdateNextHealthUpgrade();
    }

    private void UpdateNextHealthUpgrade()
    {
        void MakeNextHealthUpgradeMorePowerful()
        {
            buyableHealthPercent += 0.1f;
            var integerPercent = (int)(buyableHealthPercent * 100);
            hud.hpPercentText.text = $"+{integerPercent}%";
        }

        void IncreaseHealthUpgradeCost()
        {
            if (RandomExtension.ProbabilityCheck(.7f))
            {
                hud._healthUpgradeVideoIcon.gameObject.SetActive(true);
                hud.hpCostText.enabled = false;
                hud._healthUpgradeCoin.gameObject.SetActive(false);
            }
            else
            {
                hud._healthUpgradeVideoIcon.gameObject.SetActive(false);
                hud.hpCostText.enabled = true;
                hud._healthUpgradeCoin.gameObject.SetActive(true);
            }
            healthAmplificationCost += 10;
            hud.hpCostText.text = healthAmplificationCost.ToString();
        }
        
        MakeNextHealthUpgradeMorePowerful();
        IncreaseHealthUpgradeCost();
    }

    public void IncreaseDamage()
    {
        bool IsNotEnoughCoinsToAmplifyDamage()
        {
            return Player.instance.coins < damageCost;
        }

        if (hud._damageUpgradeVideoIcon.gameObject.activeInHierarchy)
        {
            _advertising.ShowRewarded(() =>
            {
                Player.instance.AmplifyDamage(buyableDamagePercent);
                UpdateNextDamageUpgrade();
            });
            return;
        }
        
        if (IsNotEnoughCoinsToAmplifyDamage()) return;
        
        Player.instance.SpendCoins(damageCost);
        Player.instance.AmplifyDamage(buyableDamagePercent);
        UpdateNextDamageUpgrade();
    }

    private void UpdateNextDamageUpgrade()
    {
        void IncreaseNextDamageAmplification()
        {
            buyableDamagePercent += 0.1f;
            var integerPercent = (int) (buyableDamagePercent * 100);
            hud.damagePercentText.text = $"+{integerPercent}%";
        }
        
        void IncreaseNextDamageAmplificationCost()
        {
            if (RandomExtension.ProbabilityCheck(.7f))
            {
                hud._damageUpgradeVideoIcon.gameObject.SetActive(true);
                hud.damageCostText.enabled = false;
                hud._damageUpgradeCoin.enabled = false;
            }
            else
            {
                hud._damageUpgradeVideoIcon.gameObject.SetActive(false);
                hud.damageCostText.enabled = true;
                hud._damageUpgradeCoin.enabled = true;
            }
            damageCost += 10;
            hud.damageCostText.text = damageCost.ToString();
        }

        IncreaseNextDamageAmplification();
        IncreaseNextDamageAmplificationCost();
    }
}
