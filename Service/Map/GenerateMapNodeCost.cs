namespace Game_Realtime.Service;

public class GenerateMapNodeCost
{
    public List<Vector2Int> path { get; set; }
    public int nAdjacentNode { get; set; }
    public int distance { get; set; }
    public int nTurn { get; set; }
    public GenerateMapNodeCost()
    {
        path = new List<Vector2Int>();
        nAdjacentNode = 0;
        distance = 0;
        nTurn = 0;
    }
    public GenerateMapNodeCost Clone()
    {
        var clone = new GenerateMapNodeCost();
        clone.path = new List<Vector2Int>(path);
        clone.nAdjacentNode = nAdjacentNode;
        clone.distance = distance;
        clone.nTurn = nTurn;
        return clone;
    }
    public static bool operator >(GenerateMapNodeCost lhs, GenerateMapNodeCost rhs)
    {
        if (lhs.nAdjacentNode > rhs.nAdjacentNode) return true;
        if (lhs.nAdjacentNode < rhs.nAdjacentNode) return false;
        if (lhs.distance > rhs.distance) return true;
        if (lhs.distance < rhs.distance) return false;
        if (lhs.nTurn > rhs.nTurn) return true;
        if (lhs.nTurn < rhs.nTurn) return false;
        return false;
    }
    public static bool operator <(GenerateMapNodeCost lhs, GenerateMapNodeCost rhs)
    {
        if (lhs.nAdjacentNode > rhs.nAdjacentNode) return false;
        if (lhs.nAdjacentNode < rhs.nAdjacentNode) return true;
        if (lhs.distance > rhs.distance) return false;
        if (lhs.distance < rhs.distance) return true;
        if (lhs.nTurn > rhs.nTurn) return false;
        if (lhs.nTurn < rhs.nTurn) return true;
        return false;
    }
}