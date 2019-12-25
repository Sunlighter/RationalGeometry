using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Sunlighter.RationalGeometry
{
    public abstract class IntersectionResult
    {

    }

    public enum IntersectionFailedReason
    {
        Coincident,
        Parallel,
        Skew
    }

    public class IntersectionFailed : IntersectionResult
    {
        private readonly IntersectionFailedReason reason;

        public IntersectionFailed(IntersectionFailedReason reason)
        {
            this.reason = reason;
        }

    }

    public class IntersectionVertex3 : IntersectionResult
    {
        private readonly Vertex3 value;

        public IntersectionVertex3(Vertex3 value)
        {
            this.value = value;
        }

        public Vertex3 Value { get { return value; } }
    }

    public class IntersectionLine3 : IntersectionResult
    {
        private readonly Line3 value;

        public IntersectionLine3(Line3 value)
        {
            this.value = value;
        }

        public Line3 Value { get { return value; } }
    }

    public static class Geometry
    {
        //  (y3 - y1)   (x3 - x1)
        //  --------- = ---------
        //  (y2 - y1)   (y2 - y1)

        //                        (x3 - x1)
        //  y3 = y1 + (y2 - y1) * ---------
        //                        (x2 - x1)

        public static BigRational Interpolate(BigRational x1, BigRational y1, BigRational x2, BigRational y2, BigRational x3)
        {
            return y1 + (y2 - y1) * (x3 - x1) / (x2 - x1);
        }

        public static Vertex2 Interpolate(BigRational x1, Vertex2 y1, BigRational x2, Vertex2 y2, BigRational x3)
        {
            return y1 + (y2 - y1) * (x3 - x1) / (x2 - x1);
        }

        public static Vector2 Interpolate(BigRational x1, Vector2 y1, BigRational x2, Vector2 y2, BigRational x3)
        {
            return y1 + (y2 - y1) * (x3 - x1) / (x2 - x1);
        }

        public static Vertex3 Interpolate(BigRational x1, Vertex3 y1, BigRational x2, Vertex3 y2, BigRational x3)
        {
            return y1 + (y2 - y1) * (x3 - x1) / (x2 - x1);
        }

        public static Vector3 Interpolate(BigRational x1, Vector3 y1, BigRational x2, Vector3 y2, BigRational x3)
        {
            return y1 + (y2 - y1) * (x3 - x1) / (x2 - x1);
        }

        public static IntersectionResult Intersect(Line3 line1, Line3 line2)
        {
            if (line1.IsCoincidentWith(line2))
            {
                return new IntersectionFailed(IntersectionFailedReason.Coincident);
            }
            else if (line1.IsParallelTo(line2))
            {
                return new IntersectionFailed(IntersectionFailedReason.Parallel);
            }
            else if (!(line1.Intersects(line2)))
            {
                return new IntersectionFailed(IntersectionFailedReason.Skew);
            }
            else
            {
                Vector3 convergence = line2.Direction.ComponentOrtho(line1.Direction);
                //Vector3 run = line2.Direction.ComponentAlong(line1.Direction);
                Vector3 p1 = line2.Origin - line1.Origin;
                Vector3 p2 = line2.Origin + line2.Direction - line1.Origin;
                return new IntersectionVertex3
                (
                    Interpolate(p1.ScaledLengthAlong(convergence), line2.Origin, p2.ScaledLengthAlong(convergence), line2.Origin + line2.Direction, BigRational.Zero)
                );
            }
        }

        public static IntersectionResult Intersect(Line3 line, Plane3 plane)
        {
            if (plane.Contains(line))
            {
                return new IntersectionFailed(IntersectionFailedReason.Coincident);
            }
            else if (plane.IsParallelTo(line))
            {
                return new IntersectionFailed(IntersectionFailedReason.Parallel);
            }
            else
            {
                Vector3 convergence = line.Direction.ComponentAlong(plane.Normal);
                //Vector3 run = line.Direction.ComponentOrtho(plane.Normal);
                Vector3 p1 = line.Origin - plane.Origin;
                Vector3 p2 = (line.Origin + line.Direction) - plane.Origin;
                return new IntersectionVertex3
                (
                    Interpolate(p1.ScaledLengthAlong(convergence), line.Origin, p2.ScaledLengthAlong(convergence), line.Origin + line.Direction, BigRational.Zero)
                );
            }
        }

        public static IntersectionResult Intersect(Plane3 plane1, Plane3 plane2)
        {
            if (plane1.IsCoincidentWith(plane2))
            {
                return new IntersectionFailed(IntersectionFailedReason.Coincident);
            }
            else if (plane1.IsParallelTo(plane2))
            {
                return new IntersectionFailed(IntersectionFailedReason.Parallel);
            }
            else
            {
                Vector3 binormal = plane1.Normal.Cross(plane2.Normal);
                Vertex3 origin1 = plane1.Origin;
                Vertex3 origin2 = new Plane3(origin1, binormal).NearestPointTo(plane2.Origin);

                Line3 line1 = new Line3(origin1, plane1.Normal);
                Line3 line2 = new Line3(origin2, plane2.Normal);

                Vector3 convergence = line2.Direction.ComponentOrtho(line1.Direction);
                Vector3 run = line2.Direction.ComponentAlong(line1.Direction);
                Vector3 p1 = line2.Origin - line1.Origin;
                Vector3 p2 = line2.Origin + line2.Direction - line1.Origin;

                Vertex3 isect = Interpolate(p1.ScaledLengthAlong(convergence), line2.Origin, p2.ScaledLengthAlong(convergence), line2.Origin + line2.Direction, BigRational.Zero);

                return new IntersectionLine3(new Line3(isect, binormal));
            }
        }
    }
}
