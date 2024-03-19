using System.Collections;
using UCM.IAV.Navegacion;

public class Connection
{
    public Vertex FromNode  { get; set; }   // Nodo origen
    public Vertex ToNode    { get; set; }   // Nodo destino
    public float Cost       { get; set; }   // Coste de la conexión

    public Connection(Vertex fromNode = null, Vertex toNode = null, float cost = 0)
    {
        FromNode = fromNode;
        ToNode = toNode;
        Cost = cost;
    }
}
