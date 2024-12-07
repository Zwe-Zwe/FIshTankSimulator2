using System;
using System.Numerics;

namespace FishTankSimulator
{
    public class LevelManagement
    {
        private Level _level;
        private int maxGameLevel;
        private int maxFoodCountLevel;
        private Shop _shop;
        private int maxWeaponCountLevel;

        public LevelManagement(Level level, Shop shop)
        {
            _shop = shop;
            _level = level;
            maxGameLevel = 10;
            Console.WriteLine("LevelManagement initialized with Game Level: " + _level.GameLevel);
        }

        public void LevelUpGameLevel()
        {
            if (_level.GameLevel < maxGameLevel)
            {
                _level.GameLevel++;
                Console.WriteLine("Game Level upgraded to: " + _level.GameLevel);
                UpdateMaxSnail();
                UpdateMaxFish();
                _shop.UpdateShopItems();
            }
            else
            {
                Console.WriteLine("Game Level is already at maximum.");
            }
        }

        public void LevelUpSnailLevel()
        {
            if (_level.SnailLevel < GetMaxSnailLevel())
            {
                _level.SnailLevel++;
                Console.WriteLine("Snail Level upgraded to: " + _level.SnailLevel);
            }
            else
            {
                Console.WriteLine("Snail Level is already at maximum.");
            }
        }

        public void LevelUpFoodLevel()
        {
            if (_level.FoodLevel < _level.GameLevel)
            {
                _level.FoodLevel++;
                Console.WriteLine("Food Level upgraded to: " + _level.FoodLevel);
            }
            else
            {
                Console.WriteLine("Food Level is already at maximum.");
            }
        }

        public void LevelUpWeaponLevel()
        {
            if (_level.WeaponLevel < _level.GameLevel)
            {
                _level.WeaponLevel++;
                Console.WriteLine("Weapon Level upgraded to: " + _level.WeaponLevel);
            }
            else
            {
                Console.WriteLine("Weapon Level is already at maximum.");
            }
        }

        public void LevelUpFoodCount()
        {
            if (_level.FoodCountLevel < GetMaxFoodCountLevel())
            {
                _level.FoodCountLevel++;
                Console.WriteLine("Food Count increased to: " + _level.FoodCountLevel);
            }
            else
            {
                Console.WriteLine("Food Count is already at maximum.");
            }
        }

        public void LevelUpWeaponCount()
        {
            if (_level.WeaponCountLevel < GetMaxWeaponCountLevel())
            {
                _level.WeaponCountLevel++;
                Console.WriteLine("Weapon Count increased to: " + _level.WeaponCountLevel);
            }
            else
            {
                Console.WriteLine("Weapon Count is already at maximum.");
            }
        }

        public int GetMaxSnailLevel()
        {
            int maxSnailLevel = _level.GameLevel;
            Console.WriteLine("Calculated Max Snail Level: " + maxSnailLevel);
            return maxSnailLevel;
        }

        public int GetMaxFoodCountLevel(){
            if(_level.GameLevel % 2 == 0){
                return maxFoodCountLevel = _level.GameLevel / 2;
            }
            else{
                return maxFoodCountLevel;
            }
        }
        public int GetMaxWeaponCountLevel(){
            if(_level.GameLevel % 2 == 0){
                return maxWeaponCountLevel = _level.GameLevel / 2;
            }
            else{
                return maxWeaponCountLevel;
            }
        }

        private void UpdateMaxSnail()
        {
            if (_level.GameLevel >= 7)
            {
                _level.MaxSnail = 2;
            }
            else if (_level.GameLevel >= 3)
            {
                _level.MaxSnail = 1;
            }
            else
            {
                _level.MaxSnail = 0;
            }
            Console.WriteLine("Updated Max Snail to: " + _level.MaxSnail);
        }

        private void UpdateMaxFish()
        {
            _level.MaxFish += 2;
            Console.WriteLine("Updated Max Fish to: " + _level.MaxFish);
        }

       
    }
}
