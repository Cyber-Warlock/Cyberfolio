using System.Collections.Generic;
using UnityEngine;

public abstract class SO_Labyrinth_Manager_Base : MonoBehaviour
{
    /// <summary>
    /// Returns the collection of the SO_Labyrinths. Use it to get one for generation during runtime.
    /// </summary>
    public abstract List<SO_Labyrinth> Labyrinths { get; }

    /// <summary>
    /// For specifying which SO_Labyrinth to generate in EditMode.
    /// Always returns 0 when not in EditMode.
    /// </summary>
    public abstract int GenerateIndex { get; }

    public abstract void Generate(SO_Labyrinth l);
}
