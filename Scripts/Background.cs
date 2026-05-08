using System;
using Godot;

public partial class Background : Node2D
{
    TileMapLayer tileMapLayer;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        tileMapLayer = GetNode<TileMapLayer>("TileMapLayer");
        tileMapLayer.SetCell(new(0, 0), 0, new(1, 0), 0);
        tileMapLayer.SetCell(new(1, 1), 0, new(2, 0), 0);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) { }
}
