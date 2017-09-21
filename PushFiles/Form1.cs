using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography;

namespace PushFiles
{
    public partial class frmMain : Form
    {
        public int x = 1;  //stopping variable
        public string strUID;
        public string strPassword;
        public string strComputer;



        public frmMain()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, System.EventArgs e)
        {

            #region recover saved data from C:\Config\Properties.ini

            string a = File.ReadAllText(@"C:\Config\Properties.ini"); //read from file to string
            string[] words = a.Split('|'); // creates an array words where each entry is "a" word from a separated by '|'

            strUID = words[0]; //takes first word from file to the ID
            string encryptedPassword = words[1]; //takes second word from file to the string (in encrypted form)
            cboxRemember.Checked = Convert.ToBoolean(words[2]); //takes true or false and applies it to the checkbox


            txtUID.Text = strUID;
            txtPassword.Text = Encrypt.DecryptString(encryptedPassword, strUID);

            #endregion


        }

        private void Form1_FormClosing(Object sender, FormClosingEventArgs e)
        {
            if (cboxRemember.Checked == true) //write settings to file
            {
                Directory.CreateDirectory(@"C:\Config\");
                File.WriteAllText(@"C:\Config\Properties.ini", txtUID.Text + "|" + Encrypt.EncryptString(txtPassword.Text, txtUID.Text) + "|" + cboxRemember.Checked.ToString());
            }
            else
            {
                try
                {
                    File.Delete(@"C:\Config\Properties.ini");
                }
                catch { }
            }
        }

        private void txtComputername_KeyDown(object sender, KeyEventArgs e) //supposed to stop spaces . and \ from being processed
        {
            if (e.KeyCode == Keys.Space || e.KeyCode == Keys.OemPeriod || e.KeyCode == Keys.OemBackslash)
                e.Handled = false;

        }

        private void btnSend_Click(object sender, EventArgs e)
        {


            strUID = txtUID.Text;
            strPassword = txtPassword.Text;
            strComputer = txtComputerName.Text;



            try
            {
                MapRemoteComputer(strComputer); //map remote computer to K: for file transfer
                Directory.CreateDirectory(@"X:\ClearScript\"); // Creates target directory if not already there

            }
            catch
            {
                MessageBox.Show("Unable to map remote computer.\nIs the computer name correct?", "Error");

            }



            while (x == 1)
            {
                try
                {

                    File.Copy(@"C:\ClearScript\Clear.ps1", @"X:\ClearScript\Clear.ps1"); //copy files to remote
                    File.Copy(@"C:\ClearScript\LaunchClear.bat", @"X:\ClearScript\LaunchClear.bat");
                    x = 0;
                    MessageBox.Show("Successfully created files on remote computer.", "Success!");

                }
                catch
                {
                    MessageBox.Show("Files already exist on remote computer.", "Error");
                    x = 3; // stops execution
                }
            }

        }

        private void btnDisconnect_Click_1(object sender, EventArgs e)
        {
            x = 1;
            while (x == 1)
            {
                try
                {
                    File.Delete(@"X:\ClearScript\Clear.ps1");
                    File.Delete(@"X:\ClearScript\LaunchClear.bat");
                    x = 2;
                    MessageBox.Show("Successfully deleted files on remote computer.", "Success!");

                }
                catch
                {
                    MessageBox.Show("Unable to delete files on remote computer.", "Error");
                    x = 0; // stops execution
                }
            }

            while (x == 2)
                UnMapRemoteComputer();


        }

        private void MapRemoteComputer(string computer)
        {

            string args = $@"use X: \\{computer}\c$ /user:TMM\{strUID} {strPassword}";


            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = $"/C net.exe {args}";
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit(10000); //wait 10 seconds at max

            // System.Diagnostics.Process.Start("net.exe", arguments: args);
        }

        private void UnMapRemoteComputer()
        {
            string args = "use /delete X:";

            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = $"/C net {args}";
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit(10000); //wait 10 seconds at max
        }
        

      

    }

  public static class Encrypt
        {
            // This size of the IV (in bytes) must = (keysize / 8).  Default keysize is 256, so the IV must be
            // 32 bytes long.  Using a 16 character string here gives us 32 bytes when converted to a byte array.
            private const string initVector = "pemgail9uzpgzl88";  // This constant is used to determine the keysize of the encryption algorithm
            private const int keysize = 256;

            //Encrypt
            public static string EncryptString(string plainText, string passPhrase)
            {
                byte[] initVectorBytes = Encoding.UTF8.GetBytes(initVector);
                byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
                PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, null);
                byte[] keyBytes = password.GetBytes(keysize / 8);
                RijndaelManaged symmetricKey = new RijndaelManaged();
                symmetricKey.Mode = CipherMode.CBC;
                ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes);
                MemoryStream memoryStream = new MemoryStream();
                CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
                cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                cryptoStream.FlushFinalBlock();
                byte[] cipherTextBytes = memoryStream.ToArray();
                memoryStream.Close();
                cryptoStream.Close();
                return Convert.ToBase64String(cipherTextBytes);
            }

            //Decrypt
            public static string DecryptString(string cipherText, string passPhrase)
            {
                byte[] initVectorBytes = Encoding.UTF8.GetBytes(initVector);
                byte[] cipherTextBytes = Convert.FromBase64String(cipherText);
                PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, null);
                byte[] keyBytes = password.GetBytes(keysize / 8);
                RijndaelManaged symmetricKey = new RijndaelManaged();
                symmetricKey.Mode = CipherMode.CBC;
                ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes);
                MemoryStream memoryStream = new MemoryStream(cipherTextBytes);
                CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
                byte[] plainTextBytes = new byte[cipherTextBytes.Length];
                int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                memoryStream.Close();
                cryptoStream.Close();
                return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
            }
        }
    
}
