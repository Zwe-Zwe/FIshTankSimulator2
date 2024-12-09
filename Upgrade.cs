using Raylib_cs;
using System;
using System.Numerics;

namespace FishTankSimulator
{
    public class Upgrade
    {
        const int ITEMS_PER_ROW = 3;
        const int ITEM_COUNT = 6;
        private float uiWidth;
        private float uiHeight;
        private float uiX;
        private float uiY;
        private float itemSize;
        private float itemSpacing;
        private Texture2D[] itemTextures;
        private Texture2D buttonTexture;
        private Font customFont;
        private string[] itemNames;
        private Action[] itemActions;
        private bool isActive;
        private LevelManagement _levelManagement;
        private Player _player;
        private bool isOkButtonClicked;

        public Upgrade(int screenWidth, int screenHeight, LevelManagement levelManagement, Player player)
        {
            // UI dimensions
            uiWidth = screenWidth * 0.8f;
            uiHeight = screenHeight * 0.6f;
            uiX = (screenWidth - uiWidth) / 2;
            uiY = (screenHeight - uiHeight) / 2;
            isActive = false;
            itemSize = (uiWidth / ITEMS_PER_ROW) * 0.6f; // Slightly larger items
            itemSpacing = 15; // Increased spacing
            _levelManagement = levelManagement;
            _player = player;
            isOkButtonClicked = false;
            // Load textures
            itemTextures = new Texture2D[ITEM_COUNT];
            itemTextures[0] = Raylib.LoadTexture("sprites/level.png");
            itemTextures[1] = Raylib.LoadTexture("sprites/fishfood.png");
            itemTextures[2] = Raylib.LoadTexture("sprites/harpoon.png");
            itemTextures[3] = Raylib.LoadTexture("sprites/fishfood.png");
            itemTextures[4] = Raylib.LoadTexture("sprites/harpoon.png");
            itemTextures[5] = Raylib.LoadTexture("sprites/guppy/swim_to_left/1.png");

            buttonTexture = Raylib.LoadTexture("sprites/button.png");

            // Load custom font
            customFont = Raylib.LoadFont("font/font.ttf");

            itemNames = new string[]
            {
                "Level",
                "Food Quality",
                "Weapon Damage",
                "Food Count",
                "Weapon Count",
                "Helper Level"
            };

            itemActions = new Action[]
            {
                () => _levelManagement.LevelUpGameLevel(),
                () => _levelManagement.LevelUpFoodLevel(),
                () => _levelManagement.LevelUpWeaponLevel(),
                () => _levelManagement.LevelUpFoodCount(),
                () => _levelManagement.LevelUpWeaponCount(),
                () => _levelManagement.LevelUpHelperLevel()
            };
        }

        // Inside the Upgrade class
public void Draw(int windowWidth)
{
    const float padding = 40f;
    const float bottomPadding = 60f;

    float totalItemWidth = ITEMS_PER_ROW * itemSize + (ITEMS_PER_ROW - 1) * itemSpacing;
    float totalItemHeight = (float)Math.Ceiling((float)ITEM_COUNT / ITEMS_PER_ROW) * itemSize + ((ITEM_COUNT / ITEMS_PER_ROW) - 1) * itemSpacing;

    float okButtonHeight = 70; // Larger button height
    float adjustedUiHeight = totalItemHeight + okButtonHeight + padding * 2 + bottomPadding;

    uiX = (Raylib.GetScreenWidth() - totalItemWidth - padding * 2) / 2;
    uiY = (Raylib.GetScreenHeight() - adjustedUiHeight) / 2;

    for (int i = 0; i < ITEM_COUNT; i++)
    {
        int row = i / ITEMS_PER_ROW;
        int col = i % ITEMS_PER_ROW;

        float itemX = uiX + col * (itemSize + itemSpacing) + padding;
        float itemY = uiY + row * (itemSize + itemSpacing) + padding;

        Rectangle itemRect = new Rectangle(itemX, itemY, itemSize, itemSize);
        Raylib.DrawRectangleRec(itemRect, Color.White);
        Raylib.DrawRectangleLinesEx(itemRect, 2, Color.Black);

        float imageScale = 0.5f;
        Rectangle imageRect = new Rectangle(itemX + 10, itemY + 40, 80, 80);
        float scale = Math.Min(itemSize / (float)itemTextures[i].Width, itemSize / (float)itemTextures[i].Height) * imageScale;
        int imageWidth = (int)(itemTextures[i].Width * scale);
        int imageHeight = (int)(itemTextures[i].Height * scale);
        imageRect.Width = imageWidth;
        imageRect.Height = imageHeight;
        imageRect.X = itemX + (itemSize - imageWidth) / 2;
        imageRect.Y = itemY + (itemSize - imageHeight) / 2 - 20;

        Raylib.DrawTexturePro(itemTextures[i],
            new Rectangle(0, 0, itemTextures[i].Width, itemTextures[i].Height),
            imageRect, Vector2.Zero, 0, Color.White);

        int itemNameFontSize = 28;
        Vector2 itemNameSize = Raylib.MeasureTextEx(customFont, itemNames[i], itemNameFontSize, 0);
        Raylib.DrawTextEx(customFont, itemNames[i],
            new Vector2(itemX + (itemSize - itemNameSize.X) / 2, itemY + 10),
            itemNameFontSize, 0, Color.Black);

        int currentLevel = 0;
        int cost = 0;

        switch (i)
        {
            case 0:
                currentLevel = Player.GameLevel;
                cost = _levelManagement.GetGameLevelUpdateCost();
                break;
            case 1:
                currentLevel = _player.FoodLevel;
                cost = _levelManagement.GetFoodUpdateCost();
                break;
            case 2:
                currentLevel = _player.WeaponLevel;
                cost = _levelManagement.GetWeaponUpdateCost();
                break;
            case 3:
                currentLevel = _player.FoodCountLevel;
                cost = _levelManagement.GetFoodCountUpdateCost();
                break;
            case 4:
                currentLevel = _player.WeaponCountLevel;
                cost = _levelManagement.GetWeaponCountUpdateCost();
                break;
            case 5:
                currentLevel = _player.HelperLevel;
                cost = _levelManagement.GetHelperUpdateCost();
                break;
        }

        string currentLevelText = "Level: " + currentLevel;
        int currentLevelFontSize = 24;
        Vector2 currentLevelSize = Raylib.MeasureTextEx(customFont, currentLevelText, currentLevelFontSize, 0);
        float currentLevelX = itemX + (itemSize - currentLevelSize.X) / 2;
        float currentLevelY = imageRect.Y + imageHeight + 5;
        Raylib.DrawTextEx(customFont, currentLevelText, new Vector2(currentLevelX, currentLevelY), currentLevelFontSize, 0, Color.DarkGray);

        string costText = "Cost: " + cost;
        int costFontSize = 24;
        Vector2 costSize = Raylib.MeasureTextEx(customFont, costText, costFontSize, 0);
        float costX = itemX + (itemSize - costSize.X) / 2;
        float costY = currentLevelY + currentLevelSize.Y + 5;
        Raylib.DrawTextEx(customFont, costText, new Vector2(costX, costY), costFontSize, 0, Color.DarkGray);

        Rectangle buttonRect = new Rectangle(itemX + (itemSize - 110) / 2, itemY + itemSize - 50, 110, 50);

        Color buttonColor = Color.White;
        if (Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), buttonRect))
        {
            buttonColor = Color.Lime;
        }

        Raylib.DrawTexturePro(buttonTexture,
            new Rectangle(0, 0, buttonTexture.Width, buttonTexture.Height),
            buttonRect, Vector2.Zero, 0, buttonColor);

        string buttonText = "Upgrade";
        int buttonFontSize = 28;
        Vector2 buttonTextSize = Raylib.MeasureTextEx(customFont, buttonText, buttonFontSize, 0);
        float buttonTextX = buttonRect.X + (buttonRect.Width - buttonTextSize.X) / 2;
        float buttonTextY = buttonRect.Y + (buttonRect.Height - buttonTextSize.Y) / 2;
        Raylib.DrawTextEx(customFont, buttonText,
            new Vector2(buttonTextX, buttonTextY),
            buttonFontSize, 0, Color.Black);

        if (Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), buttonRect) &&
            Raylib.IsMouseButtonPressed(MouseButton.Left))
        {
            itemActions[i]?.Invoke();
        }
    }

    float okButtonWidth = 200f;
    Rectangle okButtonRect = new Rectangle(
        uiX + (totalItemWidth - okButtonWidth) / 2 + padding,
        uiY + totalItemHeight + padding + 50,
        okButtonWidth,
        okButtonHeight
    );

    Color okButtonColor = Color.White;
    if (Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), okButtonRect))
    {
        okButtonColor = Color.Lime;

        if (Raylib.IsMouseButtonPressed(MouseButton.Left) && !isOkButtonClicked)
        {
            isOkButtonClicked = true;
            SetIsActive();
        }
    }

    if (Raylib.IsMouseButtonReleased(MouseButton.Left))
    {
        isOkButtonClicked = false;
    }

    Raylib.DrawTexturePro(buttonTexture,
        new Rectangle(0, 0, buttonTexture.Width, buttonTexture.Height),
        okButtonRect, Vector2.Zero, 0, okButtonColor);

    string okButtonText = "OK";
    int okButtonFontSize = 32;
    Vector2 okButtonTextSize = Raylib.MeasureTextEx(customFont, okButtonText, okButtonFontSize, 0);
    float okButtonTextX = okButtonRect.X + (okButtonRect.Width - okButtonTextSize.X) / 2;
    float okButtonTextY = okButtonRect.Y + (okButtonRect.Height - okButtonTextSize.Y) / 2;
    Raylib.DrawTextEx(customFont, okButtonText,
        new Vector2(okButtonTextX, okButtonTextY),
        okButtonFontSize, 0, Color.Black);
}
        public void SetIsActive(){
            isActive = !isActive;
        }
        public bool IsActive{
            get { return isActive; }
        }
    }
}
