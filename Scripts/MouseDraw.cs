using Godot;
using System;

public partial class MouseDraw : Node2D
{
    [Export] private Line2D line;

    public Func<Vector2, bool> IsValidPlacement = _ => true;
    public Func<Vector2, bool> IsTarget = _ => true;

    [Signal]
    delegate void OnCompleteEventHandler(Vector2I[] edges);
    
    public override void _Process(double delta)
    {
        var mousePosition = GetViewport().GetCamera2D()?.GetGlobalMousePosition() ??  GetViewport().GetMousePosition();
        if (Input.IsActionJustPressed("left_click"))
        {
            AddEdge(mousePosition);
        }
        
        if (Input.IsActionJustPressed("right_click"))
        {
            PopEdge();
        }

        if (line.Points.Length > 0)
        {
            line.SetPointPosition(line.Points.Length - 1, mousePosition);
        }
    }

    private void AddEdge(Vector2 pos)
    {
        if (!IsValidPlacement(pos))
        {
            return;
        }

        var isTarget = IsTarget(pos);
        var isFirst = line.Points.Length == 0;

        // Reject if the starting node wasn't a valid starting target.
        if (isFirst && !isTarget)
        {
            return;
        }
        
        // One node ahead so the last one can be previewed.
        if (isFirst)
        {
            line.AddPoint(pos);
        }

        line.AddPoint(pos);
    }

    private void PopEdge()
    {
        if (line.Points.Length == 2)
        {
            line.RemovePoint(0);
        }
        
        line.RemovePoint(line.Points.Length - 1);
    }

    private void Complete()
    {
        
    }
}
