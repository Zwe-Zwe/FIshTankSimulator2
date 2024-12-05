using NUnit.Framework;
using System.Numerics;
using Raylib_cs;

namespace FishTankSimulator.Tests
{
    /// <summary>
    /// Test suite for <see cref="Coin"/> class.
    /// </summary>
    [TestFixture]
    public class CoinTests
    {
        /// <summary>
        /// Test that verifies the <see cref="Coin.Update"/> method correctly updates the coin's position.
        /// </summary>
        [Test]
        public void Coin_UpdateTest()
        {
            // Arrange
            var coin = new Coin(new Vector2(100, 100), CoinType.Gold);
            var initialPosition = coin.Position;
            
            // Act
            coin.Update(1.0f, 600);  // Simulate 1 second of update
            
            // Assert
            Assert.AreNotEqual(initialPosition, coin.Position); // The position should have changed
        }

        /// <summary>
        /// Test that verifies the <see cref="Coin.OnPickup"/> method correctly deactivates the coin.
        /// </summary>
        [Test]
        public void Coin_OnPickupTest()
        {
            // Arrange
            var coin = new Coin(new Vector2(100, 100), CoinType.Gold);

            // Act
            coin.OnPickup();

            // Assert
            Assert.IsFalse(coin.IsActive); // The coin should be inactive after pickup
        }

        /// <summary>
        /// Test that verifies the <see cref="Coin.GetValue"/> method returns the correct value for a gold coin.
        /// </summary>
        [Test]
        public void Coin_GetValueTest()
        {
            // Arrange
            var coin = new Coin(new Vector2(100, 100), CoinType.Gold);

            // Act
            var value = coin.GetValue();

            // Assert
            Assert.AreEqual(70, value); // Gold coin has a value of 70
        }
    }
    /// <summary>
    /// Test suite for <see cref="Food"/> class.
    /// </summary>
    [TestFixture]
    public class FoodTests
    {
        /// <summary>
        /// Test that verifies the <see cref="Food.Update"/> method correctly updates the food's position.
        /// </summary>
        [Test]
        public void Food_UpdateTest()
        {
            // Arrange
            var food = new Food(new Vector2(100, 100));
            var initialPosition = food.Position;
            
            // Act
            food.Update(1.0f, 600);  // Simulate 1 second of update
            
            // Assert
            Assert.AreNotEqual(initialPosition, food.Position); // The position should have changed
        }

        /// <summary>
        /// Test that verifies the <see cref="Food.OnPickup"/> method correctly deactivates the food.
        /// </summary>
        [Test]
        public void Food_OnPickupTest()
        {
            // Arrange
            var food = new Food(new Vector2(100, 100));

            // Act
            food.OnPickup();

            // Assert
            Assert.IsFalse(food.IsActive); // The food should be inactive after pickup
        }

        /// <summary>
        /// Test that verifies the <see cref="Food.GetValue"/> method returns the correct value for a food item.
        /// </summary>
        [Test]
        public void Food_GetValueTest()
        {
            // Arrange
            var food = new Food(new Vector2(100, 100));

            // Act
            var value = food.GetValue();

            // Assert
            Assert.AreEqual(50, value); // Food item has a value of 50
        }
    }
    /// <summary>
    /// Test suite for <see cref="Treasure"/> class.
    /// </summary>
    [TestFixture]
    public class TreasureTests
    {
        /// <summary>
        /// Test that verifies the <see cref="Treasure.Update"/> method correctly updates the treasure's position.
        /// </summary>
        [Test]
        public void Treasure_UpdateTest()
        {
            // Arrange
            var treasure = new Treasure(new Vector2(100, 100), TreasureType.Diamond);
            var initialPosition = treasure.Position;
            
            // Act
            treasure.Update(1.0f, 600);  // Simulate 1 second of update
            
            // Assert
            Assert.AreNotEqual(initialPosition, treasure.Position); // The position should have changed
        }

        /// <summary>
        /// Test that verifies the <see cref="Treasure.OnPickup"/> method correctly deactivates the treasure.
        /// </summary>
        [Test]
        public void Treasure_OnPickupTest()
        {
            // Arrange
            var treasure = new Treasure(new Vector2(100, 100), TreasureType.Diamond);

            // Act
            treasure.OnPickup();

            // Assert
            Assert.IsFalse(treasure.IsActive); // The treasure should be inactive after pickup
        }

        /// <summary>
        /// Test that verifies the <see cref="Treasure.GetValue"/> method returns the correct value for a diamond treasure.
        /// </summary>
        [Test]
        public void Treasure_GetValueTest()
        {
            // Arrange
            var treasure = new Treasure(new Vector2(100, 100), TreasureType.Diamond);

            // Act
            var value = treasure.GetValue();

            // Assert
            Assert.AreEqual(200, value); // Diamond treasure has a value of 200
        }
    }


}
