﻿using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Solitaire.Game
{
    internal class Board
    {
        private const uint BoardEdgeSize = 7;

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
                    if (!IsFieldLegal(x, y))
                    {
                        continue;
                    }
                    
                    var test = new Ellipse
                    {
                        Fill = Brushes.Beige
                    };

                    Grid.SetColumn(test, x);
                    Grid.SetRow(test, y);

                    grid.Children.Add(test);
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