using UnityEngine;

namespace Benco.Graph
{
    // TODO(mderu): Maybe get rid of this too?
    public interface IEdge
    {
        INode source { get; set; }
        INode destination { get; set; }
    }
}
