# Senpai Plugin

![](http://i.imgur.com/vMsFjWd.png)

Plugin uses your username and fetches your Myanimelist, Senpai.moe current season data, compares them and displays your watching anime in order with airing dates **in your timezone**.

See **SamplePlugin.ini** for usage or use the **MySenpai_1.0.rmskin** to install.


# Important Note
Rainmeter can NOT find external libraries that is linked by your plugin. This plugin uses **JSON.NET**. The easiest solution to use another library in a plugin is
just adding the reference in VS normally, then copy/paste the external DLL to where Rainmeter.exe is. Otherwise Rainmeter will crash with `FileNotFoundException`.