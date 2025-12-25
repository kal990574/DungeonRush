using System;

//public class HUDPresenter : IDisposable
//{
//    private readonly IHUDView _view;
//    private readonly PlayerStats _playerStats;

//    // TODO : StageManager 추가 시 해당 부분 수정 필요
//    private int _currentChapter = 1;
//    private int _currentWave = 1;
//    private bool _isBoss = false;

//    public HUDPresenter(IHUDView view, PlayerStats playerStats)
//    {
//        _view = view;
//        _playerStats = playerStats;

//        SubscribeEvents();
//        UpdateAll();
//    }

//    private void SubscribeEvents()
//    {
//        _playerStats.OnHealthChanged += UpdateHP;
//        _playerStats.OnXPChanged += UpdateXP;
//        _playerStats.OnGoldChanged += UpdateGold;
//        // _stageManager.OnStageChanged += UpdateStage; // TODO: StageManager 추가 시
//    }

//    private void UpdateAll()
//    {
//        UpdateHP(_playerStats.CurrentHP, _playerStats.MaxHP);
//        UpdateXP(_playerStats.CurrentXP, _playerStats.RequiredXP);
//        UpdateGold(_playerStats.Gold);
//        UpdateStage(_currentChapter, _currentWave, _isBoss);
//    }

//    private void UpdateHP(float current, float max)
//    {
//        float ratio = max > 0 ? current / max : 0f;
//        _view.UpdateHPBar(ratio);
//        _view.UpdateHPText(current, max);
//    }

//    private void UpdateXP(int current, int required)
//    {
//        float ratio = required > 0 ? (float)current / required : 0f;
//        _view.UpdateXPBar(ratio);
//        _view.UpdateXPText(current, required);
//    }

//    private void UpdateGold(int amount)
//    {
//        _view.UpdateGold(amount);
//    }

//    private void UpdateStage(int chapter, int wave, bool isBoss)
//    {
//        _view.UpdateStage(chapter, wave, isBoss);
//    }

//    public void Dispose()
//    {
//        _playerStats.OnHealthChanged -= UpdateHP;
//        _playerStats.OnXPChanged -= UpdateXP;
//        _playerStats.OnGoldChanged -= UpdateGold;
//    }
//}
