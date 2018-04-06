# PassProt
PassProt is a locally hosted password vault I created after my cloud solutions failed me.

## How to Start
1. Clone the repository to your local desktop
1. Make changes to MainWindow.xaml.xs on lines 226-228 to personalize the hash, salt, and VI.
  ```C#
    string PasswordHash = "passwordhash";
    const string SaltKey = "saltkey";
    const string VIKey = "VIkey";
  ```
1. Build the solution
