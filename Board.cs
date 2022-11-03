﻿using System;
using System.Windows;
using System.Windows.Controls;

namespace Solitaire
{
    internal class Board
    {
        private const uint BoardEdgeSize = 7;
        private readonly bool[,] _legalFields;

        public Board()
        {
            _legalFields = GetLegalFields();
        }

        public Grid GetGrid()
        {
            var grid = new Grid
            {
                ShowGridLines = true
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

            return grid;
        }

        private static bool[,] GetLegalFields()
        {
            var legalFields = new bool[BoardEdgeSize, BoardEdgeSize];

            for (var y = 0; y < BoardEdgeSize; ++y)
            {
                for (var x = 0; x < BoardEdgeSize; ++x)
                {           
                    legalFields[x, y] = IsFieldLegal(x, y);
                }
            }

            return legalFields;
        }

        private static bool IsFieldLegal(int x, int y)
        {
            const int illegalCornerEdgeSize = 2;
            var isIllegal = 
                x < illegalCornerEdgeSize || 
                y < illegalCornerEdgeSize || 
                x > BoardEdgeSize - illegalCornerEdgeSize ||
                y > BoardEdgeSize - illegalCornerEdgeSize;

            return !isIllegal;
        }
    }
}
