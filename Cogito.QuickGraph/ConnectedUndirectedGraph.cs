using System;
using System.Collections.Generic;
using System.Linq;

using Cogito.Linq;

using QuickGraph;

namespace Cogito.QuickGraph
{

    /// <summary>
    /// An undirected graph which wraps another undirected graph
    /// </summary>
    /// <typeparam name="TVertex"></typeparam>
    public class ConnectedUndirectedGraph<TVertex> :
        DelegateUndirectedGraph<TVertex, SEquatableUndirectedEdge<TVertex>>
    {

        /// <summary>
        /// Gets the adjacent edges for the specified vertex.
        /// </summary>
        /// <param name="vertices"></param>
        /// <param name="vertext"></param>
        /// <returns></returns>
        static IEnumerable<SEquatableUndirectedEdge<TVertex>> GetAdjacentEdges(IEnumerable<TVertex> vertices, TVertex vertex)
        {
            if (vertices == null)
                throw new ArgumentNullException(nameof(vertices));

            // allow cached enumeration
            var t = vertices.Tee();
            if (t.Contains(vertex))
                return t
                    .Where(i => !Equals(i, vertex))
                    .Select(i => new SEquatableUndirectedEdge<TVertex>(vertex, i));

            return Enumerable.Empty<SEquatableUndirectedEdge<TVertex>>();
        }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="vertices"></param>
        public ConnectedUndirectedGraph(IEnumerable<TVertex> vertices) :
            base(vertices, GraphUtil.GetTryGetFunc<TVertex, IEnumerable<SEquatableUndirectedEdge<TVertex>>>(v => GetAdjacentEdges(vertices, v)), false)
        {

        }

    }

}
