using Godot;
using System;

public partial class Game : Node2D
{
	[Export] public int Gold = 100;
	[Export] public int Food = 50;
	[Export] public int Wood = 20;
	[Export] public int Stone = 20;
	[Export] public int Iron = 0;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
