[img]https://i.imgur.com/MzSA54j.png[/img]
[b]Powerful Modding Library for Klei's [i]Oxygen Not Included[/i][/b]

[h1]Features[/h1]
PipLib, on its own, does not do much to change up the game (there are some though, see below), but it provides a powerful foundation for mods to add new and exciting things to Oxygen Not Included.
[list]
[*][b]Streamlined Mod Loading[/b]
  [list]
  [*] Have your mod loaded by the library, not the other way around
  [*] Simply mod class to implement rather than having to register your mod with anything
  [/list]
[*][b]Custom Elements[/b]
  [list]
  [*] Create new elements and substances within the game with ease
  [/list]
[*][b]Overhauled Research Tree[/b]
  [list]
  [*] New dynamic research tree that is built on-the-fly instead of hard-coded in the assets
  [*] Ability to add new custom techs for your new stuff!
  [/list]
[*][b]Utilities for Buildings and Tags[/b]
  [list]
  [*] Utility methods for buildings, such as adding to plan screens and techs.
  [*] Ability to easily augment tags, both custom and vanilla.
  [/list]
[/list]


[h1]Changes to the Game[/h1]
PipLib does make some minor changes to the game to facilitate new things.
[list]
[*] [b]The vanilla research tree has been reorganized:[/b] This is a result of the tree now being dynamic
[*] [b]Element files are `.yml` not `.yaml`:[/b] To avoid a bug with the vanilla YAML loader, a different extension is used
[/list]


[h1]Usage[/h1]
To learn how to create mods using PipLib, you can either read through the documentation located on each method or view some examples over on [url=https://lab.vevox.io/games/oxygen-not-included/matts-mods]my other mods[/url]. Many of these mods are excellent examples of how to use PipLib and different aspects of it.

Steam versions are unlisted but are available on the workshop [url=https://steamcommunity.com/sharedfiles/filedetails/?id=1880615893]here[/url].
You can find both non-Steam mod versions as well as the release DLL/XML on the [url=https://lab.vevox.io/games/oxygen-not-included/piplib/-/tags]GitLab[/url].

[b]Do not[/b] pack PipLib with your mod: this [i]will[/i] break things!. Instead, upload your mod by itself to the workshop, then use the [b]Add/Remove Required Items[/b] option when editing the item.

[h1]Contributing[/h1]
[i]Please bear in mind when contributing: this project was designed to work in Visual Studio Code [b]>=1.38[/b] (with OmniSharp), not Visual Studio. Attempting to build or edit this project in VS will fail and any PRs will be rejected.[/i]

If you have a suggestion, feel free to submit an issue, fork, or contact me on Discord (see below). Contribution guidelines will appear when submitting an issue.
Source code and issue tracking can be found on my [url=https://lab.vevox.io/games/oxygen-not-included/piplib]GitLab[/url].


[h1]----[/h1]
Designed and Developed by [url=https://lab.vevox.io]@CynicalBusiness[/url].
Reach me on Discord at [b]CynicalBusiness#0001[/b] or the [url=https://discord.gg/EBncbX2]Oxygen Not Included Discord Server[/url].


[i]Myra ta Hayzel; Pal, Kifitae te Entra en na Loka[/i]
