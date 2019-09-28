using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Database;
using QuickGraph.Serialization;
using UnityEngine;
using System.Xml;
using QuickGraph;
using QuickGraph.Algorithms;
using GraphSharp;

namespace PipLib.Tech
{
    public class TechTree
    {
        public static void OnLoad ()
        {
            var tree = new TechTree();
            var vert1 = new Vertex();
            var vert2 = new Vertex();
            var vert3 = new Vertex();
            var edge1 = new Edge(vert1, vert2);
            var edge2 = new Edge(vert3, vert2);
            tree.graph.AddVertexRange(new Vertex[] { vert1, vert2, vert3 });
            tree.graph.AddEdgeRange(new Edge[] { edge1, edge2 });

            GlobalLogger.Get().Debug(tree.CreateAsset().text);
        }

        public class Vertex
        {
            public int ID { get; set; }
        }

        public class Edge : TypedEdge<Vertex>
        {
            public Edge(Vertex source, Vertex target) : base(source, target, EdgeTypes.Hierarchical) { }
        }

        public class Graph : HierarchicalGraph<Vertex, Edge>
        {
            public TextAsset SerializeToGraphMLTextAsset()
            {
                var str = new StringBuilder();
                this.SerializeToGraphML(XmlWriter.Create(str), v => v.ID.ToString(), AlgorithmExtensions.GetEdgeIdentity(this));
                return new TextAsset(str.ToString());
            }
        }

        private Graph graph = new Graph();

        public Graph GetGraph ()
        {
            return graph;
        }

        public TextAsset CreateAsset ()
        {
            return graph.SerializeToGraphMLTextAsset();
        }

    }
}
