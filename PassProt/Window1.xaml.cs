using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Text;
using System.Windows;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Linq;
using System;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            string file = "C:\\Users\\David\\Documents\\systemcreds.xml";
            string txtboxSystem = textBoxSystem.Text;
            string txtboxUsername = Encrypt(textBoxUsername.Text);
            string encryptedPassword = Encrypt(textBoxPassword.Password);
            if (FileOrDirectoryExists(file))
            {
                XDocument xDocument = XDocument.Load(file);
                XElement root = xDocument.Element("Credentials");
                IEnumerable<XElement> rows = root.Descendants("System");
                XElement firstRow = rows.FirstOrDefault();
                firstRow.AddAfterSelf(
                    new XElement("System",
                    new XAttribute("name", txtboxSystem),
                    new XAttribute("timeset", DateTime.Now.ToString()),
                    new XElement("Name", txtboxSystem),
                    new XElement("Username", txtboxUsername),
                    new XElement("Password", encryptedPassword)));
                xDocument.Save(file);
            }
            else
            {
                XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
                xmlWriterSettings.Indent = true;
                xmlWriterSettings.NewLineOnAttributes = true;
                using (XmlWriter xmlWriter = XmlWriter.Create(file, xmlWriterSettings))
                {
                    xmlWriter.WriteStartDocument();
                    xmlWriter.WriteStartElement("Credentials");

                    xmlWriter.WriteStartElement("System");
                    xmlWriter.WriteElementString("Name", txtboxSystem);
                    xmlWriter.WriteElementString("Username", txtboxUsername);
                    xmlWriter.WriteElementString("Password", encryptedPassword);
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteEndDocument();
                    xmlWriter.Flush();
                    xmlWriter.Close();
                }
            }
            textBoxSystem.Text = "";
            textBoxUsername.Text = "";
            textBoxPassword.Password = "";
        }

        internal static bool FileOrDirectoryExists(string name) { return (Directory.Exists(name) || File.Exists(name)); }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MainWindow mainWindow = new MainWindow("donotshow");
            mainWindow.Show();
            mainWindow.Visibility = Visibility.Visible;
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
    }
}
