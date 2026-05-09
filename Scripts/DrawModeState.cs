using Godot;

public class DrawModeState
{
    public required MouseDraw MouseDraw;
    public IRoadConnectionPoint Start;
    public IRoadConnectionPoint End;
    public Vector2[] Edges;
}