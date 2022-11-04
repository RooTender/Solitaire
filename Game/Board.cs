﻿using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Point = System.Drawing.Point;

namespace Solitaire.Game
{
    public class Board
    {
        private const uint BoardEdgeSize = 7;

        private readonly Grid _board;
        private readonly Field?[,] _fields;
        
        private Stack<Point> _playerPositionHistory;

        public Board()
        {
            _board = new Grid();
            _fields = new Field?[BoardEdgeSize,BoardEdgeSize];
            _playerPositionHistory = new Stack<Point>();

            Build();
        }

        public Grid GetBoard() => _board;

        private void Build()
        {
            for (var column = 0; column < BoardEdgeSize; ++column)
            {
                _board.ColumnDefinitions.Add(new ColumnDefinition
                {
                    Name = "col" + column
                });
            }

            for (var row = 0; row < BoardEdgeSize; ++row)
            {
                _board.RowDefinitions.Add(new RowDefinition
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

                    var field = new Field(x, y, FieldShapes.Circle);

                    Grid.SetColumn(field.GetElement(), x);
                    Grid.SetRow(field.GetElement(), y);

                    const uint centerOfTheBoard = BoardEdgeSize / 2;
                    if (x == centerOfTheBoard && y == centerOfTheBoard)
                    {
                        field.State = FieldState.Player;
                        _playerPositionHistory.Push(new Point(x, y));
                    }

                    _fields[x, y] = field;
                    _board.Children.Add(field.GetElement());
                }
            }

            _board.MouseDown += OnClick;

            UpdateBoard();
        }

        private void UpdateBoard()
        {
            var playerPosition = _playerPositionHistory.Peek();

            var x = playerPosition.X;
            var y = playerPosition.Y;
            
            _fields[x, y]!.State = FieldState.Player;

            if (IsFieldLegal(x + 2, y)) _fields[x + 2, y]!.State = FieldState.Available;
            if (IsFieldLegal(x - 2, y)) _fields[x - 2, y]!.State = FieldState.Available;
            if (IsFieldLegal(x, y - 2)) _fields[x, y - 2]!.State = FieldState.Available;
            if (IsFieldLegal(x, y + 2)) _fields[x, y + 2]!.State = FieldState.Available;
        }

        private void OnClick(object sender, MouseButtonEventArgs e)
        {
            foreach (var field in _fields)
            {
                if (field == null || !field.IsSelected())
                {
                    continue;
                }

                if (field.State != FieldState.Available)
                {
                    break;
                }

                var fieldCoordinates = field.GetLocation();
                _playerPositionHistory.Push(fieldCoordinates);

                UpdateBoard();
            }
        }

        private static bool IsFieldLegal(int x, int y)
        {
            if (x < 0 || y < 0) return false;
            if (x >= BoardEdgeSize || y >= BoardEdgeSize) return false;

            const int illegalCornerEdgeSize = 2;
            var atTopLeftCorner = x < illegalCornerEdgeSize && y < illegalCornerEdgeSize;
            var atTopRightCorner = x > BoardEdgeSize - illegalCornerEdgeSize - 1 && y < illegalCornerEdgeSize;
            var atBottomLeftCorner = x < illegalCornerEdgeSize && y > BoardEdgeSize - illegalCornerEdgeSize - 1;
            var atBottomRightCorner = x > BoardEdgeSize - illegalCornerEdgeSize - 1 && y > BoardEdgeSize - illegalCornerEdgeSize - 1;

            return !atTopLeftCorner && !atTopRightCorner && !atBottomLeftCorner && !atBottomRightCorner;
        }
    }
}
