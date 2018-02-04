<img src=".images/github banner.png"/>

[![Build status](https://ci.appveyor.com/api/projects/status/1f2uc16tue6h0r0l/branch/master?svg=true)](https://ci.appveyor.com/project/muhammadmuzzammil1998/swsh/branch/master)
  		  
SWSH is a SSH like console application for Windows.

**If you are not using a [prebuilt SWSH binary](https://github.com/SecureWindowsShell/SWSH/releases), you will see SWSH complain about a checksum mismatch and exit, use `--IgnoreChecksumMismatch` to stop it from exiting.**

![SWSH just doing its thing](https://raw.githubusercontent.com/muhammadmuzzammil1998/SWSH/c173fa908f0287cde2a03b25c72b933df6448277/.images/cdls.PNG)
*SWSH (beta) just doing its thing*

# Nicknames
SSH connections are saved as **nicknames**, each nickname has to be unique.

# Making a nickname with Key
To add a new nickname run ```swsh -a``` or ```swsh --add``` and follow the prompts:
```
Path to private key: C:\path\to\ssh\private.key
Username: root
Server: my.server.ssh
Unique Nickname: server.ssh
```
# Making a nickname with Password
To add a new nickname run ```swsh -a -password``` or ```swsh --add -password``` and follow the prompts:
```
Username: root
Server: my.server.ssh
Unique Nickname: server.ssh
```
# Connection to server
To connect, you must first add a connection: ```swsh -a``` or ```swsh --add```. After that, you can connect: ```swsh -c nickname``` or ```swsh --connect nickname```.

If done properly, output would be similar to the following (you will be asked for a password if you didn't add a key):
```
Waiting for response from usr@host...
Connected to usr@host...
~:/ $ 
```
For more, see our [documentation](DOCUMENTATION.md).

# Future
> “The best way to predict your future is to create it.” ~***Abraham Lincoln***

Feel free to contribute to this project.

Below are some things we are trying to add or fix in SWSH:

- [x] Delete nicknames ```swsh --delete```
- [x] Show all nicknames ```swsh --show```
- [x] Show details about a nickname ```swsh --show [nickname]```
- [x] File upload support ```upload```
- [x] Password support ```swsh -a -password```
- [x] Edit nicknames ```swsh --edit [nickname]```
- [x] Check existence of private key when adding a nickname
- [x] Ability to manipulate current working directory on Windows side ```cd``` ```ls```
- [x] Clear console ```clear```
- [x] Detailed help ```swsh --help [command]```
- [x] Check integrity of application
- [x] keygen for ssh ```swsh --keygen```
- [x] ```.swsh_history``` file to contain history of commands executed
- [x] Tab completion.
- [x] Better terminal.
- [x] New Logo.
- [ ] Drop SSH.NET.

# License
GPL v3<br>
Copyright (C) 2017  Muhammad Muzzammil

 
