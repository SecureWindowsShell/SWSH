# Documentation for SWSH

For Titan

## Index

* [Getting Started](#getting-started)
  * [Generating SSH keys](#generating-ssh-keys)
  * [Importing SSH keys](#importing-ssh-keys)
  * [Connecting to a host](#connecting-to-a-host)
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

SSH keys serve as a means of identifying yourself to an SSH server. To Generate your private and public key, SWSH uses an add-on, swsh-keygen. You can [build swsh-keygen](https://github.com/SecureWindowsShell/swsh-keygen) yourself if you want and place the executable (.exe) in SWSH's root (installation) directory.

Use command ```keygen``` to tell SWSH that you want to generate a new RSA key pair for SSH connection after that just follow the prompts.
You'll be asked for locations to store your keys, leave it blank if you want it to be default.
Output will be similar to this:

```swsh
/users/muzzammil:swsh> keygen

Generating public/private rsa key pair.
exit or -e to cancel.
Enter absolute path to save private key (%appdata%/SWSH/swsh.private):
Enter absolute path to save public key (%appdata%/SWSH/swsh.public):
Your public key:

ssh-rsa AAAAB3NzaC1yc2EAAAADAQABAAAAgQCt2MxdswuuUvmaY4JK6kP4lYIqGy0KeHCqcx1NEjB4EcqH7+MIeXGbdikACvP3wlOAEAt+7PMEhBHf7nL2S2SsOybpegJw0piiMeOIPJwQxIQFaRWyz3xn0ESItzBizsQ4yxfQiG37sFkMeQVnP5fHuc2+Z4JZ5SD56Dh1xxgnEw==
```

### Importing SSH keys

If you already have SSH keys and want to use them instead of creating a new pair, you can! Use ```keygen import``` command to do so and just follow the prompts.

NOTE: DO **NOT** SHARE YOUR PRIVATE KEY!

### Connecting to a host

To connect run ```connect username@host```.

To use a password connection, use tag `-p` like this: ```connect username@host -p```.

If done properly, output would be similar to the following:

```swsh
Waiting for response from username@host...
Connected to username@host...
~:/ $
```

## Commands

| Command                                   | Description                                                           |
|:------------------------------------------|:----------------------------------------------------------------------|
| [version](#version)                       | Check the version of swsh.                                            |
| [connect [user@host] (-p)](#connect)      | Connects to Server over SSH.                                          |
| [keygen (options)](#keygen)               | Generates SSH RSA key pair.                                           |
| [help    [command]](#help)                | Displays this help or command details.                                |
| [clear](#clear)                           | Clears the console.                                                   |
| [pwd](#pwd)                               | Prints working directory.                                             |
| [computehash [(>/>>) path]](#computehash) | Uses SHA-1 hash function to generate hashes for SWSH and swsh-keygen. |
| [exit](#exit)                             | Exits.                                                                |
| [ls](#ls)                                 | Lists all files and directories in working directory.                 |
| [cd [arg]](#cd)                           | Changes directory to 'arg'. arg = directory name.                     |
| [upload [arguments]](#upload)             | Uploads files and directories. 'upload -h' for help.                  |

### version

```swsh
Syntax: version
Checks the version of swsh.

Usage: version
```

### connect

```swsh
Syntax: connect [user@host] (-p)
Connects to Server over SSH. Use `-p` for password connection.
Usage: connect root@server.ip
```

### keygen

```swsh
Syntax: keygen (options)
Generates, imports or show SSH RSA key pair. Requires swsh-keygen.exe.
Default values are provided in parentheses.

Options:
        import          - Imports RSA key pair.
        show [private]  - Print RSA keys. By default, prints public key. Use `private` to print private key.
```

### help

```swsh
Syntax: help [command]
Displays this help or command details.
Usage: help pwd
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
Syntax: cd [arg]
Changes directory to 'arg'. arg = directory name.

Usage: cd
```

### upload

```swsh
upload [--dir]* [args] [user@host]:[location]

'args' are seperated using spaces ( ) and last 'arg' will be treated as server data which includes username and host location as well as the location of data to upload, part after the colon (:), where the data is to be uploaded. Use flag '--dir' to upload directiories. Do not use absolute paths for local path, change working directory to navigate.

Usage: upload --dir files root@43.22.56.111:/var/files
```
