﻿using System;
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
        
        public Field(int x, int y, FieldShapes shape)
        {
            _location = new Point(x, y);
            _shape = shape switch
            {
                FieldShapes.Square => new Rectangle(),
                FieldShapes.Circle => new Ellipse(),
                _ => throw new ArgumentOutOfRangeException(nameof(shape), shape, "The given shape is undefined!")
            };
            _shape.Fill = Brushes.Black;

            _shape.MouseEnter += OnMouseEnter;
            _shape.MouseLeave += OnMouseLeave;

            IsPlayer = false;
        }

        public bool IsPlayer { get; set; }

        public Shape GetElement() 
        {
            return _shape;
        }

        private void OnMouseEnter(object obj, MouseEventArgs e)
        {
            _shape.Fill = Brushes.Gray;
        }

        private void OnMouseLeave(object obj, MouseEventArgs e)
        {
            _shape.Fill = Brushes.Black;
        }
    }
}
