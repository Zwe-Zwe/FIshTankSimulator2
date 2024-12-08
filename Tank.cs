using System.Numerics;
using Raylib_cs;
using System.Collections.Generic;

namespace FishTankSimulator
{
    public class Tank
    {
        private Player _player;
        private List<CoinFish> _fishList;
        private List<Coin> _coinList;
        private List<Food> _foodList;
        private List<PredatorFish> _predatorFishList;
        private List<ExoticFish> _exoticFishList;
        private List<Treasure> _treasureList; 
        private List<Snail> _snailList;
        private Shop _shop;
        private float _predatorSpawnTimer;
        private float _predatorSpawnInterval;
         private float _exoticFishSpawnTimer;
        private float _exoticFishSpawnInterval;
        private int currentFoodCount;
        private int currentWeaponCount;
        private float timer;
        private Upgrade _upgrade;
        private InGameMenu _inGameMenu;
        private int foodCost;

        private Font customFont = Raylib.LoadFont("font/font.ttf"); // Declare a custom font

        // Constructor
        public Tank(Shop shop, Player player, Upgrade upgrade, int windowWidth, int windowHeight)
        {
            _player = player;
            _foodList = new List<Food>();
            _predatorFishList = new List<PredatorFish>();
            _exoticFishList = new List<ExoticFish>();
            _fishList = new List<CoinFish>();
            _coinList = new List<Coin>();
            _treasureList = new List<Treasure>(); // Initialize the treasure list
            _snailList = new List<Snail>();
            _shop = shop;
            _predatorSpawnTimer = 0f;
            _predatorSpawnInterval = 40f;
            _exoticFishSpawnTimer = 0f;
            _exoticFishSpawnInterval = 5f;
            currentFoodCount = 0;
            currentWeaponCount = 0;
            timer = 0;
            _upgrade = upgrade;
            _inGameMenu = new InGameMenu(windowWidth, windowHeight);
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
                if (_snailList.Count >= _player.MaxSnail)
                {
                    Console.WriteLine("Cannot buy more snails. Maximum snails reached.");
                    return false;
                }

                if (_player.Money >= fishCost)
                {
                    _player.Money -= fishCost;

                    Random random = new Random();
                    int startPosition = random.Next(1, windowWidth - 1); 
                    Snail newSnail = new Snail(this, windowHeight, startPosition, _player);

                    AddSnail(newSnail);
                    return true;
                }

                Console.WriteLine("Not enough money to buy Snail.");
                return false;
            }

            // Handle fish purchase
            if (_fishList.Count >= _player.MaxFish)
            {
                Console.WriteLine("Cannot buy more fish. Tank is full.");
                return false;
            }

            if (_player.Money >= fishCost)
            {
                _player.Money -= fishCost;

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
            foodCost = 10 + ((_player.FoodLevel - 1) * 2); // Calculate cost based on food level
            
            if (_upgrade.IsActive)
            {
                return; // Skip the rest of the update logic
            }
            if(_inGameMenu.IsMenuVisible)
            {
                _inGameMenu.Update();
                return;
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

            if (!clickHandled)
            {
                clickHandled = UpdateTreasure(deltaTime, windowHeight);
            }

            // Handle food interaction only if click wasn't consumed by coin collection or weapon
            if (!clickHandled)
            {
                HandleFoodInteraction(ref clickHandled, windowWidth);
            }

            // Update fish, food, and other objects
            UpdateFish(deltaTime, windowWidth, windowHeight);
            UpdateExorticFish(deltaTime, windowWidth, windowHeight);
            UpdateFood(deltaTime, windowHeight);
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
        private void UpdateExorticFish(float deltaTime, int windowWidth, int windowHeight)
        {
            // Handle spawning predators
            _exoticFishSpawnTimer += deltaTime;
            if (_exoticFishSpawnTimer >= _exoticFishSpawnInterval)
            {
                SpawnExoticFish(windowHeight);
                _exoticFishSpawnTimer = 0f;
            }

            // Update each predator
            for (int i = _exoticFishList.Count - 1; i >= 0; i--)
            {
                ExoticFish exoticFish = _exoticFishList[i];

                if (exoticFish == null)
                {
                    Console.WriteLine("Warning: Null predator detected. Removing it from the list.");
                    _exoticFishList.RemoveAt(i);
                    continue; // Skip to the next iteration
                }
                // Update predator's state
                exoticFish.Update(deltaTime, windowWidth, windowHeight);

                // Remove predator if it leaves the tank
                if (exoticFish.Position.X < -200 || exoticFish.Position.X > windowWidth + 200)
                {
                    Console.WriteLine($"Predator at index {i} has left the tank. Removing it.");
                    _exoticFishList.RemoveAt(i);
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
                        _player.Money += coin.GetValue(); // Add coin value to money
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

        private bool UpdateTreasure(float deltaTime, int windowHeight)
        {
            bool clickHandled = false; // Track if the click was handled by a coin

            for (int i = _treasureList.Count - 1; i >= 0; i--)
            {
                var treasure = _treasureList[i];

                // Update the coin's position every frame
                treasure.Update(deltaTime, windowHeight);

                // Check for coin interaction (mouse click) if the click isn't already handled
                if (!clickHandled && Raylib.IsMouseButtonPressed(MouseButton.Left))
                {
                    Vector2 mousePosition = Raylib.GetMousePosition();

                    // Check if the click interacts with this coin
                    if (treasure.IsClicked(mousePosition))
                    {
                        _player.Money += treasure.GetValue(); // Add coin value to money
                        _treasureList.RemoveAt(i); // Remove the coin from the tank
                        clickHandled = true; // Mark the click as handled
                        continue; // Exit this iteration
                    }
                }

                // Remove expired coins
                if (treasure.IsExpired())
                {
                    _treasureList.RemoveAt(i);
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

            if (_player.Money >= _shop.Weapon.Cost && currentWeaponCount <= (_player.WeaponCountLevel/1.5f)) // Example cost per shot
            {
                _player.Money -= _shop.Weapon.Cost;
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

                _shop.Weapon.HandleWeaponEffect(mousePosition, deltaTime, _player);

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

            foreach (var treasure in _treasureList)
                treasure.Draw();

            foreach (var food in _foodList)
                food.Draw();

            foreach (var predator in _predatorFishList)
                predator.Draw(deltaTime); 

            foreach (var exoticFish in _exoticFishList)
                exoticFish.Draw(deltaTime);

            foreach (var snail in _snailList)
                snail.Draw(deltaTime);

            DrawHUD(windowWidth);
            _shop.Weapon.HandleWeaponEffect(null, deltaTime, _player);
            if (_upgrade.IsActive)
            {
                _upgrade.Draw(windowWidth); 
            }
            _inGameMenu.Draw();
        }
        private void DrawHUD(int windowWidth)
        {
            // Draw money information
            DrawTextWithCustomFont($"Money: {_player.Money}", new Vector2(windowWidth - 500, 50));

            // Draw level information
            DrawTextWithCustomFont($"Level: {Player.GameLevel}", new Vector2(windowWidth - 500, 20));

            // Draw buttons and handle clicks using background images
            if (DrawButton("Upgrade", new Rectangle(windowWidth - 200, 20, 100, 30), "sprites/button.png", Color.LightGray, Color.White))
            {
                _upgrade.SetIsActive();
            }

            if (DrawButton("Menu", new Rectangle(windowWidth - 200, 70, 100, 30), "sprites/button.png", Color.LightGray, Color.White))
            {
                _inGameMenu.IsMenuVisible = !_inGameMenu.IsMenuVisible; // Activate the menu state
            }
        }
        private void DrawTextWithCustomFont(string text, Vector2 position)
        {
            int fontSize = 35;
            Raylib.DrawTextEx(customFont, text, position, fontSize, 2, Color.Gold);
        }

        private bool DrawButton(string label, Rectangle rect, string imagePath, Color hoverColor, Color textColor)
        {
            // Load the button texture (background image)
            Texture2D buttonTexture = Raylib.LoadTexture(imagePath);

            // Scale the texture to fit the button
            int buttonWidth = (int)rect.Width;
            int buttonHeight = (int)rect.Height;

            // Check if the mouse is over the button
            bool isHovered = Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), rect);

            // Draw the button texture (background image) and apply the hover effect
            Raylib.DrawTexturePro(
                buttonTexture, 
                new Rectangle(0, 0, buttonTexture.Width, buttonTexture.Height),  // Source rectangle (whole image)
                new Rectangle(rect.X, rect.Y, buttonWidth, buttonHeight),       // Destination rectangle (scaled size)
                Vector2.Zero,                                                   // Origin (no offset)
                0f,                                                              // Rotation (no rotation)
                isHovered ? hoverColor : Color.White                              // Apply hover effect if needed
            );

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
            // Check for "Upgrade" button click using background image
            if (DrawButton("Upgrade", new Rectangle(windowWidth - 200, 20, 100, 30), "sprites/button.png", Color.LightGray, Color.White))
            {
                return true;
            }

            // Check for "Menu" button click using background image
            if (DrawButton("Menu", new Rectangle(windowWidth - 200, 70, 100, 30), "sprites/button.png", Color.LightGray, Color.White))
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
                treasure.UnloadTextures();;

            foreach (var predator in _predatorFishList)
                predator.UnloadTextures(); // Unload predator textures

            foreach (var snail in _snailList)
                snail.UnloadTextures();

            _shop.UnloadTextures();
            _inGameMenu.Unload();
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
            _player.Money += coin.GetValue();
            _coinList.Remove(coin);
        }
        
        private void AddTreasureToTank(Treasure treasure)
        {
            _treasureList.Add(treasure);
        }
        private void SpawnPredator(int windowHeight)
        {
            Vector2 spawnPosition = new Vector2(-100, Raylib.GetRandomValue(100, windowHeight - 100));
            PredatorFish predator = new PredatorFish(spawnPosition, this, _player.GetPredatorType()); // Pass the current Tank instance

            predator.OnFishEaten += HandleFishEaten;

            _predatorFishList.Add(predator);
        }
        private void SpawnExoticFish(int windowHeight)
        {
            Vector2 spawnPosition = new Vector2(-100, Raylib.GetRandomValue(100, windowHeight - 100));
            ExoticFish exoticFish = new ExoticFish(spawnPosition, this); // Pass the current Tank instance
            _exoticFishList.Add(exoticFish);
            exoticFish.OnTreasureDropped += AddTreasureToTank;
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

                if (_player.Money >=foodCost  && currentFoodCount <= (_player.FoodCountLevel/1.5f)) // Ensure player has enough money for food
                {
                    _foodList.Add(new Food(mousePosition, _player)); // Add new food
                    _player.Money -= foodCost; // Deduct money for food
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
        public int Money => _player.Money;

        /// <summary>
        /// Adds a new treasure to the tank.
        /// </summary>
        public void AddSnail(Snail snail)
        {
            if(_snailList.Count < _player.MaxSnail)
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
            if (_fishList.Count < _player.MaxFish)
            {
                _fishList.Add(fish);
                fish.OnCoinDropped += AddCoinToTank;
            }
        }
        public void RemoveFish(CoinFish fish)
        {
            _fishList.Remove(fish);
        }


        
    }
}
