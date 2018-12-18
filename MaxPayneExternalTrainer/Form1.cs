using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MaxPayneExternalTrainer
{
    public partial class Form1 : Form
    {
        MemUtil mu;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            mu = new MemUtil();
        }

        private void WriteAmmo()
        {
            IntPtr adress = mu.GetMainModuleAdress() + 0x004B709C;
            adress = mu.ReadPtr(adress);
            adress += 0x398; 
            adress = mu.ReadPtr(adress);
            adress += 0xC;
            adress = mu.ReadPtr(adress);
            adress += 0x1CB;
            Debug.WriteLine("Ammo: 0x" + adress.ToString("X4"));
            //mu.WritePatternFromAddr(adress, new byte[] { 0x12, 0x00, 0x00, 0x00 });
        }

        public void WriteHealth()
        {
            IntPtr adress = mu.GetMainModuleAdress() + 0x004B709C;
            adress = mu.ReadPtr(adress);
            adress += 0x3A0;
            adress = mu.ReadPtr(adress);
            adress += 0x68;
            adress = mu.ReadPtr(adress);
            adress += 0x10F;
            Debug.WriteLine("Health: 0x" + adress.ToString("X4"));
            //mu.WritePatternFromAddr(adress, new byte[] { 0x00, 0x00, 0x70, 0x42 });
        }

        public void WritePainKiller()
        {
            IntPtr adress = mu.GetMainModuleAdress() + 0x004B709C;
            adress = mu.ReadPtr(adress);
            adress += 0x398;
            adress = mu.ReadPtr(adress);
            adress += 0x10;
            adress = mu.ReadPtr(adress);
            adress += 0x186;
            Debug.WriteLine("PainKiller: 0x" + adress.ToString("X4"));
            //mu.WritePatternFromAddr(adress, new byte[] { 0x08, 0x00, 0x00, 0x00 });

        }

        public void WriteSlowMotion()
        {
           
        }

       

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            
        }


        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {

            WriteHealth();
            WriteAmmo();
            WritePainKiller();
            WriteSlowMotion();


        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (mu.OpenHandle("maxpayne"))
            {
                timer1.Stop();
                label1.Text = "Attached.";
                checkBox1.Enabled = true;
                checkBox2.Enabled = true;
                checkBox3.Enabled = true;
            }
        }
    }
}
