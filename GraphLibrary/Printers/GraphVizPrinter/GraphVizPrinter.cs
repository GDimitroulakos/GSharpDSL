﻿using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Channels;
using System.Text;
using GraphLibrary.Aglorithms;
using GraphLibrary.Generics;
using GraphLibrary.Printers.GraphVizPrinter;

namespace GraphLibrary {
    
    /// <summary>
    /// This class exports the graph in graphviz form. It accepts the graph
    /// and optionally a class that defined the labelling of nodes. If the
    /// client doen't provide a labelling class a default class (GraphVizNodeLabeling)
    /// takes over  the labelling of nodes
    /// </summary>
    /// <seealso cref="GraphLibrary.CGraphPrinter" />
    public class CGraphVizPrinter : CGraphPrinter {

        public CGraphVizPrinter(CGraph graph, CGraphLabeling<CGraphNode> nodeLabeling = null,
            CGraphLabeling<CGraphEdge> edgeLabeling=null ) : base(graph,nodeLabeling,edgeLabeling) {
        }

        /// <summary>
        /// Prints the graph into a StringBuilder object.
        /// Optionally the header and footer of the .dot file can be ommited for use of the
        /// graph edges in the multi layer graph printer.
        /// </summary>
        /// <param name="onlyedges">if set to <c>true</c> [onlyedges] the graph is printed as a standalone graph.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override StringBuilder Print(){
            // Allocate a stringbuiler object and allocate space for 1000 characters
            StringBuilder graphvizStringBuilder = new StringBuilder(1000);
            string graphedge_operator = " ";
            string header = "";
            string headerProperties = "";
            CIt_GraphNodes itn = new CIt_GraphNodes(m_graph);
            CIt_GraphEdges itg = new CIt_GraphEdges(m_graph);

            switch (m_graph.M_GraphType) {
                case GraphType.GT_UNDIRECTED:
                    graphedge_operator = "--";
                    header = "graph G" + m_graph.M_SerialNumber + "{\r\n";
                    header += headerProperties;
                    break;
                case GraphType.GT_DIRECTED:
                    graphedge_operator = "->";
                        header = "digraph G" + m_graph.M_SerialNumber + "{\r\n";
                        header += headerProperties;
                    break;
                case GraphType.GT_MIXED:
                    break;
            }

            // Print header if necessary
            graphvizStringBuilder.Append(header);


            // Print all  nodes
            for (itn.Begin(); !itn.End(); itn.Next()) {
                graphvizStringBuilder.Append("\"" +itn.M_CurrentItem.M_Label+"\";");
            }
            // Print all edges of the graph
            for (itg.Begin(); !itg.End(); itg.Next()) {
                CGraphEdge g = itg.M_CurrentItem;

                string source, target;
                source = g.M_Source.M_Label;
                target = g.M_Target.M_Label; 

                graphvizStringBuilder.AppendFormat("\"{0}\"" + graphedge_operator + "\"{1}\"",
                   source, target);
                graphvizStringBuilder.AppendFormat(" [style = bold, label = \""+g.M_Label +"\"]");

                graphvizStringBuilder.Append(";\r\n");
            }
            // Print footer if necessary
            graphvizStringBuilder.Append("}\r\n");
            return graphvizStringBuilder;
        }

        /// <summary>
        /// Generates the GraphViz .dot file and optionally calls the graphviz to
        /// generate the picture of the graph.
        /// </summary>
        /// <param name="filepath">The full path to which the file is generated</param>
        /// <param name="executeGenerator">If true calls the dot tool to produce the graph in a picture</param>
        public override void Generate(string filepath, bool executeGenerator = true){
            // Open a streamwriter
            using (StreamWriter fstream = new StreamWriter(filepath)){
                fstream.WriteLine(Print());
                fstream.Close();
            }
            // Prepare the process dot to run
            ProcessStartInfo start = new ProcessStartInfo();
            // Enter in the command line arguments, everything you would enter after the executable name itself
            start.Arguments = "-Tgif " +
                                Path.GetFileName(filepath) + " -o " +
                                Path.GetFileNameWithoutExtension(filepath) + ".gif";
            // Enter the executable to run, including the complete path
            start.FileName = "dot";
            // Do you want to show a console window?
            start.WindowStyle = ProcessWindowStyle.Hidden;
            start.CreateNoWindow = true;
            int exitCode;

            // Run the external process & wait for it to finish
            using (Process proc = Process.Start(start)){
                proc.WaitForExit();

                // Retrieve the app's exit code
                exitCode = proc.ExitCode;
            }
        }
    }
}