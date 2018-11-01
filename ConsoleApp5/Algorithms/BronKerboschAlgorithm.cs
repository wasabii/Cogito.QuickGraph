using System.Collections.Generic;
using System.Linq;

using QuickGraph;
using QuickGraph.Algorithms.Services;

namespace ConsoleApp5.Algorithms
{

    /// <summary>
    /// Implements the standard form of the Bron-Kerbosch algorithm.
    /// </summary>
    /// <typeparam name="TVertex"></typeparam>
    /// <typeparam name="TEdge"></typeparam>
    public class BronKerboschAlgorithm<TVertex, TEdge> :
        BronKerboschAlgorithmBase<TVertex, TEdge>
        where TEdge : IEdge<TVertex>
    {

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="visitedGraph"></param>
        public BronKerboschAlgorithm(IUndirectedGraph<TVertex, TEdge> visitedGraph) :
            base(visitedGraph)
        {

        }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="host"></param>
        /// <param name="visitedGraph"></param>
        public BronKerboschAlgorithm(IAlgorithmComponent host, IUndirectedGraph<TVertex, TEdge> visitedGraph) :
            base(host, visitedGraph)
        {

        }

        /// <summary>
        /// Computes the maximal set of cliques.
        /// </summary>
        protected override IList<ISet<TVertex>> ComputeInternal()
        {
            var d = VisitedGraph.Vertices.ToDictionary(i => i, i => (ISet<TVertex>)new HashSet<TVertex>(VisitedGraph.AdjacentEdges(i).Select(j => !Equals(i, j.Source) ? j.Source : j.Target)));
            return MaximalCliquesNaive(d);
        }

    }

}
