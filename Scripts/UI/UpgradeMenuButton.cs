using Godot;
using System;

public partial class UpgradeMenuButton : Button
{
	// Called when the node enters the scene tree for the first time.
	[Export]
	private Control UpgradeMenu;
	public override void _Ready()
	{
		Pressed += OnPressed;
	}

	public void OnPressed()
	{
	
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
