namespace Game_Realtime.Service;

 public class MapGraph
    {
        Dictionary<Vector2Int, LinkedList<Vector2Int>> linkedListArray;
        private int mapHeight;
        private int mapWidth;
        private int columnIndexSplit;
        public MapGraph(int height, int width)
        {
            linkedListArray = new Dictionary<Vector2Int, LinkedList<Vector2Int>>();
            this.mapHeight = height;
            this.mapWidth = width;
            this.columnIndexSplit = (height - 1) / 2;
        }

        /// The method takes two nodes for which to add edge.
        public void AddEdge(Vector2Int u, Vector2Int v, bool blnBiDir = true)
        {
            LinkedList<Vector2Int> value;
            if (linkedListArray.TryGetValue(u, out value))
            {
                var last = value.Last;
                value.AddAfter(last, v);
            }
            else
            {
                linkedListArray.Add(u, new LinkedList<Vector2Int>());
                linkedListArray[u].AddFirst(v);
            }

            if (blnBiDir)
            {
                if (linkedListArray.TryGetValue(v, out value))
                {
                    var last = value.Last;
                    value.AddAfter(last, u);
                }
                else
                {
                    linkedListArray.Add(v, new LinkedList<Vector2Int>());
                    linkedListArray[v].AddFirst(u);
                }
            }
        }

        internal GenerateMapNodeCost DFSHelper(Vector2Int src, Vector2Int des, GenerateMapNodeCost cost)
        {
            if (src == des)
            {
                return cost;
            }
            GenerateMapNodeCost bestCost = null;
            if (linkedListArray[src] != null)
            {
                foreach (var item in linkedListArray[src])
                {
                    var costClone = cost.Clone();
                    var path = cost.path;
                    // check if the item is in path or adjacent any node in path
                    if (path.Contains(item))
                    {
                        continue;
                    }
                    bool isValid = true;
                    for (int i = 0; i < path.Count - 1; i++)
                    {
                        var node = path[i];
                        if ((node.x > 0 && item.x == node.x - 1 && item.y == node.y)
                            || (node.x < mapWidth - 1 && item.x == node.x + 1 && item.y == node.y)
                            || (node.y > 0 && item.x == node.x && item.y == node.y - 1)
                            || (node.y < mapHeight - 1 && item.x == node.x && item.y == node.y + 1))
                        {
                            isValid = false;
                            break;
                        }
                    }
                    if (!isValid)
                    {
                        continue;
                    }
                    // check if the item is adjacent any holes
                    LinkedList<Vector2Int> value;
                    if ((item.y > 0 && !linkedListArray.TryGetValue(new Vector2Int(item.x, item.y - 1), out value))
                        || (item.y < mapHeight - 1 && !linkedListArray.TryGetValue(new Vector2Int(item.x, item.y + 1), out value))
                        || (item.x > columnIndexSplit + 1 && !linkedListArray.TryGetValue(new Vector2Int(item.x - 1, item.y), out value))
                        || (item.x < mapWidth - 2 && !linkedListArray.TryGetValue(new Vector2Int(item.x + 1, item.y), out value))
                        || (item.y > 0 && item.x > columnIndexSplit + 1 && !linkedListArray.TryGetValue(new Vector2Int(item.x - 1, item.y - 1), out value))
                        || (item.y < mapHeight - 1 && item.x > columnIndexSplit + 1 && !linkedListArray.TryGetValue(new Vector2Int(item.x - 1, item.y + 1), out value))
                        || (item.y > 0 && item.x < mapWidth - 2 && !linkedListArray.TryGetValue(new Vector2Int(item.x + 1, item.y - 1), out value))
                        || (item.y < mapHeight - 1 && item.x < mapWidth - 2 && !linkedListArray.TryGetValue(new Vector2Int(item.x + 1, item.y + 1), out value)))
                    {
                        costClone.nAdjacentNode++;
                    }
                    // increase distance
                    costClone.distance++;
                    // check if the item is a turn
                    if (path.Count > 1
                        && !(item.x - path[path.Count - 1].x == path[path.Count - 1].x - path[path.Count - 2].x
                            && item.y - path[path.Count - 1].y == path[path.Count - 1].y - path[path.Count - 2].y))
                    {
                        costClone.nTurn++;
                    }
                    costClone.path.Add(item);
                    var checkCost = DFSHelper(item, des, costClone);
                    if (bestCost == null || (checkCost != null && checkCost > bestCost))
                    {
                        bestCost = checkCost;
                    }
                }
            }
            return bestCost;
        }

        internal List<Vector2Int> DFS(Vector2Int src, Vector2Int des)
        {
            GenerateMapNodeCost cost = new GenerateMapNodeCost();
            cost.path.Add(src);
            // Stopwatch stopwatch = new Stopwatch();
            // stopwatch.Start();
            var resCost = DFSHelper(src, des, cost);
            // stopwatch.Stop();
            // UnityEngine.Debug.Log("Elapsed Time is {0} ms" + stopwatch.ElapsedMilliseconds.ToString());
            if (resCost == null)
            {
                throw new Exception("Path is null. Maybe the graph is invalid.");
            }
            return resCost.path;
        }
    }