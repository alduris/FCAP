# Five Cycles at Pebbles (FCAP)

A fully-fledged Five Nights at Freddy's-inspired gamemode in Rain World!

## Credits

* Alduris - Programming, concept, assets, writing, nightcat's ballad
* Beep - Artist (thank you so much, you did such an amazing job!)
* DeepL.com - Initial translations
* VerityOfFaith - Chinese translation help
* Detrax, Mold223, TempleOfArt - Russian translation help
* CHEPPR - Korean translation help
* Portal79 - French translation help
* Aspari - Spanish translation help

## Installing

If you have Steam, you can install it through the Steam Workshop: https://steamcommunity.com/sharedfiles/filedetails/?id=3357397880

If you do not have Steam, download the [latest release](https://github.com/alduris/FCAP/releases/latest) (only download "fcap-1.x.x.zip"!) and put its contents in your Rain World's mods folder. If you're unsure where that is, locate your Rain World installation folder, then go to `./RainWorld_Data/StreamingAssets/mods`. You will also need to install [Slugbase](https://github.com/SlimeCubed/SlugBaseRemix/releases/latest).


## Building

All references necessary to build the code are included, so all that should be necessary is forking it and opening the solution file. Builds directly to `mod/plugins`. I recommend setting up a symbolic link in your Rain World's "mods" folder linking to the "mod" folder in this repository, as it will make development easier.

## Contributing

I did an initial pass-over of all the translations using the website DeepL.com, though they are not perfect and I need proofreaders! If you are fluent in one of these languages and English and would like to correct a translation, it would be greatly appreciated! The following are where the translations are stored and their reference material:

* Phone guy scripts are in `mod/phoneguy/<language>/` and the reference texts are in `mod/phoneguy/eng`
* Menu text is in `mod/text/text_<language>/strings.txt`, reference text is included in the file.