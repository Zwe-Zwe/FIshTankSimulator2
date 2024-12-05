using System;

namespace FishTankSimulator
{
    public class Level
    {
        private float _predatorMultiplier;
        private float _tankMaxCapacity;
        private float _unlockedSpecies;
        private Fish _requiredSpecies;
        private float _requiredCoinBalance;
        private int _currentLevel;
        private float _timeLimit;
        private int _foodCountPerSecond;
        private int _maxFoodLevel;
        private int _maxSnailLevel;

        public Level(int currentLevel)
        {
            _currentLevel = currentLevel;
            LevelUpReset();
        }

        public int CurrentLevel
        {
            get => _currentLevel;
            private set => _currentLevel = value;
        }

        public float TankMaxCapacity => _tankMaxCapacity;
        public float TimeLimit => _timeLimit;

        public void LevelUpReset()
        {
            _predatorMultiplier = 1 + (_currentLevel * 0.1f);
            _tankMaxCapacity = 10 + (_currentLevel * 2);
            _unlockedSpecies = _currentLevel; // New species unlocked every level
            _requiredCoinBalance = _currentLevel * 100;
            _timeLimit = 60 + (_currentLevel * 15); // 1-minute base + 15 seconds per level
            _foodCountPerSecond = Math.Min(5, _currentLevel); // Max limit of 5
            _maxFoodLevel = Math.Min(10, _currentLevel);
            _maxSnailLevel = Math.Min(5, _currentLevel);
        }

        public bool LevelUp()
        {
            // Check if level goals are met
            if (_requiredCoinBalance <= 0) // Add actual condition
            {
                _currentLevel++;
                LevelUpReset();
                return true;
            }
            return false;
        }

        public bool Lost => _timeLimit <= 0;
    }
}
