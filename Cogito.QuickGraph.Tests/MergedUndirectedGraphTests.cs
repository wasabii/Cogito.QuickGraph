using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuickGraph;

namespace Cogito.QuickGraph.Tests
{

    [TestClass]
    public class MergedUndirectedGraphTests
    {

        [TestMethod]
        public void Can_create()
        {
            var g1 = new ConnectedUndirectedGraph<string>(new[] { "v1", "v2", "v3", "v4" });
            var g2 = new ConnectedUndirectedGraph<string>(new[] { "v5", "v6", "v7", "v8" });

            // allow link between a few select elements but that's it
            var g3 = new UndirectedGraph<string, SEquatableUndirectedEdge<string>>(false);
            g3.AddVerticesAndEdge(new SEquatableUndirectedEdge<string>("v1", "v5"));
            g3.AddVerticesAndEdge(new SEquatableUndirectedEdge<string>("v4", "v8"));

            var g = new MergedUndirectedGraph<string, SEquatableUndirectedEdge<string>>(new IUndirectedGraph<string, SEquatableUndirectedEdge<string>>[] { g1, g2, g3 });

            g.AdjacentEdges("v1").Select(i => i.GetOtherVertex("v1")).Should().HaveCount(4).And.Contain(new[] { "v2", "v3", "v4", "v5" });
            g.AdjacentEdges("v2").Select(i => i.GetOtherVertex("v2")).Should().HaveCount(3).And.Contain(new[] { "v1", "v3", "v4" });
            g.AdjacentEdges("v3").Select(i => i.GetOtherVertex("v3")).Should().HaveCount(3).And.Contain(new[] { "v1", "v2", "v4" });
            g.AdjacentEdges("v4").Select(i => i.GetOtherVertex("v4")).Should().HaveCount(4).And.Contain(new[] { "v1", "v2", "v3", "v8" });

            g.AdjacentEdges("v5").Select(i => i.GetOtherVertex("v5")).Should().HaveCount(4).And.Contain(new[] { "v6", "v7", "v8", "v1" });
            g.AdjacentEdges("v6").Select(i => i.GetOtherVertex("v6")).Should().HaveCount(3).And.Contain(new[] { "v5", "v7", "v8" });
            g.AdjacentEdges("v7").Select(i => i.GetOtherVertex("v7")).Should().HaveCount(3).And.Contain(new[] { "v5", "v6", "v8" });
            g.AdjacentEdges("v8").Select(i => i.GetOtherVertex("v8")).Should().HaveCount(4).And.Contain(new[] { "v5", "v6", "v7", "v4" });
        }


    }

}
