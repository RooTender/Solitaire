using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using JetBrains.Annotations;

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
            panel.Children.Add(menuBar);
            DockPanel.SetDock(menuBar, Dock.Top);

            var board = new Board();
            panel.Children.Add(board.GetGrid());

            Content = panel;
        }
    }
}
