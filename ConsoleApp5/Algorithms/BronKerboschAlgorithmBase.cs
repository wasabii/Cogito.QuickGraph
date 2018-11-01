using System.Collections.Generic;
using System.Linq;

using Cogito.Collections;

using QuickGraph;
using QuickGraph.Algorithms.Services;

namespace ConsoleApp5.Algorithms
{

    /// <summary>
    /// Implements the standard form of the Bron-Kerbosch algorithm.
    /// </summary>
    /// <typeparam name="TVertex"></typeparam>
    /// <typeparam name="TEdge"></typeparam>
    public abstract class BronKerboschAlgorithmBase<TVertex, TEdge> :
        QuickGraph.Algorithms.Cliques.MaximumCliqueAlgorithmBase<TVertex, TEdge>
        where TEdge : IEdge<TVertex>
    {

        /// <summary>
        /// Compares the order of the given vertices for the degeneracy.
        /// </summary>
        class DegeneracyOrderComparator : Comparer<TVertex>
        {

            readonly IDictionary<TVertex, ISet<TVertex>> neighbours;

            /// <summary>
            /// Initializes a new instance.
            /// </summary>
            /// <param name="neighbours"></param>
            public DegeneracyOrderComparator(IDictionary<TVertex, ISet<TVertex>> neighbours)
            {
                this.neighbours = neighbours;
            }

            public override int Compare(TVertex u, TVertex v)
            {
                return neighbours[u].Count - neighbours[v].Count;
            }

        }

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

        protected List<ISet<TVertex>> MaximalCliquesNaive(IDictionary<TVertex, ISet<TVertex>> neighbours)
        {
            var result = new List<ISet<TVertex>>();
            Naive(new HashSet<TVertex>(), Vertices(neighbours), new HashSet<TVertex>(), neighbours, result);
            return new List<ISet<TVertex>>(result);
        }

        protected List<ISet<TVertex>> MaximalCliquesPivot(IDictionary<TVertex, ISet<TVertex>> neighbours)
        {
            var result = new List<ISet<TVertex>>();
            Pivot(new HashSet<TVertex>(), Vertices(neighbours), new HashSet<TVertex>(), neighbours, result);
            return new List<ISet<TVertex>>(result);
        }

        protected List<ISet<TVertex>> MaximalCliquesDegeneracy(IDictionary<TVertex, ISet<TVertex>> neighbours)
        {
            var result = new List<ISet<TVertex>>();
            Degeneracy(new HashSet<TVertex>(), Vertices(neighbours), new HashSet<TVertex>(), neighbours, result);
            return new List<ISet<TVertex>>(result);
        }

        protected ISet<TVertex> Vertices(IDictionary<TVertex, ISet<TVertex>> neighbours)
        {
            var vertices = new HashSet<TVertex>();
            vertices.AddRange(neighbours.Keys);
            return vertices;
        }

        protected TVertex PickFrom(ISet<TVertex> A)
        {
            return A.First();
        }

        protected ISet<TVertex> Union(ISet<TVertex> A, ISet<TVertex> B)
        {
            var result = new HashSet<TVertex>();
            result.AddRange(A);
            result.AddRange(B);
            return result;
        }

        protected ISet<TVertex> Union(ISet<TVertex> A, TVertex b)
        {
            var result = new HashSet<TVertex>();
            result.AddRange(A);
            result.Add(b);
            return result;
        }

        protected ISet<TVertex> Intersection(ISet<TVertex> A, ISet<TVertex> B)
        {
            var result = new HashSet<TVertex>();
            result.AddRange(A);
            result.IntersectWith(B);
            return result;
        }

        protected IEnumerable<TVertex> Minus(ISet<TVertex> A, ISet<TVertex> B)
        {
            var result = new HashSet<TVertex>();
            result.AddRange(A);
            result.ExceptWith(B);
            return result;
        }

        protected void Naive(ISet<TVertex> R, ISet<TVertex> P, ISet<TVertex> X, IDictionary<TVertex, ISet<TVertex>> neighbours, List<ISet<TVertex>> result)
        {
            if (P.Any() == false && X.Any() == false)
                result.Add(new HashSet<TVertex>(R));

            while (P.Any())
            {
                var vertex = PickFrom(P);
                var neighbourhood = neighbours[vertex];
                Naive(Union(R, vertex), Intersection(P, neighbourhood), Intersection(X, neighbourhood), neighbours, result);
                P.Remove(vertex);
                X.Add(vertex);
            }
        }

        protected void Pivot(ISet<TVertex> R, ISet<TVertex> P, ISet<TVertex> X, IDictionary<TVertex, ISet<TVertex>> neighbours, List<ISet<TVertex>> result)
        {
            if (P.Any() == false && X.Any() == false)
            {
                result.Add(new HashSet<TVertex>(R));
            }
            else
            {
                var pivot = PickFrom(Union(P, X));
                var candidates = Minus(P, neighbours[pivot]);
                foreach (var vertex in candidates)
                {
                    var neighbourhood = neighbours[vertex];
                    Pivot(Union(R, vertex), Intersection(P, neighbourhood), Intersection(X, neighbourhood), neighbours, result);
                    P.Remove(vertex);
                    X.Add(vertex);
                }
            }
        }

        protected void Degeneracy(ISet<TVertex> R, ISet<TVertex> P, ISet<TVertex> X, IDictionary<TVertex, ISet<TVertex>> neighbours, List<ISet<TVertex>> result)
        {
            foreach (var vertex in DegeneracyOrder(neighbours))
            {
                var neighbourhood = neighbours[vertex];
                Pivot(Union(R, vertex), Intersection(P, neighbourhood), Intersection(X, neighbourhood), neighbours, result);
                P.Remove(vertex);
                X.Add(vertex);
            }
        }

        protected List<TVertex> DegeneracyOrder(IDictionary<TVertex, ISet<TVertex>> neighbours)
        {
            var degeneracyOrder = new List<TVertex>();
            degeneracyOrder.AddRange(neighbours.Keys);
            degeneracyOrder.Sort(new DegeneracyOrderComparator(neighbours));
            return degeneracyOrder;
        }

    }

}
