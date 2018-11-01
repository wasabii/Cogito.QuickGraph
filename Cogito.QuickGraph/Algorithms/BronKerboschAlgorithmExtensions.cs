using System.Collections.Generic;

using QuickGraph;

namespace Cogito.QuickGraph.Algorithms
{

    public static class BronKerboschAlgorithmExtensions
    {

        public static IList<ISet<TVertex>> BronKerbosh<TVertex, TEdge>(this IUndirectedGraph<TVertex, TEdge> graph)
            where TEdge : IEdge<TVertex>
        {
            var algorithm = new BronKerboschAlgorithm<TVertex, TEdge>(graph);
            algorithm.Compute();
            return algorithm.MaximalCliques;
        }

        public static IList<ISet<TVertex>> BronKerboshPivot<TVertex, TEdge>(this IUndirectedGraph<TVertex, TEdge> graph)
            where TEdge : IEdge<TVertex>
        {
            var algorithm = new BronKerboschPivotAlgorithm<TVertex, TEdge>(graph);
            algorithm.Compute();
            return algorithm.MaximalCliques;
        }

        public static IList<ISet<TVertex>> BronKerboshDegeneracy<TVertex, TEdge>(this IUndirectedGraph<TVertex, TEdge> graph)
            where TEdge : IEdge<TVertex>
        {
            var algorithm = new BronKerboschDegeneracyAlgorithm<TVertex, TEdge>(graph);
            algorithm.Compute();
            return algorithm.MaximalCliques;
        }

    }

}
