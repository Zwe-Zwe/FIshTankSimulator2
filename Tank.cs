using System.Numerics;
using Raylib_cs;
using System.Collections.Generic;

namespace FishTankSimulator
{
    public class Tank
    {
        private List<CoinFish> _fishList;
        private List<Coin> _coinList;
        private List<Food> _foodList;
        private List<PredatorFish> _predatorFishList;
        private List<Treasure> _treasureList; 
        private int _maxFish;
        private int _money;
        private Shop _shop;
        private float _predatorSpawnTimer = 0f;
        private float _predatorSpawnInterval = 40f; 
        private Font customFont = Raylib.LoadFont("font/font.ttf"); // Declare a custom font

        // Constructor
        public Tank(Shop shop)
        {
            _foodList = new List<Food>();
            _predatorFishList = new List<PredatorFish>();
            _fishList = new List<CoinFish>();
            _coinList = new List<Coin>();
             _treasureList = new List<Treasure>(); // Initialize the treasure list
            _maxFish = 10;
            _money = shop.GoldAmount; // Initialize from shop
            _shop = shop;
        }


        // Properties
        public List<Food> FoodList
        {
            get => _foodList;
            private set => _foodList = value;
        }
        public List<Treasure> TreasureList => _treasureList;
        public int Money => _money;

        /// <summary>
        /// Adds a new treasure to the tank.
        /// </summary>
        public void AddTreasure(Treasure treasure)
        {
            _treasureList.Add(treasure);
        }

        /// <summary>
        /// Attempts to buy a fish from the shop and adds it to the tank if successful.
        /// </summary>
        public bool TryBuyFish(int fishIndex)
        {
            // Get the cost of the selected fish
            int fishCost = _shop.GetFishCost(fishIndex);

            if (_fishList.Count >= _maxFish)
            {
                Console.WriteLine("Cannot buy more fish. Tank is full.");
                return false;
            }

            if (_money >= fishCost)
            {
                _money -= fishCost;

                Vector2 initialPosition = GetRandomPosition();
                // Pass the CoinFishType to the CoinFish constructor
                CoinFish newFish = new CoinFish(initialPosition, (CoinFishType)fishIndex, this);

                AddFish(newFish);
                return true;
            }

            return false;
        }

        private bool IsShopInteracting()
        {
            // Check if any fish frame or the weapon frame is clicked
            int? clickedFishIndex = _shop.GetClickedFishIndex();
            bool isWeaponFrameClicked = Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), _shop.WeaponFrameRect);
            return clickedFishIndex.HasValue || isWeaponFrameClicked;
        }



        /// <summary>
        /// Adds a new fish to the tank and subscribes to its coin drop event.
        /// </summary>
        public void AddFish(CoinFish fish)
        {
            if (_fishList.Count < _maxFish)
            {
                _fishList.Add(fish);
                fish.OnCoinDropped += AddCoinToTank;
            }
        }

        public void RemoveFish(CoinFish fish)
        {
            _fishList.Remove(fish);
        }

        public void AddPredator(PredatorFish predator)
        {
            _predatorFishList.Add(predator);
        }

        public void RemovePredator(PredatorFish predator)
        {
            _predatorFishList.Remove(predator);
        }

        /// <summary>
        /// Updates all objects in the tank (fish, coins, and food).
        /// </summary>
        public void Update(float deltaTime, int windowWidth, int windowHeight, bool clickConsumed)
        {
            bool clickHandled = clickConsumed; // Track if the click has already been handled

            // Handle shop interactions (weapon frame click should not stop other actions)
            if (Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), _shop.WeaponFrameRect))
            {
                // If hovering over the weapon frame, don't stop coin movement, but check if click triggers weapon use
                if (Raylib.IsMouseButtonPressed(MouseButton.Left) && !clickHandled)
                {
                    _shop.ActivateWeapon(); // Activate weapon if clicked
                    clickHandled = true; // Mark click as handled
                }
            }
            else
            {
                // Only proceed with other actions like adding coins or dropping food if not hovering over the weapon frame
                if (!clickHandled)
                {
                    _shop.HandleClick(clickHandled);
                    clickHandled = _shop.GetClickedFishIndex().HasValue;
                }
            }

            // ** Weapon interaction logic **
            // Place this block here to allow weapon usage after shop interactions but before handling other actions
            if (!clickHandled && _shop.IsWeaponActivated() && Raylib.IsMouseButtonPressed(MouseButton.Left))
            {
                UseWeapon();
                
                clickHandled = true;
            }

            // Update coins and handle interactions, ensuring coin movement is unaffected
            if (!clickHandled)
            {
                clickHandled = UpdateCoins(deltaTime, windowHeight);
            }

            // Handle food interaction only if click wasn't consumed by coin collection or weapon
            if (!clickHandled)
            {
                HandleFoodInteraction(ref clickHandled);
            }

            // Update fish, food, and other objects
            UpdateFish(deltaTime, windowWidth, windowHeight);
            UpdateFood(deltaTime, windowHeight);
            UpdateTreasures(deltaTime, windowHeight, ref clickHandled);
            UpdatePredators(deltaTime, windowWidth, windowHeight);

            // Draw the weapon effect regardless of clickHandled to allow interaction with weapon while keeping other interactions intact
            
        }

        private void UseWeapon()
        {
            Vector2 mousePosition = Raylib.GetMousePosition();

            if (_money >= 10) // Example cost per shot
            {
                _money -= 10;

                bool predatorHit = false;

                foreach (var predator in _predatorFishList)
                {
                    if (predator.IsWithinBounds(mousePosition))
                    {
                        _shop.Weapon.Attack(predator); // Attack the predator
                        predatorHit = true;
                        Console.WriteLine($"Weapon affected predator at {predator.Position}.");
                    }
                }

                _shop.Weapon.StartWeaponEffect(mousePosition);
                _shop.Weapon.DrawWeaponEffect();

                if (!predatorHit)
                {
                    Console.WriteLine("No predators within range of the weapon.");
                }
            }
            else
            {
                Console.WriteLine("Not enough money to use the weapon!");
            }
        }



        /// <summary>
        /// Renders all objects in the tank (fish, coins, food) and the player's money.
        /// </summary>
      public void Draw(float deltaTime)
        {
            foreach (var fish in _fishList)
                fish.Draw(deltaTime);

            foreach (var coin in _coinList)
                coin.Draw();

            foreach (var food in _foodList)
                food.Draw();

            foreach (var treasure in _treasureList)
                treasure.Draw();

            foreach (var predator in _predatorFishList)
                predator.Draw(deltaTime); // Draw predators

            DrawMoney();
        }


        /// <summary>
        /// Unloads all textures used in the tank, including treasures.
        /// </summary>
       public void UnloadAllTextures()
        {
            foreach (var fish in _fishList)
                fish.UnloadTextures();

            foreach (var coin in _coinList)
                coin.UnloadTextures();

            foreach (var treasure in _treasureList)
                treasure.UnloadTexture();

            foreach (var predator in _predatorFishList)
                predator.UnloadTextures(); // Unload predator textures

            _shop.UnloadTextures();
        }

        // Private Methods

        /// <summary>
        /// Adds a coin to the tank's coin list when dropped by a fish.
        /// </summary>
        private void AddCoinToTank(Coin coin)
        {
            _coinList.Add(coin);
        }

        private void UpdateFish(float deltaTime, int windowWidth, int windowHeight)
        {
            for (int i = _fishList.Count - 1; i >= 0; i--)
            {
                var fish = _fishList[i];
                fish.Update(deltaTime, windowWidth, windowHeight);

                if (fish.Health.Current<= 0)
                {
                    fish.OnCoinDropped -= AddCoinToTank;
                    _fishList.RemoveAt(i);
                }
            }
        }

        private bool UpdateCoins(float deltaTime, int windowHeight)
        {
            bool clickHandled = false; // Track if the click was handled by a coin

            for (int i = _coinList.Count - 1; i >= 0; i--)
            {
                var coin = _coinList[i];

                // Update the coin's position every frame
                coin.Update(deltaTime, windowHeight);

                // Check for coin interaction (mouse click) if the click isn't already handled
                if (!clickHandled && Raylib.IsMouseButtonPressed(MouseButton.Left))
                {
                    Vector2 mousePosition = Raylib.GetMousePosition();

                    // Check if the click interacts with this coin
                    if (coin.IsClicked(mousePosition))
                    {
                        _money += coin.GetValue(); // Add coin value to money
                        _coinList.RemoveAt(i); // Remove the coin from the tank
                        clickHandled = true; // Mark the click as handled
                        continue; // Exit this iteration
                    }
                }

                // Remove expired coins
                if (coin.IsExpired())
                {
                    _coinList.RemoveAt(i);
                }
            }

            return clickHandled;
        }
        private void UpdatePredators(float deltaTime, int windowWidth, int windowHeight)
        {
            // Handle spawning predators
            _predatorSpawnTimer += deltaTime;
            if (_predatorSpawnTimer >= _predatorSpawnInterval)
            {
                SpawnPredator();
                _predatorSpawnTimer = 0f;
            }

            // Update each predator
            for (int i = _predatorFishList.Count - 1; i >= 0; i--)
            {
                PredatorFish predator = _predatorFishList[i];

                // Null check for predator
                if (predator == null)
                {
                    Console.WriteLine("Warning: Null predator detected. Removing it from the list.");
                    _predatorFishList.RemoveAt(i);
                    continue; // Skip to the next iteration
                }

                // Null check for predator's Health component
                if (predator.Health == null)
                {
                    Console.WriteLine($"Warning: Predator at index {i} has a null Health component. Removing it.");
                    _predatorFishList.RemoveAt(i);
                    continue; // Skip to the next iteration
                }

                // Update predator's state
                predator.Update(deltaTime, windowWidth, windowHeight);

                // Remove predator if health is depleted
                if (predator.Health.Current <= 0)
                {
                    Console.WriteLine($"Predator at index {i} has died. Removing it from the list.");
                    _predatorFishList.RemoveAt(i);
                    continue; // Skip further checks
                }

                // Remove predator if it leaves the tank
                if (predator.Position.X < -100 || predator.Position.X > windowWidth + 100)
                {
                    Console.WriteLine($"Predator at index {i} has left the tank. Removing it.");
                    _predatorFishList.RemoveAt(i);
                }
            }
        }


        private void SpawnPredator()
        {
            Vector2 spawnPosition = new Vector2(-100, Raylib.GetRandomValue(100, 1080 - 100));
            PredatorFish predator = new PredatorFish(spawnPosition, this); // Pass the current Tank instance

            predator.OnFishEaten += HandleFishEaten;

            _predatorFishList.Add(predator);
        }

        private void HandleFishEaten(Fish eatenFish)
        {
            if (eatenFish is CoinFish coinFish)
            {
                RemoveFish(coinFish); // Remove the fish from the tank
                Console.WriteLine("A fish was eaten by the predator!");
            }
        }

        private void UpdateFood(float deltaTime, int windowHeight)
        {
            for (int i = _foodList.Count - 1; i >= 0; i--)
            {
                var food = _foodList[i];
                food.Update(deltaTime, windowHeight);

                if (!food.IsActive)
                {
                    _foodList.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Updates all treasures in the tank.
        /// </summary>
        private void UpdateTreasures(float deltaTime, int windowHeight, ref bool clickHandled)
        {
            for (int i = _treasureList.Count - 1; i >= 0; i--)
            {
                var treasure = _treasureList[i];
                treasure.Update(deltaTime, windowHeight);

                if (Raylib.IsMouseButtonPressed(MouseButton.Left))
                {
                    Vector2 mousePosition = Raylib.GetMousePosition();
                    if (!clickHandled && treasure.IsClicked(mousePosition))
                    {
                        _money += treasure.Value; // Collect treasure and add value to money
                        _treasureList.RemoveAt(i);
                        clickHandled = true;
                        continue;
                    }
                }

                if (!treasure.IsActive)
                {
                    _treasureList.RemoveAt(i); // Remove inactive treasures
                }
            }
        }

        public Food FindNearestFood(Vector2 fishPosition)
        {
            Food nearestFood = null;
            float minDistance = float.MaxValue;

            foreach (var food in _foodList)
            {
                if (!food.IsActive) continue; // Skip inactive food

                float distance = Vector2.Distance(fishPosition, food.Position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestFood = food;
                }
            }

            return nearestFood;
        }

        /// <summary>
        /// Finds the nearest CoinFish to the given position, excluding PredatorFish if implemented.
        /// </summary>
        /// <param name="currentPosition">The position to search from.</param>
        /// <returns>The nearest CoinFish or null if none are found.</returns>
        public CoinFish FindNearestFish(Vector2 currentPosition)
        {
            CoinFish nearestFish = null;
            float minDistance = float.MaxValue;

            foreach (var fish in _fishList)
            {
                // Calculate the distance to the current fish
                float distance = Vector2.Distance(currentPosition, fish.Position);

                // Check if this fish is the nearest so far
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestFish = fish;
                }
            }

            return nearestFish;
        }
        private void HandleFoodInteraction(ref bool clickHandled)
        {
            // If the click is already handled or the weapon is active, do nothing
            if (clickHandled || _shop.IsWeaponActivated())
                return;

            // Check if the player clicks to drop food
            if (Raylib.IsMouseButtonPressed(MouseButton.Left) && !IsShopInteracting())
            {
                Vector2 mousePosition = Raylib.GetMousePosition();

                if (_money >= 7) // Ensure player has enough money for food
                {
                    _foodList.Add(new Food(mousePosition)); // Add new food
                    _money -= 7; // Deduct money for food
                    clickHandled = true; // Mark the click as handled
                }
            }
        }


        private void DrawMoney()
        {
            string moneyText = $"Money: {_money}";
            Vector2 position = new Vector2(1700, 30);
            int fontSize = 35;

            // Draw the text using the custom font
            Raylib.DrawTextEx(customFont, moneyText, position, fontSize, 2, Color.Gold);
        }

        private Vector2 GetRandomPosition()
        {
            int x = Raylib.GetRandomValue(100, 1920 - 100);
            int y = Raylib.GetRandomValue(100, 1080 - 100);
            return new Vector2(x, y);
        }

        
    }
}
