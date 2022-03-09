# BeatSaber_v1.20.0_Beatmaps_Fixer
Fixer for beatmaps that could not be loaded in Beat Saber

In Beat Saber 1.20.0 was added new beatmaps format. Wit it, in this version was broken support of some beatmap (especially generated via [Beat Sage](https://beatsage.com/) or [Osu2Saber](https://github.com/Ivan-Alone/Osu2Saber)), but also some handmaded beatmaps was broken too (from my collection, [Julius Dreisig - Invisible \(by *iilikechickennuggetz*\)](https://beatsaver.com/maps/1556d), [Egzod & Maestro Chives - Royalty \(by *ProtoPhonix*\)](https://beatsaver.com/maps/1c1b9)).

Listed beatmaps can't be played in Beat Saber 1.20.0 because bug that happens in game dev process.

## Bug description
It problem affect beatmaps, that has any formatting symbols (spaces, \t, \r, \n and etc.) in "difficulty.dat" file. JSON specifications are compatible with formatting, but from 1.20.0 BeatSaber does not.

Example:

```json
{"_version":"2.0.0","_events":[],"_notes":[],"_obstacles":[]}
```
![Ok empty map](https://github.com/Ivan-Alone/BeatSaber_v1.20.0_Beatmaps_Fixer/blob/main/img/ok.jpg?raw=true)

It’s valid empty level beatmap. No spaces, no any formatting, nothing extra. But this:

```json
{
  "_version": "2.0.0",
  "_events": [],
  "_notes": [],
  "_obstacles": []
}
```
![Infinite loading](https://github.com/Ivan-Alone/BeatSaber_v1.20.0_Beatmaps_Fixer/blob/main/img/loading.jpg?raw=true)
is invalid level, and beatmap with it will show infinite loading ring in menu.

Usually, auto generated beatmaps can contain formatting, but it can happen with any custom beatmap created with third-party editor, for example.

## How to fix
1) Wait fix from modding community
2) Wait for update game to new version with fixed algorhytm
3) Download my fixer and fix all you Beatmaps via this.

## Fixer usage
1) Download fixer from [here](https://github.com/Ivan-Alone/BeatSaber_v1.20.0_Beatmaps_Fixer/releases/download/1.20.0/BeatSaberBeatmapsFixer.7z)
2) Unpack it in any directory (DO NOT try to run it from archive!!!)
3) Run `BeatSaberBeatmapsFixer.exe`
4) Select your `CustomLevels` directory (usually, located in `C:\Program Files (x86)\Steam\steamapps\common\Beat Saber\Beat Saber_Data\CustomLevels`)
5) Wait for analyzis and select beatmaps you need to fix (just enter numbers)
![Application](https://github.com/Ivan-Alone/BeatSaber_v1.20.0_Beatmaps_Fixer/blob/main/img/consle.jpg?raw=true)
6) Press Enter, and beatmaps will fixed
7) Enjoy playing!
