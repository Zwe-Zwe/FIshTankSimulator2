using System;
using System.Collections.Generic;
using System.Numerics;
using Raylib_cs;

namespace FishTankSimulator
{
    public class Weapon
    {
        private float damage;
        private float range;
        private float cost;
        private bool isActived;
        private float stunDuration;

        // Effect state for drawing
        private bool isEffectActive;
        private Vector2 effectPosition;
        private float effectRadius;
        private int effectFrame;
        private const int effectDuration = 15;
        private const float maxEffectRadius = 50.0f;

        public Weapon()
        {
            damage = 50;
            range = 30;
            cost = 20;
            isActived = false;
            stunDuration = 0.7f;

            // Initialize effect state
            isEffectActive = false;
            effectRadius = 0;
            effectFrame = 0;
        }

        public bool IsActived
        {
            get => isActived;
            set => isActived = value;
        }

        public float Range
        {
            get => range;
        }

        public void Attack(PredatorFish shark)
        {
            shark.GetAttacked(damage, stunDuration);
        }

        
        public void StartWeaponEffect(Vector2 position)
        {
            isEffectActive = true;
            effectPosition = position;
            effectRadius = 0;
            effectFrame = 0;
        }

        public void DrawWeaponEffect()
        {
            if (isEffectActive)
            {
                // Increase effect radius and draw the expanding circle
                effectRadius += maxEffectRadius / effectDuration; // Smoothly expand radius
                Raylib.DrawCircle((int)effectPosition.X, (int)effectPosition.Y, effectRadius, Color.Red);

                effectFrame++;
                if (effectFrame >= effectDuration)
                {
                    isEffectActive = false; // Disable the effect after the duration
                }
            }
        }


    }
}
