using UnityEngine;

namespace Benco.Graph
{
    public interface IEdge
    {
        INode source { get; set; }
        INode destination { get; set; }
    }
}
