namespace JH24Utils.Graphs;

public static class GraphUtils
{
    /// <summary>
    /// Standard Dijkstra's algorithm for distance to all nodes in the graph.
    /// </summary>
    /// <param name="start">The starting node (distance 0).</param>
    /// <returns>Dictionary of INode node to int distance from start and including start node.</returns>
    public static Dictionary<INode, int> Dijkstras(INode start)
    {
        Dictionary<INode, int> connections = new() { { start, 0 } };
        Dictionary<INode, int> paths = new();

        while (connections.Count > 0)
        {
            (INode node, int distance) = connections.MinBy(c => c.Value);
            connections.Remove(node);
            paths[node] = distance;

            foreach ((INode newNode, int arcDistance) in node.Arcs)
            {
                if (paths.ContainsKey(newNode)) continue;

                int newDistance = distance + arcDistance;
                if (connections.TryGetValue(newNode, out int oldDistance))
                {
                    if (newDistance < oldDistance)
                        paths[newNode] = newDistance;
                }
                else connections[newNode] = newDistance;
            }
        }

        return paths;
    }

    public static HashSet<Node> CreateGraph(string input)
    {
        HashSet<Node> nodes = new();
        Dictionary<string, (Node node, (string name, int weight)[] connections)> nodeLegend = new();
        string[] lines = input.Split("\r\n");
        for (int i = 0; i < lines.Length; i++)
        {
            string[] words = lines[i].Split(',');
            string name = words[0];
            (string, int)[] connections = words[1..].Select(c => c.Split(':')).Select(c => (c[0], int.Parse(c[1]))).ToArray();
            Node node = new(new());
            nodes.Add(node);
            nodeLegend.Add(name, (node, connections));
        }

        foreach (var kvp in nodeLegend)
        {
            kvp.Value.node.Arcs = kvp.Value.connections.ToDictionary(c => (INode)nodeLegend[c.name].node, c => c.weight);
        }

        return nodes;
    }
}