# Senpai Plugin - WIP

![](http://i.imgur.com/4jymgJw.jpg)

Plugin uses your username and fetches your Myanimelist, Senpai.moe current season data, compares them and displays your watching anime in order with airing dates **in your timezone**.


# How to install

* Close Rainmeter
* Download [this](https://www.dropbox.com/s/kdg7terrjmmvxe9/MySenpai.zip?dl=0) file.
* Go to `%appdata%/Rainmeter/Plugins`
* Copy & Paste MALSenpaiPlugin.dll
* Copy FrostedGlass.dll to there (Optional, remove [BlurBehind] section in MAL.ini if you skip this)
* Go to C:/Program Files/Rainmeter
* Copy Newtonsoft.Json.dll to there (should be where Rainmeter.exe is, see Important Note section)
* Go To Documents/Rainmeter/Skins
* Create a folder named 'MySenpai'
* Copy JSON.lua MAL.ini MAL.lua
* Open MAL.ini
* Find "MalUser"
* Change it to your name


# Important Note
Rainmeter can NOT find external libraries that is linked by your plugin (or I don't know how). This plugin uses **JSON.NET**. The easiest solution to use another library in a plugin is
just adding the reference in VS normally, then copy/paste the external DLL to where Rainmeter.exe is. Otherwise Rainmeter will crash with `FileNotFoundException`. The reason copying the DLL
to where Rainmeter.exe works is because the default search path when Windows OS loads a DLL is the binary directory.
