using System;
using System.Collections.Generic;
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

        private Window _statsWindow;
        private Stack<(Point, Point)> _gameHistory;
        private Point? _currentlyMarkedField;

        public Board()
        {
            _board = new Grid();
            _fields = new Field?[BoardEdgeSize,BoardEdgeSize];
            _gameHistory = new Stack<(Point, Point)>();
            _statsWindow = new Window
            {
                Width = 640,
                Height = 480,
                ResizeMode = ResizeMode.CanMinimize
            };

            Build();
        }

        public Grid GetBoard() => _board;

        private void Build()
        {
            for (var column = 0; column < BoardEdgeSize; ++column)
            {
                _board.ColumnDefinitions.Add(new ColumnDefinition());
                _board.RowDefinitions.Add(new RowDefinition());
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
                        field.State = FieldState.Available;
                    }

                    _fields[x, y] = field;
                    _board.Children.Add(field.GetElement());
                }
            }

            _board.MouseDown += OnClick;
        }

        private void OnClick(object sender, MouseButtonEventArgs e)
        {
            foreach (var field in _fields)
            {
                if (field == null || !field.IsSelected())
                {
                    continue;
                }

                if (_currentlyMarkedField != null && field.State == FieldState.Occupied)
                {
                    continue;
                }

                if (_currentlyMarkedField == null && field.State == FieldState.Occupied)
                {
                    _currentlyMarkedField = field.GetLocation();
                    field.State = FieldState.Marked;

                    return;
                }

                if (_currentlyMarkedField == field.GetLocation() && field.State == FieldState.Marked)
                {
                    _currentlyMarkedField = null;
                    field.State = FieldState.Occupied;

                    return;
                }

                if (TryToMove(_currentlyMarkedField, field.GetLocation()))
                {
                    CheckGameState();
                }
            }
        }

        private void CheckGameState()
        {
            var pawns = 0;
            foreach (var field in _fields)
            {
                if (field == null || field.State == FieldState.Available)
                {
                    continue;
                }

                ++pawns;

                if (HasLegalMove(field))
                {
                    return;
                }
            }

            if (pawns == 1)
            {
                // Win
                _statsWindow.Content = new UserControl1
                {
                    StatusText = { Text = "You won!!!" },
                    DescriptionText = { Text = $"Left pawns: {pawns}" }
                };
                _statsWindow.Show();
            }

            // Lose
            _statsWindow.Content = new UserControl1
            {
                StatusText = { Text = "You lost..." },
                DescriptionText = { Text = $"Left pawns: {pawns}" }
            };

            _statsWindow.Show();
        }

        private bool HasLegalMove(Field field)
        {
            var x = field.GetLocation().X;
            var y = field.GetLocation().Y;

            var isMoveUpLegal =
                IsFieldLegal(x, y - 1) && _fields[x, y - 1]!.State != FieldState.Available &&
                IsFieldLegal(x, y - 2) && _fields[x, y - 2]!.State == FieldState.Available;

            var isMoveDownLegal =
                IsFieldLegal(x, y + 1) && _fields[x, y + 1]!.State != FieldState.Available &&
                IsFieldLegal(x, y + 2) && _fields[x, y + 2]!.State == FieldState.Available;

            var isMoveLeftLegal =
                IsFieldLegal(x - 1, y) && _fields[x - 1, y]!.State != FieldState.Available &&
                IsFieldLegal(x - 2, y) && _fields[x - 2, y]!.State == FieldState.Available;

            var isMoveRightLegal =
                IsFieldLegal(x + 1, y) && _fields[x + 1, y]!.State != FieldState.Available &&
                IsFieldLegal(x + 2, y) && _fields[x + 2, y]!.State == FieldState.Available;

            return isMoveUpLegal || isMoveDownLegal || isMoveLeftLegal || isMoveRightLegal;
        }

        private bool TryToMove(Point? currentLocation, Point nextLocation)
        {
            if (currentLocation == null)
            {
                return false;
            }

            var current = currentLocation.Value;
            var middle = new Point((current.X + nextLocation.X) / 2, (current.Y + nextLocation.Y) / 2);

            if (Math.Abs(current.X - nextLocation.X) > 2 || Math.Abs(current.Y - nextLocation.Y) > 2)
            {
                return false;
            }

            var currentField = _fields[current.X, current.Y];
            var middleField = _fields[middle.X, middle.Y];
            var finalField = _fields[nextLocation.X, nextLocation.Y];

            if (currentField == null || middleField == null || finalField == null)
            {
                return false;
            }

            if (middleField.State != FieldState.Occupied || finalField.State != FieldState.Available)
            {
                return false;
            }

            _currentlyMarkedField = null;

            currentField.State = FieldState.Available;
            middleField.State = FieldState.Available;
            finalField.State = FieldState.Occupied;

            _gameHistory.Push((currentField.GetLocation(), finalField.GetLocation()));

            return true;
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
