using System;
using UCM.IAV.Navegacion;

public class NodeRecord : IComparable<NodeRecord>
{
    public Vertex Node { get; set; }                // Nodo seleccionado
    public Connection Connection { get; set; }      // Conexión seleccionada a partir del nodo seleccionado
    public float CostSoFar { get; set; }            // Coste acumulado hasta el nodo seleccionado
    public float EstimatedTotalCost { get; set; }   // Coste total estimado hasta el nodo seleccionado

    public NodeRecord(Vertex node = null, Connection connection = null, float costSoFar = 0, float estimatedTotalCost = 0)
    {
        Node = node;
        Connection = connection;
        CostSoFar = costSoFar;
        EstimatedTotalCost = estimatedTotalCost;
    }

    public int CompareTo(NodeRecord other)
    {
        if (other == null) 
            return 1;
        return EstimatedTotalCost.CompareTo(other.EstimatedTotalCost);
    }
}
