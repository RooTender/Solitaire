using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Point = System.Drawing.Point;

namespace Solitaire.Game
{
    public enum FieldShapes
    {
        Square = 0,
        Circle
    }

    public class Field
    {
        private readonly Point _location;
        private readonly Shape _shape;
        private readonly SolidColorBrush _playerColor;
        private bool _isPlayer;
        
        public Field(int x, int y, FieldShapes shape)
        {
            _location = new Point(x, y);
            _shape = shape switch
            {
                FieldShapes.Square => new Rectangle(),
                FieldShapes.Circle => new Ellipse(),
                _ => throw new ArgumentOutOfRangeException(nameof(shape), shape, "The given shape is undefined!")
            };
            _playerColor = Brushes.Crimson;
            _isPlayer = false;

            _shape.Fill = Brushes.Black;

            _shape.MouseEnter += OnMouseEnter;
            _shape.MouseLeave += OnMouseLeave;
            _shape.MouseDown  += OnMouseDown;
        }

        public bool IsPlayer
        {
            get => _isPlayer;

            set
            {
                _isPlayer = value;
                _shape.Fill = (_isPlayer) ? _playerColor : Brushes.Black;
            }
        }

        public Shape GetElement() 
        {
            return _shape;
        }

        private void OnMouseDown(object obj, MouseEventArgs e)
        {
            _shape.Fill = (IsPlayer) ? Brushes.DeepPink : Brushes.Gray;
        }

        private void OnMouseEnter(object obj, MouseEventArgs e)
        {
            _shape.Fill = (IsPlayer) ? Brushes.DeepPink : Brushes.Gray;
        }

        /*private void ColorShapeBasedOnSituation()
        {
            if (IsPlayer)
            {

            }
            else if ()
        }*/
        
        private void OnMouseLeave(object obj, MouseEventArgs e)
        {
            _shape.Fill = (IsPlayer) ? _playerColor : Brushes.Black;
        }
    }
}
