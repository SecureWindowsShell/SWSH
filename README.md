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
# Connection to server
To connect, you must create add it first using ```swsh -a``` or ```swsh --add```. After that, run ```swsh -c nickname``` or ```swsh --connect nickname``` to connect.

If done properly, output would be similar to this:
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
* Password support
* Edit nicknames
* Check existence of private key when adding a nickname 
* Custom directory to save data
* *~Clear console~* ```clear```

Fix:
* Make ```cd ..``` work 

### Note: Public release will be made after these features and fixes are added in SWSH.