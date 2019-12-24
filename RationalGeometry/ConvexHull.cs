using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace Sunlighter.RationalGeometry
{
    public enum PointStatus
    {
        Outside,
        OnCorner,
        OnEdge,
        OnFace,
        Inside
    }

    public abstract class ConvexHull
    {
        public abstract PointStatus GetPointStatus(Vertex3 vt);

        public abstract ConvexHull Add(Vertex3 vt);

        private static ConvexHull _empty = new CH_Empty();

        public static ConvexHull Empty { get { return _empty; } }
    }

    public class CH_Empty : ConvexHull
    {
        public CH_Empty() { }

        public override PointStatus GetPointStatus(Vertex3 vt)
        {
            return PointStatus.Outside;
        }

        public override ConvexHull Add(Vertex3 vt)
        {
            return new CH_SinglePoint(vt);
        }
    }

    public class CH_SinglePoint : ConvexHull
    {
        private Vertex3 vertex;

        public CH_SinglePoint(Vertex3 vertex)
        {
            this.vertex = vertex;
        }

        public Vertex3 Vertex { get { return vertex; } }

        public override PointStatus GetPointStatus(Vertex3 vt)
        {
            if (vt == vertex) return PointStatus.OnCorner;
            else return PointStatus.Outside;
        }

        public override ConvexHull Add(Vertex3 vt)
        {
            if (vt == vertex) return this;
            return new CH_LineSegment(vertex, vt);
        }
    }

    public class CH_LineSegment : ConvexHull
    {
        private Vertex3 v1;
        private Vertex3 v2;

        public CH_LineSegment(Vertex3 v1, Vertex3 v2)
        {
            this.v1 = v1;
            this.v2 = v2;
        }

        public Vertex3 Vertex1 { get { return v1; } }
        public Vertex3 Vertex2 { get { return v2; } }

        public Line3 Line { get { return Line3.FromTwoPoints(v1, v2); } }

        public override PointStatus GetPointStatus(Vertex3 vt)
        {
            if (v1 == vt || v2 == vt)
            {
                return PointStatus.OnCorner;
            }
            else if (this.Line.Contains(vt))
            {
                BigRational scaledPos = (vt - v1).ScaledLengthAlong(v2 - v1);
                if (scaledPos < BigRational.Zero || scaledPos > BigRational.One)
                {
                    return PointStatus.Outside;
                }
                else
                {
                    return PointStatus.OnEdge;
                }
            }
            else
            {
                return PointStatus.Outside;
            }
        }

        public override ConvexHull Add(Vertex3 vt)
        {
            if (this.Line.Contains(vt))
            {
                BigRational scaledPos = (vt - v1).ScaledLengthAlong(v2 - v1);
                if (scaledPos < BigRational.Zero)
                {
                    return new CH_LineSegment(vt, v2);
                }
                else if (scaledPos > BigRational.One)
                {
                    return new CH_LineSegment(v1, vt);
                }
                else return this;
            }
            else
            {
                return new CH_Polygon(new Vertex3[] { v1, v2, vt }.ToImmutableList());
            }
        }
    }

    public class CH_Polygon : ConvexHull
    {
        private ImmutableList<Vertex3> vertices;
        private Vector3 normal;

        public CH_Polygon(ImmutableList<Vertex3> vertices)
        {
            System.Diagnostics.Debug.Assert(vertices.Count >= 3);

            this.vertices = vertices;
            this.normal = (vertices[1] - vertices[0]).Cross(vertices[2] - vertices[1]);

            CheckIntegrity();
        }

        private void CheckIntegrity()
        {
            int iEnd = vertices.Count;
            foreach (Edge e in this.Edges)
            {
                for (int i = 0; i < iEnd; ++i)
                {
                    if (i == e.StartIndex || i == e.EndIndex) continue;
                    if (!(e.Plane.Includes(vertices[i]))) throw new ArgumentException("Invalid polygon");
                }
            }
        }

        public int Count { get { return vertices.Count; } }

        public Plane3 Plane
        {
            get
            {
                return new Plane3(vertices[0], normal);
            }
        }

        public Vector3 Normal { get { return normal; } }

        public ImmutableList<Vertex3> Vertices { get { return vertices; } }

        public CH_Polygon Flip()
        {
            int iEnd = vertices.Count;
            ImmutableList<Vertex3> v2 = ImmutableList<Vertex3>.Empty;
            for (int i = 0; i < iEnd; ++i) v2 = v2.Add(vertices[iEnd - i - 1]);
            return new CH_Polygon(v2);
        }

        public CH_Polygon FlipToInclude(Vertex3 v)
        {
            if (this.Plane.Contains(v)) throw new ArgumentException("Polygon cannot be flipped to include a point that lies in the polygon's plane");
            if (this.Plane.Includes(v)) return this;
            else return this.Flip();
        }

        public class Edge
        {
            private CH_Polygon parent;
            private int startIndex;

            public Edge(CH_Polygon parent, int startIndex)
            {
                this.parent = parent;
                this.startIndex = startIndex;
            }

            public int StartIndex { get { return startIndex; } }
            public Vertex3 Start { get { return parent.vertices[startIndex]; } }
            public int EndIndex { get { int e = startIndex + 1; int end = parent.vertices.Count; return (e >= end) ? 0 : e; } }
            public Vertex3 End { get { return parent.vertices[EndIndex]; } }
            public int BeyondIndex { get { int b = startIndex + 2; int end = parent.vertices.Count; return (b >= end) ? (b - end) : b; } }
            public Vertex3 Beyond { get { return parent.vertices[BeyondIndex]; } }

            public Line3 Line { get { return Line3.FromTwoPoints(Start, End); } }
            public Plane3 Plane { get { return Plane3.FromFourPoints(Start, End, Start + parent.normal, Beyond); } }
        }

        public ImmutableList<Edge> Edges
        {
            get
            {
                return Enumerable.Range(0, vertices.Count).Select(index => new Edge(this, index)).ToImmutableList();
            }
        }

        public override PointStatus GetPointStatus(Vertex3 vt)
        {
            if (Plane.Contains(vt))
            {
                foreach (Edge e in Edges)
                {
                    if (e.Plane.Excludes(vt)) return PointStatus.Outside;
                    if (e.Start == vt || e.End == vt) return PointStatus.OnCorner;
                    if (e.Line.Contains(vt))
                    {
                        BigRational b = e.Line.ScaledCoordinateOf(vt);
                        if (b < BigRational.Zero || b > BigRational.One) return PointStatus.Outside;
                        else return PointStatus.OnEdge;
                    }
                }
                return PointStatus.OnFace;
            }
            else
            {
                return PointStatus.Outside;
            }
        }

        public override ConvexHull Add(Vertex3 vt)
        {
            if (Plane.Contains(vt))
            {
                if (!Edges.Any(x => x.Plane.Excludes(vt))) return this;

                ImmutableList<Edge> d = Edges;
                int sentryCount = d.Count;
                while (true)
                {
                    if (!(d[0].Plane.Includes(vt)) && d[d.Count - 1].Plane.Includes(vt)) break;
                    Edge e = d[0];
                    d = d.Add(e);
                    --sentryCount;
                    if (sentryCount < 0) throw new InvalidOperationException("Bug in convex hull routine would have caused an infinite loop");
                }
                while (!(d[0].Plane.Includes(vt)))
                {
                    d = d.RemoveAt(0);
                }
                ImmutableList<Vertex3> vertexlist = ImmutableList<Vertex3>.Empty
                    .Add(vt)
                    .AddRange(d.Select(x => x.Start))
                    .Add(d[d.Count - 1].End);
                return new CH_Polygon(vertexlist);
            }
            else
            {
                return CH_Polyhedron.Make(ImmutableList<CH_Polygon>.Empty.Add(FlipToInclude(vt)), vt);
            }
        }
    }

    public class CH_Polyhedron : ConvexHull
    {
        private ImmutableList<CH_Polygon> faces;

        public CH_Polyhedron(ImmutableList<CH_Polygon> faces)
        {
            this.faces = faces;
            CheckIntegrity();
        }

        private void CheckIntegrity()
        {
            int iEnd = faces.Count;
            DualIndexedSet<Tuple<Vertex3, Vertex3>> edgeMap = new DualIndexedSet<Tuple<Vertex3, Vertex3>>(x => new Tuple<Vertex3, Vertex3>(x.Item2, x.Item1));
            Dictionary<int, int> edgeToFace = new Dictionary<int, int>();
            for (int i = 0; i < iEnd; ++i)
            {
                foreach (CH_Polygon.Edge edge in faces[i].Edges)
                {
                    var (edgeMap2, eIndex, isNew) = edgeMap.EnsureAdded(new Tuple<Vertex3, Vertex3>(edge.Start, edge.End));
                    edgeMap = edgeMap2;
                    if (edgeToFace.ContainsKey(eIndex)) throw new ArgumentException("Invalid polyhedron (edge traversed in same direction by more than one face)");
                    edgeToFace.Add(eIndex, i);
                }
                for (int j = 0; j < iEnd; ++j)
                {
                    if (i == j) continue;
                    if (faces[i].Plane.IsCoincidentWith(faces[j].Plane)) throw new ArgumentException("Invalid polyhedron (two faces have the same plane)");
                    if (faces[j].Vertices.Any(x => faces[i].Plane.Excludes(x))) throw new ArgumentException("Invalid polyhedron (one face cuts another)");
                }
            }
            int iEnd2 = edgeMap.Count;
            for (int i = 0; i < iEnd2; ++i)
            {
                if (!(edgeToFace.ContainsKey(i)) || !(edgeToFace.ContainsKey(~i)))
                {
                    throw new ArgumentException("Invalid polyhedron (loose edge)");
                }
            }
        }

        public ImmutableList<CH_Polygon> Faces { get { return faces; } }

        public override PointStatus GetPointStatus(Vertex3 vt)
        {
            if (faces.Any(x => x.Plane.Excludes(vt))) return PointStatus.Outside;
            foreach (CH_Polygon f in faces)
            {
                PointStatus ps = f.GetPointStatus(vt);
                if (ps != PointStatus.Outside) return ps;
            }
            return PointStatus.Inside;
        }

        public static CH_Polyhedron Make(ImmutableList<CH_Polygon> facesAway, Vertex3 v)
        {
            ImmutableList<CH_Polygon> facesAway1 = facesAway;
            foreach (CH_Polygon face in facesAway1)
            {
                if (!(face.Plane.Includes(v))) throw new ArgumentException("Polygons must face away from new point");
            }

            DualIndexedSet<Tuple<Vertex3, Vertex3>> edgeMap = new DualIndexedSet<Tuple<Vertex3, Vertex3>>(x => new Tuple<Vertex3, Vertex3>(x.Item2, x.Item1));
            ImmutableDictionary<int, int> edgeToFace = ImmutableDictionary<int, int>.Empty;
            int iEnd = facesAway1.Count;
            for (int i = 0; i < iEnd; ++i)
            {
                foreach (CH_Polygon.Edge edge in facesAway1[i].Edges)
                {
                    var (edgeMap2, eIndex, isNew) = edgeMap.EnsureAdded(new Tuple<Vertex3, Vertex3>(edge.Start, edge.End));
                    edgeMap = edgeMap2;
                    if (edgeToFace.ContainsKey(eIndex)) throw new ArgumentException("Invalid polyhedron (edge traversed in same direction by more than one face)");
                    edgeToFace = edgeToFace.Add(eIndex, i);
                }
            }
            ImmutableDictionary<Vertex3, int> startToEdge = ImmutableDictionary<Vertex3, int>.Empty;
            int iEnd2 = edgeMap.Count;
            for (int i = 0; i < iEnd2; ++i)
            {
                if (!(edgeToFace.ContainsKey(i))) startToEdge = startToEdge.Add(edgeMap[i].Item1, i);
                if (!(edgeToFace.ContainsKey(~i))) startToEdge = startToEdge.Add(edgeMap[~i].Item1, ~i);
            }

            ImmutableList<int> edges = ImmutableList<int>.Empty;
            edges = edges.Add(startToEdge.First().Value);
            int sentinel = startToEdge.Count;
            while (true)
            {
                Vertex3 end = edgeMap[edges[edges.Count - 1]].Item2;
                if (!(startToEdge.ContainsKey(end))) throw new ArgumentException("Unable to walk loose edges");
                int nextEdge = startToEdge[end];
                if (nextEdge == edges[0]) break;
                edges = edges.Add(nextEdge);
                --sentinel;
                if (sentinel < 0) throw new InvalidOperationException("Loose edges don't loop properly");
            }
            if (edges.Count != startToEdge.Count) throw new InvalidOperationException("Not all loose edges were used");

            sentinel = edges.Count;
            Func<int, int, bool> inSamePlane = delegate (int edge1, int edge2)
            {
                Tuple<Vertex3, Vertex3> e1 = edgeMap[edge1];
                Tuple<Vertex3, Vertex3> e2 = edgeMap[edge2];
                Plane3 p = Plane3.FromThreePoints(e1.Item1, e1.Item2, v);
                return p.Contains(e2.Item1) && p.Contains(e2.Item2);
            };

            while (true)
            {
                if (!inSamePlane(edges[0], edges[edges.Count - 1])) break;
                int x = edges[0];
                edges = edges.RemoveAt(0).Add(x);
                --sentinel;
                if (sentinel < 0) throw new InvalidOperationException("Loose edges all lie in the same plane as the vertex (?!)");
            }

            ImmutableList<CH_Polygon> newPolys = ImmutableList<CH_Polygon>.Empty;
            ImmutableList<Vertex3> corners = ImmutableList<Vertex3>.Empty;
            int? lastEdge = null;

            Action flush = delegate ()
            {
                corners = corners.Add(edgeMap[lastEdge.Value].Item2).Add(v);
                newPolys = newPolys.Add(new CH_Polygon(corners));
            };

            Action<int> addEdge = delegate (int edge)
            {
                if (lastEdge == null || inSamePlane(edge, lastEdge.Value))
                {
                    corners = corners.Add(edgeMap[edge].Item1);
                    lastEdge = edge;
                }
                else
                {
                    flush();
                    corners = ImmutableList<Vertex3>.Empty
                        .Add(edgeMap[edge].Item1);
                    lastEdge = edge;
                }
            };

            foreach (int edge in edges) addEdge(edge);
            flush();

            return new CH_Polyhedron(newPolys.AddRange(facesAway1));
        }

        public override ConvexHull Add(Vertex3 vt)
        {
            if (GetPointStatus(vt) != PointStatus.Outside) return this;

            return Make(faces.Where(x => x.Plane.Includes(vt)).ToImmutableList(), vt);
        }
    }
}
