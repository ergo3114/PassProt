using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for Window2.xaml
    /// </summary>
    public partial class Window2 : Window
    {
        public Window2()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.Registry.CurrentUser.CreateSubKey("PassProt\\");
            Microsoft.Win32.Registry.CurrentUser.OpenSubKey("PassProt\\", true).SetValue("mp", (string)Encrypt(textBox.Text), Microsoft.Win32.RegistryValueKind.String);
            this.Close();
            MainWindow createmainwindow = new MainWindow();
            createmainwindow.Show();
            createmainwindow.Visibility = Visibility.Visible;
        }

        public static string Encrypt(string plainText)
        {
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            string PasswordHash = "P@@Sw0rdH@$H";
            const string SaltKey = "S@LTY&KEY";
            const string VIKey = "@1B2c3D4e5F6g7H8";

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
    }
}
