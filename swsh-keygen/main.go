package main

import (
 "crypto/rsa"
 "crypto/rand" 
 "encoding/pem" 
 "crypto/x509" 
 "golang.org/x/crypto/ssh" 
 "io/ioutil"
 "os"
 "flag"
)

func main(){
	var pubKeyPath, privateKeyPath string
    flag.StringVar(&pubKeyPath, "pub", "nil", "PublicKey path")
    flag.StringVar(&privateKeyPath, "pri", "nil", "PrivateKey path")
    flag.Parse();
	privateKey, err := rsa.GenerateKey(rand.Reader, 1024)
    if err != nil { panic(err) }
    privateKeyFile, err := os.Create(privateKeyPath)
    defer privateKeyFile.Close()
    if err != nil { panic(err) }
    privateKeyPEM := &pem.Block{Type: "RSA PRIVATE KEY", Bytes: x509.MarshalPKCS1PrivateKey(privateKey)}
    if err := pem.Encode(privateKeyFile, privateKeyPEM); err != nil { panic(err) }
    pub, err := ssh.NewPublicKey(&privateKey.PublicKey)
    if err != nil { panic(err) }
    ioutil.WriteFile(pubKeyPath, ssh.MarshalAuthorizedKey(pub), 0655)
}