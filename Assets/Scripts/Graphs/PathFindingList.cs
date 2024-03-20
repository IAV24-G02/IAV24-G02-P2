using System.Collections.Generic;
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
            if (record != null && record.PreviousNode != null && record.PreviousNode.Node != null)
            {
                if (record.PreviousNode.Node == node)
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

    public void Clear() { records.Clear(); }
}
