namespace JH24Utils.Graphs;

public class Node : INode
{
    private Dictionary<INode, int> arcs;
    private int? value;
    public Dictionary<INode, int> Arcs { get => arcs; set => arcs = value; }
    public int? Value { get => value; set => this.value = value; }
    public Node(Dictionary<INode, int> arcs, int? value) : this(arcs) => this.value = value;
    public Node(Dictionary<INode, int> arcs) => this.arcs = arcs;
}
