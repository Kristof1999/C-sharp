
namespace Malom.Persistence
{
    public struct Helper
    {
        public Table table;
        public int curPlayer, player1FigureCount, player2FigureCount;
        public bool canAttack;
    }
    public class Point
    {
        public int row, col;
        public Point(int row, int col)
        {
            this.row = row;
            this.col = col;
        }
    }
    public class Field
    {
        public Point up, down, left, right;
        public FIELDS value;
        public Field(FIELDS val, Point up, Point down, Point left, Point right)
        {
            value = val;
            this.up = up;
            this.down = down;
            this.left = left;
            this.right = right;
        }
        public Field(FIELDS val)
        {
            value = val;
        }
    }
    public enum FIELDS { NaF, empty, player1, player2, markedPlayer1, markedPlayer2 }; //NaF: Not a Field
    
    public class Table
    {
        public Field[,] table;
        public Table()
        {
            table = new Field[7, 7];
            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    table[i, j] = new Field(FIELDS.NaF);
                }
            }
            table[0, 0].value = FIELDS.empty;
            table[0, 0].up = null;
            table[0, 0].down = new Point(3, 0);
            table[0, 0].left = null;
            table[0, 0].right = new Point(0, 3);
            table[0, 3].value = FIELDS.empty;
            table[0, 3].up = null;
            table[0, 3].down = new Point(1, 3);
            table[0, 3].left = new Point(0, 0);
            table[0, 3].right = new Point(0, 6);
            table[0, 6].value = FIELDS.empty;
            table[0, 6].up = null;
            table[0, 6].down = new Point(3, 6);
            table[0, 6].left = new Point(0, 3);
            table[0, 6].right = null;
            table[1, 1].value = FIELDS.empty;
            table[1, 1].up = null;
            table[1, 1].down = new Point(3, 1);
            table[1, 1].left = null;
            table[1, 1].right = new Point(1, 3);
            table[1, 3].value = FIELDS.empty;
            table[1, 3].up = new Point(0, 3);
            table[1, 3].down = new Point(2, 3);
            table[1, 3].left = new Point(1, 1);
            table[1, 3].right = new Point(1, 5);
            table[1, 5].value = FIELDS.empty;
            table[1, 5].up = null;
            table[1, 5].down = new Point(3, 5);
            table[1, 5].left = new Point(1, 3);
            table[1, 5].right = null;
            table[2, 2].value = FIELDS.empty;
            table[2, 2].up = null;
            table[2, 2].down = new Point(3, 2);
            table[2, 2].left = null;
            table[2, 2].right = new Point(2, 3);
            table[2, 3].value = FIELDS.empty;
            table[2, 3].up = new Point(1, 3);
            table[2, 3].down = null;
            table[2, 3].left = new Point(2, 2);
            table[2, 3].right = new Point(2, 4);
            table[2, 4].value = FIELDS.empty;
            table[2, 4].up = null;
            table[2, 4].down = new Point(3, 4);
            table[2, 4].left = new Point(2, 3);
            table[2, 4].right = null;
            table[3, 0].value = FIELDS.empty;
            table[3, 0].up = new Point(0, 0);
            table[3, 0].down = new Point(6, 0);
            table[3, 0].left = null;
            table[3, 0].right = new Point(3, 1);
            table[3, 1].value = FIELDS.empty;
            table[3, 1].up = new Point(1, 1);
            table[3, 1].down = new Point(5, 1);
            table[3, 1].left = new Point(3, 0);
            table[3, 1].right = new Point(3, 2);
            table[3, 2].value = FIELDS.empty;
            table[3, 2].up = new Point(2, 2);
            table[3, 2].down = new Point(4, 2);
            table[3, 2].left = new Point(3, 1);
            table[3, 2].right = null;
            table[3, 4].value = FIELDS.empty;
            table[3, 4].up = new Point(2, 4);
            table[3, 4].down = new Point(4, 4);
            table[3, 4].left = null;
            table[3, 4].right = new Point(3, 5);
            table[3, 5].value = FIELDS.empty;
            table[3, 5].up = new Point(1, 5);
            table[3, 5].down = new Point(5, 5);
            table[3, 5].left = new Point(3, 4);
            table[3, 5].right = new Point(3, 6);
            table[3, 6].value = FIELDS.empty;
            table[3, 6].up = new Point(0, 6);
            table[3, 6].down = new Point(6, 6);
            table[3, 6].left = new Point(3, 5);
            table[3, 6].right = null;
            table[4, 2].value = FIELDS.empty;
            table[4, 2].up = new Point(3, 2);
            table[4, 2].down = null;
            table[4, 2].left = null;
            table[4, 2].right = new Point(4, 3);
            table[4, 3].value = FIELDS.empty;
            table[4, 3].up = null;
            table[4, 3].down = new Point(5, 3);
            table[4, 3].left = new Point(4, 2);
            table[4, 3].right = new Point(4, 4);
            table[4, 4].value = FIELDS.empty;
            table[4, 4].up = new Point(3, 4);
            table[4, 4].down = null;
            table[4, 4].left = new Point(4, 3);
            table[4, 4].right = null;
            table[5, 1].value = FIELDS.empty;
            table[5, 1].up = new Point(3, 1);
            table[5, 1].down = null;
            table[5, 1].left = null;
            table[5, 1].right = new Point(5, 3);
            table[5, 3].value = FIELDS.empty;
            table[5, 3].up = new Point(4, 3);
            table[5, 3].down = new Point(6, 3);
            table[5, 3].left = new Point(5, 1);
            table[5, 3].right = new Point(5, 5);
            table[5, 5].value = FIELDS.empty;
            table[5, 5].up = new Point(3, 5);
            table[5, 5].down = null;
            table[5, 5].left = new Point(5, 3);
            table[5, 5].right = null;
            table[6, 0].value = FIELDS.empty;
            table[6, 0].up = new Point(3, 0);
            table[6, 0].down = null;
            table[6, 0].left = null;
            table[6, 0].right = new Point(6, 3);
            table[6, 3].value = FIELDS.empty;
            table[6, 3].up = new Point(5, 3);
            table[6, 3].down = null;
            table[6, 3].left = new Point(6, 0);
            table[6, 3].right = new Point(6, 6);
            table[6, 6].value = FIELDS.empty;
            table[6, 6].up = new Point(3, 6);
            table[6, 6].down = null;
            table[6, 6].left = new Point(6, 3);
            table[6, 6].right = null;
        }
    }
}
