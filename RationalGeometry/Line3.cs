using System;
using System.Collections.Generic;
using System.Text;

namespace Sunlighter.RationalGeometry
{
    public class Line3
    {
        private readonly Vertex3 origin;
        private readonly Vector3 direction;

        public Line3(Vertex3 origin, Vector3 direction)
        {
            this.origin = origin;
            this.direction = direction;
        }

        public Vertex3 Origin => origin;
        public Vector3 Direction => direction;

        public bool Contains(Vertex3 pt)
        {
            return direction.IsParallelTo(pt - origin);
        }

        public bool IsParallelTo(Line3 line)
        {
            return direction.IsParallelTo(line.direction);
        }

        public bool IsParallelTo(Plane3 plane)
        {
            return direction.IsPerpendicularTo(plane.Normal);
        }

        public bool Intersects(Line3 line)
        {
            if (this.IsParallelTo(line)) return false;
            return new Plane3(this.origin, this.direction.Cross(line.direction)).Contains(line.origin);
        }

        public bool IsCoincidentWith(Line3 line)
        {
            return this.IsParallelTo(line) && this.Contains(line.Origin);
        }

        public Vertex3 NearestPointTo(Vertex3 pt)
        {
            return origin + ((pt - origin).ComponentAlong(direction));
        }

        public BigRational ScaledCoordinateOf(Vertex3 pt)
        {
            return (pt - origin).ScaledLengthAlong(direction);
        }

        public static Line3 FromTwoPoints(Vertex3 a, Vertex3 b)
        {
            return new Line3(a, b - a);
        }
    }
}
