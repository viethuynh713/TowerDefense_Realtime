namespace Game_Realtime.Service;

public class HoleTileLinkedList
{
    public LinkedList<InitHole1Node> treeTileLinkedList;
    public int nTree;
    public HoleTileLinkedList(List<Vector2Int> tileList, int nTree)
    {
        // create a link list with the first node is start state
        treeTileLinkedList = new LinkedList<InitHole1Node>();
        treeTileLinkedList.AddLast(new InitHole1Node(tileList, null));
        this.nTree = nTree;
    }
    public List<Vector2Int> createAllHole()
    {
        while (treeTileLinkedList.Count < nTree)
        {
            SelectTile();
        }
        List<Vector2Int> res = new List<Vector2Int>();
        foreach (InitHole1Node node in treeTileLinkedList)
        {
            res.Add(node.selectedTile.Value);
        }
        return res;
    }
    public void SelectTile()
    {
        if (treeTileLinkedList.Last.Value.CanAdd())
        {
            InitHole1Node lastNode = treeTileLinkedList.Last.Value;
            treeTileLinkedList.AddLast(new InitHole1Node(lastNode.restTile, lastNode.selectedTile));
        }
        else
        {
            treeTileLinkedList.RemoveLast();
            treeTileLinkedList.Last.Value.RemoveDo();
        }
    }
}

public class InitHole1Node
{
    public List<Vector2Int> restTile;
    public Vector2Int? selectedTile;
    public InitHole1Node(List<Vector2Int> prevRestTile, Vector2Int? prevSelectedTile)
    {
        restTile = prevRestTile;
        if (prevSelectedTile != null)
        {
            for (int i = prevSelectedTile.Value.x - 1; i <= prevSelectedTile.Value.x + 1; i++)
            {
                for (int j = prevSelectedTile.Value.y - 1; j <= prevSelectedTile.Value.y + 1; j++)
                {
                    restTile.Remove(new Vector2Int(i, j));
                }
            }
        }
        if (restTile.Count == 0)
        {
            selectedTile = null;
        }
        else
        {
            Random random = new Random();
            selectedTile = restTile[random.Next(0, restTile.Count)];
        }
    }
    public bool CanAdd()
    {
        return selectedTile != null;
    }
    public void RemoveDo()
    {
        restTile.Remove(selectedTile.Value);
        Random random = new Random();
        selectedTile = restTile[random.Next(0, restTile.Count +1)];
    }
}