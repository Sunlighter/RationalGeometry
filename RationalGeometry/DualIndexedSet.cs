using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace Sunlighter.RationalGeometry
{
    public class DualIndexedSet<T>
    {
        private readonly ImmutableList<T> items;
        private readonly ImmutableDictionary<T, int> indices;
        private readonly Func<T, T> getDual;

        public DualIndexedSet(Func<T, T> getDual)
        {
            items = ImmutableList<T>.Empty;
            indices = ImmutableDictionary<T, int>.Empty;
            this.getDual = getDual;
        }

        private DualIndexedSet(ImmutableList<T> items, ImmutableDictionary<T, int> indices, Func<T, T> getDual)
        {
            this.items = items;
            this.indices = indices;
            this.getDual = getDual;
        }

        public int Count { get { return items.Count; } }

        public bool Contains(T item) { return indices.ContainsKey(item) || indices.ContainsKey(getDual(item)); }

        public int IndexOf(T item)
        {
            if (indices.ContainsKey(item)) return indices[item];
            else if (indices.ContainsKey(getDual(item))) return ~indices[item];
            else throw new KeyNotFoundException();
        }

        public T this[int index]
        {
            get
            {
                if (index < 0) return getDual(items[~index]);
                else return items[index];
            }
        }

        public ValueTuple<DualIndexedSet<T>, int, bool> EnsureAdded(T item)
        {
            if (indices.ContainsKey(item)) return (this, indices[item], false);
            else
            {
                T dual = getDual(item);
                if (indices.ContainsKey(dual)) return (this, ~indices[dual], false);
                else
                {
                    int index = items.Count;
                    ImmutableList<T> items2 = items.Add(item);
                    ImmutableDictionary<T, int> indices2 = indices.Add(item, index);
                    return (new DualIndexedSet<T>(items2, indices2, getDual), index, true);
                }
            }
        }
    }
}
