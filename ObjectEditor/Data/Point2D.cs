using Emigre.Json;
using System;

namespace Emigre.Data
{
    public class Point2D : DataObject
    {
        public int x, y;

        public Point2D() : this(0, 0) { }

        public Point2D(int x, int y)
        {
            Set(x, y);
        }

        public void Set(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public float Distance(int x, int y)
        {
            return (float)Math.Sqrt(Math.Pow(x - this.x, 2) + Math.Pow(y - this.y, 2));
        }

        public virtual void AddFields(FieldData fields)
        {
            x = fields.add(x, "x");
            y = fields.add(y, "y");
        }

        public Point2D Bound(int xMin, int yMin, int xMax, int yMax)
        {
            x = Math.Min(Math.Max(x, xMin), xMax);
            y = Math.Min(Math.Max(y, yMin), yMax);
            return this;
        }

        public override string ToString()
        {
            return "{" + x + ", " + y + "}";
        }
    }
}
