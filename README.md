<img src=".images/github banner.png"/>

[![Build status](https://ci.appveyor.com/api/projects/status/1f2uc16tue6h0r0l/branch/master?retina=true)](https://ci.appveyor.com/project/muhammadmuzzammil1998/swsh/branch/master)

SWSH is a console application that offers SSH-like connectivity with ease to users which grants them the ability to operate remotely on SSH protocol. It is also Open source, so feel free to contribute.

**If you are not using a [prebuilt SWSH binary](https://github.com/SecureWindowsShell/SWSH/releases), you will see SWSH complain about a checksum mismatch and exit, use `--IgnoreChecksumMismatch` to stop it from exiting.**

![SWSH just doing its thing](https://user-images.githubusercontent.com/12321712/36257898-bfaba952-127e-11e8-9bd9-b63d4885f649.png)
*SWSH (beta) just doing its thing*

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

| Command                             | Description                                                            |
|:------------------------------------|:-----------------------------------------------------------------------|
| version                             | Check the version of swsh.                                             |
| add    [-password]                  | Add a new connection either using private key or password (-password). |
| show    [nickname]                  | Show nicknames/Details of a nickname.                                  |
| connect [nickname]                  | Connects to Server over SSH.                                           |
| delete  [nickname]                  | Deletes connection's nickname.                                         |
| edit    [nickname] [arg]]           | Edits nickname, use one argument at a time.                            |
| keygen                              | Generates SSH RSA key pair.                                            |
| help    [command]                   | Displays this help or command details.                                 |
| clear                               | Clears the console.                                                    |
| pwd                                 | Prints working directory.                                              |
| computehash [(>/>>) path/to/file]   | Uses SHA-1 hash function to generate hashes for SWSH and swsh-keygen.  |
| exit                                | Exits.                                                                 |
| ls                                  | Lists all files and directories in working directory.                  |
| cd [arg]                            | Changes directory to 'arg'. arg = directory name.                      |
| upload [args] [nickname]:[location] | Uploads files and directories. 'upload -h' for help.                   |

For more, see our [documentation](DOCUMENTATION.md).

# License

GPL v3

Copyright (C) 2017  Muhammad Muzzammil
