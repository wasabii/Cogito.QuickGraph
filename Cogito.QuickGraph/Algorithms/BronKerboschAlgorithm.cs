using System.Collections.Generic;

using QuickGraph;
using QuickGraph.Algorithms.Services;

namespace Cogito.QuickGraph.Algorithms
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
            return MaximalCliquesNaive();
        }

    }

}
