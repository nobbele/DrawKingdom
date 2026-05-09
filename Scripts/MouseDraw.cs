using Godot;
using System;

public partial class MouseDraw : Node2D
{
    [Export] private Line2D line;
    
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
        if (line.Points.Length == 0)
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
}
