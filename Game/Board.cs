using System.Windows.Controls;

namespace Solitaire.Game
{
    internal class Board
    {
        private const uint BoardEdgeSize = 7;
        private bool[,] _isFieldLegal = new bool[BoardEdgeSize,BoardEdgeSize];

        public Grid GetGrid()
        {
            var grid = new Grid
            {
                //ShowGridLines = true
            };

            for (var column = 0; column < BoardEdgeSize; ++column)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition
                {
                    Name = "col" + column
                });
            }

            for (var row = 0; row < BoardEdgeSize; ++row)
            {
                grid.RowDefinitions.Add(new RowDefinition
                {
                    Name = "row" + row
                });
            }

            for (var y = 0; y < BoardEdgeSize; ++y)
            {
                for (var x = 0; x < BoardEdgeSize; ++x)
                {
                    _isFieldLegal[x, y] = true;

                    if (!IsFieldLegal(x, y))
                    {
                        _isFieldLegal[x, y] = false;
                        continue;
                    }

                    var field = new Field(x, y, FieldShapes.Circle, ref _isFieldLegal);

                    Grid.SetColumn(field.GetElement(), x);
                    Grid.SetRow(field.GetElement(), y);

                    const uint centerOfTheBoard = BoardEdgeSize / 2;
                    if (x == centerOfTheBoard && y == centerOfTheBoard)
                    {
                        field.IsPlayer = true;
                    }

                    grid.Children.Add(field.GetElement());
                }
            }

            return grid;
        }

        private static bool IsFieldLegal(int x, int y)
        {
            const int illegalCornerEdgeSize = 2;
            var atTopLeftCorner = x < illegalCornerEdgeSize && y < illegalCornerEdgeSize;
            var atTopRightCorner = x > BoardEdgeSize - illegalCornerEdgeSize - 1 && y < illegalCornerEdgeSize;
            var atBottomLeftCorner = x < illegalCornerEdgeSize && y > BoardEdgeSize - illegalCornerEdgeSize - 1;
            var atBottomRightCorner = x > BoardEdgeSize - illegalCornerEdgeSize - 1 && y > BoardEdgeSize - illegalCornerEdgeSize - 1;

            return !atTopLeftCorner && !atTopRightCorner && !atBottomLeftCorner && !atBottomRightCorner;
        }
    }
}
