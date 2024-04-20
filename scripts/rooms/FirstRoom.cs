using DialogueManagerRuntime;
using Godot;
using System;

public partial class FirstRoom : BaseRoom
{
    [Export]
    public Resource DialogueResource { get; set; }

    public override async void OnReady()
    {
        base.OnReady();
        global.CanWalk = false;      
        await PlayCutscene("player_intro");     
        DialogueManager.ShowDialogueBalloon(DialogueResource, "intro_cutscene");
        global.CanWalk = true;
    }
}