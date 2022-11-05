using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using JetBrains.Annotations;
using Solitaire.Game;

namespace Solitaire
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [UsedImplicitly]
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            var panel = new DockPanel
            {
                Height = double.NaN,
                VerticalAlignment = VerticalAlignment.Stretch
            };

            var menuBar = new Grid
            {
                VerticalAlignment = VerticalAlignment.Top,
                Height = 30,
                Background = Brushes.FloralWhite
            };
            menuBar.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            menuBar.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            menuBar.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(8, GridUnitType.Star) });

            var newGameButton = new Button
            {
                Content = "New game",
                Margin = new Thickness(1)
            };
            Grid.SetColumn(newGameButton, 0);
            
            var undoButton = new Button
            {
                Content = "Undo move",
                Margin = new Thickness(1)
            };
            Grid.SetColumn(undoButton, 1);

            var board = new Board();

            board.SetNewGameButtonTrigger(ref newGameButton);
            board.SetUndoButtonTrigger(ref undoButton);

            menuBar.Children.Add(newGameButton);
            menuBar.Children.Add(undoButton);

            panel.Children.Add(menuBar);
            DockPanel.SetDock(menuBar, Dock.Top);

            panel.Children.Add(board.GetBoard());

            Content = panel;
        }
    }
}
