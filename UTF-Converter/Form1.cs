using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UTF_Converter
{
    public partial class Form1 : System.Windows.Forms.Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public String ChangeBytes32(String input)
        {
            input = input.Replace(" ", "");
            String output = "";
            for (int i = 0; i < input.Length; i += 8)
            {
                int tmp = Convert.ToInt32(input.Substring(i, 8), 16);
                int reversedBytes = System.Net.IPAddress.NetworkToHostOrder(tmp);
                String start = reversedBytes.ToString("X").PadLeft(8, '0');
                output += start;
            }
            return output;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            byte[] bytes = { };
            int characters = 0;
            if (radioButton1.Checked)
            {
                bytes = Encoding.UTF8.GetBytes(textBox1.Text);
                characters = 2;
            }
            else if (radioButton2.Checked && !checkBox1.Checked)
            {
                bytes = Encoding.BigEndianUnicode.GetBytes(textBox1.Text);
                characters = 4;
            }
            else if (radioButton2.Checked && checkBox1.Checked)
            {
                bytes = Encoding.Unicode.GetBytes(textBox1.Text);
                characters = 4;
            }
            else if (radioButton3.Checked)
            {
                bytes = Encoding.UTF32.GetBytes(textBox1.Text);
                characters = 8;
            }
            textBox2.Text = BitConverter.ToString(bytes).Replace("-", "");
            textBox2.Text = ((radioButton3.Checked && !checkBox1.Checked) ? ChangeBytes32(textBox2.Text) : textBox2.Text);
            if (checkBox2.Checked)
                textBox2.Text = Regex.Replace(textBox2.Text, ".{" + characters + "}", "$0" + textBox3.Text);
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            checkBox1.Enabled = false;
            checkBox2.Text = "Seperate each byte with:";
            button3.Text = "Load UTF-8 encoded file";
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            checkBox1.Enabled = true;
            checkBox2.Text = "Seperate each 2 bytes with:";
            button3.Text = (checkBox1.Checked ? "Load little endian UTF-16 encoded file" : "Load big endian UTF-16 encoded file");
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            checkBox1.Enabled = true;
            checkBox2.Text = "Seperate each 4 bytes with:";
            button3.Text = (checkBox1.Checked ? "Load little endian UTF-32 encoded file" : "Load big endian UTF-32 encoded file");
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked) // i would love to do a switch statement but sadly these are all booleans so it's not practical. this is the most effective way to do it
                button3.Text = "Load UTF-8 encoded file";
            else if (radioButton2.Checked && checkBox1.Checked)
                button3.Text = "Load little endian UTF-16 encoded file";
            else if (radioButton3.Checked && checkBox1.Checked)
                button3.Text = "Load little endian UTF-32 encoded file";
            else if (radioButton2.Checked && !checkBox1.Checked)
                button3.Text = "Load big endian UTF-16 encoded file";
            else
                button3.Text = "Load big endian UTF-32 encoded file";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialogue = new OpenFileDialog();
            dialogue.Title = "Open Text File";
            textBox1.Text = "Opening...";
            if (dialogue.ShowDialog() != DialogResult.OK)
                textBox1.Text = "";
            else
                textBox1.Text = File.ReadAllText(dialogue.FileName);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialogue = new OpenFileDialog();
            dialogue.Title = "Open UTF File";
            textBox1.Text = "Opening...";
            if (dialogue.ShowDialog() != DialogResult.OK)
            {
                textBox1.Text = textBox2.Text = "";
            }
            else
            {
                FileStream fs = new FileStream(dialogue.FileName, FileMode.Open);
                int input;
                textBox1.Text = "";
                textBox2.Text = "";
                for (int i = 0; (input = fs.ReadByte()) != -1; i++)
                {
                    String hex = string.Format("{0:X2}", input);
                    textBox2.Text += hex;
                }
            }
        }

        private byte[] FromHex(string hex)
        {
            hex = hex.Replace(textBox3.Text, "");
            byte[] raw = new byte[hex.Length / 2];
            for (int i = 0; i < raw.Length; i++)
            {
                raw[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            }
            return raw;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            byte[] data = ((radioButton3.Checked && !checkBox1.Checked) ? FromHex(ChangeBytes32(textBox2.Text)) : FromHex(textBox2.Text));
            if (radioButton1.Checked)
            {
                textBox1.Text = Encoding.ASCII.GetString(data);
            }
            else if (radioButton2.Checked && checkBox1.Checked)
            {
                textBox1.Text = Encoding.Unicode.GetString(data);
            }
            else if (radioButton2.Checked && !checkBox1.Checked)
            {
                textBox1.Text = Encoding.BigEndianUnicode.GetString(data);
            }
            else if (radioButton3.Checked)
            {
                textBox1.Text = Encoding.UTF32.GetString(data);
            }
        }
    }
}
