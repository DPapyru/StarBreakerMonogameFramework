# GameMake

A YAML-driven UI framework for narrative-focused games, built on MonoGame.

## Language

**Layout**:
A YAML file defining a hierarchy of Entities, their Components, and their relationships.
_Avoid_: UI definition, screen, view

**Scene**:
A named group of Entities within a Layout, representing a distinct game screen (e.g., MainMenu, GameHUD). A Scene can be entered and exited; entering makes it visible, exiting hides it. Only one Scene is active at a time.
_Avoid_: screen, state, mode

**Entity**:
A node in the runtime UI tree, identified by a unique string Id. An Entity has a parent, children, and a list of Components. No behavior — it's a container.
_Avoid_: element, widget, control

**Component**:
A data-only attachable block on an Entity that defines appearance (RectTransform, ButtonRenderer, TextRenderer) or behavior reference (ScriptComponent). Components have no logic — they are read by Systems.
_Avoid_: attribute, property, module

**Script**:
A C# class compiled at runtime via Roslyn. Scripts attach to Entities via ScriptComponent and expose optional lifecycle hooks (OnCreate, OnClick, etc.). Scripts are stateless by design — state lives in Components.
_Avoid_: behaviour, action, callback

**System**:
A stateless processor that reads Components from the Entity tree to produce behavior. RenderSystem renders, LayoutSystem computes positions, InputDispatcher handles hits.
_Avoid_: manager, service, handler

**Mod**:
A self-contained content/behavior extension package. Mods can add or override YAML entities by Id and provide new Scripts. Future: component registration API, script sandboxing, versioned dependencies.
_Avoid_: plugin, addon, DLC

**Hot Reload**:
Automatic recompilation of Scripts (L1) or full Layout reload (L2) triggered by file changes via FileSystemWatcher. State resets on reload for deterministic debugging.
_Avoid_: live reload, refresh

**Layout Pipeline**:
The data flow from YAML file to runtime Entity tree: Deserialize → Merge (mod overlays) → Build (YAML → Entities with Components). Re-runs on L2 hot reload.

## Relationships

- A **Layout** contains one or more **Scenes**
- A **Scene** groups multiple **Entities** under a shared name
- An **Entity** has zero or more **Components**
- A **Script** attaches to an **Entity** via a **ScriptComponent**
- **Systems** read **Components** from the **Entity** tree; they never own state
- A **Mod** overlays a **Layout** by matching **Entity** Ids

## Example dialogue

> **Dev:** "When the player clicks the start button, do I switch Scenes or toggle Entity visibility?"
> **Domain expert:** "Switch Scenes. The MainMenu Scene gets replaced by the GameHUD Scene. The old Scene's resources unload when the new one loads."
> **Dev:** "So the Script calls `_ui.SwitchScene("GameHUD")`?"
> **Domain expert:** "Correct. Never walk the tree and flip Visible by hand."

**DialogueRenderer**:
A Component that renders a single dialogue line: speaker name, text (with typewriter effect), avatar, and option buttons for Branch selections. DialogueRenderer is **reactive** — it displays whatever state it's given and never drives the story forward itself.
_Avoid_: dialog box, text window

**ScriptParser**:
A framework-provided utility that reads a script YAML file and produces a `List<ScriptInstruction>`. Instructions are: Say, Branch, Label, Jump, Callback. The ScriptParser does NOT execute instructions — it only parses.
_Avoid_: script engine, dialogue engine

**ScriptInstruction**:
A single parsed node from a script file: one of Say (speaker, text, expression, speed, autoNext), Branch (text, options), Label (name), Jump (target), Callback (name).
_Avoid_: command, opcode

**SpeakerConfig**:
A YAML-defined mapping from speaker ID to display properties: DisplayName, Expressions (dictionary of expression name → avatar path), default Color. Loaded via `SpeakerConfig.Load(path)` into a dictionary.
_Avoid_: character sheet, role definition

**AvatarSprite**:
A Component that displays a character portrait (full-body or bust) on screen, typically updated when dialogue advances. Supports crossfade transitions between expressions.
_Avoid_: character sprite, portrait

**GameSample**:
The sample game project (`GameSample/`) is not a framework demo — it IS the game "碎筑成序". It serves as both the first-party game and the canonical example of how to use the framework. Changes to the framework must keep GameSample runnable.

## Flagged ambiguities

- "ECS" was used loosely — this is an Entity-Component tree with standalone Systems, not a classical ECS archetype engine. The term "System" is kept but means "stateless processor," not "ECS System."
- "Screen" and "scene" were used interchangeably — resolved to **Scene** as the canonical term.
- "Hot reload" vs "live reload" — resolved to **Hot Reload** per Unity/game industry convention.
