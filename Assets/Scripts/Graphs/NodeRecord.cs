using System.Collections;
using System.Collections.Generic;
using UCM.IAV.Navegacion;
using UnityEngine;

public class NodeRecord
{
    private Vertex node; // Nodo seleccionado
    private Connection connection; // Conexión seleccionada a partir del nodo seleccionado
    private float costSoFar; // Coste acumulado hasta el nodo seleccionado
    private float estimatedTotalCost; // Coste total estimado hasta el nodo seleccionado

    public NodeRecord(Vertex node = null, Connection connection = null, float costSoFar = 0, float estimatedTotalCost = 0)
    {
        Node = node;
        Connection = connection;
        CostSoFar = costSoFar;
        EstimatedTotalCost = estimatedTotalCost;
    }

    public Vertex Node { get { return node; } set { node = value; } }
    public Connection Connection { get { return connection; } set { connection = value; } }
    public float CostSoFar { get { return costSoFar; } set { costSoFar = value; } }
    public float EstimatedTotalCost { get { return estimatedTotalCost; } set { estimatedTotalCost = value; } }
}
