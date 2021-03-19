using System.Drawing;
using System.Windows.Forms;
using System;
using Model;
using System.IO;
using Persistence;

namespace GameForm
{
    public partial class GameForm : Form
    {
        #region Fields_and_Constructor
        ModelClass model;
        public GameForm()
        {
            InitializeComponent();

            model = new ModelClass(new FileDataAccess());
            model.InformPlayerEvent += refreshLabel;
            model.GameOverEvent += gameOver;

            setupTable();

            tableLayoutPanel1.Paint += drawLines;

            var items = fileMenuItem.DropDownItems;
            items[0].Click += saveGame;
            items[1].Click += loadGame;

            newGameMenuItem.Click += newGame;
        }
        #endregion

        #region Load-Save-NewGame_Methods
        private void newGame(Object sender, EventArgs e)
        {
            label1.Text = "Új játék";
            model.newGame();
            setupTable();
        }
        private async void saveGame(Object sender, EventArgs e)
        {
            FileDialog saveFileDialog = new SaveFileDialog();
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    await model.saveGameAsync(saveFileDialog.FileName);
                }
                catch (UnauthorizedAccessException)
                {
                    MessageBox.Show(this, "Hiba! Nincs jogosultsága ide írni.", "Malom", MessageBoxButtons.OK);
                }
                catch (DirectoryNotFoundException)
                {
                    MessageBox.Show(this, "Hiba! A mappa nem található.", "Malom", MessageBoxButtons.OK);
                }
                catch (InvalidOperationException err)
                {
                    MessageBox.Show(this, err.Message, "Malom", MessageBoxButtons.OK);
                }
            }
        }
        private async void loadGame(Object sender, EventArgs e)
        {
            FileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    await model.loadGameAsync(openFileDialog.FileName);
                    label1.Text = "Játék betöltve";
                }
                catch (FileNotFoundException)
                {
                    MessageBox.Show(this, "Hiba! " + openFileDialog.FileName + " fájl nem található", "Malom", MessageBoxButtons.OK);
                }
                catch (InvalidOperationException err)
                {
                    MessageBox.Show(this, err.Message, "Malom", MessageBoxButtons.OK);
                }
                setupTable();
            }
        }
        #endregion

        #region HandlerMethods

        private void refreshLabel(Object sender, string e)
        {
            label1.Text = e;
        }
        private void buttonClicked(Object sender, EventArgs e)
        {
            int x = tableLayoutPanel1.GetRow(sender as Button);
            int y = tableLayoutPanel1.GetColumn(sender as Button);
            try
            {
                model.step(x, y);
            }
            catch (OutOfRangeException)
            {
                MessageBox.Show(this, "Hiba! Táblán kívüli elérés.", "Malom", MessageBoxButtons.OK);
            }
            catch (NaFException)
            {
                MessageBox.Show(this, "Hiba! Ide nem lehet lépni.", "Malom", MessageBoxButtons.OK);
            }
            catch (NotEmptyFieldException)
            {
                MessageBox.Show(this, "Hiba! Ez a mező nem üres, ide nem lehet tenni bábút.", "Malom", MessageBoxButtons.OK);
            }
            catch (IllegalMoveException)
            {
                MessageBox.Show(this, "Hiba! Csak a szomszédos, összekötött területekre lehet lépni!", "Malom", MessageBoxButtons.OK);
            }
            catch (CannotMoveException)
            {
                MessageBox.Show(this, "A játékos nem tud lépni, így a másik játékos lép.", "Malom", MessageBoxButtons.OK);
            }
            catch (InvalidAttackException)
            {
                MessageBox.Show(this, "Helytelen támadás. Csak a másik játékos azon bábúiból vehetünk le egyet, amelyik nem egy malom része.", "Malom", MessageBoxButtons.OK);
            }
            catch (BadMarkException)
            {
                MessageBox.Show(this, "Rossz választás. Válassz másik figurát.", "Malom", MessageBoxButtons.OK);
            }
            setupTable();
        }

        private void drawLines(Object sender, PaintEventArgs e)
        {
            Graphics gr = e.Graphics;
            Pen pen = new Pen(Color.Black, 10);
            //külső négyzet
            gr.DrawLine(pen, 10, 20, 700, 20);
            gr.DrawLine(pen, 30, 30, 30, 350);
            gr.DrawLine(pen, 30, 330, 700, 330);
            gr.DrawLine(pen, 650, 20, 650, 350);
            //középső négyzet
            gr.DrawLine(pen, 90, 70, 600, 70);
            gr.DrawLine(pen, 110, 70, 110, 270);
            gr.DrawLine(pen, 550, 70, 550, 270);
            gr.DrawLine(pen, 110, 290, 550, 290);
            //belső négyzet
            gr.DrawLine(pen, 200, 130, 450, 130);
            gr.DrawLine(pen, 210, 130, 210, 250);
            gr.DrawLine(pen, 450, 130, 450, 250);
            gr.DrawLine(pen, 210, 230, 450, 230);
            //négyzetek közötti vonalak
            gr.DrawLine(pen, 330, 20, 330, 150);
            gr.DrawLine(pen, 330, 220, 330, 330);
            gr.DrawLine(pen, 10, 180, 250, 180);
            gr.DrawLine(pen, 400, 180, 700, 180);
        }
        private void gameOver(Object sender, string e)
        {
            MessageBox.Show(this, e, "Malom", MessageBoxButtons.OK);
            label1.Text = "Új játék";
            model.newGame();
            setupTable();
        }
        #endregion

        #region Misc
        private void setupTable()
        {
            for(int i = 0; i < 7; i++)
            {
                for(int j = 0; j < 7; j++)
                {
                    var field = tableLayoutPanel1.GetControlFromPosition(j, i);
                    if(field != null)
                    {
                        switch (model.getField(i, j))
                        {
                            case FIELDS.empty:
                                field.BackColor = Color.White;
                                break;
                            case FIELDS.player1:
                                field.BackColor = Color.Red;
                                break;
                            case FIELDS.player2:
                                field.BackColor = Color.Blue;
                                break;
                            case FIELDS.markedPlayer1:
                            case FIELDS.markedPlayer2:
                                field.BackColor = Color.Black;
                                break;
                        }
                    }
                }
            }
        }
        #endregion
    }
}
