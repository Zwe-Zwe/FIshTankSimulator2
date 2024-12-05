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
        private int tankLevel;
        public Level(){
            gameLevel = 1;
            snailLevel = 1;
            foodLevel = 1;
            foodCountLevel = 1;
            weaponLevel = 1;
            weaponCountLevel = 1;
            tankLevel = 1;
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
        public int FoodCountLevel{
            get { return foodCountLevel; }
            set { foodCountLevel = value; }
        }
        public int WeaponLevel{
            get { return weaponLevel; }
            set { weaponLevel = value; }
        }
        public int WeaponCountLevel{
            get { return weaponCountLevel; }
            set { weaponCountLevel = value; }
        }
        public int TankLevel{
            get { return tankLevel; }
            set { tankLevel = value; }
        }
        
    }
}