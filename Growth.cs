using System;
using System.Collections.Generic;

namespace FishTankSimulator{
    public class Growth
{
    private Dictionary<AgeStage, (float healthMultiplier, int eatToGrow)> growthData;
    public AgeStage CurrentStage { get; private set; }
    public int FoodEaten { get; private set; }

    public Growth()
    {
        growthData = new Dictionary<AgeStage, (float, int)>
        {
            { AgeStage.Hatchling, (1.0f, 4) },
            { AgeStage.Juvenile, (1.5f, 6) },
            { AgeStage.Adult, (2.0f, -1) }
        };
        CurrentStage = AgeStage.Hatchling;
    }

    public float GetHealthMultiplier() => growthData[CurrentStage].healthMultiplier;

    public void Eat()
    {
        if (CurrentStage < AgeStage.Adult && ++FoodEaten >= growthData[CurrentStage].eatToGrow)
        {
            Grow();
        }
    }

    private void Grow()
    {
        CurrentStage++;
        FoodEaten = 0;
    }
}

}