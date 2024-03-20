using System;
using UCM.IAV.Navegacion;

public class NodeRecord : IComparable<NodeRecord>
{
    public Vertex Node { get; set; }                // Nodo seleccionado
    public NodeRecord PreviousNode { get; set; }    // Referencia al nodo anterior
    public float Cost { get; set; }                 // Coste de la conexión entre node y previousNode
    public float CostSoFar { get; set; }            // Coste acumulado hasta el nodo seleccionado
    public float EstimatedTotalCost { get; set; }   // Coste total estimado hasta el nodo seleccionado

    public NodeRecord(Vertex node = null, NodeRecord previousNode = null, float cost = 0, float costSoFar = 0, float estimatedTotalCost = 0)
    {
        Node = node;
        PreviousNode = previousNode;
        Cost = cost;
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
