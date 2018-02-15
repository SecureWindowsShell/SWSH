# Documentation for SWSH

For Version beta-3.0

## Index

* [Getting Started](#getting-started)
  * [Nicknames](#nicknames)
  * [Generating SSH keys](#generating-ssh-keys)
  * [Making a nickname with Private key](#making-a-nickname-with-key)
  * [Making a nickname with Password](#making-a-nickname-with-password)
* [Commands](#commands)
  * [version](#version)
  * [add    [-password]](#add)
  * [show    [nickname]](#show)
  * [connect [nickname]](#connect)
  * [delete  [nickname]](#delete)
  * [edit    [nickname] [arg]](#edit)
  * [keygen](#keygen)
  * [help    [command]](#help)
  * [clear](#clear)
  * [pwd](#pwd)
  * [exit](#exit)
  * [ls](#ls)
  * [cd [arg]](#cd)
  * [upload [args] [nickname]:[location]](#upload)

## Getting Started

### Nicknames

SSH connections are saved as **nicknames**, each nickname has to be unique.

### Generating SSH keys

SSH keys serve as a means of identifying yourself to an SSH server. To Generate your private and public key, SWSH uses an add-on, swsh-keygen. You can [build swsh-keygen](https://github.com/SecureWindowsShell/swsh-keygen) yourself if you want and place the executable (.exe) in SWSH's root directory. 

Use command ```keygen``` to tell SWSH that you want to generate a new RSA key pair for SSH connection after that just follow the prompts. 
You'll be asked for locations to store your keys, leave it blank if you want it to be default.
Output will be similar to this:

```swsh
/users/muzzammil:swsh> keygen
exit or -e to cancel.
Enter path to save private key (swsh.private): 
Enter path to save public key (swsh.public):
Your public key:

ssh-rsa AAAAB3NzaC1yc2EAAAADAQABAAAAgQDIAHPhxzRApbQgcgDCXysDqkBezHgHBHJTeBpfcGXfkHyGKUlbv7X1Ftz5Qyl6lEPwTg2vOR+FCMKbOOVbv5ISZXJJyGSiPPqis0Jfp58wmSjPuyS78N+ZgqynD6SXbcKbJhEYtriPBKueraj3lY3DYQjRQR42YoeAqjcAg2Riew==
```

NOTE: DO **NOT** SHARE YOUR PRIVATE KEY!

### Making a nickname with Key

To add a new nickname run ```add``` and follow the prompts:

```swsh
Path to private key: C:\path\to\ssh\private.key
Username: root
Server: my.server.ssh
Unique Nickname: server.ssh
```

### Making a nickname with Password

To add a new nickname run ```add -password``` and follow the prompts:

```swsh
Username: root
Server: my.server.ssh
Unique Nickname: server.ssh
```

### Connecting to server

To connect run ```connect nickname```.

If done properly, output would be similar to the following (you will be asked for a password if you didn't add a key):

```swsh
Waiting for response from usr@host...
Connected to usr@host...
~:/ $
```

## Commands

| Command                                        | Description                                                            |
|:-----------------------------------------------|:-----------------------------------------------------------------------|
| [version](#version)                            | Check the version of swsh.                                             |
| [add    [-password]](#add)                     | Add a new connection either using private key or password (-password). |
| [show    [nickname]](#show)                    | Show nicknames/Details of a nickname.                                  |
| [connect [nickname]](#connect)                 | Connects to Server over SSH.                                           |
| [delete  [nickname]](#delete)                  | Deletes connection's nickname.                                         |
| [edit    [nickname] [arg]](#edit)              | Edits nickname, use one argument at a time.                            |
| [keygen](#keygen)                              | Generates SSH RSA key pair.                                            |
| [help    [command]](#help)                     | Displays this help or command details.                                 |
| [clear](#clear)                                | Clears the console.                                                    |
| [pwd](#pwd)                                    | Prints working directory.                                              |
| [exit](#exit)                                  | Exits.                                                                 |
| [ls](#ls)                                      | Lists all files and directories in working directory.                  |
| [cd [arg]](#cd)                                | Changes directory to 'arg'. arg = directory name.                      |
| [upload [args] [nickname]:[location]](#upload) | Uploads files and directories. 'upload -h' for help.                   |

### version

```swsh
Syntax: version
Checks the version of swsh.

Usage: version
```

### add

```swsh
Syntax: add [-password]
Add a new connection either using private key or password.

Usage to add using private key: add
Usage to add using a password: add -password

You'll be asked for password each time as SWSH doesn't store them.
```

### show

```swsh
Syntax: show [nickname]
Show nicknames if no arguments are given. If nickname is provided, shows details of a nickname.
Usage to check all nicknames: show
Usage to check a nickname: show myserver
```

### connect

```swsh
Syntax: connect [nickname]
Connects to Server over SSH.
Usage: connect myserver
```

### delete

```swsh
Syntax: delete [nickname]
Deletes connection's nickname.
Usage: delete myserver
```

### edit

```swsh
Syntax: edit [nickname] [arg]
arg:
        -user [newUserName]
        -key [newKey]
        -server [newServer]
Edits nickname, use one argument at a time.
Usage: edit myserver -user newUSER
```

### keygen

```swsh
Syntax: keygen
Generates SSH RSA key pair. Requires swsh-keygen.exe.

Default values are provided in parentheses.
Usage: keygen
```

### help

```swsh
Syntax: help [command]
Displays this help or command details.
Usage: help add
```

### clear

```swsh
Syntax: clear
Clears the console.

Usage: clear
```

### pwd

```swsh
Syntax: pwd
Prints working directory.
Usage: pwd
```

### exit

```swsh
Syntax: exit
Exits.

Usage: exit
```

### ls

```swsh
Syntax: ls
Lists all files and directories in working directory.

Usage: ls
```

### cd

```swsh
Syntax: cd
Changes directory to 'arg'. arg = directory name.

Usage: cd
```

### upload

```swsh
Syntax: upload [--dir]* [args] [nickname]:[location]
Uploads files and directories. 'upload -h' for help.

'args' are seperated using spaces ( ) and last 'arg' will be treated as server data which includes nickname as well as the location, part after the colon (:), where the data is to be uploaded. Use flag '--dir' to upload directiories. Do not use absolute paths for local path, change working directory to navigate.

Usage: upload --dir files nickname:/var/files
```
