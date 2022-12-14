using System;
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

    public enum FieldState
    {
        Occupied = 0,
        Available,
        Marked
    }

    public class Field
    {
        private readonly Point _location;
        private readonly Shape _shape;
        private FieldState _state;
        private bool _isSelected;
        
        public Field(int x, int y, FieldShapes shape)
        {
            _location = new Point(x, y);
            _shape = shape switch
            {
                FieldShapes.Square => new Rectangle(),
                FieldShapes.Circle => new Ellipse(),
                _ => throw new ArgumentOutOfRangeException(nameof(shape), shape, "The given shape is undefined!")
            };
            _state = FieldState.Occupied;

            _shape.MouseEnter += OnMouseEnter;
            _shape.MouseLeave += OnMouseLeave;

            SetFieldColor();
        }

        public Point GetLocation() => _location;

        public bool IsSelected() => _isSelected;

        public FieldState State 
        {
            get => _state;

            set
            {
                _state = value;
                SetFieldColor();
            }
        }

        public Shape GetElement() 
        {
            return _shape;
        }

        private void OnMouseEnter(object sender, MouseEventArgs e)
        {
            _shape.Fill = _state switch
            {
                FieldState.Occupied => Brushes.Gray,
                FieldState.Available => Brushes.WhiteSmoke,
                FieldState.Marked => Brushes.Gold,
                _ => Brushes.Transparent
            };

            _isSelected = true;
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            SetFieldColor();

            _isSelected = false;
        }

        private void SetFieldColor()
        {
            _shape.Fill = _state switch
            {
                FieldState.Occupied => Brushes.Black,
                FieldState.Available => Brushes.WhiteSmoke,
                FieldState.Marked => Brushes.Gold,
                _ => Brushes.Transparent
            };
        }
    }
}
