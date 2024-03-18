using System.Collections;
using System.Collections.Generic;
using UCM.IAV.Navegacion;
using UnityEngine;

public class Connection : MonoBehaviour
{

    private Vertex fromNode; // Nodo origen
    private Vertex toNode; // Nodo destino
    private float cost; // Coste de la conexión

    public Vertex FromNode { get { return fromNode; } set {  fromNode = value; } }
    public Vertex ToNode { get { return toNode; } set {  toNode = value; } }
    public float Cost { get { return cost; } set {  cost = value; } }
}
