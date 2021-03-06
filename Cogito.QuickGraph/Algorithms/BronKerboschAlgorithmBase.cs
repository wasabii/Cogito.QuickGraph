﻿using System;
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
        class DegeneracyOrderComparator : Comparer<TVertex>
        {

            readonly Func<TVertex, ISet<TVertex>> neighbours;

            /// <summary>
            /// Initializes a new instance.
            /// </summary>
            /// <param name="neighbours"></param>
            public DegeneracyOrderComparator(Func<TVertex, ISet<TVertex>> neighbours)
            {
                this.neighbours = neighbours;
            }

            public override int Compare(TVertex u, TVertex v)
            {
                return neighbours(u).Count - neighbours(v).Count;
            }

        }

        readonly Dictionary<TVertex, HashSet<TVertex>> adjacentVerticesCache = new Dictionary<TVertex, HashSet<TVertex>>();
        IList<ISet<TVertex>> cliques;

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
        public IList<ISet<TVertex>> MaximalCliques => cliques;

        /// <summary>
        /// Computes the maximal set of cliques.
        /// </summary>
        protected override void InternalCompute()
        {
            if (cliques == null)
                cliques = ComputeInternal();
        }

        /// <summary>
        /// Invokes the algoritm implementation.
        /// </summary>
        /// <returns></returns>
        protected abstract IList<ISet<TVertex>> ComputeInternal();

        protected List<ISet<TVertex>> MaximalCliquesNaive()
        {
            var result = new List<ISet<TVertex>>();
            Naive(new HashSet<TVertex>(), new HashSet<TVertex>(VisitedGraph.Vertices), new HashSet<TVertex>(), result);
            return new List<ISet<TVertex>>(result);
        }

        protected List<ISet<TVertex>> MaximalCliquesPivot()
        {
            var result = new List<ISet<TVertex>>();
            Pivot(new HashSet<TVertex>(), new HashSet<TVertex>(VisitedGraph.Vertices), new HashSet<TVertex>(), result);
            return new List<ISet<TVertex>>(result);
        }

        protected List<ISet<TVertex>> MaximalCliquesDegeneracy()
        {
            var result = new List<ISet<TVertex>>();
            Degeneracy(new HashSet<TVertex>(), new HashSet<TVertex>(VisitedGraph.Vertices), new HashSet<TVertex>(), result);
            return new List<ISet<TVertex>>(result);
        }

        /// <summary>
        /// Returns an enumerable containing the unique items of both sets.
        /// </summary>
        /// <param name="A"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        protected ISet<TVertex> Union(IEnumerable<TVertex> A, TVertex b)
        {
            var h = new HashSet<TVertex>(A);
            h.Add(b);
            return h;
        }

        protected ISet<TVertex> Intersection(ISet<TVertex> A, ISet<TVertex> B)
        {
            var result = new HashSet<TVertex>(A);
            result.IntersectWith(B);
            return result;
        }

        protected IEnumerable<TVertex> Minus(ISet<TVertex> A, ISet<TVertex> B)
        {
            var result = new HashSet<TVertex>(A);
            result.ExceptWith(B);
            return result;
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

        protected void Naive(ISet<TVertex> R, ISet<TVertex> P, ISet<TVertex> X, List<ISet<TVertex>> result)
        {
            if (P.Any() == false && X.Any() == false)
                result.Add(new HashSet<TVertex>(R));

            while (P.Any())
            {
                var vertex = P.First();
                var neighbourhood = AdjacentVertices(vertex);
                Naive(Union(R, vertex), Intersection(P, neighbourhood), Intersection(X, neighbourhood), result);
                P.Remove(vertex);
                X.Add(vertex);
            }
        }

        protected void Pivot(ISet<TVertex> R, ISet<TVertex> P, ISet<TVertex> X, List<ISet<TVertex>> result)
        {
            if (P.Any() == false && X.Any() == false)
            {
                result.Add(new HashSet<TVertex>(R));
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

        protected void Degeneracy(ISet<TVertex> R, ISet<TVertex> P, ISet<TVertex> X, List<ISet<TVertex>> result)
        {
            foreach (var vertex in DegeneracyOrder())
            {
                var neighbourhood = AdjacentVertices(vertex);
                Pivot(Union(R, vertex), Intersection(P, neighbourhood), Intersection(X, neighbourhood), result);
                P.Remove(vertex);
                X.Add(vertex);
            }
        }

        protected IEnumerable<TVertex> DegeneracyOrder()
        {
            return VisitedGraph.Vertices.OrderBy(i => i, new DegeneracyOrderComparator(vertex => AdjacentVertices(vertex)));
        }

    }

}
