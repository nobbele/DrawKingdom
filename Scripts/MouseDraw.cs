using Godot;
using System;
using Godot.Collections;

public partial class MouseDraw : Node2D
{
    [Export] private Line2D _line;

    public Func<Vector2, bool> IsValidPlacement = _ => true;
    public Func<Vector2, bool> IsTarget = _ => true;

    [Signal]
    public delegate void OnCompleteEventHandler(Array<Vector2> edges);
    
    public override void _Process(double delta)
    {
        if (IsQueuedForDeletion())
            return;
        
        var mousePosition = GetViewport().GetCamera2D()?.GetGlobalMousePosition() ??  GetViewport().GetMousePosition();
        if (Input.IsActionJustPressed("left_click"))
        {
            AddEdge(mousePosition);
        }
        
        if (Input.IsActionJustPressed("right_click"))
        {
            PopEdge();
        }

        if (_line.Points.Length > 0)
        {
            _line.SetPointPosition(_line.Points.Length - 1, mousePosition);
        }
    }

    private void AddEdge(Vector2 pos)
    {
        if (!IsValidPlacement(pos)) return;

        var isTarget = IsTarget(pos);
        var isFirst = _line.Points.Length == 0;

        // Reject if the starting node wasn't a valid starting target.
        if (isFirst && !isTarget) return;

        // Complete the path if we found a second target.
        if (!isFirst && isTarget)
        {
            Complete();
            return;
        }
        
        // One node ahead so the last one can be previewed.
        if (isFirst) 
            _line.AddPoint(pos);

        _line.AddPoint(pos);
    }

    private void PopEdge()
    {
        if (_line.Points.Length == 2)
            _line.RemovePoint(0);
        
        _line.RemovePoint(_line.Points.Length - 1);
    }

    private void Complete()
    {
        EmitSignalOnComplete(new Array<Vector2>(_line.Points));
        QueueFree();
    }
}
