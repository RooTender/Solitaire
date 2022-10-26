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
            var board = new Board();
            var grid = board.GetGrid();

            Content = grid;
        }
    }
}
