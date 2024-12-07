using System;
using System.Numerics;

namespace FishTankSimulator{
    public class Level{
        private int gameLevel;
        private int snailLevel;
        private int foodLevel;
        private int foodCountLevel;
        private int weaponLevel;
        private int weaponCountLevel;
        private int maxFish;
        private int maxSnail;
        private int money;
        public Level(){
            gameLevel = 1;
            snailLevel = 1;
            foodLevel = 1;
            weaponLevel = 1;
            maxFish = 5;
            maxSnail = 0;
            money = 30000;
            foodCountLevel = 1;
            weaponCountLevel = 1;
        }
        public int GameLevel{
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