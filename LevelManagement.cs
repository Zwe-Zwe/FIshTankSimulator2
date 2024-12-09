using System;
using System.Numerics;

namespace FishTankSimulator
{
    public class LevelManagement
    {
        private Player _player;
        private int maxGameLevel;
        private int maxFoodCountLevel;
        private Shop _shop;
        private int maxWeaponCountLevel;
        public LevelManagement(Player player, Shop shop)
        {
            _shop = shop;
            _player = player;
            maxGameLevel = 10;
            Console.WriteLine("LevelManagement initialized with Game Level: " + Player.GameLevel);
        }

        public void LevelUpGameLevel()
        {
            if (Player.GameLevel < maxGameLevel)
            {
                // Check if the player has enough money
                var cost = GetGameLevelUpdateCost();
                if (_player.Money >= cost)
                {
                    _player.Money -= cost;
                    Player.GameLevel++;
                    Console.WriteLine("Game Level upgraded to: " + Player.GameLevel);
                    UpdateMaxHelper();
                    UpdateMaxFish();
                    _shop.UpdateShopItems();
                }
                else
                {
                    Console.WriteLine("Not enough money to upgrade Game Level.");
                }
            }
            else
            {
                Console.WriteLine("Game Level is already at maximum.");
            }
        }

        public void LevelUpHelperLevel()
        {
            if (_player.HelperLevel < GetMaxHelperLevel())
            {
                // Check if the player has enough money
                var cost = GetHelperUpdateCost();
                if (_player.Money >= cost)
                {
                    _player.Money -= cost;
                    _player.HelperLevel++;
                    Console.WriteLine("Helper Level upgraded to: " + _player.HelperLevel);
                }
                else
                {
                    Console.WriteLine("Not enough money to upgrade Helper Level.");
                }
            }
            else
            {
                Console.WriteLine("Helper Level is already at maximum.");
            }
        }

        public void LevelUpFoodLevel()
        {
            if (_player.FoodLevel < Player.GameLevel)
            {
                // Check if the player has enough money
                var cost = GetFoodUpdateCost();
                if (_player.Money >= cost)
                {
                    _player.Money -= cost;
                    _player.FoodLevel++;
                    Console.WriteLine("Food Level upgraded to: " + _player.FoodLevel);
                }
                else
                {
                    Console.WriteLine("Not enough money to upgrade Food Level.");
                }
            }
            else
            {
                Console.WriteLine("Food Level is already at maximum.");
            }
        }

        public void LevelUpWeaponLevel()
        {
            if (_player.WeaponLevel < Player.GameLevel)
            {
                // Check if the player has enough money
                var cost = GetWeaponUpdateCost();
                if (_player.Money >= cost)
                {
                    _player.Money -= cost;
                    _player.WeaponLevel++;
                    Console.WriteLine("Weapon Level upgraded to: " + _player.WeaponLevel);
                }
                else
                {
                    Console.WriteLine("Not enough money to upgrade Weapon Level.");
                }
            }
            else
            {
                Console.WriteLine("Weapon Level is already at maximum.");
            }
        }

        public void LevelUpFoodCount()
        {
            if (_player.FoodCountLevel < GetMaxFoodCountLevel())
            {
                // Check if the player has enough money
                var cost = GetFoodCountUpdateCost();
                if (_player.Money >= cost)
                {
                    _player.Money -= cost;
                    _player.FoodCountLevel++;
                    Console.WriteLine("Food Count increased to: " + _player.FoodCountLevel);
                }
                else
                {
                    Console.WriteLine("Not enough money to upgrade Food Count.");
                }
            }
            else
            {
                Console.WriteLine("Food Count is already at maximum.");
            }
        }

        public void LevelUpWeaponCount()
        {
            if (_player.WeaponCountLevel < GetMaxWeaponCountLevel())
            {
                // Check if the player has enough money
                var cost = GetWeaponCountUpdateCost();
                if (_player.Money >= cost)
                {
                    _player.Money -= cost;
                    _player.WeaponCountLevel++;
                    Console.WriteLine("Weapon Count increased to: " + _player.WeaponCountLevel);
                }
                else
                {
                    Console.WriteLine("Not enough money to upgrade Weapon Count.");
                }
            }
            else
            {
                Console.WriteLine("Weapon Count is already at maximum.");
            }
        }

        public int GetMaxHelperLevel()
        {
            if (Player.GameLevel < 3)
            {
                Console.WriteLine("Helper upgrades are not available until Game Level 3.");
                return 0; 
            }

            int maxHelperLevel = Player.GameLevel - 2;
            return maxHelperLevel;
        }


        public int GetMaxFoodCountLevel(){
            if(Player.GameLevel % 2 == 0){
                return maxFoodCountLevel = Player.GameLevel / 2;
            }
            else{
                return maxFoodCountLevel;
            }
        }
        public int GetMaxWeaponCountLevel(){
            if(Player.GameLevel % 2 == 0){
                return maxWeaponCountLevel = Player.GameLevel / 2;
            }
            else{
                return maxWeaponCountLevel;
            }
        }

        private void UpdateMaxHelper()
        {
            if (Player.GameLevel >= 9)
            {
                _player.MaxHelper = 3;
            }
            else if (Player.GameLevel >= 6)
            {
                _player.MaxHelper = 2;
            }
            else if (Player.GameLevel >= 3)
            {
                _player.MaxHelper = 1;
            }
            else
            {
                _player.MaxHelper = 0;
            }
            Console.WriteLine("Updated Max Helper to: " + _player.MaxHelper);
        }
        private void UpdateMaxFish()
        {
            _player.MaxFish += 2;
            Console.WriteLine("Updated Max Fish to: " + _player.MaxFish);
        }
        public int GetFoodUpdateCost(){
            if(_player.FoodLevel <= 5){
                return 200 * _player.FoodLevel;
            }
            else{
                return 500 * _player.FoodLevel - 1000;
            }
        }
        public int GetWeaponUpdateCost(){
            if(_player.WeaponLevel <= 5){
                return 200 * _player.WeaponLevel;
            }
            else{
                return 500 * _player.WeaponLevel - 1000;
            }
        } 
        public int GetFoodCountUpdateCost(){
            return 400 * (int)Math.Pow(2, _player.FoodCountLevel - 1);
        }
        public int GetWeaponCountUpdateCost(){
            return 400 * (int)Math.Pow(2, _player.WeaponCountLevel - 1);
        }
        public int GetGameLevelUpdateCost(){
            return 2000 + 2000 * (Player.GameLevel - 1);
        }
        public int GetHelperUpdateCost(){
            return 1000 + 1000 * (_player.HelperLevel - 1);
        }

    }
}
