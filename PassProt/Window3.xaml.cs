using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Xml;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for Window3.xaml
    /// </summary>
    public partial class Window3 : Window
    {
        public Window3()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            string masterpassword = Decrypt((string)Microsoft.Win32.Registry.CurrentUser.OpenSubKey("PassProt\\", true).GetValue("mp"));
            if (textBox.Password == masterpassword)
            {
                Successful();
                this.Close();
            }
            else
            {
                masterpassword = null;
                string message = "The password you entered is incorrect.";
                string caption = "Master Password is incorrect!";
                this.Visibility = Visibility.Hidden;
                DialogResult result = System.Windows.Forms.MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                GC.Collect();
                GC.WaitForPendingFinalizers();
                this.Close();
            }
        }

        public static string Decrypt(string encryptedText)
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

        private void Successful()
        {
            if (comboBox.SelectedIndex != 0)
            {
                Microsoft.Win32.Registry.CurrentUser.OpenSubKey("PassProt\\", true).SetValue("Delay", comboBox.SelectedIndex, Microsoft.Win32.RegistryValueKind.String);
                Microsoft.Win32.Registry.CurrentUser.OpenSubKey("PassProt\\", true).SetValue("Time", DateTime.Now.ToString("d MMMM yyyy hh:mm:ss tt"), Microsoft.Win32.RegistryValueKind.String);
            }
            
            foreach (Window window in System.Windows.Application.Current.Windows)
            {
                if (window.GetType() == typeof(MainWindow))
                {
                    string file = "C:\\Users\\David\\Documents\\systemcreds.xml";
                    (window as MainWindow).Visibility = Visibility.Visible;
                    XmlDocument xDoc = new XmlDocument();
                    xDoc.Load(file);
                    XmlNodeList systems = xDoc.GetElementsByTagName("Name");
                    foreach (XmlNode node in systems)
                    {
                        (window as MainWindow).comboBox.Items.Add(node.InnerText);
                    }
                }
            }
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        //private void textBox_KeyUp(object sender, KeyEventArgs e)
        //{
        //    if(e.KeyCode == Keys.Enter)
        //    {
        //        string masterpassword = Decrypt((string)Microsoft.Win32.Registry.CurrentUser.OpenSubKey("PassProt\\", true).GetValue("mp"));
        //        if (textBox.Password == masterpassword)
        //        {
        //            Successful();
        //            this.Close();
        //        }
        //        else
        //        {
        //            masterpassword = null;
        //            string message = "The password you entered is incorrect.";
        //            string caption = "Master Password is incorrect!";
        //            this.Visibility = Visibility.Hidden;
        //            DialogResult result = System.Windows.Forms.MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
        //            GC.Collect();
        //            GC.WaitForPendingFinalizers();
        //            this.Close();
        //        }
        //    }
        //}
    }
}
