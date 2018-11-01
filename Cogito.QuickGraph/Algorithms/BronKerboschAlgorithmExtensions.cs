using System.Collections.Generic;

using QuickGraph;

namespace Cogito.QuickGraph.Algorithms
{

    public static class BronKerboschAlgorithmExtensions
    {

        public static ISet<IUndirectedGraph<TVertex, IEdge<TVertex>>> BronKerboshNaive<TVertex, TEdge>(this IUndirectedGraph<TVertex, TEdge> graph)
            where TEdge : IEdge<TVertex>
        {
            var algorithm = new BronKerboschNaiveAlgorithm<TVertex, TEdge>(graph);
            algorithm.Compute();
            return algorithm.MaximalCliques;
        }

        public static ISet<IUndirectedGraph<TVertex, IEdge<TVertex>>> BronKerboshPivot<TVertex, TEdge>(this IUndirectedGraph<TVertex, TEdge> graph)
            where TEdge : IEdge<TVertex>
        {
            var algorithm = new BronKerboschPivotAlgorithm<TVertex, TEdge>(graph);
            algorithm.Compute();
            return algorithm.MaximalCliques;
        }

        public static ISet<IUndirectedGraph<TVertex, IEdge<TVertex>>> BronKerboshDegeneracy<TVertex, TEdge>(this IUndirectedGraph<TVertex, TEdge> graph)
            where TEdge : IEdge<TVertex>
        {
            var algorithm = new BronKerboschDegeneracyAlgorithm<TVertex, TEdge>(graph);
            algorithm.Compute();
            return algorithm.MaximalCliques;
        }

    }

}
