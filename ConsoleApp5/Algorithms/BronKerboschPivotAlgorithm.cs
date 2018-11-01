using System.Collections.Generic;
using System.Linq;

using QuickGraph;
using QuickGraph.Algorithms.Services;

namespace ConsoleApp5.Algorithms
{

    /// <summary>
    /// Implements the pivot form of the Bron-Kerbosch algorithm.
    /// </summary>
    /// <typeparam name="TVertex"></typeparam>
    /// <typeparam name="TEdge"></typeparam>
    public class BronKerboschPivotAlgorithm<TVertex, TEdge> :
        BronKerboschAlgorithmBase<TVertex, TEdge>
        where TEdge : IEdge<TVertex>
    {

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="visitedGraph"></param>
        public BronKerboschPivotAlgorithm(IUndirectedGraph<TVertex, TEdge> visitedGraph) :
            base(visitedGraph)
        {

        }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="host"></param>
        /// <param name="visitedGraph"></param>
        public BronKerboschPivotAlgorithm(IAlgorithmComponent host, IUndirectedGraph<TVertex, TEdge> visitedGraph) :
            base(host, visitedGraph)
        {

        }

        /// <summary>
        /// Computes the maximal set of cliques.
        /// </summary>
        protected override IList<ISet<TVertex>> ComputeInternal()
        {
            return MaximalCliquesPivot();
        }

    }

}
