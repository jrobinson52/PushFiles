using System;
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
            if (Properties.Settings.Default.UID != string.Empty)
                txtUID.Text = Properties.Settings.Default.UID;
            if (Properties.Settings.Default.Password != string.Empty)
                txtPassword.Text = Properties.Settings.Default.Password;
            if (Properties.Settings.Default.Remember == true)
                cboxRemember.Checked = Properties.Settings.Default.Remember;
        }

        private void Form1_FormClosing(Object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.Save();
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e) //supposed to stop spaces . and \ from being processed
        {
            if (e.KeyCode == Keys.Space || e.KeyCode == Keys.OemPeriod || e.KeyCode == Keys.OemBackslash)
                e.Handled = false;

        }

        public SecureString ConvertToSecureString(string password) //creates secure password string for pushing files
        {
            if (password == null)
                throw new ArgumentNullException("password");

            var securePassword = new SecureString();

            foreach (char c in password)
                securePassword.AppendChar(c);

            securePassword.MakeReadOnly();
            return securePassword;
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (cboxRemember.Checked == true)
            {
                Properties.Settings.Default.UID = txtUID.Text;
                Properties.Settings.Default.Password = txtPassword.Text;
                Properties.Settings.Default.Remember = cboxRemember.Checked;
            }

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
}
