using System;
using System.Drawing;
using System.Windows.Shapes;

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

        Field(int x, int y, FieldShapes shape)
        {
            _location = new Point(x, y);
            _shape = shape switch
            {
                FieldShapes.Square => new System.Windows.Shapes.Rectangle(),
                FieldShapes.Circle => new Ellipse(),
                _ => throw new ArgumentOutOfRangeException(nameof(shape), shape, "The given shape is undefined!")
            };
        }
    }
}
