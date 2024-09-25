using Godot;
using System;
using TheWizardCoder.Abstractions;

namespace TheWizardCoder.Rooms
{
	public partial class TestRoom : BaseRoom
	{
		private async void TestCutscene()
		{
			await PlayCutscene("test_anim");
		}
	}
}