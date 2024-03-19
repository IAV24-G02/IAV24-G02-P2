using System.Collections;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;
using UCM.IAV.Navegacion;
using System.Linq;

public class PathFindingList
{
    private List<NodeRecord> records; // Lista de nodos

    public PathFindingList() { records = new List<NodeRecord>(); }

    public void Add(NodeRecord record) { if (record != null) records.Add(record); }
    public void Remove(NodeRecord record) { if (record != null) records.Remove(record); }
    public bool Contains(Vertex node)
    {
        return Find(node) != null;
    }
    public NodeRecord Find(Vertex node)
    {
        if (node == null || records == null || records.Count == 0) return null;

        foreach (NodeRecord record in records)
        {
            if (record != null && record.Connection != null && record.Connection.ToNode != null)
            {
                if (record.Connection.ToNode == node)
                    return record;
            }
        }

        return null;
    }
    public NodeRecord SmallestElement()
    {
        if (records == null || records.Count == 0) 
            return null;
        return records.Min();
    }

    public int Length() { return records.Count; }
}
