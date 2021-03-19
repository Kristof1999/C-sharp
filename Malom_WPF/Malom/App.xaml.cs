using System;
using System.Windows;
using Malom.Model;
using Malom.ViewModel;
using Malom.View;
using System.Linq;
using Malom.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Configuration;

namespace Malom
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private MainWindow view;
        private ModelClass model;
        private ViewModelGame viewModel;
        private SaveWindow saveWindow;
        private LoadWindow loadWindow;
        private IPersistence dataAccess;
        public App()
        {
            Startup += App_Startup;
        }

        private void App_Startup(object sender, StartupEventArgs e)
        {
            var contextOptions = new DbContextOptionsBuilder<GameContext>().UseSqlServer(ConfigurationManager.ConnectionStrings["MalomModelv2"].ConnectionString).Options;
            dataAccess = new DbDataAcces(contextOptions);
            model = new ModelClass(dataAccess);
            model.GameOverEvent += Model_GameOverEvent;

            viewModel = new ViewModelGame(model);
            viewModel.ExceptionEvent += ViewModel_ExceptionEvent;
            viewModel.ExitEvent += ViewModel_ExitEvent;
            viewModel.LoadGameOpen += ViewModel_LoadGameOpen;
            viewModel.LoadGameClose += ViewModel_LoadGameClose;
            viewModel.SaveGameOpen += ViewModel_SaveGameOpen;
            viewModel.SaveGameClose += ViewModel_SaveGameClose;

            view = new MainWindow();
            view.DataContext = viewModel;
            view.Show();
        }

        private async void ViewModel_SaveGameClose(object sender, string name)
        {
            if (name != null)
            {
                try
                {
                    var games = await model.ListGamesAsync();
                    if (games.All(g => g.Name != name) ||
                        MessageBox.Show("Biztos, hogy felülírja a meglévő mentést?", "Malom",
                            MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        await model.saveGameAsync(name);
                    }
                }
                catch
                {
                    MessageBox.Show("Játék mentése sikertelen!", "Hiba!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            saveWindow.Close();
        }

        private void ViewModel_SaveGameOpen(object sender, EventArgs e)
        {
            viewModel.SelectedGame = null;
            viewModel.NewName = String.Empty;

            saveWindow = new SaveWindow();
            saveWindow.DataContext = viewModel;
            saveWindow.ShowDialog();
        }

        private async void ViewModel_LoadGameClose(object sender, string name)
        {
            if (name != null)
            {
                try
                {
                    await model.loadGameAsync(name);
                }
                catch
                {
                    MessageBox.Show("Játék betöltése sikertelen!", "Hiba!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            loadWindow.Close();
        }

        private void ViewModel_LoadGameOpen(object sender, EventArgs e)
        {
            viewModel.SelectedGame = null;

            loadWindow = new LoadWindow();
            loadWindow.DataContext = viewModel;
            loadWindow.ShowDialog();
        }

        private void ViewModel_ExitEvent(object sender, EventArgs e)
        {
            view.Close();
        }

        private void Model_GameOverEvent(object sender, string e)
        {
            MessageBox.Show(e, "Malom", MessageBoxButton.OK);
        }

        private void ViewModel_ExceptionEvent(object sender, string e)
        {
            MessageBox.Show(e, "Malom", MessageBoxButton.OK);
        }
    }
}
