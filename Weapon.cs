using System;
using System.Collections.Generic;
using System.Numerics;
using Raylib_cs;

namespace FishTankSimulator
{
    public class Weapon
    {
        private float damage;
        private int cost;
        private bool isActived;
        // Effect state for drawing
        private bool isEffectActive;
        private Vector2 effectPosition;
        private float effectRadius;
        private const float effectDuration = 0.2f;
        private const float maxEffectRadius = 30.0f;
        private float effectTimer = 0f;

        public Weapon()
        {
            isActived = false;
            // Initialize effect state
            isEffectActive = false;
            effectRadius = 0;
        }

        public bool IsActived
        {
            get => isActived;
            set => isActived = value;
        }

        public int Cost
        {
            get => cost;
        }

        public void Attack(PredatorFish shark)
        {
            shark.GetAttacked(damage);
        }

        
        public void HandleWeaponEffect(Vector2? startPosition, float deltaTime, Player player)
        {
            damage = 50 + ((player.WeaponLevel - 1) * 10);
            cost = 20 + ((player.WeaponLevel - 1) * 5);
            // Start the effect if a position is provided
            if (startPosition.HasValue)
            {
                isEffectActive = true;
                effectPosition = startPosition.Value;
                effectRadius = maxEffectRadius; // Start with the maximum radius
                effectTimer = 0f; // Reset the timer
            }

            // Update and draw the effect if active
            if (isEffectActive)
            {
                // Increment effect timer
                effectTimer += deltaTime;

                // Calculate the current radius based on elapsed time
                effectRadius = maxEffectRadius * (1 - (effectTimer / effectDuration));

                // Calculate the fading alpha based on elapsed time
                int alpha = (int)(255 * (1 - (effectTimer / effectDuration)));

                // Ensure alpha is clamped between 0 and 255
                alpha = Math.Clamp(alpha, 0, 255);

                // Draw the shrinking and fading circle
                Raylib.DrawCircleV(effectPosition, effectRadius, new Color(255, 0, 0, alpha));

                // Disable the effect if the timer exceeds the duration
                if (effectTimer >= effectDuration)
                {
                    isEffectActive = false;
                }
            }
        }




    }
}
