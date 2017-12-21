# swsh-keygen
SSH key-pair generator for [SWSH](https://github.com/SecureWindowsShell/SWSH), but it can be used separately.

# Build
    $ git clone https://github.com/SecureWindowsShell/SWSH.git
    $ cd SWSH/swsh-keygen
    $ go build
# Arguments

    -pri string
            PrivateKey path (default "nil")
    -pub string
            PublicKey path (default "nil") 

# Run
    $ ./swsh-keygen.exe -pub=location.pub -pri=location.pri
# Help
    $ ./swsh-keygen.exe -help
#### Prerequisite: [Go](https://golang.org/doc/install), [git](https://git-scm.com/book/en/v2/Getting-Started-Installing-Git)
