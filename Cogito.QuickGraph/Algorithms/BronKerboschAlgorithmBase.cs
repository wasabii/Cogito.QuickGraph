using System;
using System.Collections.Generic;
using System.Linq;

using Cogito.Collections;

using QuickGraph;
using QuickGraph.Algorithms.Services;

namespace Cogito.QuickGraph.Algorithms
{

    /// <summary>
    /// Implements the standard form of the Bron-Kerbosch algorithm.
    /// </summary>
    /// <typeparam name="TVertex"></typeparam>
    /// <typeparam name="TEdge"></typeparam>
    public abstract class BronKerboschAlgorithmBase<TVertex, TEdge> :
        global::QuickGraph.Algorithms.Cliques.MaximumCliqueAlgorithmBase<TVertex, TEdge>
        where TEdge : IEdge<TVertex>
    {

        /// <summary>
        /// Compares the order of the given vertices for the degeneracy.
        /// </summary>
        struct DegeneracyOrderComparator : IComparer<TVertex>
        {

            readonly Func<TVertex, ISet<TVertex>> neighbours;

            /// <summary>
            /// Initializes a new instance.
            /// </summary>
            /// <param name="neighbours"></param>
            public DegeneracyOrderComparator(Func<TVertex, ISet<TVertex>> neighbours)
            {
                this.neighbours = neighbours ?? throw new ArgumentNullException(nameof(neighbours));
            }

            public int Compare(TVertex u, TVertex v)
            {
                return neighbours(u).Count - neighbours(v).Count;
            }

        }

        readonly Dictionary<TVertex, HashSet<TVertex>> adjacentVerticesCache = new Dictionary<TVertex, HashSet<TVertex>>();
        ISet<IUndirectedGraph<TVertex, IEdge<TVertex>>> cliques;

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="visitedGraph"></param>
        public BronKerboschAlgorithmBase(IUndirectedGraph<TVertex, TEdge> visitedGraph) :
            base(visitedGraph)
        {

        }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="host"></param>
        /// <param name="visitedGraph"></param>
        public BronKerboschAlgorithmBase(IAlgorithmComponent host, IUndirectedGraph<TVertex, TEdge> visitedGraph) :
            base(host, visitedGraph)
        {

        }

        /// <summary>
        /// Gets the discovered set of maximal cliques.
        /// </summary>
        public ISet<IUndirectedGraph<TVertex, IEdge<TVertex>>> MaximalCliques => cliques;

        /// <summary>
        /// Computes the maximal set of cliques.
        /// </summary>
        protected override void InternalCompute()
        {
            var l = new HashSet<IUndirectedGraph<TVertex, IEdge<TVertex>>>();
            var r = ComputeInternal();

            foreach (var i in r)
            {
                var n = i;

                // gets the edges for the given vertex
                bool TryGetEdges(TVertex v, out IEnumerable<IEdge<TVertex>> edges)
                {
                    edges = n.Where(j => !Equals(v, j)).Select(j => (IEdge<TVertex>)new SEquatableUndirectedEdge<TVertex>(v, j));
                    return true;
                }

                // generate new graph which enumerates a fully connected graph
                var g = new DelegateUndirectedGraph<TVertex, IEdge<TVertex>>(n, TryGetEdges, false);
                l.Add(g);
            }

            // set as result
            cliques = l;
        }

        /// <summary>
        /// Invokes the algoritm implementation.
        /// </summary>
        /// <returns></returns>
        protected abstract IList<ISet<TVertex>> ComputeInternal();

        /// <summary>
        /// Implements the naive version of the Bron-Kerbosh algorithm.
        /// </summary>
        /// <returns></returns>
        protected List<ISet<TVertex>> MaximalCliquesNaive()
        {
            var result = new List<ISet<TVertex>>();
            Naive(new HashSet<TVertex>(), new HashSet<TVertex>(VisitedGraph.Vertices), new HashSet<TVertex>(), result);
            return result;
        }

        /// <summary>
        /// Implements the pivoting version of the Bron-Kerbosh algorithm.
        /// </summary>
        /// <returns></returns>
        protected List<ISet<TVertex>> MaximalCliquesPivot()
        {
            var result = new List<ISet<TVertex>>();
            Pivot(new HashSet<TVertex>(), new HashSet<TVertex>(VisitedGraph.Vertices), new HashSet<TVertex>(), result);
            return result;
        }

        /// <summary>
        /// Implements the degeneracy version of the Bron-Kerbosch algorithm.
        /// </summary>
        /// <returns></returns>
        protected List<ISet<TVertex>> MaximalCliquesDegeneracy()
        {
            var result = new List<ISet<TVertex>>();
            Degeneracy(new HashSet<TVertex>(), new HashSet<TVertex>(VisitedGraph.Vertices), new HashSet<TVertex>(), result);
            return result;
        }

        /// <summary>
        /// Returns an enumerable containing the unique items of both sets.
        /// </summary>
        /// <param name="A"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        ISet<TVertex> Union(IEnumerable<TVertex> A, TVertex b)
        {
            var h = new HashSet<TVertex>(A, EqualityComparer<TVertex>.Default);
            h.Add(b);
            return h;
        }

        /// <summary>
        /// Returns an enumerable containing the items that exist in both sets.
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        ISet<TVertex> Intersection(ISet<TVertex> A, ISet<TVertex> B)
        {
            var h = new HashSet<TVertex>(A, EqualityComparer<TVertex>.Default);
            h.IntersectWith(B);
            return h;
        }

        /// <summary>
        /// Returns an enumerable of those items in the first set not in the second.
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        IEnumerable<TVertex> Minus(ISet<TVertex> A, ISet<TVertex> B)
        {
            var h = new HashSet<TVertex>(A, EqualityComparer<TVertex>.Default);
            h.ExceptWith(B);
            return h;
        }

        /// <summary>
        /// Gets the set of vertices adjacent to the specified vertex.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        ISet<TVertex> AdjacentVertices(TVertex v)
        {
            return adjacentVerticesCache.GetOrAdd(v, k =>
            {
                var h = new HashSet<TVertex>();

                foreach (var i in VisitedGraph.AdjacentEdges(k))
                {
                    if (!Equals(i.Source, k))
                        h.Add(i.Source);
                    else
                        h.Add(i.Target);
                }

                return h;
            });
        }

        /// <summary>
        /// Implements the standard Bron-Kerbosh algorithm.
        /// </summary>
        /// <param name="R"></param>
        /// <param name="P"></param>
        /// <param name="X"></param>
        /// <param name="result"></param>
        void Naive(ISet<TVertex> R, ISet<TVertex> P, ISet<TVertex> X, List<ISet<TVertex>> result)
        {
            if (P.Any() == false && X.Any() == false)
                result.Add(new HashSet<TVertex>(R, EqualityComparer<TVertex>.Default));

            while (P.Any())
            {
                var vertex = P.First();
                var neighbourhood = AdjacentVertices(vertex);
                Naive(Union(R, vertex), Intersection(P, neighbourhood), Intersection(X, neighbourhood), result);
                P.Remove(vertex);
                X.Add(vertex);
            }
        }

        /// <summary>
        /// Implements a version of the algorithm that pivots.
        /// </summary>
        /// <param name="R"></param>
        /// <param name="P"></param>
        /// <param name="X"></param>
        /// <param name="result"></param>
        void Pivot(ISet<TVertex> R, ISet<TVertex> P, ISet<TVertex> X, List<ISet<TVertex>> result)
        {
            if (P.Any() == false && X.Any() == false)
            {
                result.Add(new HashSet<TVertex>(R, EqualityComparer<TVertex>.Default));
            }
            else
            {
                var pivot = P.Concat(X).First();
                var candidates = Minus(P, AdjacentVertices(pivot));
                foreach (var vertex in candidates)
                {
                    var neighbourhood = AdjacentVertices(vertex);
                    Pivot(Union(R, vertex), Intersection(P, neighbourhood), Intersection(X, neighbourhood), result);
                    P.Remove(vertex);
                    X.Add(vertex);
                }
            }
        }

        /// <summary>
        /// Implements a version that pivots on a degeneracy, suitable for sparse graphs.
        /// </summary>
        /// <param name="R"></param>
        /// <param name="P"></param>
        /// <param name="X"></param>
        /// <param name="result"></param>
        void Degeneracy(ISet<TVertex> R, ISet<TVertex> P, ISet<TVertex> X, List<ISet<TVertex>> result)
        {
            foreach (var vertex in DegeneracyOrder())
            {
                var neighbourhood = AdjacentVertices(vertex);
                Pivot(Union(R, vertex), Intersection(P, neighbourhood), Intersection(X, neighbourhood), result);
                P.Remove(vertex);
                X.Add(vertex);
            }
        }

        IEnumerable<TVertex> DegeneracyOrder()
        {
            return VisitedGraph.Vertices.OrderBy(i => i, new DegeneracyOrderComparator(vertex => AdjacentVertices(vertex)));
        }

    }

}
