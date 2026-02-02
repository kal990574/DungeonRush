namespace DungeonRush.UI.HUD
{
    public class StageDisplayModel : ViewModelBase
    {
        private int _chapter;
        private int _wave;
        private bool _isBossWave;

        public int Chapter => _chapter;
        public int Wave => _wave;
        public bool IsBossWave => _isBossWave;

        public string StageText => _isBossWave
            ? $"{_chapter}-BOSS"
            : $"{_chapter}-{_wave}";

        public void SetStage(int chapter, int wave, bool isBossWave)
        {
            _chapter = chapter;
            _wave = wave;
            _isBossWave = isBossWave;
            NotifyChanged();
        }
    }
}
