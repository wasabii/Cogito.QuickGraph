using System.Collections.Generic;
using System.Linq;

using QuickGraph;

namespace Cogito.QuickGraph
{

    public class MergedUndirectedGraph<TVertex, TEdge> :
        DelegateUndirectedGraph<TVertex, TEdge>
        where TEdge : IEdge<TVertex>
    {

        /// <summary>
        /// Returns the total merged vertices of the underlying graphs.
        /// </summary>
        /// <param name="visitedGraphs"></param>
        /// <returns></returns>
        static IEnumerable<TVertex> MergeVertices(IEnumerable<IUndirectedGraph<TVertex, TEdge>> visitedGraphs)
        {
            return visitedGraphs.SelectMany(i => i.Vertices);
        }

        /// <summary>
        /// Returns the merged edges of the underlying graphs.
        /// </summary>
        /// <param name="visitedGraphs"></param>
        /// <param name="vertex"></param>
        /// <returns></returns>
        static IEnumerable<TEdge> MergeAdjacentEdges(IEnumerable<IUndirectedGraph<TVertex, TEdge>> visitedGraphs, TVertex vertex)
        {
            return visitedGraphs.Where(i => i.ContainsVertex(vertex)).SelectMany(i => i.AdjacentEdges(vertex));
        }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="visitedGraphs"></param>
        public MergedUndirectedGraph(IEnumerable<IUndirectedGraph<TVertex, TEdge>> visitedGraphs) :
            base(MergeVertices(visitedGraphs), GraphUtil.GetTryGetFunc<TVertex, IEnumerable<TEdge>>(v => MergeAdjacentEdges(visitedGraphs, v)), false)
        {

        }

    }

}
