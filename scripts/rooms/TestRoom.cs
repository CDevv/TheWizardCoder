using Godot;
using System;

public partial class TestRoom : BaseRoom
{
	private async void TestCutscene()
	{
		await PlayCutscene("test_anim");
	}
}
