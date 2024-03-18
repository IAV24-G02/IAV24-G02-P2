using System.Collections;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;
using UCM.IAV.Navegacion;

public class PathFindingList : MonoBehaviour
{
    private List<NodeRecord> records; // Lista de nodos
    
    void Awake()
    {
        records = new List<NodeRecord>();
    
    }

    public void Add(NodeRecord record) {  records.Add(record); }
    public void Remove(NodeRecord record) { records.Remove(record); }
    public bool Contains(Vertex node) {  return records.Find(v => v == node) != null; }
    public NodeRecord Find(Vertex node) { return records.Find(v => v == node); }
    public NodeRecord SmallestElement()
    {
        if (records == null || records.Count == 0) return null;
        NodeRecord smallest = records[0];
        foreach (NodeRecord record in records)
        {
            if (record.EstimatedTotalCost < smallest.EstimatedTotalCost)
            {
                smallest = record;
            }
        }
        return smallest;     
    }

    public int Length() { return records.Count; }
}
