using Microsoft.VisualStudio.TestTools.UnitTesting;
using Malom.Model;
using Malom.Persistence;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace ModelTest
{
    class MockPersistence : IPersistence
    {
        public async Task<Helper> LoadAsync(string path)
        {
            Helper helper = new Helper();
            helper.canAttack = false;
            helper.curPlayer = 1;
            helper.player1FigureCount = helper.player2FigureCount = 9;
            helper.table = new Table();
            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    helper.table.table[i, j].value = FIELDS.NaF;
                }
            }
            return helper;
        }
        public async Task SaveAsync(String path, Helper helper)
        {

        }
        public async Task<ICollection<SaveEntry>>ListAsync()
        { return null; }
    }

    [TestClass]
    public class ModelTest
    {
        private ModelClass model;
        private MockPersistence dataAccess;

        [TestInitialize]
        public void Initialize()
        {
            dataAccess = new MockPersistence();
            model = new ModelClass(dataAccess);
            model.GameOverEvent += GameOver;
            model.InformPlayerEvent += InformPlayer;
        }
        [TestMethod]
        public async Task TestLoadSave()
        {
            await model.loadGameAsync("");
            Assert.AreEqual(model.CanAttack, false);
            Assert.AreEqual(model.CurPlayer, 1);
            Assert.AreEqual(model.Player1FigureCount, 9);
            Assert.AreEqual(model.Player2FigureCount, 9);
        }

        [TestMethod]
        public void TestNewGame()
        {
            model.newGame();
            Assert.AreEqual(model.CanAttack, false);
            Assert.AreEqual(model.CurPlayer, 1);
            Assert.AreEqual(model.Player1FigureCount, 9);
            Assert.AreEqual(model.Player2FigureCount, 9);
            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    if (model.getField(i, j) != FIELDS.NaF)
                        Assert.AreEqual(model.getField(i, j), FIELDS.empty);
                }
            }
        }

        [TestMethod]
        public void TestStep()
        {
            Assert.ThrowsException<OutOfRangeException>(() => model.step(-1, -1));
            Assert.ThrowsException<OutOfRangeException>(() => model.step(-1, 2));
            Assert.ThrowsException<OutOfRangeException>(() => model.step(8, -1));
            Assert.ThrowsException<OutOfRangeException>(() => model.step(2, 9));
            Assert.ThrowsException<NaFException>(() => model.step(1, 2));
            Assert.AreEqual(model.CurPlayer, 1);
            Assert.AreEqual(model.CanAttack, false);
            Assert.AreEqual(model.Player1FigureCount, 9);
            Assert.AreEqual(model.Player2FigureCount, 9);

            model.step(0, 0);
            Assert.AreEqual(model.getField(0, 0), FIELDS.player1);
            Assert.AreEqual(model.CurPlayer, 2);
            Assert.AreEqual(model.CanAttack, false);
            Assert.AreEqual(model.Player1FigureCount, 8);
            Assert.AreEqual(model.Player2FigureCount, 9);

            Assert.ThrowsException<NotEmptyFieldException>(() => model.step(0, 0));
            Assert.AreEqual(model.getField(0, 0), FIELDS.player1);
            Assert.AreEqual(model.getField(0, 1), FIELDS.NaF);
            Assert.AreEqual(model.CurPlayer, 2);
            Assert.AreEqual(model.CanAttack, false);
            Assert.AreEqual(model.Player1FigureCount, 8);
            Assert.AreEqual(model.Player2FigureCount, 9);

            model.step(0, 3);
            model.step(0, 6);
            model.step(1, 1);
            model.step(1, 3);
            model.step(1, 5);
            model.step(2, 2);
            model.step(2, 4);
            model.step(2, 3);
            model.step(3, 0);
            model.step(6, 0);
            model.step(6, 3);
            model.step(6, 6);
            model.step(3, 6);
            model.step(5, 3);
            model.step(4, 3);
            model.step(4, 4);
            model.step(4, 2);
            Assert.AreEqual(model.Player1FigureCount, 0);
            Assert.AreEqual(model.Player2FigureCount, 0);
            Assert.ThrowsException<BadMarkException>(() => model.step(0, 0));
            Assert.AreEqual(model.CurPlayer, 1);
            model.step(4, 4);
            Assert.AreEqual(model.getField(4, 4), FIELDS.markedPlayer1);
            Assert.ThrowsException<NotEmptyFieldException>(() => model.step(4, 3));
            Assert.ThrowsException<IllegalMoveException>(() => model.step(3, 5));
            model.step(3, 4);
            Assert.AreEqual(model.getField(4, 4), FIELDS.empty);
            Assert.AreEqual(model.getField(3, 4), FIELDS.player1);
            Assert.AreEqual(model.CurPlayer, 2);
            Assert.ThrowsException<BadMarkException>(() => model.step(3, 4));
        }
        private void GameOver(Object sender, string e)
        {
            Assert.AreEqual(model.CanAttack, false);
            Assert.AreEqual(model.Player1FigureCount, 0);
            Assert.AreEqual(model.Player2FigureCount, 0);
        }
        private void InformPlayer(Object sender, string e)
        {

        }
    }
}
