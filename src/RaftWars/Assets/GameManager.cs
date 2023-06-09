using System;
using System.Collections.Generic;
using DefaultNamespace.Common;
using DG.Tweening;
using Infrastructure;
using Infrastructure.States;
using InputSystem;
using Interface;
using RaftWars.Infrastructure;
using RaftWars.Infrastructure.Services;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public ClickDetector ClickDetector;
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
    private PlayerService player;
    private Arrow _arrow;
    private Camera _camera;
    public event Action GameStarted;

    public void Construct(MapGenerator mapGenerator, StateMachine stateMachine, Arrow arrow, Camera camera,
        Pause pause)
    {
        _pause = pause;
        map = mapGenerator;
        hud = Game.Hud;
        player = Game.PlayerService;
        _arrow = arrow;
        _camera = camera;
        hud.SetActiveTapToPlay(false);
        hud.Replay.onClick.AddListener(RestartLevel);
        hud.BuyHealth.onClick.AddListener(IncreaseHealth);
        hud.BuyDamage.onClick.AddListener(IncreaseDamage);
        hud.NextStage.onClick.AddListener(NextStage);
        hud.MoneyForAdvertisingEndMenu.transform.parent.gameObject.SetActive(false);
        hud.Revive.onClick.AddListener(QueryRevive);
        var currentLevel = CrossLevelServices.LevelService.Level.ToString();
        hud.ShowPreviousLevel(currentLevel);
        hud.ShowNextLevel((CrossLevelServices.LevelService.Level + 1).ToString());
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
    }

    public void EnableBonusMenu()
    {
        Debug.Log($"{nameof(GameManager)}.{nameof(EnableBonusMenu)} bonus menu enabled scenario");
        hud.BonusMenu.Show(StartGame);
        Game.Hud.DisableTapToPlay();
        Game.Hud.BonusMenu.DeclineBonusesPicking.onClick.AddListener(StartGame);
    }

    public void DisableBonusMenu(bool andTapToStart = false)
    {
        hud.BonusMenu.Hide();
        ClickDetector.Clicked += StartGame;
        if(andTapToStart)
            Game.Hud.DisableTapToPlay();
        Debug.Log($"{nameof(GameManager)}.{nameof(DisableBonusMenu)} bonus menu disabled scenario");
    }

    private bool _wonTheGame;
public bool GamePaused;
private Pause _pause;
private BossAppearing _appearing;

public void StartGame()
{
    Debug.Log($"{nameof(GameManager)}.{nameof(StartGame)} called. Started: {_started}");
    if(_started)
        return; 
    _arrow.Construct(player, _camera, boss);
    GameStarted?.Invoke();
    _input.Enable();
    _started = true;
    hud.tapToPlay.SetActive(false);
    Player.instance.canPlay = true;
    _pause.ShowButton();
    map.GenerateBoss();
    Game.Hud.HideBonusWindow();
    Game.AdverisingService.ShowInterstitial();
    ClickDetector.Clicked -= StartGame;
}

private void Update()
    {
        // if (!_started)
        // {
        // }
        // else if(_stage == 5 && boss == null && !hud.stagePanel.activeSelf)
        // {
        //     if(_wonTheGame)
        //         return;
        //     _wonTheGame = true;
        //     WinTheGame();
        // }
        // else if(_stage != 5 && boss == null && !hud.stagePanel.activeSelf)
        // {
        //     IncrementStage();
        // }
        //
        for(var i = 0; i < _enemies.Count; i++)
        {
            if (_enemies[i] == null)
                _enemies.RemoveAt(i);
        }
    }

private void IncrementStage()
    {
        hud.HidePauseButton();
        _input.Disable();
        Player.instance.canPlay = false;
        hud.blackBG.SetActive(true);
        WarmupUiStats();
        WarmupStats();
        hud.stagePanel.SetActive(true);
        var progress = _stage switch
        {
            1 => 0.223f,
            2 => 0.434f,
            3 => 0.644f,
            4 => 0.854f
        };
        hud.progressFill.DOValue(progress, 1f);
        hud.sliderOnLoseScreen.value = progress;
        hud.NewSpecialPlatformProgress.SetValue((float)_stage / 5);
        _stage++;
    }

private void QueryRevive()
    {
        _advertising.ShowRewarded(ExecuteRevive);
    }

    private void ExecuteRevive()
    {
        _input.Enable();
        hud.blackBG.SetActive(false);
        hud.failedPanel.SetActive(false);
        Game.PlayerService.Revive();
        hud.ShowPauseButton();
    }

    private void WarmupStats()
    {
        hud.platformsCountAdd.text = "+" + (Player.instance.PlatformsCount - platformsCountOnStart);
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
        platformsCountOnStart = Player.instance.PlatformsCount;
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
        hud.HidePauseButton();
        hud.buttonGroup.gameObject.SetActive(false);
        hud.stagePanel.SetActive(true);
        hud.blackBG.SetActive(true);
        _input.Disable();
        Player.instance.canPlay = false;
        hud.progressFill.DOValue(1f, 1f).OnComplete(() =>
        {
            hud._nextLevelImage.sprite = hud._completedStageSprite;
        });
        hud.MoneyForAdvertisingEndMenu.transform.parent.gameObject.SetActive(true);
        hud.MoneyForAdvertisingEndMenu.onClick.AddListener(() =>
        {
            _advertising.ShowRewarded(() =>
            {
                Game.MoneyService.AddCoins(100);
            });
        });
        _advertising.ShowInterstitial();
        CrossLevelServices.LevelService.Increment();
        hud.NextStage.onClick.RemoveAllListeners();
        hud.NextStage.onClick.AddListener(Continue);
        hud.NewSpecialPlatformProgress.SetValue(1f);
    }

    public void AddEnemy(Enemy enemy)
    {
        _enemies.Add(enemy);
    }

    public void NextStage()
    {
        hud.blackBG.SetActive(false);
        hud.stagePanel.SetActive(false);
        hud.BonusMenu.Show(LaunchNextStage);
        hud.BonusMenu.DeclineBonusesPicking.onClick.RemoveAllListeners();
        hud.BonusMenu.DeclineBonusesPicking.onClick.AddListener(() =>
        {
            hud.BonusMenu.Hide();
            LaunchNextStage();
        }); 
    }

    private void LaunchNextStage()
    {
        hud.ShowPauseButton();
        _input.Enable();
        hud.progressText.text = _stage + "/5";
        map.Generate(_stage);
        Player.instance.canPlay = true;
        Game.AdverisingService.ShowInterstitial();
        map.GenerateBoss();
    }

    public void Continue()
    {
        _stateMachine.Enter<RewardedSpecialPlatformState>();
    }

    public void RestartLevel()
    {
        _advertising.ShowInterstitial();
        _stateMachine.Enter<LoadLevelState, int>(CrossLevelServices.LevelService.Level);
    }

    public void PlayerLost()
    {
        hud.blackBG.SetActive(true);
        hud.failedPanel.SetActive(true);
        _input.Disable();
        Game.AudioService.PlayLooseSound();
        hud.HidePauseButton();
    }

    public void IncreaseHealth()
    {
        bool IsNotEnoughMoneyToIncreaseHealth()
        {
            return Game.MoneyService.Amount < healthAmplificationCost;
        }

        if (hud._healthUpgradeVideoIcon.gameObject.activeInHierarchy)
        {
            _advertising.ShowRewarded(() =>
            {
                Player.instance.IncreaseHealth(.1f);
                UpdateNextHealthUpgrade();
            });
            return;
        }
        if (IsNotEnoughMoneyToIncreaseHealth()) return;
        
        Game.MoneyService.Spend(healthAmplificationCost);
        Player.instance.IncreaseHealth(.1f);
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
            return Game.MoneyService.Amount < damageCost;
        }

        if (hud._damageUpgradeVideoIcon.gameObject.activeInHierarchy)
        {
            _advertising.ShowRewarded(() =>
            {
                Player.instance.AmplifyDamage(.1f);
                UpdateNextDamageUpgrade();
            });
            return;
        }
        Game.AdverisingService.ShowInterstitial();
        
        if (IsNotEnoughCoinsToAmplifyDamage()) return;
        
        Game.MoneyService.Spend(damageCost);
        Player.instance.AmplifyDamage(.1f);
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

    public void BossDied()
    { 
        if (!_started)
        {
        }
        else if(_stage == 5 && !hud.stagePanel.activeSelf)
        {
            if(_wonTheGame)
                return;
            _wonTheGame = true;
            WinTheGame();
        }
        else if(_stage != 5 && !hud.stagePanel.activeSelf)
        {
            IncrementStage();
        }

        for(var i = 0; i < _enemies.Count; i++)
        {
            if (_enemies[i] == null)
                _enemies.RemoveAt(i);
        }
    }

    public void DestroyAll()
    {
        foreach (Enemy enemy in FindObjectsOfType<Enemy>())
        {
            _enemies.Remove(enemy);
            Destroy(enemy.gameObject);
        }

        Game.PlayerService.Dispose();
    }
}
