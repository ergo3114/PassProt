using System.IO;
using System.Windows;
using System.Xml;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Win32;
using System;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace WpfApplication1
{
    /// <summary>
    /// ToDo:
    /// -X- Delay master password
    /// -- Add view help manual
    /// -X- Add about page
    /// -X- Set master password text boxes to password fields
    /// -- Options for window styles
    /// -X- Delete system option
    /// -X- Update system password
    /// -- Sort ComboBox items
    /// </summary>
    public partial class MainWindow : Window
    {
        private string flag = null;
        public System.Timers.Timer myTimer { get; set; }
        int counter;

        public MainWindow()
        {
            InitializeComponent();
        }

        public MainWindow(string something)
        {
            flag = something;
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Window1 createpasswordform = new Window1();
            this.Close();
            createpasswordform.Show();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            labelTimer.Content = "300";
            myTimer = new System.Timers.Timer();
            myTimer.Interval = 1000;
            myTimer.Elapsed += new System.Timers.ElapsedEventHandler(TimerC);
            myTimer.Start();

            string file = "C:\\Users\\David\\Documents\\systemcreds.xml";
            //put an if statement here to prevent this when flag is not null
            if (String.IsNullOrEmpty(flag)) {
                if (Registry.CurrentUser.OpenSubKey("PassProt\\", true).GetValue("Delay") == null)
                {
                    Registry.CurrentUser.OpenSubKey("PassProt\\", true).SetValue("Delay", "0", RegistryValueKind.String);
                }
                #region Initial Master Password
                if ((string)Registry.CurrentUser.OpenSubKey("PassProt\\", true).GetValue("Delay") == "0")
                {
                    this.Visibility = Visibility.Hidden;
                    string keyName = @"HKEY_CURRENT_USER\PassProt";
                    string valueName = "mp";
                    if (Registry.GetValue(keyName, valueName, null) == null)
                    {
                        string message = "You do not have a master password. Do you want to create one now?";
                        string caption = "Master Password is Missing";
                        DialogResult result = System.Windows.Forms.MessageBox.Show(message, caption, MessageBoxButtons.YesNo);
                        if (result == System.Windows.Forms.DialogResult.Yes)
                        {
                            Window2 createmasterpasswordform = new Window2();
                            this.Close();
                            createmasterpasswordform.Show();
                        }
                    }
                    else
                    {
                        Window3 entermasterpasswordform = new Window3();
                        this.Visibility = Visibility.Hidden;
                        entermasterpasswordform.Show();
                    }
                    if (FileOrDirectoryExists(file))
                    {
                        XmlDocument xDoc = new XmlDocument();
                        xDoc.Load(file);
                        XmlNodeList systems = xDoc.GetElementsByTagName("Name");
                        foreach (XmlNode node in systems)
                        {
                            comboBox.Items.Add(node.InnerText);
                        }
                    }
                }
                #endregion
                #region Set Delay to 15 minutes
                else if ((string)Registry.CurrentUser.OpenSubKey("PassProt\\", true).GetValue("Delay") == "1")
                {
                    if (DateTime.Now > (Convert.ToDateTime((string)Registry.CurrentUser.OpenSubKey("PassProt\\", true).GetValue("Time"))).AddMinutes(15))
                    {
                        Window3 entermasterpasswordform = new Window3();
                        this.Visibility = Visibility.Hidden;
                        entermasterpasswordform.Show();
                        Registry.CurrentUser.OpenSubKey("PassProt\\", true).SetValue("Delay", "0", RegistryValueKind.String);
                    }
                    else
                    {
                        XmlDocument xDoc = new XmlDocument();
                        xDoc.Load(file);
                        XmlNodeList systems = xDoc.GetElementsByTagName("Name");
                        foreach (XmlNode node in systems)
                        {
                            comboBox.Items.Add(node.InnerText);
                        }
                    }
                }
                #endregion
                #region Set Delay to 30 minutes
                else if ((string)Registry.CurrentUser.OpenSubKey("PassProt\\", true).GetValue("Delay") == "2")
                {
                    if (DateTime.Now > (Convert.ToDateTime(Registry.CurrentUser.OpenSubKey("PassProt\\", true).GetValue("Time"))).AddMinutes(30))
                    {
                        Window3 entermasterpasswordform = new Window3();
                        this.Visibility = Visibility.Hidden;
                        entermasterpasswordform.Show();
                        Registry.CurrentUser.OpenSubKey("PassProt\\", true).SetValue("Delay", "0", RegistryValueKind.String);
                    }
                    else
                    {
                        XmlDocument xDoc = new XmlDocument();
                        xDoc.Load(file);
                        XmlNodeList systems = xDoc.GetElementsByTagName("Name");
                        foreach (XmlNode node in systems)
                        {
                            comboBox.Items.Add(node.InnerText);
                        }
                    }
                }
                #endregion
                #region Set Delay to 1 hour
                else if ((string)Registry.CurrentUser.OpenSubKey("PassProt\\", true).GetValue("Delay") == "3")
                {
                    if (DateTime.Now > (Convert.ToDateTime(Registry.CurrentUser.OpenSubKey("PassProt\\", true).GetValue("Time"))).AddHours(1))
                    {
                        Window3 entermasterpasswordform = new Window3();
                        this.Visibility = Visibility.Hidden;
                        entermasterpasswordform.Show();
                        Registry.CurrentUser.OpenSubKey("PassProt\\", true).SetValue("Delay", "0", RegistryValueKind.String);
                    }
                    else
                    {
                        XmlDocument xDoc = new XmlDocument();
                        xDoc.Load(file);
                        XmlNodeList systems = xDoc.GetElementsByTagName("Name");
                        foreach (XmlNode node in systems)
                        {
                            comboBox.Items.Add(node.InnerText);
                        }
                    }
                }
                #endregion
            }
            else
            {
                XmlDocument xDoc = new XmlDocument();
                xDoc.Load(file);
                XmlNodeList systems = xDoc.GetElementsByTagName("Name");
                foreach (XmlNode node in systems)
                {
                    comboBox.Items.Add(node.InnerText);
                }
            }
        }

        /// <summary>
        /// Returns true or false based on if the file exists or not
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        internal static bool FileOrDirectoryExists(string name) { return (Directory.Exists(name) || File.Exists(name)); }

        private void TimerC(object sender, EventArgs e)
        {
            if (++counter == 300)
            {
                Environment.Exit(0);
            }
            else
            {
                this.Dispatcher.Invoke(() =>
                {
                    int seconds = 0;
                    Int32.TryParse(labelTimer.Content.ToString(), out seconds);
                    labelTimer.Content = (seconds - 1).ToString();
                });
            }
        }

        private void comboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            string file = "C:\\Users\\David\\Documents\\systemcreds.xml";
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(file);
            XmlNodeList username = xDoc.GetElementsByTagName("Username");
            XmlNode usernameNode = username[comboBox.SelectedIndex];
            XmlNodeList password = xDoc.GetElementsByTagName("Password");
            XmlNode passwordNode = password[comboBox.SelectedIndex];
            XmlNodeList system = xDoc.GetElementsByTagName("System");
            XmlNode systemNode = system[comboBox.SelectedIndex].Attributes[1];
            labelName.Content = comboBox.SelectedValue;
            labelUsername.Content = Decrypt(usernameNode.InnerText);
            string originalData = Decrypt(passwordNode.InnerText);
            labelPassword.Content = originalData;
            labelTimeSet.Content = systemNode;
        }

        private static string Encrypt(string plainText)
        {
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            string PasswordHash = "passwordhash";
            const string SaltKey = "saltkey";
            const string VIKey = "VIkey";

            byte[] keyBytes = new Rfc2898DeriveBytes(PasswordHash, Encoding.ASCII.GetBytes(SaltKey)).GetBytes(256 / 8);
            var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.Zeros };
            var encryptor = symmetricKey.CreateEncryptor(keyBytes, Encoding.ASCII.GetBytes(VIKey));

            byte[] cipherTextBytes;

            using (var memoryStream = new MemoryStream())
            {
                using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                    cryptoStream.FlushFinalBlock();
                    cipherTextBytes = memoryStream.ToArray();
                    cryptoStream.Close();
                }
                memoryStream.Close();
            }
            return Convert.ToBase64String(cipherTextBytes);
        }

        private static string Decrypt(string encryptedText)
        {
            string PasswordHash = "P@@Sw0rdH@$H";
            const string SaltKey = "S@LTY&KEY";
            const string VIKey = "@1B2c3D4e5F6g7H8J8";

            byte[] cipherTextBytes = Convert.FromBase64String(encryptedText);
            byte[] keyBytes = new Rfc2898DeriveBytes(PasswordHash, Encoding.ASCII.GetBytes(SaltKey)).GetBytes(256 / 8);
            var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.None };

            var decryptor = symmetricKey.CreateDecryptor(keyBytes, Encoding.ASCII.GetBytes(VIKey));
            var memoryStream = new MemoryStream(cipherTextBytes);
            var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            byte[] plainTextBytes = new byte[cipherTextBytes.Length];

            int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
            memoryStream.Close();
            cryptoStream.Close();
            return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount).TrimEnd("\0".ToCharArray());
        }

        private static void KillLockedFile(string filename)
        {
            Process tool = new Process();
            tool.StartInfo.FileName = "handle.exe";
            tool.StartInfo.Arguments = filename + " /accepteula";
            tool.StartInfo.UseShellExecute = false;
            tool.StartInfo.RedirectStandardOutput = true;
            tool.Start();
            tool.WaitForExit();
            string outputTool = tool.StandardOutput.ReadToEnd();

            string matchPattern = @"(?<=\s+pid:\s+)\b(\d+)\b(?=\s+)";
            foreach (Match match in Regex.Matches(outputTool, matchPattern))
            {
                Process.GetProcessById(int.Parse(match.Value)).Kill();
            }
        }

        private void ChangeMasterPassword_Click(object sender, RoutedEventArgs e)
        {
            //accidentily deleted the MASTER password word change window :/
            //this.Visibility = Visibility.Hidden;
            //UpdateMasterPassword updatemasterpassword = new UpdateMasterPassword();
            //updatemasterpassword.Show();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ViewHelp_Click(object sender, RoutedEventArgs e)
        {

        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
            About about = new About();
            about.Show();
        }

        private void buttonUpdate_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
            UpdatePassword updatepassword = new UpdatePassword(labelName.Content.ToString());
            updatepassword.Show();
        }

        private void buttonDelete_Click(object sender, RoutedEventArgs e)
        {
            string file = "C:\\Users\\David\\Documents\\systemcreds.xml";
            string system = comboBox.SelectedItem.ToString();
            DialogResult dr = System.Windows.Forms.MessageBox.Show("Are you sure you want to delete '" + system + "'?", "Delete System Information", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

            if (dr == System.Windows.Forms.DialogResult.Yes)
            {
                XmlDocument xDoc = new XmlDocument();
                using (Stream s = File.OpenRead(file))
                {
                    xDoc.Load(s);
                }
                XmlNode node = xDoc.SelectSingleNode("//Credentials/System[@name='" + system + "']");
                if (node != null)
                {
                    XmlNode parent = node.ParentNode;
                    parent.RemoveChild(node);
                    string newXML = xDoc.OuterXml;
                    xDoc.Save(file);
                }
                using (Stream s = File.OpenRead(file))
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(s);
                    comboBox.SelectedIndex = 0;
                    comboBox.Items.Remove(system);
                }
            }
        }
    }
}