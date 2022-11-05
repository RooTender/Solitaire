using System;
using System.Collections.Generic;
using System.Linq;
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
        
        private readonly Stack<(Point, Point)> _gameHistory;
        private Point? _currentlyMarkedField;

        public Board()
        {
            _board = new Grid { ContextMenu = new ContextMenu() };
            _board.ContextMenu.Items.Add(new MenuItem { Command = ApplicationCommands.Undo });

            _fields = new Field?[BoardEdgeSize,BoardEdgeSize];
            _gameHistory = new Stack<(Point, Point)>();

            Build();
        }

        public Grid GetBoard() => _board;

        public CommandBinding GetCommandBinding()
        {
            return new CommandBinding(ApplicationCommands.Undo, UndoCommandTrigger);
        }

        public void SetNewGameButtonTrigger(ref Button newGameButton)
        {
            newGameButton.Click += NewGameTrigger;
        }

        public void SetUndoButtonTrigger(ref Button undoButton)
        {
            undoButton.Click += UndoMoveTrigger;
        }

        private void NewGameTrigger(object sender, RoutedEventArgs e)
        {
            while (_gameHistory.Count > 0)
            {
                UndoMove();
            }
        }

        private void UndoMoveTrigger(object sender, RoutedEventArgs e)
        {
            UndoMove();
        }

        private void UndoCommandTrigger(object sender, ExecutedRoutedEventArgs e)
        {
            UndoMove();
        }

        private void UndoMove()
        {
            if (_gameHistory.Count == 0) return;

            foreach (var field in _fields)
            {
                if (field is { State: FieldState.Marked })
                {
                    field.State = FieldState.Occupied;
                }
            }

            var (a, b) = _gameHistory.Peek();

            var midPoint = new Point(
                (a.X + b.X) / 2,
                (a.Y + b.Y) / 2);

            _fields[a.X, a.Y]!.State = FieldState.Occupied;
            _fields[midPoint.X, midPoint.Y]!.State = FieldState.Occupied;
            _fields[b.X, b.Y]!.State = FieldState.Available;

            _gameHistory.Pop();
        }

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
            if (CheckGameState())
            {
                return;
            }

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

        private bool CheckGameState()
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
                    return false;
                }
            }

            if (Application.Current.Windows.Cast<Window>().Any(x => x.Title == "Stats"))
            {
                return true;
            }

            var statsWindow = new Window
            {
                Title = "Stats",
                Width = 640,
                Height = 480,
                ResizeMode = ResizeMode.CanMinimize
            };
            
            if (pawns == 1)
            {
                // Win
                statsWindow.Content = new UserControl1
                {
                    StatusText = { Text = "You won!!!" },
                    DescriptionText = { Text = $"Left pawns: {pawns}" }
                };
            }
            else
            {
                // Lose
                statsWindow.Content = new UserControl1
                {
                    StatusText = { Text = "You lost..." },
                    DescriptionText = { Text = $"Left pawns: {pawns}" }
                };
            }

            statsWindow.Show();
            return true;
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
