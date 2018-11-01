using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;

using Cogito.QuickGraph.Algorithms;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using QuickGraph;

namespace Cogito.QuickGraph.Tests
{

    [TestClass]
    public class BronKerboschTests
    {
        static readonly XNamespace ns3 = "urn:com.journaltech:ecourt:ecf:extension:ZipCodeValue";

        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Test_BronKerbosch()
        {
            var zipCodeList = XDocument.Load("XMLFile1.xml")
                .Root.Element("SimpleCodeList")
                .Elements("Row")
                .Elements("Value")
                .Elements("ComplexValue")
                .Select(i => new
                {
                    ZipCode = (string)i.Element(ns3 + "zipCode"),
                    LocationCode = (string)i.Element(ns3 + "locationCode"),
                    CaseType = (string)i.Element(ns3 + "caseType"),
                    CaseCategory = (string)i.Element(ns3 + "caseCategory")
                })
                .ToList();

            var categoryList = XDocument.Load("XMLFile2.xml")
                .Root.Element("SimpleCodeList")
                .Elements("Row")
                .Elements("Value")
                .Where(i => (string)i.Attribute("ColumnRef") == "code")
                .Elements("SimpleValue")
                .Select(i => (string)i)
                .ToList();

            var zipCodes = zipCodeList.Select(i => i.ZipCode).Where(i => i != null).Distinct().ToList();
            var locations = zipCodeList.Select(i => i.LocationCode).Where(i => i != null).Distinct().ToList();
            var types = zipCodeList.Select(i => i.CaseType).Where(i => i != null).Distinct().ToList();
            var categories = categoryList.Where(i => i != null).Distinct().ToList();

            SEquatableUndirectedEdge<string> CreateEdge(string item1, string item2)
            {
                return Comparer<string>.Default.Compare(item1, item2) <= 0 ? new SEquatableUndirectedEdge<string>(item1, item2) : new SEquatableUndirectedEdge<string>(item2, item1);
            }

            var g = new UndirectedGraph<string, SEquatableUndirectedEdge<string>>();

            foreach (var zipCode in zipCodes)
                g.AddVertex("ZIPCODE:" + zipCode);

            foreach (var location in locations)
                g.AddVertex("LOCATION:" + location);

            foreach (var type in types)
                g.AddVertex("TYPE:" + type);

            foreach (var category in categories)
                g.AddVertex("CATEGORY:" + category);

            foreach (var items in zipCodeList.Where(i => i.CaseType != null).Distinct())
                g.AddVertex("TYPE:" + items.CaseType);

            foreach (var items in zipCodeList.Where(i => i.CaseCategory != null).Distinct())
                g.AddVertex("CATEGORY:" + items.CaseCategory);

            foreach (var zipcode in zipCodes)
                foreach (var zipCode2 in zipCodes)
                    if (zipcode != zipCode2)
                        g.AddVerticesAndEdge(CreateEdge("ZIPCODE:" + zipcode, "ZIPCODE:" + zipCode2));

            foreach (var location in locations)
                foreach (var location2 in locations)
                    if (location != location2)
                        g.AddVerticesAndEdge(CreateEdge("LOCATION:" + location, "LOCATION:" + location2));

            foreach (var type in types)
                foreach (var type2 in types)
                    if (type != type2)
                        g.AddVerticesAndEdge(CreateEdge("TYPE:" + type, "TYPE:" + type2));

            foreach (var category in categories)
                foreach (var category2 in categories)
                    if (category != category2)
                        g.AddVerticesAndEdge(CreateEdge("CATEGORY:" + category, "CATEGORY:" + category2));

            foreach (var zipCode in zipCodes)
                foreach (var item in zipCodeList.Where(i => i.ZipCode == zipCode))
                {
                    if (item.LocationCode != null)
                        g.AddEdge(CreateEdge("ZIPCODE:" + zipCode, "LOCATION:" + item.LocationCode));
                    else
                        g.AddEdgeRange(types.Select(location => CreateEdge("ZIPCODE:" + zipCode, "LOCATION:" + location)));

                    if (item.CaseType != null)
                        g.AddEdge(CreateEdge("ZIPCODE:" + zipCode, "TYPE:" + item.CaseType));
                    else
                        g.AddEdgeRange(types.Select(type => CreateEdge("ZIPCODE:" + zipCode, "TYPE:" + type)));

                    if (item.CaseCategory != null)
                        g.AddEdge(CreateEdge("ZIPCODE:" + zipCode, "CATEGORY:" + item.CaseCategory));
                    else
                        g.AddEdgeRange(categories.Select(category => CreateEdge("ZIPCODE:" + zipCode, "CATEGORY:" + category)));
                }

            foreach (var location in locations)
                foreach (var item in zipCodeList.Where(i => i.LocationCode == location))
                {
                    if (item.ZipCode != null)
                        g.AddEdge(CreateEdge("LOCATION:" + location, "ZIPCODE:" + item.ZipCode));
                    else
                        g.AddEdgeRange(zipCodes.Select(zipCode => CreateEdge("LOCATION:" + location, "ZIPCODE:" + zipCode)));

                    if (item.CaseType != null)
                        g.AddEdge(CreateEdge("LOCATION:" + location, "TYPE:" + item.CaseType));
                    else
                        g.AddEdgeRange(types.Select(type => CreateEdge("LOCATION:" + location, "TYPE:" + type)));

                    if (item.CaseCategory != null)
                        g.AddEdge(CreateEdge("LOCATION:" + location, "CATEGORY:" + item.CaseCategory));
                    else
                        g.AddEdgeRange(categories.Select(category => CreateEdge("LOCATION:" + location, "CATEGORY:" + category)));
                }

            foreach (var type in types)
                foreach (var item in zipCodeList.Where(i => i.CaseType == type))
                {
                    if (item.ZipCode != null)
                        g.AddEdge(CreateEdge("TYPE:" + type, "ZIPCODE:" + item.ZipCode));
                    else
                        g.AddEdgeRange(zipCodes.Select(zipCode => CreateEdge("TYPE:" + type, "ZIPCODE:" + zipCode)));

                    if (item.LocationCode != null)
                        g.AddEdge(CreateEdge("TYPE:" + type, "LOCATION:" + item.LocationCode));
                    else
                        g.AddEdgeRange(locations.Select(location => CreateEdge("TYPE:" + type, "LOCATION:" + location)));

                    if (item.CaseCategory != null)
                        g.AddEdge(CreateEdge("TYPE:" + type, "CATEGORY:" + item.CaseCategory));
                    else
                        g.AddEdgeRange(categories.Select(category => CreateEdge("TYPE:" + type, "CATEGORY:" + category)));
                }

            foreach (var category in categories)
                foreach (var item in zipCodeList.Where(i => i.CaseCategory == category))
                {
                    if (item.ZipCode != null)
                        g.AddEdge(CreateEdge("CATEGORY:" + category, "ZIPCODE:" + item.ZipCode));
                    else
                        g.AddEdgeRange(zipCodes.Select(zipCode => CreateEdge("CATEGORY:" + category, "ZIPCODE:" + zipCode)));

                    if (item.LocationCode != null)
                        g.AddEdge(CreateEdge("CATEGORY:" + category, "LOCATION:" + item.LocationCode));
                    else
                        g.AddEdgeRange(locations.Select(location => CreateEdge("CATEGORY:" + category, "LOCATION:" + location)));

                    if (item.CaseType != null)
                        g.AddEdge(CreateEdge("CATEGORY:" + category, "TYPE:" + item.CaseType));
                    else
                        g.AddEdgeRange(types.Select(type => CreateEdge("CATEGORY:" + category, "TYPE:" + type)));
                }

            var sw = new Stopwatch();

            sw.Start();
            //var r1 = g.BronKerbosh();
            sw.Stop();
            TestContext.WriteLine($"MaximalCliques took {sw.Elapsed}.");
            sw.Reset();

            sw.Start();
            var r2 = g.BronKerboshDegeneracy();
            sw.Stop();
            TestContext.WriteLine($"MaximalCliquesDegeneracy took {sw.Elapsed}.");
            sw.Reset();

            sw.Start();
            var r3 = g.BronKerboshPivot();
            sw.Stop();
            TestContext.WriteLine($"MaximalCliquesPivot took {sw.Elapsed}.");
            sw.Reset();

            bool ContainsRequiredValues(IEnumerable<string> a)
            {
                return a.Any(j => j.StartsWith("ZIPCODE")) && a.Any(j => j.StartsWith("LOCATION")) && a.Any(j => j.StartsWith("TYPE")) && a.Any(j => j.StartsWith("CATEGORY"));
            }

            var e2 = r2.Where(i => ContainsRequiredValues(i.Vertices)).ToList();
            var e3 = r3.Where(i => ContainsRequiredValues(i.Vertices)).ToList();

            e2.Should().HaveCount(72);
            e3.Should().HaveCount(72);
        }

    }

}