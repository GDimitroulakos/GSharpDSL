using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphLibrary;
using GraphLibrary.Generics;

namespace GSharpDSL {

    class CTranslationUnit
    {

        public CDipole dipole()
        {
            return new CDipole(this);
        }
    };

    class CDipole
    {
        private CTranslationUnit father;
        public CDipole(CTranslationUnit father)
        {
            this.father = father;
        }

        public CDipole algorithm()
        {
            return this; 
        }

        public CConnections connections()
        {
            return new CConnections(this);
        }

        public CTranslationUnit end()
        {
            return father;
        }
    };

    class CConnections
    {
        private CDipole father;

        public CConnections(CDipole father)
        {
            this.father = father;
        }

        public CConnection connection()
        {
            return new CConnection(this);
        }

        public CTranslationUnit end()
        {
            return father.end();
        }
    };

    class CConnection
    {
        private CConnections father;

        private CGraph m_graph;
        private GraphElementType m_elementType;
        private Object m_key;
        private Type m_infoType;

        public CConnection(CConnections father)
        {
            this.father = father;
        }

        public CConnection GRAPH(CGraph g)
        {
            m_graph = g;
            return this;
        }

        public CConnection GRAPHELEMENTTYPE(GraphElementType type)
        {
            m_elementType = type;
            return this;
        }

        public CConnection KEY(Object key)
        {
            m_key = key;
            return this;
        }

        public CConnection INFOTYPE(Type infoType)
        {
            m_infoType = infoType;
            return this;
        }

        public CConnections end()
        {
            return father;
        }
        
    };

    class Algorithm
    {
        private Dictionary<Object, Object> m_inputs;
        private Dictionary<Object, Object> m_outputs;

        public CAlgIO inputs()
        {
            return new CAlgIO(this);
        }
    }

    class CAlgIO
    {
        private Algorithm father;

        public CAlgIO(Algorithm father)
        {
            this.father = father;
        }

        public CAlgIO NOTETYPE(Type type)
        {

            return this;
        }
    }

    class Program {
        static void Main(string[] args) {

            CTranslationUnit tr = new CTranslationUnit();

            CGraph graph = CGraph.CreateGraph();

            tr.dipole()
                .algorithm()
                .algorithm()
                .connections()
                    .connection()
                        .GRAPH(graph)
                        .GRAPHELEMENTTYPE(GraphElementType.ET_NODE)
                        .KEY(graph.GetHashCode())
                        .INFOTYPE(typeof(int))
                    .end()
                .end();
                

        }
    }
}
