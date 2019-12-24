using System;
using System.Collections.Generic;
using System.Text;

namespace Sunlighter.RationalGeometry
{
    public class Plane3
    {
        private Vertex3 origin;
        private Vector3 normal;

        public Plane3(Vertex3 origin, Vector3 normal)
        {
            this.origin = origin;
            this.normal = normal;
        }

        public Vertex3 Origin { get { return origin; } }
        public Vector3 Normal { get { return normal; } }

        public Plane3 Flip()
        {
            return new Plane3(origin, -normal);
        }

        public bool Contains(Vertex3 pt)
        {
            return normal.IsPerpendicularTo(pt - origin);
        }

        public bool IsParallelTo(Line3 line)
        {
            return line.Direction.IsPerpendicularTo(normal);
        }

        public bool IsParallelTo(Plane3 plane)
        {
            return plane.normal.IsParallelTo(normal);
        }

        public bool IsPerpendicularTo(Line3 line)
        {
            return line.Direction.IsParallelTo(normal);
        }

        public bool Contains(Line3 line)
        {
            return this.IsParallelTo(line) && this.Contains(line.Origin);
        }

        public bool IsCoincidentWith(Plane3 plane)
        {
            return this.IsParallelTo(plane) && this.Contains(plane.Origin);
        }

        public Vertex3 NearestPointTo(Vertex3 pt)
        {
            return origin + ((pt - origin).ComponentOrtho(normal));
        }

        public BigRational ScaledDistanceTo(Vertex3 pt)
        {
            return (pt - origin).ScaledLengthAlong(normal);
        }

        public bool Includes(Vertex3 pt)
        {
            return this.ScaledDistanceTo(pt) < BigRational.Zero;
        }

        public bool Excludes(Vertex3 pt)
        {
            return this.ScaledDistanceTo(pt) > BigRational.Zero;
        }

        public Plane3 FlipToInclude(Vertex3 pt)
        {
            if (this.Includes(pt)) return this; else return this.Flip();
        }

        public static Plane3 FromTwoPoints(Vertex3 a, Vertex3 b)
        {
            return new Plane3(a, b - a);
        }

        public static Plane3 FromThreePoints(Vertex3 a, Vertex3 b, Vertex3 c)
        {
            Vector3 normal = (b - a).Cross(c - a);
            if (normal == Vector3.Zero) throw new Exception("Points are colinear");
            return new Plane3(a, (b - a).Cross(c - a));
        }

        public static Plane3 FromFourPoints(Vertex3 a, Vertex3 b, Vertex3 c, Vertex3 keep)
        {
            Plane3 p = Plane3.FromThreePoints(a, b, c);
            if (p.Contains(keep))
            {
                throw new Exception("Fourth point lies in the plane of the first three");
            }
            else
            {
                return p.FlipToInclude(keep);
            }
        }
    }
}
