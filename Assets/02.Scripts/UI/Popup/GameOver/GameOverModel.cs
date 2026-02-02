using System;

namespace DungeonRush.UI.Popup
{
    public class GameOverModel : ViewModelBase
    {
        private int _chapter;
        private int _wave;
        private int _kills;
        private float _time;
        private int _score;

        public int Chapter => _chapter;
        public int Wave => _wave;
        public int Kills => _kills;
        public float Time => _time;
        public int Score => _score;

        public string StageText => $"{_chapter}-{_wave}";

        public string TimeText
        {
            get
            {
                var timeSpan = TimeSpan.FromSeconds(_time);
                return $"{(int)timeSpan.TotalMinutes:D2}:{timeSpan.Seconds:D2}";
            }
        }

        public void SetResult(int chapter, int wave, int kills, float time, int score)
        {
            _chapter = chapter;
            _wave = wave;
            _kills = kills;
            _time = time;
            _score = score;
            NotifyChanged();
        }
    }
}
