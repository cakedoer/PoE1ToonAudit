## PoE1ToonAudit

Simple demo tool that takes a character's equipped items and evaluates whether they have a life roll and provide enough resistances for a mapping character. Pretty much does the same thing as the in-game character window except it doesn't account for skills or anything on your passive tree. 

Uses legacy Path of Exile character window endpoints. Optionally takes a session cookie to authenticate.

### Authentication

To use it authenticated, login to Path of Exile's official website and grab your cookie called "POESESSID" using your browser's dev tools. Take the string after the colon (do not share it with anyone) and either use your IDE or the dotnet CLI to store your secrets as follows:

Navigate to the project folder, then do

`dotnet user-secrets init`

After that, save your session ID to your machine's secure local storage

`dotnet user-secrets set "PathOfExile:SessionId" "your_copied_poesessid_here"`
