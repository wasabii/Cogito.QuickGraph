using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cogito.QuickGraph.Tests
{

    [TestClass]
    public class ConnectedUndirectedGraphTests
    {

        [TestMethod]
        public void Can_create()
        {
            var v = new[] { "v1", "v2", "v3", "v4" };
            var g = new ConnectedUndirectedGraph<string>(v);

            g.AdjacentEdges("v1").Select(i => i.Target).Should().HaveCount(3).And.Contain(new[] { "v2", "v3", "v4" });
            g.AdjacentEdges("v2").Select(i => i.Target).Should().HaveCount(3).And.Contain(new[] { "v1", "v3", "v4" });
            g.AdjacentEdges("v3").Select(i => i.Target).Should().HaveCount(3).And.Contain(new[] { "v1", "v2", "v4" });
            g.AdjacentEdges("v4").Select(i => i.Target).Should().HaveCount(3).And.Contain(new[] { "v1", "v2", "v3" });
        }


    }

}
