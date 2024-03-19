using System.Collections;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;
using UCM.IAV.Navegacion;
using System.Linq;

public class PathFindingList
{
    private List<NodeRecord> records; // Lista de nodos

    void Awake()
    {
        records = new List<NodeRecord>();
    }

    public PathFindingList() {}

    public void Add(NodeRecord record) { records.Add(record); }
    public void Remove(NodeRecord record) { records.Remove(record); }
    public bool Contains(Vertex node) { 
        return Find(node) != null;
    }
    public NodeRecord Find(Vertex node) {

        foreach (NodeRecord record in records)
        {
            if (record.Node == node) return record;
        }
        return null;
    }
    public NodeRecord SmallestElement()
    {
        if (records == null || records.Count == 0) return null;
        return records.Min();
    }

    public int Length() { return records.Count; }
}
