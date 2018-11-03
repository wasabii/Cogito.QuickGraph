using System;
using System.Collections.Generic;
using System.Linq;

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
        /// Returns the edges for a given dimension value.
        /// </summary>
        /// <param name="getter"></param>
        /// <returns></returns>
        static global::QuickGraph.TryFunc<T, TResult> GetTryGetFunc<T, TResult>(Func<T, TResult> getter)
        {
            // capture implementation
            bool TryGetFuncImpl(T arg, out TResult result)
            {
                result = getter(arg);
                return result != null;
            }

            // return internal impl
            return TryGetFuncImpl;
        }

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

            return vertices
                .Where(i => !Equals(i, vertex))
                .Select(i => new SEquatableUndirectedEdge<TVertex>(vertex, i));
        }

        public ConnectedUndirectedGraph(IEnumerable<TVertex> vertices) : 
            base(vertices, GetTryGetFunc<TVertex, IEnumerable<SEquatableUndirectedEdge<TVertex>>>(v => GetAdjacentEdges(vertices, v)), false)
        {

        }

    }

}
