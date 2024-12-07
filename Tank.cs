using System.Numerics;
using Raylib_cs;
using System.Collections.Generic;

namespace FishTankSimulator
{
    public class Tank
    {
        private Level _level;
        private List<CoinFish> _fishList;
        private List<Coin> _coinList;
        private List<Food> _foodList;
        private List<PredatorFish> _predatorFishList;
        private List<Treasure> _treasureList; 
        private List<Snail> _snailList;
        private int _maxSnails;
        private int _maxFish;
        private Shop _shop;
        private float _predatorSpawnTimer;
        private float _predatorSpawnInterval;
        private int currentFoodCount;
        private int currentWeaponCount;
        private float timer;
        private bool isMenuActive;
        private Upgrade _upgrade;
        private int foodCost;

        private Font customFont = Raylib.LoadFont("font/font.ttf"); // Declare a custom font

        // Constructor
        public Tank(Shop shop, Level level, Upgrade upgrade)
        {
            _level = level;
            _foodList = new List<Food>();
            _predatorFishList = new List<PredatorFish>();
            _fishList = new List<CoinFish>();
            _coinList = new List<Coin>();
            _treasureList = new List<Treasure>(); // Initialize the treasure list
            _snailList = new List<Snail>();
            _maxSnails = 2;
            _maxFish = 10;
            _shop = shop;
            _predatorSpawnTimer = 0f;
            _predatorSpawnInterval = 40f;
            currentFoodCount = 0;
            currentWeaponCount = 0;
            timer = 0;
            isMenuActive = false;
            _upgrade = upgrade;
        }

        /// <summary>
        /// Attempts to buy a fish from the shop and adds it to the tank if successful.
        /// </summary>
        public bool TryBuyFish(int fishIndex, int windowWidth, int windowHeight)
        {
            // Get the cost of the selected item (fish or snail)
            int fishCost = _shop.GetFishCost(fishIndex);

            // Check if the selected index corresponds to the Snail
            if (fishIndex == _shop.FishIcons.Count - 1 && _shop.FishIcons.Count > 1) // Assuming the last icon is for the Snail
            {
                if (_snailList.Count >= _maxSnails)
                {
                    Console.WriteLine("Cannot buy more snails. Maximum snails reached.");
                    return false;
                }

                if (_level.Money >= fishCost)
                {
                    _level.Money -= fishCost;

                    Random random = new Random();
                    int startPosition = random.Next(1, windowWidth - 1); 
                    Snail newSnail = new Snail(this, windowHeight, startPosition);

                    AddSnail(newSnail);
                    return true;
                }

                Console.WriteLine("Not enough money to buy Snail.");
                return false;
            }

            // Handle fish purchase
            if (_fishList.Count >= _maxFish)
            {
                Console.WriteLine("Cannot buy more fish. Tank is full.");
                return false;
            }

            if (_level.Money >= fishCost)
            {
                _level.Money -= fishCost;

                Vector2 initialPosition = GetRandomPosition(windowWidth, windowHeight);
                // Pass the CoinFishType to the CoinFish constructor
                CoinFish newFish = new CoinFish(initialPosition, (CoinFishType)fishIndex, this);

                AddFish(newFish);
                return true;
            }

            Console.WriteLine("Not enough money to buy Fish.");
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
        /// Updates all objects in the tank (fish, coins, and food).
        /// </summary>
        public void Update(float deltaTime, int windowWidth, int windowHeight, bool clickConsumed)
        {
            foodCost = 10 + ((_level.FoodLevel - 1) * 2); // Calculate cost based on food level
            
            if (_upgrade.IsActive)
            {
                return; // Skip the rest of the update logic
            }
            timer += deltaTime;
            if(timer >= 1f){
                timer = 0;
                currentFoodCount = 0;
                currentWeaponCount = 0;
            }

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
                UseWeapon(deltaTime);
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
                HandleFoodInteraction(ref clickHandled, windowWidth);
            }

            // Update fish, food, and other objects
            UpdateFish(deltaTime, windowWidth, windowHeight);
            UpdateFood(deltaTime, windowHeight);
            UpdateTreasures(deltaTime, windowHeight, ref clickHandled);
            UpdatePredators(deltaTime, windowWidth, windowHeight);
            UpdateSnails(deltaTime, windowWidth, windowHeight);
            // Draw the weapon effect regardless of clickHandled to allow interaction with weapon while keeping other interactions intact    
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

        private void UpdateSnails(float deltaTime, int windowWidth, int windowHeight)
        {
            for (int i = _snailList.Count - 1; i >= 0; i--)
            {
                var snail = _snailList[i];
                snail.Update(deltaTime, windowWidth, windowHeight);
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
                        _level.Money += coin.GetValue(); // Add coin value to money
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
                        _level.Money += treasure.Value; // Collect treasure and add value to money
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
        private void UpdatePredators(float deltaTime, int windowWidth, int windowHeight)
        {
            // Handle spawning predators
            _predatorSpawnTimer += deltaTime;
            if (_predatorSpawnTimer >= _predatorSpawnInterval)
            {
                SpawnPredator(windowHeight);
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
        private void UseWeapon(float deltaTime)
        {
            Vector2 mousePosition = Raylib.GetMousePosition();

            if (_level.Money >= _shop.Weapon.Cost && currentWeaponCount <= (_level.WeaponCountLevel/1.5f)) // Example cost per shot
            {
                _level.Money -= _shop.Weapon.Cost;
                currentWeaponCount++;

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

                _shop.Weapon.HandleWeaponEffect(mousePosition, deltaTime, _level);

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
        public void Draw(float deltaTime, int windowWidth)
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

            foreach (var snail in _snailList)
                snail.Draw(deltaTime);

            DrawHUD(windowWidth);
            _shop.Weapon.HandleWeaponEffect(null, deltaTime, _level);
            if (_upgrade.IsActive)
            {
                _upgrade.Draw(windowWidth);  // Draw the upgrade menu
            }
        }
        private void DrawHUD(int windowWidth)
        {
            // Draw money information
            DrawTextWithCustomFont($"Money: {_level.Money}", new Vector2(windowWidth - 400, 50));

            // Draw level information
            DrawTextWithCustomFont($"Level: {_level.GameLevel}", new Vector2(windowWidth - 400, 20));

            // Draw buttons and handle clicks
            if (DrawButton("Upgrade", new Rectangle(windowWidth - 200, 20, 120, 40), Color.DarkGray, Color.LightGray, Color.White))
            {
                _upgrade.SetIsActive();
            }

            if (DrawButton("Menu", new Rectangle(windowWidth - 200, 70, 120, 40), Color.DarkGray, Color.LightGray, Color.White))
            {
                isMenuActive = true; // Activate the menu state
            }
        }
        
        public void SetIsMenuActive()
        {
            isMenuActive = false;
        }

        private void DrawTextWithCustomFont(string text, Vector2 position)
        {
            int fontSize = 35;
            Raylib.DrawTextEx(customFont, text, position, fontSize, 2, Color.Gold);
        }

        private bool DrawButton(string label, Rectangle rect, Color bgColor, Color hoverColor, Color textColor)
        {
            // Check if the mouse is over the button
            bool isHovered = Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), rect);
            
            // Change the button color when hovered
            Raylib.DrawRectangleRec(rect, isHovered ? hoverColor : bgColor);

            // Draw the button label
            int fontSize = 20;
            Vector2 textSize = Raylib.MeasureTextEx(customFont, label, fontSize, 2);
            Vector2 textPosition = new Vector2(
                rect.X + (rect.Width - textSize.X) / 2,
                rect.Y + (rect.Height - textSize.Y) / 2
            );
            Raylib.DrawTextEx(customFont, label, textPosition, fontSize, 2, textColor);

            // Return true if the button is clicked
            return isHovered && Raylib.IsMouseButtonPressed(MouseButton.Left);
        }
        private bool IsButtonClicked(int windowWidth)
        {
            // Check for "Upgrade" button click
            if (DrawButton("Upgrade", new Rectangle(windowWidth - 200, 20, 120, 40), Color.DarkGray, Color.LightGray, Color.White))
            {
                return true;
            }

            // Check for "Menu" button click
            if (DrawButton("Menu", new Rectangle(windowWidth - 200, 70, 120, 40), Color.DarkGray, Color.LightGray, Color.White))
            {
                return true;
            }

            return false;
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

            foreach (var snail in _snailList)
                snail.UnloadTextures();

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

        
        public void CollectCoin(Coin coin)
        {
            _level.Money += coin.GetValue();
            _coinList.Remove(coin);
        }
        
        private void SpawnPredator(int windowHeight)
        {
            Vector2 spawnPosition = new Vector2(-100, Raylib.GetRandomValue(100, windowHeight - 100));
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
        private void HandleFoodInteraction(ref bool clickHandled, int windowWidth)
        {
            // If the click is already handled or the weapon is active, do nothing
            if (clickHandled || _shop.IsWeaponActivated())
                return;

            // Check if the player clicks to drop food
            if (Raylib.IsMouseButtonPressed(MouseButton.Left) && !IsShopInteracting() && !IsButtonClicked(windowWidth))
            {
                Vector2 mousePosition = Raylib.GetMousePosition();

                if (_level.Money >=foodCost  && currentFoodCount <= (_level.FoodCountLevel/1.5f)) // Ensure player has enough money for food
                {
                    _foodList.Add(new Food(mousePosition, _level)); // Add new food
                    _level.Money -= foodCost; // Deduct money for food
                    clickHandled = true; // Mark the click as handled
                    currentFoodCount++;
                }
            }
        }
        private Vector2 GetRandomPosition(int windowWidth, int windowHeight)
        {
            int x = Raylib.GetRandomValue(100, windowWidth - 100);
            int y = Raylib.GetRandomValue(100, windowHeight - 100);
            return new Vector2(x, y);
        }
        public List<Coin> CoinList
        {
            get => _coinList;
            set => _coinList = value;
        }
        // Properties
        public List<Food> FoodList
        {
            get => _foodList;
            private set => _foodList = value;
        }
        public List<Treasure> TreasureList => _treasureList;
        public int Money => _level.Money;

        /// <summary>
        /// Adds a new treasure to the tank.
        /// </summary>
        public void AddTreasure(Treasure treasure)
        {
            _treasureList.Add(treasure);
        }
        public void AddSnail(Snail snail)
        {
            _snailList.Add(snail);
        }
        public void RemoveSnail(Snail snail)
        {
            _snailList.Remove(snail);
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

        
    }
}
