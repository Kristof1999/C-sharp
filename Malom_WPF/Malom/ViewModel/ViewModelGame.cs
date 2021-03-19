using System;
using System.Collections.ObjectModel;
using Malom.Model;
using Malom.Persistence;

namespace Malom.ViewModel
{
    class ViewModelGame : ViewModelBase
    {
        private ModelClass model;
        private string labelText;

        public string LabelText
        {
            get { return labelText; }
            set
            {
                labelText = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<FieldViewModel> Fields { get; set; } //nézetmodellhez
        public ObservableCollection<SaveEntry> Games { get; set; } //adatbázishoz (elmentett játékok)
        private string newName = String.Empty;
        private SaveEntry selectedGame;
        public String NewName
        {
            get { return newName; }
            set
            {
                newName = value;

                OnPropertyChanged();
                SaveGameCloseCommand.RaiseCanExecuteChanged();
            }
        }
        public SaveEntry SelectedGame
        {
            get { return selectedGame; }
            set
            {
                selectedGame = value;
                if (selectedGame != null)
                    NewName = String.Copy(selectedGame.Name);

                OnPropertyChanged();
                LoadGameCloseCommand.RaiseCanExecuteChanged();
                SaveGameCloseCommand.RaiseCanExecuteChanged();
            }
        }

        public event EventHandler<string> ExceptionEvent;
        public event EventHandler ExitEvent;
        public event EventHandler LoadGameOpen;
        public event EventHandler<string> LoadGameClose;
        public event EventHandler SaveGameOpen;
        public event EventHandler<string> SaveGameClose;

        public DelegateCommand NewGameCommand { get; set; }
        public DelegateCommand ExitCommand { get; set; }
        public DelegateCommand LoadGameOpenCommand { get; set; }
        public DelegateCommand SaveGameOpenCommand { get; set; }
        public DelegateCommand SaveGameCloseCommand { get; set; }
        public DelegateCommand LoadGameCloseCommand { get; set; }
        public ViewModelGame(ModelClass model)
        {
            this.model = model;
            model.InformPlayerEvent += RefreshLabel;

            NewGameCommand = new DelegateCommand(p => NewGame());
            ExitCommand = new DelegateCommand(p => OnExitGame());
            LoadGameOpenCommand = new DelegateCommand(async param =>
            {
                Games = new ObservableCollection<SaveEntry>(await model.ListGamesAsync());
                OnLoadGameOpen();
            });
            LoadGameCloseCommand = new DelegateCommand(
                param => SelectedGame != null,
                param => { OnLoadGameClose(SelectedGame.Name); });
            SaveGameOpenCommand = new DelegateCommand(async param =>
            {
                Games = new ObservableCollection<SaveEntry>(await model.ListGamesAsync());
                OnSaveGameOpen();
            });
            SaveGameCloseCommand = new DelegateCommand(
                param => NewName.Length > 0,
                param => { OnSaveGameClose(NewName); });

            Fields = new ObservableCollection<FieldViewModel>();
            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    Fields.Add(new FieldViewModel
                    {
                        Number = i * 7 + j,
                        FieldType = FIELDS.NaF,
                        StepCommand = new DelegateCommand(p => Step(Convert.ToInt32(p))),
                        Row = i,
                        Col = j
                    });
                }
            }
            RefreshTable();
        }
        private void Step(int fieldNumber)
        {
            int row = fieldNumber / 7;
            int col = fieldNumber % 7;
            try
            {
                model.step(row, col);
                RefreshTable();
            }
            catch (OutOfRangeException)
            {
                OnExceptionEvent("Hiba! Táblán kívüli elérés.");
            }
            catch (NaFException)
            {
                OnExceptionEvent("Hiba! Ide nem lehet lépni.");
            }
            catch (NotEmptyFieldException)
            {
                OnExceptionEvent("Hiba! Ez a mező nem üres, ide nem lehet tenni bábút.");
            }
            catch (IllegalMoveException)
            {
                OnExceptionEvent("Hiba! Csak a szomszédos, összekötött területekre lehet lépni!");
            }
            catch (CannotMoveException)
            {
                OnExceptionEvent("A játékos nem tud lépni, így a másik játékos lép.");
            }
            catch (InvalidAttackException)
            {
                OnExceptionEvent("Helytelen támadás. Csak a másik játékos azon bábúiból vehetünk le egyet, amelyik nem egy malom része.");
            }
            catch (BadMarkException)
            {
                OnExceptionEvent("Rossz választás. Válassz másik figurát.");
            }
        }
        private void RefreshLabel(object sender, string e)
        {
            LabelText = e;
        }
        private void OnExceptionEvent(string str)
        {
            if (ExceptionEvent != null)
                ExceptionEvent(this, str);
        }
        private void RefreshTable()
        {
            foreach (var field in Fields)
                field.FieldType = model.getField(field.Number / 7, field.Number % 7);
        }
        private void NewGame()
        {
            model.newGame();
            RefreshTable();
        }
        private void OnExitGame()
        {
            if (ExitEvent != null)
                ExitEvent(this, null);
        }
        private void OnLoadGameOpen()
        {
            LoadGameOpen?.Invoke(this, null);
        }
        private void OnLoadGameClose(string gameName)
        {
            LoadGameClose?.Invoke(this, gameName);
        }
        private void OnSaveGameOpen()
        {
            SaveGameOpen?.Invoke(this, null);
        }
        private void OnSaveGameClose(string newName)
        {
            SaveGameClose?.Invoke(this, newName);
        }
    }
}
