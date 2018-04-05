using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Xml;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for UpdatePassword.xaml
    /// </summary>
    public partial class UpdatePassword : Window
    {
        public UpdatePassword()
        {
            InitializeComponent();
        }

        public UpdatePassword(string system)
        {
            InitializeComponent();
            textBoxSystem.Text = system;
            PopulateTextBoxeswithXMLValueFromSystem(textBoxSystem.Text);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MainWindow mainWindow = new MainWindow("donotshow");
            mainWindow.Show();
            mainWindow.Visibility = Visibility.Visible;
        }

        private void buttonUpdate_Click(object sender, RoutedEventArgs e)
        {
            buttonUpdate.IsEnabled = false;
            XmlTextReader reader = new XmlTextReader("C:\\Users\\David\\Documents\\systemcreds.xml");
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(reader);
            string system = textBoxSystem.Text;
            xDoc.SelectSingleNode("/Credentials/System [@name='" + system + "']")["Password"].InnerText = Encrypt(textBoxPassword.Password.ToString());
            xDoc.SelectSingleNode("/Credentials/System [@name='" + system + "']").Attributes[1].Value = DateTime.Now.ToString();
            reader.Close();
            xDoc.Save("C:\\Users\\David\\Documents\\systemcreds.xml");
            buttonUpdate.IsEnabled = true;
            this.Close();
        }

        private static string Encrypt(string plainText)
        {
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            string PasswordHash = "P@@Sw0rdH@$H";
            const string SaltKey = "S@LTY&KEY";
            const string VIKey = "@1B2c3D4e5F6g7H8J8";

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
            const string VIKey = "@1B2c3D4e5F6g7H8";

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

        private void PopulateTextBoxeswithXMLValueFromSystem(string system)
        {
            string file = "C:\\Users\\David\\Documents\\systemcreds.xml";
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(file);
            XmlNodeList xmlList = xDoc.SelectNodes("/Credentials/System [@name='" + system +"']");
            foreach(XmlNode xn in xmlList)
            {
                string username = xn["Username"].InnerText;
                textBoxUsername.Text = Decrypt(username);
            }
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
