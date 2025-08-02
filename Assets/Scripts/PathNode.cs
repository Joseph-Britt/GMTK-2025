using UnityEngine;

public class PathNode {

    public Vector2Int cell;
    public int g;
    public float h;
    public PathNode previous;

    public PathNode(Vector2Int cell, int g, float h, PathNode previous) {
        this.cell = cell;
        this.g = g;
        this.h = h;
        this.previous = previous;
    }

}
