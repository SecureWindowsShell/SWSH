# Documentation for SWSH

For Version beta-3.0

## Index

* [Getting Started](#getting-started)
  * [Generating SSH keys](#generating-ssh-keys)
* [Commands](#commands)
  * [version](#version)
  * [connect [nickname]](#connect)
  * [keygen](#keygen)
  * [help    [command]](#help)
  * [clear](#clear)
  * [pwd](#pwd)
  * [computehash [(>/>>) path/to/file]](#computehash)
  * [exit](#exit)
  * [ls](#ls)
  * [cd [arg]](#cd)
  * [upload [args] [nickname]:[location]](#upload)

## Getting Started

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

### Connecting to server

To connect run ```connect nickname```.

If done properly, output would be similar to the following (you will be asked for a password if you didn't add a key):

```swsh
Waiting for response from usr@host...
Connected to usr@host...
~:/ $
```

## Commands

| Command                                           | Description                                                            |
|:--------------------------------------------------|:-----------------------------------------------------------------------|
| [version](#version)                               | Check the version of swsh.                                             |
| [connect [nickname]](#connect)                    | Connects to Server over SSH.                                           |
| [keygen](#keygen)                                 | Generates SSH RSA key pair.                                            |
| [help    [command]](#help)                        | Displays this help or command details.                                 |
| [clear](#clear)                                   | Clears the console.                                                    |
| [pwd](#pwd)                                       | Prints working directory.                                              |
| [computehash [(>/>>) path/to/file]](#computehash) | Uses SHA-1 hash function to generate hashes for SWSH and swsh-keygen.  |
| [exit](#exit)                                     | Exits.                                                                 |
| [ls](#ls)                                         | Lists all files and directories in working directory.                  |
| [cd [arg]](#cd)                                   | Changes directory to 'arg'. arg = directory name.                      |
| [upload [args] [nickname]:[location]](#upload)    | Uploads files and directories. 'upload -h' for help.                   |

### version

```swsh
Syntax: version
Checks the version of swsh.

Usage: version
```

### connect

```swsh
Syntax: connect [nickname]
Connects to Server over SSH.
Usage: connect myserver
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

### computehash

```swsh
Syntax: computehash [(>/>>) path/to/file]
Uses SHA-1 hash function to generate hashes for SWSH and swsh-keygen.

Usage:
To overwrite-> computehash > path/to/file
To append-> computehash >> path/to/file
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
