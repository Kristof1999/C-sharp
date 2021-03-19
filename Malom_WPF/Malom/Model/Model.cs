using System;
using System.Threading.Tasks;
using Malom.Persistence;
using System.Collections.Generic;

namespace Malom.Model
{
    public class ModelClass
    {
        #region DataFields
        private Table table;
        private int player1FigureCount, player2FigureCount;
        private int curPlayer;
        private bool canAttack;
        private IPersistence dataAccess;
        public event EventHandler<string> InformPlayerEvent;
        public event EventHandler<string> GameOverEvent;
        public int Player1FigureCount { get { return player1FigureCount; } }
        public int Player2FigureCount { get { return player2FigureCount; } }
        public int CurPlayer { get { return curPlayer; } }
        public bool CanAttack { get { return canAttack; } }
        #endregion

        #region Constructor
        public ModelClass(IPersistence dataAccess)
        {
            table = new Table();
            player1FigureCount = player2FigureCount = 9;
            canAttack = false;
            curPlayer = 1;
            this.dataAccess = dataAccess;
        }
        #endregion

        #region Load-Save-NewGame_Methods
        public async Task loadGameAsync(string name)
        {
            if (dataAccess == null)
                throw new InvalidOperationException("No data access is provided.");
            Helper helper = new Helper();
            helper = await dataAccess.LoadAsync(name);
            curPlayer = helper.curPlayer;
            player1FigureCount = helper.player1FigureCount;
            player2FigureCount = helper.player2FigureCount;
            canAttack = helper.canAttack;
            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    table.table[i, j].value = helper.table.table[i, j].value;
                }
            }
        }

        public void newGame()
        {
            curPlayer = 1;
            player1FigureCount = player2FigureCount = 9;
            canAttack = false;
            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    if (table.table[i, j].value != FIELDS.NaF)
                        table.table[i, j].value = FIELDS.empty;
                }
            }
            InformPlayerEvent(this, "Új játék: piros jön.");
        }

        public async Task saveGameAsync(string name)
        {
            if (dataAccess == null)
                throw new InvalidOperationException("No data access is provided.");
            Helper helper = new Helper();
            helper.curPlayer = curPlayer;
            helper.player1FigureCount = player1FigureCount;
            helper.player2FigureCount = player2FigureCount;
            helper.canAttack = canAttack;
            helper.table = new Table();
            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    helper.table.table[i, j].value = table.table[i, j].value;
                }
            }
            await dataAccess.SaveAsync(name, helper);
        }
        public async Task<ICollection<SaveEntry>> ListGamesAsync()
        {
            if (dataAccess == null)
                throw new InvalidOperationException("No data access is provided.");

            return await dataAccess.ListAsync();
        }
        #endregion

        #region MoveMethods
        public void step(int row, int col)
        {
            if (row < 0 || col < 0 || row > 7 || col > 7)
                throw new OutOfRangeException();
            if (table.table[row, col].value == FIELDS.NaF)
                throw new NaFException();
            if (player1FigureCount == 0 && player2FigureCount == 0) //ha vége a rakodási fázisnak
            {
                if (canMove())
                {
                    if (canAttack)
                        attack(row, col);
                    else
                    {
                        if (numberOfMarkedFields() == 0)
                            markFigure(row, col);
                        else
                            moveFigure(row, col);
                    }
                }
                else
                    throw new CannotMoveException();
            }
            else
            {
                if (canAttack)
                    attack(row, col);
                else
                    placeFigure(row, col);
                if (player1FigureCount == 0 && player2FigureCount == 0)
                    InformPlayerEvent(this, "Rakodási fázis vége. Mozgatási fázis kezdődik. Piros választ.");
            }
        }

        private void placeFigure(int row, int col)
        {
            if (table.table[row, col].value == FIELDS.empty)
            {
                if (curPlayer == 1)
                {
                    table.table[row, col].value = FIELDS.player1;
                    player1FigureCount = player1FigureCount > 0 ? player1FigureCount - 1 : 0;
                }
                else
                {
                    table.table[row, col].value = FIELDS.player2;
                    player2FigureCount = player2FigureCount > 0 ? player2FigureCount - 1 : 0;
                }
            }
            else
            {
                throw new NotEmptyFieldException();
            }
            if (check3Group(row, col))
            {
                if (isAttackable(curPlayer == 1 ? FIELDS.player2 : FIELDS.player1))
                {
                    canAttack = true;
                    InformPlayerEvent(this, "Rakodási fázis: " + (curPlayer == 1 ? "piros " : "kék ") + " támad.");
                }
                else
                {
                    InformPlayerEvent(this, (curPlayer == 1 ? "Piros" : "Kék") + " nem tud támadni, " + (curPlayer == 1 ? "kék" : "piros") + " jön.");
                    curPlayer = (curPlayer % 2) + 1;
                }
            }
            else
            {
                curPlayer = (curPlayer % 2) + 1;
                InformPlayerEvent(this, "Rakodási fázis: " + (curPlayer == 1 ? "piros " : "kék ") + " jön.");
            }
        }
        private void markFigure(int row, int col)
        {
            if (!isNeighbourEmpty(row, col))
                throw new BadMarkException();
            if (curPlayer == 1 && table.table[row, col].value == FIELDS.player1)
            {
                table.table[row, col].value = FIELDS.markedPlayer1;
                InformPlayerEvent(this, "Mozgatási fázis: figura megjelölve, piros lép.");
            }
            else if (curPlayer == 2 && table.table[row, col].value == FIELDS.player2)
            {
                table.table[row, col].value = FIELDS.markedPlayer2;
                InformPlayerEvent(this, "Mozgatási fázis: figura megjelölve, kék lép.");
            }
            else
                throw new BadMarkException();
        }
        private void moveFigure(int row, int col)
        {
            if (table.table[row, col].value == FIELDS.empty)
            {
                for (int i = 0; i < 7; i++)
                {
                    for (int j = 0; j < 7; j++)
                    {
                        if (table.table[i, j].value == FIELDS.markedPlayer1 || table.table[i, j].value == FIELDS.markedPlayer2)
                        {
                            if (isNeighbour(i, j, row, col))
                                table.table[i, j].value = FIELDS.empty;
                            else
                                throw new IllegalMoveException();
                        }
                    }
                }
                if (curPlayer == 1)
                    table.table[row, col].value = FIELDS.player1;
                else
                    table.table[row, col].value = FIELDS.player2;
                if (check3Group(row, col))
                {
                    if (isAttackable(curPlayer == 1 ? FIELDS.player2 : FIELDS.player1))
                    {
                        canAttack = true;
                        InformPlayerEvent(this, "Mozgatási fázis: " + (curPlayer == 1 ? "piros " : "kék ") + " támad.");
                    }
                    else
                    {
                        InformPlayerEvent(this, (curPlayer == 1 ? "Piros" : "Kék") + " nem tud támadni, " + (curPlayer == 1 ? "kék" : "piros") + " jön.");
                        curPlayer = (curPlayer % 2) + 1;
                    }
                }
                else
                {
                    curPlayer = (curPlayer % 2) + 1;
                    InformPlayerEvent(this, "Mozgatási fázis: " + (curPlayer == 1 ? "piros " : "kék ") + " választ mozgatandó figurát.");
                }
            }
            else
            {
                throw new NotEmptyFieldException();
            }
        }
        private bool isAttackable(FIELDS player)
        {
            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    if (table.table[i, j].value != FIELDS.NaF && table.table[i, j].value == player && !check3Group(i, j))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        private void attack(int row, int col)
        {
            if (curPlayer == 1)
            {
                if (table.table[row, col].value == FIELDS.player2 && !check3Group(row, col))
                {
                    table.table[row, col].value = FIELDS.empty;
                    canAttack = false;
                    curPlayer = (curPlayer % 2) + 1;
                    if (player1FigureCount == 0 && player2FigureCount == 0)
                        InformPlayerEvent(this, "Támadás vége, mozgatási fázis folytatódik, kék lép.");
                    else
                        InformPlayerEvent(this, "Támadás vége, rakodási fázis folytatódik, kék lép.");
                    if (countPlayerFigureCount(FIELDS.player2) < 3 && player1FigureCount == 0 && player2FigureCount == 0)
                        GameOverEvent(this, "Vége a játéknak. Piros győzött.");
                }
                else
                    throw new InvalidAttackException();
            }
            else
            {
                if (table.table[row, col].value == FIELDS.player1 && !check3Group(row, col))
                {
                    table.table[row, col].value = FIELDS.empty;
                    canAttack = false;
                    curPlayer = (curPlayer % 2) + 1;
                    if (player1FigureCount == 0 && player2FigureCount == 0)
                        InformPlayerEvent(this, "Támadás vége, mozgatási fázis folytatódik, piros lép.");
                    else
                        InformPlayerEvent(this, "Támadás vége, rakodási fázis folytatódik, piros lép.");
                    if (countPlayerFigureCount(FIELDS.player1) < 3 && player1FigureCount == 0 && player2FigureCount == 0)
                        GameOverEvent(this, "Vége a játéknak. Kék győzött.");
                }
                else
                    throw new InvalidAttackException();
            }
        }
        #endregion

        #region MoveHelperMethods
        public FIELDS getField(int row, int col)
        {
            return table.table[row, col].value;
        }
        private bool isNeighbourEmpty(int i, int j)
        {
            if (table.table[i, j].up != null && table.table[table.table[i, j].up.row, table.table[i, j].up.col].value == FIELDS.empty)
                return true;
            if (table.table[i, j].down != null && table.table[table.table[i, j].down.row, table.table[i, j].down.col].value == FIELDS.empty)
                return true;
            if (table.table[i, j].left != null && table.table[table.table[i, j].left.row, table.table[i, j].left.col].value == FIELDS.empty)
                return true;
            if (table.table[i, j].right != null && table.table[table.table[i, j].right.row, table.table[i, j].right.col].value == FIELDS.empty)
                return true;
            return false; //ha egyik szoszédja sem üres
        }
        private bool canMove()
        {
            bool canMove = false;
            if (curPlayer == 1)
            {
                for (int i = 0; i < 7 && canMove == false; i++)
                {
                    for (int j = 0; j < 7 && canMove == false; j++)
                    {
                        if (table.table[i, j].value == FIELDS.player1)
                        {
                            canMove = isNeighbourEmpty(i, j);
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < 7 && canMove == false; i++)
                {
                    for (int j = 0; j < 7 && canMove == false; j++)
                    {
                        if (table.table[i, j].value == FIELDS.player2)
                        {
                            canMove = isNeighbourEmpty(i, j);
                        }
                    }
                }
            }
            return canMove;
        }
        private bool isNeighbour(int fromRow, int fromCol, int toRow, int toCol)
        {
            if (table.table[fromRow, fromCol].up != null && table.table[fromRow, fromCol].up.row == toRow && table.table[fromRow, fromCol].up.col == toCol)
                return true;
            if (table.table[fromRow, fromCol].down != null && table.table[fromRow, fromCol].down.row == toRow && table.table[fromRow, fromCol].down.col == toCol)
                return true;
            if (table.table[fromRow, fromCol].left != null && table.table[fromRow, fromCol].left.row == toRow && table.table[fromRow, fromCol].left.col == toCol)
                return true;
            if (table.table[fromRow, fromCol].right != null && table.table[fromRow, fromCol].right.row == toRow && table.table[fromRow, fromCol].right.col == toCol)
                return true;
            return false; //ha table.table[fromRow,fromCol] nem szomszédja a table.table[toRow, toCol] elemnek
        }
        private int numberOfMarkedFields()
        {
            int ctr = 0;
            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    if (table.table[i, j].value == FIELDS.markedPlayer1 || table.table[i, j].value == FIELDS.markedPlayer2)
                        ctr++;
                }
            }
            return ctr;
        }
        private int countPlayerFigureCount(FIELDS player)
        {
            int counter = 0;
            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    if (table.table[i, j].value == player)
                        counter++;
                }
            }
            return counter;
        }
        private enum DIRECTION { up, down, left, right };
        //amíg lehet az adott irányba menni, megnézi, hogy az adott irányban a mezők megegyeznek-e fieldValue-val
        private bool checkDirection(int row, int col, DIRECTION direction, FIELDS fieldValue)
        {
            if (fieldValue == table.table[row, col].value)
            {
                switch (direction)
                {
                    case DIRECTION.up:
                        if (table.table[row, col].up != null)
                            return checkDirection(table.table[row, col].up.row, table.table[row, col].up.col, DIRECTION.up, fieldValue);
                        else
                            return true;//ha felfele nem tudunk tovább menni, akkor az azt jelenti, hogy az addig bejárt mezők
                                        //value értéke megegyezik a fieldValue-val és visszatérhetünk igazzal rekurzívan
                    case DIRECTION.down:
                        if (table.table[row, col].down != null)
                            return checkDirection(table.table[row, col].down.row, table.table[row, col].down.col, DIRECTION.down, fieldValue);
                        else
                            return true;
                    case DIRECTION.left:
                        if (table.table[row, col].left != null) //ha tudunk még balra menni, akkor nézzük meg, hogy annak a mezőnek a value-ja egyenlő-e a fieldValue-val
                            return checkDirection(table.table[row, col].left.row, table.table[row, col].left.col, DIRECTION.left, fieldValue);
                        else
                            return true;
                    case DIRECTION.right:
                        if (table.table[row, col].right != null)
                            return checkDirection(table.table[row, col].right.row, table.table[row, col].right.col, DIRECTION.right, fieldValue);
                        else
                            return true;
                }
                return true; //ha switch ágban egyik sem futott volna le (hiba!)
            }
            else
                return false; //ha az adott irányban találunk olyan mezőt, amelynek value-ja nem egyenlő fieldValue-val,
                              //akkor visszatérünk hamissal, és a rekuzió miatt az előző rekurzív hívások is hamissal térnek vissza
        }
        private bool check3Group(int row, int col)
        {
            bool isSameField = true;
            if (table.table[row, col].up != null)
            {
                isSameField = checkDirection(table.table[row, col].up.row, table.table[row, col].up.col, DIRECTION.up, table.table[row, col].value);
            }
            if (table.table[row, col].down != null && isSameField == true)
            {
                isSameField = checkDirection(table.table[row, col].down.row, table.table[row, col].down.col, DIRECTION.down, table.table[row, col].value);
            }
            if (isSameField == true) //ha az adott mező egy mező 3-as szélén van, akkor vagy a felfele vagy a lefele menetel ad igazat, a másik elágazás le sem fut
                return true;        //ha az adott mező egy mező 3-as közepén van, és ettől felfele levő mezővel megegyezik a value, majd az ettől lefele levővel is megegyezik
            isSameField = true;
            if (table.table[row, col].left != null)
            {
                isSameField = checkDirection(table.table[row, col].left.row, table.table[row, col].left.col, DIRECTION.left, table.table[row, col].value);
            }
            if (table.table[row, col].right != null && isSameField == true)
            {
                isSameField = checkDirection(table.table[row, col].right.row, table.table[row, col].right.col, DIRECTION.right, table.table[row, col].value);
            }
            if (isSameField == true)
                return true;
            return false; //ha nem találtunk 3-as mezőcsoportot, amelyek value-ja megegyezik
        }
        #endregion
    }
}

