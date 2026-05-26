using System;
using System.Collections.Generic;
using System.IO;
using GameMake.UI;
using GameMake.UI.Core;
using GameMake.UI.Core.Components;
using Scripting = GameMake.UI.Scripting;

/// <summary>
/// Dialogue controller that advances through chapter scripts on each click.
/// Used via ScriptComponent referencing Scripts/GameDialogController.cs.
/// Expects a DialogueRenderer (and optionally AvatarSprite) on the same entity.
/// </summary>
public class GameDialogController
{
    /// <summary>
    /// Persisted state across OnCreate / OnClick calls, stored as ctx.
    /// </summary>
    public class DialogState
    {
        public List<Scripting.ScriptInstruction> Instructions { get; set; } = new();
        public int CurrentIndex { get; set; } = 0;
        public Dictionary<string, Scripting.SpeakerDef> Speakers { get; set; } = new();
        public bool IsActive { get; set; } = false;
    }

    public static object OnCreate(UIEntity self)
    {
        var state = new DialogState();
        var baseDir = AppDomain.CurrentDomain.BaseDirectory;

        // Load speaker definitions
        var speakersPath = Path.Combine(baseDir, "Scripts", "speakers.yaml");
        state.Speakers = Scripting.SpeakerConfig.Load(speakersPath);
        if (state.Speakers.Count > 0)
        {
            Console.WriteLine($"[Dialog] Loaded {state.Speakers.Count} speaker(s)");
        }

        // Load and parse the first chapter script
        var scriptPath = Path.Combine(baseDir, "Scripts", "chapter1.yaml");
        if (File.Exists(scriptPath))
        {
            var yaml = File.ReadAllText(scriptPath);
            state.Instructions = Scripting.ScriptParser.Parse(yaml);
            Console.WriteLine($"[Dialog] Loaded {state.Instructions.Count} instruction(s)");
        }

        // Apply the first instruction to the dialogue renderer
        var dialogue = self.Get<DialogueRenderer>();
        if (dialogue != null && state.Instructions.Count > 0)
        {
            ApplyInstruction(self, dialogue, state.Instructions[0], state.Speakers);
            state.IsActive = true;
        }

        return state;
    }

    public static void OnClick(UIEntity self, object ctx)
    {
        var state = (DialogState)ctx;
        var dialogue = self.Get<DialogueRenderer>();
        if (dialogue == null || !state.IsActive || state.Instructions.Count == 0)
            return;

        // Fast-forward if text is still typing out
        if (dialogue.State == DialogueState.Typing)
        {
            dialogue.FastForward();
            return;
        }

        // Advance to the next instruction
        if (dialogue.State == DialogueState.WaitingForClick ||
            dialogue.State == DialogueState.Idle)
        {
            state.CurrentIndex++;
            ExecuteCurrent(self, state, dialogue);
        }
        // During a branch, click selects the first option by default
        else if (dialogue.State == DialogueState.Branching)
        {
            SelectBranchOption(self, state, dialogue, 0);
        }
    }

    // ----------------------------------------------------------------
    // Internal helpers
    // ----------------------------------------------------------------

    static void ExecuteCurrent(UIEntity self, DialogState state, DialogueRenderer dialogue)
    {
        // End of script
        if (state.CurrentIndex >= state.Instructions.Count)
        {
            dialogue.DialogueText = "";
            dialogue.SpeakerName = "";
            dialogue.State = DialogueState.Idle;
            state.IsActive = false;
            Console.WriteLine("[Dialog] Script complete");
            return;
        }

        var inst = state.Instructions[state.CurrentIndex];

        switch (inst.Type)
        {
            case Scripting.ScriptInstructionType.Say:
                ApplyInstruction(self, dialogue, inst, state.Speakers);
                break;

            case Scripting.ScriptInstructionType.Branch:
                dialogue.DialogueText = inst.Text ?? "";
                dialogue.Options = new List<BranchOption>();
                if (inst.Options != null)
                {
                    foreach (var o in inst.Options)
                        dialogue.Options.Add(new BranchOption { Text = o.Text, JumpTarget = o.JumpTarget });
                }
                dialogue.State = DialogueState.Branching;
                // If only one branch option exists, auto-select it
                if (dialogue.Options.Count == 1)
                    SelectBranchOption(self, state, dialogue, 0);
                break;

            case Scripting.ScriptInstructionType.Label:
                // Labels are no-ops – advance past them
                state.CurrentIndex++;
                ExecuteCurrent(self, state, dialogue);
                break;

            case Scripting.ScriptInstructionType.Jump:
                var idx = FindLabelIndex(state.Instructions, inst.Target);
                if (idx >= 0)
                {
                    state.CurrentIndex = idx;
                    ExecuteCurrent(self, state, dialogue);
                }
                else
                {
                    Console.WriteLine($"[Dialog] Label '{inst.Target}' not found, skipping");
                    state.CurrentIndex++;
                    ExecuteCurrent(self, state, dialogue);
                }
                break;

            case Scripting.ScriptInstructionType.Callback:
                HandleCallback(inst.CallbackName, self, state);
                state.CurrentIndex++;
                ExecuteCurrent(self, state, dialogue);
                break;
        }
    }

    static void ApplyInstruction(UIEntity self, DialogueRenderer dialogue,
        Scripting.ScriptInstruction inst, Dictionary<string, Scripting.SpeakerDef> speakers)
    {
        dialogue.DialogueText = inst.Text ?? "";
        dialogue.SpeakerName = "";
        dialogue.TextColor = Microsoft.Xna.Framework.Color.White;

        if (!string.IsNullOrEmpty(inst.Speaker) && speakers.TryGetValue(inst.Speaker, out var speaker))
        {
            dialogue.SpeakerName = speaker.DisplayName ?? inst.Speaker;
            if (speaker.Color.HasValue)
                dialogue.TextColor = speaker.Color.Value;
        }
        else if (!string.IsNullOrEmpty(inst.Speaker))
        {
            dialogue.SpeakerName = inst.Speaker;
        }

        if (inst.Speed.HasValue)
            dialogue.TextSpeed = inst.Speed.Value;

        dialogue.DisplayProgress = 0f;
        dialogue.Options?.Clear();
        dialogue.State = DialogueState.Typing;

        // Update avatar expression if AvatarSprite is present
        UpdateAvatar(self, inst, speakers);
    }

    static void UpdateAvatar(UIEntity self, Scripting.ScriptInstruction inst,
        Dictionary<string, Scripting.SpeakerDef> speakers)
    {
        var avatar = self.Get<AvatarSprite>();
        if (avatar == null) return;

        if (string.IsNullOrEmpty(inst.Expression) || inst.Speaker == null ||
            !speakers.TryGetValue(inst.Speaker, out var speaker) ||
            !speaker.Expressions.TryGetValue(inst.Expression, out var texPath))
        {
            // No expression – clear the avatar
            avatar.TexturePath = null;
            return;
        }

        avatar.TexturePath = texPath;
    }

    static int FindLabelIndex(List<Scripting.ScriptInstruction> instructions, string name)
    {
        for (int i = 0; i < instructions.Count; i++)
        {
            if (instructions[i].Type == Scripting.ScriptInstructionType.Label &&
                string.Equals(instructions[i].Name, name, StringComparison.OrdinalIgnoreCase))
                return i;
        }
        return -1;
    }

    static void SelectBranchOption(UIEntity self, DialogState state, DialogueRenderer dialogue, int optionIndex)
    {
        if (optionIndex < 0 || optionIndex >= dialogue.Options.Count) return;

        var selected = dialogue.Options[optionIndex];
        dialogue.Options?.Clear();

        if (!string.IsNullOrEmpty(selected.JumpTarget))
        {
            var idx = FindLabelIndex(state.Instructions, selected.JumpTarget);
            if (idx >= 0)
            {
                state.CurrentIndex = idx;
                ExecuteCurrent(self, state, dialogue);
                return;
            }
        }

        state.CurrentIndex++;
        ExecuteCurrent(self, state, dialogue);
    }

    static void HandleCallback(string name, UIEntity self, DialogState state)
    {
        Console.WriteLine($"[Dialog] Callback: {name}");
        switch (name)
        {
            case "OnChapter1End":
                Console.WriteLine("[Dialog] Chapter 1 complete -- switching to game HUD");
                break;
        }
    }
}
