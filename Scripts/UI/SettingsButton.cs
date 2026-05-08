using Godot;
using System;

public partial class SettingsButton : Button
{
	[Export]
	private Control settingsUi;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Pressed += OnPressed;
	}
	
	private void OnPressed()
	{
		if (settingsUi.Visible)
		{
			settingsUi.Visible = false;
		} else
		{
			settingsUi.Visible = true;
		}
	}
	
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
