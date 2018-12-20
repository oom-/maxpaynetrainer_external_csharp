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
        bool needToLookProcess, ammo, health, painkiller, slowmotion;
        MemUtil mu;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            needToLookProcess = true;
            ammo = false;
            health = false;
            painkiller = false;
            slowmotion = false;
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
            mu.WritePatternFromAddr(adress, new byte[] { 0x12, 0x00, 0x00, 0x00 });
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
            mu.WritePatternFromAddr(adress, new byte[] { 0x00, 0x00, 0x70, 0x42 });
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
            mu.WritePatternFromAddr(adress, new byte[] { 0x08, 0x00, 0x00, 0x00 });

        }

        public void WriteSlowMotion()
        {
            IntPtr adress = mu.GetModuleAdresseByName("sndmfc.dll") + 0x000159B4;
            adress = mu.ReadPtr(adress);
            adress += 0xC;
            adress = mu.ReadPtr(adress);
            adress += 0x8;
            adress = mu.ReadPtr(adress);
            adress += 0xF4;
            adress = mu.ReadPtr(adress);
            adress += 0x7A4;
            Debug.WriteLine("SlowMotion 0x" + adress.ToString("X4"));
            mu.WritePatternFromAddr(adress, new byte[] { 0x00, 0x00, 0x80, 0x3F });
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            health = checkBox1.Checked;
        }


        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            slowmotion = checkBox2.Checked;
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            ammo = checkBox3.Checked;
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            painkiller = checkBox4.Checked;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (needToLookProcess)
            {
                timer1.Stop();
                if (mu.OpenHandle("maxpayne"))
                {
                    label1.Text = "Attached.";
                    checkBox1.Enabled = true;
                    checkBox2.Enabled = true;
                    checkBox3.Enabled = true;
                    checkBox4.Enabled = true;
                    needToLookProcess = false;
                }
                timer1.Start();
            }
            else
            {
                timer1.Stop();
                try
                {
                    if (health)
                        WriteHealth();
                    if (ammo)
                        WriteAmmo();
                    if (painkiller)
                        WritePainKiller();
                    if (slowmotion)
                        WriteSlowMotion();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Maxpayne is close. Run it again and run the trainer again when the save will be loaded.");
                    Application.Exit();
                }
                timer1.Start();
            }
        }
    }
}
