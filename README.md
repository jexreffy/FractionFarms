# Fraction Farms

An interactive educational game that teaches students how to work with fractions. Built in Unity and deployed as a WebGL application through the League of Legends Learning (LoL SDK) platform, it guides students through three progressive lesson modules using a visual, tile-based fraction model.

## What Students Learn

Students work through three lessons in sequence:

| Lesson | Concept | What they do |
|---|---|---|
| **Basic Fractions** | Numerator / denominator | Set a grid size and select the correct number of tiles |
| **Improper Fractions** | Values greater than 1 | Span tiles across multiple unit grids |
| **Mixed Fractions** | Whole + fractional part | Pick the whole number and the fractional remainder separately |

Each lesson presents a series of instruction steps (with animations) followed by interactive problems. A hint system activates after 5 seconds if the student hasn't answered, and incorrect attempts are flagged with feedback before the student tries again. Score increases for correct answers and decreases for errors.

## How It Works

1. **Title screen** → student presses Start
2. Three lesson scenes play in order: Basics → Improper Fractions → Mixed Fractions
3. Each scene loads question data from JSON (platform-provided in production, mock data in the editor)
4. **Ending screen** shows final score and completion

Accessibility features include text-to-speech via the LoL SDK and full language localisation support through `StreamingAssets/language.json`.

## Tech Stack

- **Engine:** Unity 2019.1.10f1
- **Language:** C#
- **Build target:** WebGL
- **Platform SDK:** LoL (League of Legends Learning) SDK — handles authentication, analytics, and game data delivery
- **UI:** TextMeshPro
- **Data:** JSON (SimpleJSON) for questions and localised strings

## Project Structure

```
Assets/Jexreffy/
  Scripts/
    Controllers/
      SectionController.cs    # Base class for all lesson sections
      BasicController.cs      # Basic fractions lesson logic
      ImproperController.cs   # Improper fractions lesson logic
      MixedController.cs      # Mixed fractions lesson logic
    UI/
      UnitTile.cs             # Interactive fraction tile component
      SequenceStep.cs         # Defines a single instruction or problem step
      TitleButtons.cs         # Start screen interaction
      EndingScreen.cs         # Completion / score display
    LoL/
      PlatformController.cs   # LoL SDK integration, scoring, analytics
  Scenes/
    Initialization.unity
    Title.unity
    Basics.unity
    ImproperFractions.unity
    MixedFractions.unity
    Ending.unity
Assets/StreamingAssets/
  language.json     # Localised UI strings and instructions
  questions.json    # Question/problem data (mock data for editor)
  startGame.json    # Game initialisation payload
```

## Setup

1. Install **Unity 2019.1.10f1** (see `ProjectSettings/ProjectVersion.txt`)
2. Open the project and load the **Initialization** scene
3. Press **Play** — the editor uses mock data from `StreamingAssets/` automatically

### Editor vs WebGL build

| Environment | Data source |
|---|---|
| Unity Editor | `StreamingAssets/` JSON files (mock data) |
| WebGL (LoL platform) | Live data injected by the LoL SDK at runtime |

To produce a production build, target **WebGL** and deploy through the LoL platform — the SDK handles data delivery, scoring submission, and analytics automatically.
