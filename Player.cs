using System;
using System.Numerics;

namespace FishTankSimulator{
    public class Player{
        private static int gameLevel;
        private int snailLevel;
        private int foodLevel;
        private int foodCountLevel;
        private int weaponLevel;
        private int weaponCountLevel;
        private int maxFish;
        private int maxSnail;
        private int money;
        public Player(){
            gameLevel = 1;
            snailLevel = 1;
            foodLevel = 1;
            weaponLevel = 1;
            maxFish = 5;
            maxSnail = 0;
            money = 3000000;
            foodCountLevel = 1;
            weaponCountLevel = 1;
        }
        public PredatorFishType GetPredatorType(){
            if(gameLevel <= 3){
                return PredatorFishType.Small;
            }
            else if(gameLevel <= 6){
                return PredatorFishType.Medium;
            }
            else{
                return PredatorFishType.Big;
            }
        }
        public static int GameLevel{
            get { return gameLevel; }
            set { gameLevel = value; }
        }
        public int SnailLevel{
            get { return snailLevel; }
            set { snailLevel = value; }
        }
        public int FoodLevel{
            get { return foodLevel; }
            set { foodLevel = value; }
        }
        public int WeaponLevel{
            get { return weaponLevel; }
            set { weaponLevel = value; }
        }
        public int MaxFish{
            get { return maxFish; }
            set { maxFish = value; }
        }
        public int MaxSnail{
            get { return maxSnail; }
            set { maxSnail = value; }
        }
        public int Money{
            get { return money; }
            set { money = value; }
        }
        public int FoodCountLevel{
            get { return foodCountLevel; }
            set { foodCountLevel = value; }
        }
        public int WeaponCountLevel{
            get { return weaponCountLevel; }
            set { weaponCountLevel = value; }
        }

    }
}