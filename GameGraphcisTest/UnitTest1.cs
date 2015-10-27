using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework.Graphics;
using Game1 = GameGraphcisTest.Game1;

namespace GameGraphcisTest
{
    [TestClass]
    public class UnitTest1
    {
        Game1 Game = new Game1();
        [TestMethod]
        public void LoadingTextureWorksCorrectly()
        {
            Texture2D texture = Game.GameGrapahics.LoadTexture(@"C:\GameCraftData\Content\bin\test.xnb");
        }
    }
}
