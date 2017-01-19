# Senpai Plugin

![](http://i.imgur.com/4jymgJw.jpg)

Plugin uses your username and fetches your Myanimelist, Senpai.moe current season data, compares them and displays your watching anime in order with airing dates **in your timezone**.

See ExampleUsage folder. Copy pasting the folder into your skins folder should work if you compiled the DLL yourself.


# Important Note
Rainmeter can NOT find external libraries that is linked by your plugin (or I don't know how). This plugin uses **JSON.NET**. The easiest solution to use another library in a plugin is
just adding the reference in VS normally, then copy/paste the external DLL to where Rainmeter.exe is. Otherwise Rainmeter will crash with `FileNotFoundException`. The reason copying the DLL
to where Rainmeter.exe works is because the default search path when Windows OS loads a DLL is the binary directory.
