using CRPTAuthLib35;
using CRPTAuthLib35.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TestApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AuthLib35 authLib = new AuthLib35();
            AuthData authData = authLib.Reg("https://int01.gismt.crpt.tech/api/v3/true-api/auth/key");
            authData = authLib.Auth(authData, "https://int01.gismt.crpt.tech/api/v3/true-api/auth/simpleSignIn", textBox1.Text);
            
            if(!string.IsNullOrEmpty(authData.error_message))
            {
                textBox2.Text = authData.error_message;
            }
            else
            {
                textBox2.Text = authData.token;
            }
        }
    }
}
