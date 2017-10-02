# SWSH
Secure Windows Shell (SWSH - Pronounced swish)

[![Build status](https://ci.appveyor.com/api/projects/status/1rpo0bsdo53749l4?svg=true)](https://ci.appveyor.com/project/nabeelomer/swsh)

SWSH is a Secure Shell-like console application for Windows written in C# using [SSH.NET](https://github.com/sshnet/SSH.NET) library.
It allows you to execute commands over SSH to your server.

**If you are not using [a binary that we built](https://github.com/SecureWindowsShell/SWSH/releases) or are using an older version of SWSH you will get an error about checksum mismatches, this happens because we only upload the checksum for the latest version of SWSH. Use the flag `--IgnoreChecksumMismatch` to prevent SWSH from exiting on checksum mismatches.**

# Run
Download the executable from the website (will be available after a proper release) or build it your self from source.

# Commands
Commands and their functions can be found in-app by running ```swsh -h``` or ```swsh --help```.

# Nicknames
SSH connections are saved as **nicknames**, each should be unique.
Nicknames contains the data such as SSH private key, username and host in following structure:
```
nickname.swsh
└──C:\path\to\ssh\private.key
└──username
└──host
```
# Making a nickname with Key
To add a new nickname run ```swsh -a``` or ```swsh --add``` and fill the asked details like:
```
Path to private key: C:\path\to\ssh\private.key
Username: root
Server: my.server.ssh
Unique Nickname: server.ssh
```
# Making a nickname with Password
To add a new nickname run ```swsh -a -password``` or ```swsh --add -password``` and fill the asked details like:
```
Username: root
Server: my.server.ssh
Unique Nickname: server.ssh
```
# Connection to server
To connect, you must create add it first using ```swsh -a``` or ```swsh --add```. After that, run ```swsh -c nickname``` or ```swsh --connect nickname``` to connect.

If done properly, output would be similar to this (you will be asked for password if you didn't add key):
```
Waiting for response from usr@host...
Connected to usr@host...
~:/ $ 
```
# Future
> “The best way to predict your future is to create it.” ~***Abraham Lincoln***

Feel free to contribute this project and add a feature :)

Below are some items I am trying to add or fix in SWSH:

Add features:
* *~Delete nicknames~* ```swsh --delete```
* *~Show all nicknames~* ```swsh --show```
* *~Show details about a nickname~* ```swsh --show [nickname]```
* scp support
* *~Password support~* ```swsh -a -password```
* *~Edit nicknames~* ```swsh --edit [nickname]```
* *~Check existence of private key when adding a nickname~* 
* ~Custom directory to save data~ (nah, not gonna work...) 
* *~Ability to manipulate current working directory on Windows side~* ```cd``` ```ls```
* *~Clear console~* ```clear```
* *~Detailed help~* ```swsh --help [command]```
* *~Check integrity of application~*

Fix:
* *~```swsh --show``` not working~. Issue #8* 
* *~Make ```cd ..``` work~. Issue #2* 
* *~A connection attempt failed while using password. Issue #1~*
### Note: Public release will be made after beta-2.0 is fully tested. Current release: beta-1.0.
### Note: SWSH can be downloaded from [here](https://github.com/muhammadmuzzammil1998/SWSH/releases/tag/beta-1.1) if you want to give it a go.
