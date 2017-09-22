# SWSH
Secure Windows Shell (SWSH - Pronounced like Swish)

SWSH is a Secure Shell-like console application for Windows written in C# using [SSH.NET](https://github.com/sshnet/SSH.NET) library.
It allows you to execute commands over SSH to your server.

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
# Making a nickname
To add a new nickname run ```swsh -a``` or ```swsh --add``` and fill the asked details like:
```
Path to private key: C:\path\to\ssh\private.key
Username: root
Server: my.server.ssh
Unique Nickname: server.ssh
```
# Future
> “The best way to predict your future is to create it.” ~***Abraham Lincoln***

Feel free to contribute this project and add a feature :)

Below are some items I am trying to add or fix in SWSH:

Add features:
* Delete nicknames
* Show all nicknames
* Show details about a nickname
* scp support
* Password support
* Edit nicknames
* Check existence of private key when adding a nickname 
* Custom directory to save data

Fix:
* Make ```cd ..``` work 
