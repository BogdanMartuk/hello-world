using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Rozpiznavannya_obraziv
{
    public partial class Zadat_EPR : Form
    {
        public Zadat_EPR()
        {
            InitializeComponent();
        }

        public double R = 0;
        public double K = 0;
        public double elementRazreshenie = 0;

        public double EPR_Etalon=0;
        public double EPR_Test = 0;

        public double Power_Etalon = 0;
        public double Power_Test = 0;

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                R = Convert.ToDouble(textBox1.Text);
                K = Convert.ToDouble(textBox2.Text);
                elementRazreshenie = Convert.ToDouble(textBox2.Text);

                if (radioButton1.Checked==true)
                {
                    EPR_Etalon = Convert.ToDouble(textBox4.Text);
                    EPR_Test = Convert.ToDouble(textBox5.Text);
                }

                if (radioButton2.Checked == true)
                {
                    Power_Etalon = Convert.ToDouble(textBox6.Text);
                    Power_Test = Convert.ToDouble(textBox7.Text);
                }

                this.DialogResult = DialogResult.OK;
            }
            catch
            {
                MessageBox.Show("Перевірте задані параметри");
            }
        }
    }
}
