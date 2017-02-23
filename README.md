# Client

Yggdrasil client software

## Features

* Written in C#
* Highly scalable thanks to the .NET framework
* Is completely open source (GPLv3)
* Makes it easy to use Yggdrasil

## How to install

Download the compiled package from http://yggdrasil.96.lt/

## How to compile

Clone this repository and open it in Visual Studio or MonoDevelop. Then press the F5 key and you will find it in <Repo>/Yggdrasil/bin/Debug/

Note that you need to copy the following files/folder from VLC:

```
08.02.2017  22:55    <DIR>          plugins
01.06.2016  15:17           144.832 libvlc.dll
01.06.2016  15:19         2.632.640 libvlccore.dll
01.06.2016  15:17           137.152 vlc.exe
```

If you search the files for the setup, they are [here](https://julain.wolkesicher.de/index.php/s/n4Xg9yMKUx9rvuw/download). Just make the paths fit to your needs. You will need InnoSetup. Make sure you rename the file vlc.exe to yggplayer.exe.

*Have a nice day :)*
