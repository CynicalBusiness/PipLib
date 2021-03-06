![Piplib](/PipLib/res/logo.png)

**Powerful Modding Library for Klei's _Oxygen Not Included_**

![License](https://img.shields.io/badge/license-GPL--v3.0-blue?style=flat-square)
![PipLib Version](https://img.shields.io/badge/dynamic/json?color=blue&label=version&query=%24%5B0%5D.name&url=https%3A%2F%2Flab.vevox.io%2Fapi%2Fv4%2Fprojects%2F29%2Frepository%2Ftags&style=flat-square)
![Pipeline](https://lab.vevox.io/games/oxygen-not-included/piplib/badges/master/pipeline.svg?style=flat-square)

---

# Features

PipLib, on its own, does not do much to change up the game (there are some though, see below), but it provides a powerful
foundation for mods to add new and exciting things to Oxygen Not Included.

-   **Streamlined Mod Loading**
    -   Have your mod loaded by the library, not the other way around
    -   Simply mod class to implement rather than having to register your mod with anything
-   **Custom Elements**
    -   Create new elements and substances within the game with ease
-   **Overhauled Research Tree**
    -   New dynamic research tree that is built on-the-fly instead of hard-coded in the assets
    -   Ability to add new custom techs for your new stuff!
-   **Utilities for Buildings and Tags**
    -   Utility methods for buildings, such as adding to plan screens and techs.
    -   Ability to easily augment tags, both custom and vanilla.

### Changes to the Game

PipLib does make some minor changes to the game to facilitate new things.

-   **The vanilla research tree has been reorganized:** This is a result of the tree now being dynamic
-   **Element files are `.yml` not `.yaml`:** To avoid a bug with the vanilla YAML loader, a different extension is used

## Usage

To learn how to create mods using PipLib, you can either read through the documentation located on each method or view some examples
over on [my other mods](../matts-mods). Many of these mods are excellent examples of how to use PipLib and different aspects of it.

Steam versions are unlisted but are available on the workshop [here](https://steamcommunity.com/sharedfiles/filedetails/?id=1880615893).
You can find both non-Steam mod versions as well as the release DLL/XML on our [Lab](https://lab.vevox.io/games/oxygen-not-included/piplib/-/tags).

**Do not** pack PipLib with your mod: this _will_ break things!. Instead, upload your mod by itself to the workshop, then use the `Add/Remove Required Items` option when editing the item.

## Contributing

_Please bear in mind when contributing: this project was designed to work in Visual Studio Code `>=1.38` (with OmniSharp), not Visual Studio. Attempting to build or edit this project in VS will fail and any PRs will be rejected._

If you have a suggestion, feel free to submit an issue, fork, or contact me on Discord (see below). Contribution
guidelines will appear when submitting an issue.

---

Designed and Developed by [@CynicalBusiness](/CynicalBusiness).  
Reach me on Discord at `CynicalBusiness#0001` or the [Oxygen Not Included Discord Server](https://discord.gg/EBncbX2).

_Myra ta Hayzel; Pal, Kifitae te Entra en na Loka_
