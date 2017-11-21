using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Rozpiznavannya_obraziv
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        List<Bitmap> Im = new List<Bitmap>();
        Bitmap Etalon;
        Bitmap Test;
        int KilkistZobr = 0;
        double PersentBin = 0;

        int otsuThreshold(List<byte> image, int colorNumb)
        {
            // Проверки на NULL и проч. опустим, чтобы сконцетрироваться
            // на работе метода

            // Посчитаем минимальную и максимальную яркость всех пикселей
            int min = image[0];
            int max = image[0];

            for (int i = 1; i < image.Count; i++)
            {
                int value = image[i];

                if (value < min)
                    min = value;

                if (value > max)
                    max = value;
            }

            // Гистограмма будет ограничена снизу и сверху значениями min и max,
            // поэтому нет смысла создавать гистограмму размером 256 бинов
            int histSize = max - min + 1;
            int[] hist = new int[histSize];

            // Заполним гистограмму нулями
            for (int t = 0; t < histSize; t++)
                hist[t] = 0;

            // И вычислим высоту бинов
            for (int i = 0; i < image.Count; i++)
                hist[image[i] - min]++;

            if (colorNumb == 1)
            {
                chart1.Series[0].Points.Clear();
                chart1.Series[1].Points.Clear();
                chart1.ChartAreas[0].AxisY.Minimum = 0;
                chart1.ChartAreas[0].AxisY.Maximum = Math.Round((double)hist.Max() / image.Count, 4);
                chart1.ChartAreas[0].AxisX.Minimum = min;
                chart1.ChartAreas[0].AxisX.Maximum = max;
                for (int i = 0; i < histSize; i++)
                {
                    chart1.Series[0].Points.AddXY(min + i, Math.Round((double)hist[i] / image.Count, 4));
                }
            }
            if (colorNumb == 2)
            {
                chart2.Series[0].Points.Clear();
                chart2.Series[1].Points.Clear();
                chart2.ChartAreas[0].AxisY.Minimum = 0;
                chart2.ChartAreas[0].AxisY.Maximum = Math.Round((double)hist.Max() / image.Count, 4);
                chart2.ChartAreas[0].AxisX.Minimum = min;
                chart2.ChartAreas[0].AxisX.Maximum = max;
                for (int i = 0; i < histSize; i++)
                {
                    chart2.Series[0].Points.AddXY(min + i, Math.Round((double)hist[i] / image.Count, 4));
                }
            }
            if (colorNumb == 3)
            {
                chart3.Series[0].Points.Clear();
                chart3.Series[1].Points.Clear();
                chart3.ChartAreas[0].AxisY.Minimum = 0;
                chart3.ChartAreas[0].AxisY.Maximum = Math.Round((double)hist.Max() / image.Count, 4);
                chart3.ChartAreas[0].AxisX.Minimum = min;
                chart3.ChartAreas[0].AxisX.Maximum = max;
                for (int i = 0; i < histSize; i++)
                {
                    chart3.Series[0].Points.AddXY(min + i, Math.Round((double)hist[i] / image.Count, 4));
                }
            }
            if (colorNumb == 4)
            {
                chart4.Series[0].Points.Clear();
                chart4.Series[1].Points.Clear();
                chart4.ChartAreas[0].AxisY.Minimum = 0;
                chart4.ChartAreas[0].AxisY.Maximum = Math.Round((double)hist.Max() / image.Count, 4);
                chart4.ChartAreas[0].AxisX.Minimum = min;
                chart4.ChartAreas[0].AxisX.Maximum = max;
                for (int i = 0; i < histSize; i++)
                {
                    chart4.Series[0].Points.AddXY(min + i, Math.Round((double)hist[i] / image.Count, 4));
                }
            }


            // Введем два вспомогательных числа:
            int m = 0; // m - сумма высот всех бинов, домноженных на положение их середины
            int n = 0; // n - сумма высот всех бинов
            for (int t = 0; t <= max - min; t++)
            {
                m += t * hist[t];
                n += hist[t];
            }

            float maxSigma = -1; // Максимальное значение межклассовой дисперсии
            int threshold = 0; // Порог, соответствующий maxSigma

            int alpha1 = 0; // Сумма высот всех бинов для класса 1
            int beta1 = 0; // Сумма высот всех бинов для класса 1, домноженных на положение их середины

            // Переменная alpha2 не нужна, т.к. она равна m - alpha1
            // Переменная beta2 не нужна, т.к. она равна n - alpha1

            // t пробегается по всем возможным значениям порога
            for (int t = 0; t < max - min; t++)
            {
                alpha1 += t * hist[t];
                beta1 += hist[t];

                // Считаем вероятность класса 1.
                float w1 = (float)beta1 / n;
                // Нетрудно догадаться, что w2 тоже не нужна, т.к. она равна 1 - w1

                // a = a1 - a2, где a1, a2 - средние арифметические для классов 1 и 2
                float a = (float)alpha1 / beta1 - (float)(m - alpha1) / (n - beta1);

                // Наконец, считаем sigma
                float sigma = w1 * (1 - w1) * a * a;

                // Если sigma больше текущей максимальной, то обновляем maxSigma и порог
                if (sigma > maxSigma)
                {
                    maxSigma = sigma;
                    threshold = t;
                }
            }

            // Не забудем, что порог отсчитывался от min, а не от нуля
            threshold += min;

            if (colorNumb == 1)
            {
                chart1.Series[1].Points.AddXY(threshold, Math.Round((double)hist.Max() / image.Count, 4));
            }
            if (colorNumb == 2)
            {
                chart2.Series[1].Points.AddXY(threshold, Math.Round((double)hist.Max() / image.Count, 4));
            }
            if (colorNumb == 3)
            {
                chart3.Series[1].Points.AddXY(threshold, Math.Round((double)hist.Max() / image.Count, 4));
            }
            if (colorNumb == 4)
            {
                chart4.Series[1].Points.AddXY(threshold, Math.Round((double)hist.Max() / image.Count, 4));
            }

            // Все, порог посчитан, возвращаем его наверх :)
            return threshold;
        }

        void Gist_RGB_and_GREY()
        {
            List<byte> ColorR = new List<byte>();
            List<byte> ColorG = new List<byte>();
            List<byte> ColorB = new List<byte>();
            List<byte> ColorGREY = new List<byte>();
            byte[,,] rgb = RGB.BitmapToByteRgbNaive(Im[comboBox1.SelectedIndex]);
            int width = Im[comboBox1.SelectedIndex].Width,
                height = Im[comboBox1.SelectedIndex].Height;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    ColorR.Add(rgb[0, y, x]);
                    ColorG.Add(rgb[1, y, x]);
                    ColorB.Add(rgb[2, y, x]);
                    ColorGREY.Add(RGB.Limit(0.2125 * rgb[0, y, x] + 0.7154 * rgb[1, y, x] + 0.0721 * rgb[2, y, x]));
                }
            }
            int Porig1R = otsuThreshold(ColorR, 1);
            int Porig1G = otsuThreshold(ColorG, 2);
            int Porig1B = otsuThreshold(ColorB, 3);
            int Porig1GREY = otsuThreshold(ColorGREY, 4);
        }

        private void зчитатиЗображенняToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                KilkistZobr++;

                Bitmap bmp = RGB.LoadBitmap(openFileDialog1.FileName);

                int width = bmp.Width,
                    height = bmp.Height;

                Im.Add(bmp);

                label1.Text = "W = " + width.ToString() + ", H = " + height.ToString() + ", W*H = " + (width * height).ToString();

                comboBox1.Items.Add("Zobr" + (KilkistZobr).ToString());
                comboBox1.Text = "Zobr" + (KilkistZobr).ToString();

                chart1.Series[0].Points.Clear();
                chart2.Series[0].Points.Clear();
                chart3.Series[0].Points.Clear();
                chart4.Series[0].Points.Clear();
                chart1.Series[1].Points.Clear();
                chart2.Series[1].Points.Clear();
                chart3.Series[1].Points.Clear();
                chart4.Series[1].Points.Clear();

                Gist_RGB_and_GREY();
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            pictureBox1.Image = Im[comboBox1.SelectedIndex];
        }

        private void градаціяСірогоToolStripMenuItem_Click(object sender, EventArgs e)
        {
            byte[,,] rgb = RGB.BitmapToByteRgbNaive(Im[comboBox1.SelectedIndex]);

            int width = Im[comboBox1.SelectedIndex].Width,
                height = Im[comboBox1.SelectedIndex].Height;

            byte[,,] rgbN = new byte[3, height, width];

            double buf = 0;

            dataGridView1.ColumnCount = width;
            dataGridView1.RowCount = height;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    buf = 0.2125 * rgb[0, y, x] + 0.7154 * rgb[1, y, x] + 0.0721 * rgb[2, y, x];

                    rgbN[0, y, x] = RGB.Limit(buf);
                    rgbN[1, y, x] = RGB.Limit(buf);
                    rgbN[2, y, x] = RGB.Limit(buf);

                    if (rgbN[0, y, x] != 255)
                        dataGridView1.Rows[y].Cells[x].Value = Convert.ToString(rgbN[0, y, x]);
                }
            }

            Etalon = RGB.RgbToBitmapNaive(rgbN);

            Im.Add(RGB.RgbToBitmapNaive(rgbN));

            comboBox1.Items.Add(comboBox1.Text + "+GS");
            comboBox1.Text = comboBox1.Text + "+GS";

            label2.Text = "GS";
        }

        private void побудуватиГістограмуПоКольорамToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Gist_RGB_and_GREY();
        }

        private void однаГраницяToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<byte> ColorGREY = new List<byte>();
            byte[,,] rgb = RGB.BitmapToByteRgbNaive(Im[comboBox1.SelectedIndex]);
            int width = Im[comboBox1.SelectedIndex].Width,
                height = Im[comboBox1.SelectedIndex].Height;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    ColorGREY.Add(RGB.Limit(0.2125 * rgb[0, y, x] + 0.7154 * rgb[1, y, x] + 0.0721 * rgb[2, y, x]));
                }
            }
            int Porig1GREY = otsuThreshold(ColorGREY, 4);

            byte[,,] rgbN = new byte[3, height, width];

            double buf = 0;

            PersentBin = 0;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    buf = 0.2125 * rgb[0, y, x] + 0.7154 * rgb[1, y, x] + 0.0721 * rgb[2, y, x];

                    if (buf <= Porig1GREY)
                    {
                        PersentBin++;
                        rgbN[0, y, x] = RGB.Limit(255);
                        rgbN[1, y, x] = RGB.Limit(255);
                        rgbN[2, y, x] = RGB.Limit(255);
                    }
                    else
                    {
                        rgbN[0, y, x] = RGB.Limit(0);
                        rgbN[1, y, x] = RGB.Limit(0);
                        rgbN[2, y, x] = RGB.Limit(0);
                    }

                    //if (buf <= Porig1GREY)
                    //{
                    //    PersentBin++;
                    //    rgbN[0, y, x] = RGB.Limit(0);
                    //    rgbN[1, y, x] = RGB.Limit(0);
                    //    rgbN[2, y, x] = RGB.Limit(0);
                    //}
                    //else
                    //{
                    //    rgbN[0, y, x] = RGB.Limit(255);
                    //    rgbN[1, y, x] = RGB.Limit(255);
                    //    rgbN[2, y, x] = RGB.Limit(255);
                    //}

                }
            }

            PersentBin /= (width * height);

            Im.Add(RGB.RgbToBitmapNaive(rgbN));

            comboBox1.Items.Add(comboBox1.Text + "+Bin");
            comboBox1.Text = comboBox1.Text + "+Bin";

            label2.Text = "Bin";
            label3.Text = Math.Round(PersentBin, 4).ToString();

        }

        int[,] StructElement(int numb)
        {
            int[,] S = new int[3, 3];

            if (numb == 0)
            {
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        S[i, j] = 1;
                    }
                }
            }
            if (numb == 1)
            {
                S[0, 0] = 0; S[0, 1] = 1; S[0, 2] = 0;
                S[1, 0] = 1; S[1, 1] = 1; S[1, 2] = 1;
                S[2, 0] = 0; S[2, 1] = 1; S[2, 2] = 0;
            }

            return S;
        }

        private void нарощенняToolStripMenuItem_Click(object sender, EventArgs e)
        {
            byte[,,] rgb = RGB.BitmapToByteRgbNaive(Im[comboBox1.SelectedIndex]);
            int width = Im[comboBox1.SelectedIndex].Width,
                height = Im[comboBox1.SelectedIndex].Height;

            byte[,,] rgbN = new byte[3, height, width];

            int Rozmir_maski_S = 1;
            int[,] S = StructElement(toolStripComboBox1.SelectedIndex);

            for (int y = Rozmir_maski_S; y < height - Rozmir_maski_S; y++)
            {
                for (int x = Rozmir_maski_S; x < width - Rozmir_maski_S; x++)
                {
                    if (rgb[0, y, x] == 255)
                    {
                        for (int i = -Rozmir_maski_S; i <= Rozmir_maski_S; i++)
                        {
                            for (int j = -Rozmir_maski_S; j <= Rozmir_maski_S; j++)
                            {
                                if (S[1 + i, 1 + j] == 1)
                                {
                                    rgbN[0, y + i, x + j] = RGB.Limit(255);
                                    rgbN[1, y + i, x + j] = RGB.Limit(255);
                                    rgbN[2, y + i, x + j] = RGB.Limit(255);
                                }
                            }
                        }
                    }
                }
            }

            Im.Add(RGB.RgbToBitmapNaive(rgbN));

            comboBox1.Items.Add(comboBox1.Text + "+N" + toolStripComboBox1.SelectedIndex);
            comboBox1.Text = comboBox1.Text + "+N" + toolStripComboBox1.SelectedIndex;

            label2.Text = "N" + toolStripComboBox1.SelectedIndex;
        }

        private void ерозіяToolStripMenuItem_Click(object sender, EventArgs e)
        {
            byte[,,] rgb = RGB.BitmapToByteRgbNaive(Im[comboBox1.SelectedIndex]);
            int width = Im[comboBox1.SelectedIndex].Width,
                height = Im[comboBox1.SelectedIndex].Height;

            byte[,,] rgbN = new byte[3, height, width];

            int Rozmir_maski_S = 1;
            int[,] S = StructElement(toolStripComboBox1.SelectedIndex);

            bool nullPoint = false;

            for (int y = Rozmir_maski_S; y < height - Rozmir_maski_S; y++)
            {
                for (int x = Rozmir_maski_S; x < width - Rozmir_maski_S; x++)
                {
                    if (rgb[0, y, x] == 255)
                    {
                        nullPoint = false;

                        for (int i = -Rozmir_maski_S; i <= Rozmir_maski_S; i++)
                        {
                            for (int j = -Rozmir_maski_S; j <= Rozmir_maski_S; j++)
                            {
                                if (S[1 + i, 1 + j] == 1)
                                {
                                    if (rgb[0, y + i, x + j] == 0)
                                    {
                                        nullPoint = true;
                                    }
                                }
                            }
                        }

                        if (nullPoint == true)
                        {
                            rgbN[0, y, x] = RGB.Limit(0);
                            rgbN[1, y, x] = RGB.Limit(0);
                            rgbN[2, y, x] = RGB.Limit(0);
                        }
                        else
                        {
                            rgbN[0, y, x] = RGB.Limit(255);
                            rgbN[1, y, x] = RGB.Limit(255);
                            rgbN[2, y, x] = RGB.Limit(255);
                        }
                    }
                }
            }

            Im.Add(RGB.RgbToBitmapNaive(rgbN));

            comboBox1.Items.Add(comboBox1.Text + "+E" + toolStripComboBox1.SelectedIndex);
            comboBox1.Text = comboBox1.Text + "+E" + toolStripComboBox1.SelectedIndex;

            label2.Text = "E" + toolStripComboBox1.SelectedIndex;
        }

        private void замкненняToolStripMenuItem_Click(object sender, EventArgs e)
        {
            byte[,,] rgb = RGB.BitmapToByteRgbNaive(Im[comboBox1.SelectedIndex]);
            int width = Im[comboBox1.SelectedIndex].Width,
                height = Im[comboBox1.SelectedIndex].Height;

            byte[,,] rgbN = new byte[3, height, width];
            byte[,,] rgbNE = new byte[3, height, width];

            int Rozmir_maski_S = 1;
            int[,] S = StructElement(toolStripComboBox1.SelectedIndex);

            for (int y = Rozmir_maski_S; y < height - Rozmir_maski_S; y++)
            {
                for (int x = Rozmir_maski_S; x < width - Rozmir_maski_S; x++)
                {
                    if (rgb[0, y, x] == 255)
                    {
                        for (int i = -Rozmir_maski_S; i <= Rozmir_maski_S; i++)
                        {
                            for (int j = -Rozmir_maski_S; j <= Rozmir_maski_S; j++)
                            {
                                if (S[1 + i, 1 + j] == 1)
                                {
                                    rgbN[0, y + i, x + j] = RGB.Limit(255);
                                    rgbN[1, y + i, x + j] = RGB.Limit(255);
                                    rgbN[2, y + i, x + j] = RGB.Limit(255);
                                }
                            }
                        }
                    }
                }
            }

            bool nullPoint = false;

            for (int y = Rozmir_maski_S; y < height - Rozmir_maski_S; y++)
            {
                for (int x = Rozmir_maski_S; x < width - Rozmir_maski_S; x++)
                {
                    if (rgbN[0, y, x] == 255)
                    {
                        nullPoint = false;

                        for (int i = -Rozmir_maski_S; i <= Rozmir_maski_S; i++)
                        {
                            for (int j = -Rozmir_maski_S; j <= Rozmir_maski_S; j++)
                            {
                                if (S[1 + i, 1 + j] == 1)
                                {
                                    if (rgbN[0, y + i, x + j] == 0)
                                    {
                                        nullPoint = true;
                                    }
                                }
                            }
                        }

                        if (nullPoint == true)
                        {
                            rgbNE[0, y, x] = RGB.Limit(0);
                            rgbNE[1, y, x] = RGB.Limit(0);
                            rgbNE[2, y, x] = RGB.Limit(0);
                        }
                        else
                        {
                            rgbNE[0, y, x] = RGB.Limit(255);
                            rgbNE[1, y, x] = RGB.Limit(255);
                            rgbNE[2, y, x] = RGB.Limit(255);
                        }
                    }
                }
            }

            Im.Add(RGB.RgbToBitmapNaive(rgbNE));

            comboBox1.Items.Add(comboBox1.Text + "+Z" + toolStripComboBox1.SelectedIndex);
            comboBox1.Text = comboBox1.Text + "+Z" + toolStripComboBox1.SelectedIndex;

            label2.Text = "Z" + toolStripComboBox1.SelectedIndex;
        }

        private void розмиканняToolStripMenuItem_Click(object sender, EventArgs e)
        {
            byte[,,] rgb = RGB.BitmapToByteRgbNaive(Im[comboBox1.SelectedIndex]);
            int width = Im[comboBox1.SelectedIndex].Width,
                height = Im[comboBox1.SelectedIndex].Height;

            byte[,,] rgbE = new byte[3, height, width];
            byte[,,] rgbEN = new byte[3, height, width];

            int Rozmir_maski_S = 1;
            int[,] S = StructElement(toolStripComboBox1.SelectedIndex);

            bool nullPoint = false;

            for (int y = Rozmir_maski_S; y < height - Rozmir_maski_S; y++)
            {
                for (int x = Rozmir_maski_S; x < width - Rozmir_maski_S; x++)
                {
                    if (rgb[0, y, x] == 255)
                    {
                        nullPoint = false;

                        for (int i = -Rozmir_maski_S; i <= Rozmir_maski_S; i++)
                        {
                            for (int j = -Rozmir_maski_S; j <= Rozmir_maski_S; j++)
                            {
                                if (S[1 + i, 1 + j] == 1)
                                {
                                    if (rgb[0, y + i, x + j] == 0)
                                    {
                                        nullPoint = true;
                                    }
                                }
                            }
                        }

                        if (nullPoint == true)
                        {
                            rgbE[0, y, x] = RGB.Limit(0);
                            rgbE[1, y, x] = RGB.Limit(0);
                            rgbE[2, y, x] = RGB.Limit(0);
                        }
                        else
                        {
                            rgbE[0, y, x] = RGB.Limit(255);
                            rgbE[1, y, x] = RGB.Limit(255);
                            rgbE[2, y, x] = RGB.Limit(255);
                        }
                    }
                }
            }

            for (int y = Rozmir_maski_S; y < height - Rozmir_maski_S; y++)
            {
                for (int x = Rozmir_maski_S; x < width - Rozmir_maski_S; x++)
                {
                    if (rgbE[0, y, x] == 255)
                    {
                        for (int i = -Rozmir_maski_S; i <= Rozmir_maski_S; i++)
                        {
                            for (int j = -Rozmir_maski_S; j <= Rozmir_maski_S; j++)
                            {
                                if (S[1 + i, 1 + j] == 1)
                                {
                                    rgbEN[0, y + i, x + j] = RGB.Limit(255);
                                    rgbEN[1, y + i, x + j] = RGB.Limit(255);
                                    rgbEN[2, y + i, x + j] = RGB.Limit(255);
                                }
                            }
                        }
                    }
                }
            }

            Im.Add(RGB.RgbToBitmapNaive(rgbEN));

            comboBox1.Items.Add(comboBox1.Text + "+R" + toolStripComboBox1.SelectedIndex);
            comboBox1.Text = comboBox1.Text + "+R" + toolStripComboBox1.SelectedIndex;

            label2.Text = "R" + toolStripComboBox1.SelectedIndex;

        }

        int[,] El_poshuku(int a, int b)
        {
            int[,] Rez = new int[2 * b + 1, 2 * a + 1];

            double f = 0;

            for (int y = 0; y <= b; y++)
            {
                for (int x = -a; x <= a; x++)
                {
                    f = Math.Sqrt((double)(-b) * b * ((double)x * x / (a * a) - 1d));

                    if (y > f)
                    {
                        Rez[b + y, a + x] = 0;
                        Rez[b - y, a + x] = 0;
                    }
                    else
                    {
                        Rez[b + y, a + x] = 1;
                        Rez[b - y, a + x] = 1;
                    }


                }

            }

            return Rez;
        }

        private void зчитатиЗначПіввісейToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int a = Convert.ToInt32(toolStripTextBox1.Text);
            int b = Convert.ToInt32(toolStripTextBox2.Text);

            double alpha = Convert.ToDouble(toolStripTextBox5.Text);

            dataGridView1.ColumnCount = 2 * a + 1;
            dataGridView1.RowCount = 2 * b + 1;

            int[,] El = El_poshuku(a, b);

            for (int y = 0; y < 2 * b + 1; y++)
            {
                for (int x = 0; x < 2 * a + 1; x++)
                {
                    dataGridView1.Rows[y].Cells[x].Value = Convert.ToString(El[y, x]);
                }
            }

        }

        double CalcOrientashion(byte[,,] rgb, int width, int height)
        {
            double Rez = 0;

            byte[,,] rgbN = new byte[3, height, width];

            int cX = 0, cY = 0, k = 0;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (rgb[0, y, x] == 255 && rgb[1, y, x] == 255 && rgb[2, y, x] == 255)
                        ;
                    else
                    {
                        cY += y;
                        cX += x;
                        k++;
                    }
                }
            }

            cY = (int)Math.Round((double)cY / k);
            cX = (int)Math.Round((double)cX / k);

            MessageBox.Show("cY = " + cY.ToString() + " cX = " + cX.ToString());

            // 2. вычисление нескольких вспомогательных величин
            double SumUx = 0, SumUy = 0, SumUxy = 0, Ux, Uy, Uxy, C;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (rgb[0, y, x] == 255 && rgb[1, y, x] == 255 && rgb[2, y, x] == 255)
                        ;
                    else
                    {
                        SumUx = SumUx + Math.Pow(x - cX, 2);
                        SumUy = SumUy + Math.Pow(y - cY, 2);
                        SumUxy = SumUxy + (y - cY) * (x - cX);
                    }
                }
            }

            Ux = 1d / 12 + SumUx * 1 / k;
            Uy = 1d / 12 + SumUy * 1 / k;
            Uxy = 1d / 12 + SumUxy * 1 / k;
            C = Math.Sqrt(Math.Pow(Ux - Uy, 2) + 4 * Math.Pow(Uxy, 2));

            // 3. вычисление ориентации объекта
            Rez = 180d / Math.PI * Math.Atan2((Uy - Ux + C), (2 * Uxy)) - 90;
            if (Rez < 0)
                Rez = 180 + Rez;

            MessageBox.Show("Ugol = " + Rez.ToString());

            return Rez;
        }

        private void повернутиЕталонНаКутToolStripMenuItem_Click(object sender, EventArgs e)
        {
            byte[,,] rgb = RGB.BitmapToByteRgbNaive(Im[comboBox1.SelectedIndex]);
            int width = Im[comboBox1.SelectedIndex].Width,
                height = Im[comboBox1.SelectedIndex].Height;

            byte[,,] rgbN = new byte[3, height + 2, width + 2];
            byte[,] tmp = new byte[height + 2, width + 2];

            double alpha = CalcOrientashion(rgb, width, height);

            double phi = Convert.ToDouble(toolStripTextBox6.Text);

            List<double> nY = new List<double>();
            List<double> nX = new List<double>();

            double dx = 0, dy = 0;

            int count = 0;
            int pidizri = 0;
            int nevirno = 0;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (rgb[0, y, x] == 255 && rgb[1, y, x] == 255 && rgb[2, y, x] == 255)
                        ;
                    else
                    {
                        nX.Add(Math.Round((x - (int)(width / 2d)) * Math.Cos(phi * Math.PI / 180) + (y - (int)(height / 2d)) * Math.Sin(phi * Math.PI / 180), 4));
                        nY.Add(Math.Round(-1.0 * (x - (int)(width / 2d)) * Math.Sin(phi * Math.PI / 180) + (y - (int)(height / 2d)) * Math.Cos(phi * Math.PI / 180), 4));

                        //dx = Math.Abs();

                        count++;

                        if (tmp[(int)(height / 2d) + (int)nY[nY.Count - 1], (int)(width / 2d) + (int)nX[nX.Count - 1]] == 0)
                        {
                            tmp[(int)(height / 2d) + (int)nY[nY.Count - 1], (int)(width / 2d) + (int)nX[nX.Count - 1]] = 1;
                        }
                        else
                        {
                            pidizri++;

                            if ((int)(height / 2d) + (int)nY[nY.Count - 1] - 1 >= 0 &&
                                (int)(width / 2d) + (int)nX[nX.Count - 1] - 1 >= 0 &&
                                (int)(height / 2d) + (int)nY[nY.Count - 1] + 1 < height &&
                                (int)(width / 2d) + (int)nX[nY.Count - 1] + 1 < width)
                            {
                                if (tmp[(int)(height / 2d) + (int)nY[nY.Count - 1] - 1, (int)(width / 2d) + (int)nX[nX.Count - 1]] == 0)//-1,0
                                    tmp[(int)(height / 2d) + (int)nY[nY.Count - 1] - 1, (int)(width / 2d) + (int)nX[nX.Count - 1]] = 1;

                                else if (tmp[(int)(height / 2d) + (int)nY[nY.Count - 1], (int)(width / 2d) + (int)nX[nX.Count - 1] - 1] == 0)//0,-1
                                    tmp[(int)(height / 2d) + (int)nY[nY.Count - 1], (int)(width / 2d) + (int)nX[nX.Count - 1] - 1] = 1;

                                else if (tmp[(int)(height / 2d) + (int)nY[nY.Count - 1], (int)(width / 2d) + (int)nX[nY.Count - 1] + 1] == 0)//0,+1
                                    tmp[(int)(height / 2d) + (int)nY[nY.Count - 1], (int)(width / 2d) + (int)nX[nY.Count - 1] + 1] = 1;

                                else if (tmp[(int)(height / 2d) + (int)nY[nY.Count - 1] + 1, (int)(width / 2d) + (int)nX[nY.Count - 1]] == 0)//+1,0
                                    tmp[(int)(height / 2d) + (int)nY[nY.Count - 1] + 1, (int)(width / 2d) + (int)nX[nY.Count - 1]] = 1;

                                else if (tmp[(int)(height / 2d) + (int)nY[nY.Count - 1] - 1, (int)(width / 2d) + (int)nX[nY.Count - 1] - 1] == 0)//-1,-1
                                    tmp[(int)(height / 2d) + (int)nY[nY.Count - 1] - 1, (int)(width / 2d) + (int)nX[nY.Count - 1] - 1] = 1;

                                else if (tmp[(int)(height / 2d) + (int)nY[nY.Count - 1] - 1, (int)(width / 2d) + (int)nX[nY.Count - 1] + 1] == 0)//-1,+1
                                    tmp[(int)(height / 2d) + (int)nY[nY.Count - 1] - 1, (int)(width / 2d) + (int)nX[nY.Count - 1] + 1] = 1;

                                else if (tmp[(int)(height / 2d) + (int)nY[nY.Count - 1] + 1, (int)(width / 2d) + (int)nX[nY.Count - 1] - 1] == 0)//+1,-1
                                    tmp[(int)(height / 2d) + (int)nY[nY.Count - 1] + 1, (int)(width / 2d) + (int)nX[nY.Count - 1] - 1] = 1;

                                else if (tmp[(int)(height / 2d) + (int)nY[nY.Count - 1] + 1, (int)(width / 2d) + (int)nX[nY.Count - 1] + 1] == 0)//+1,+1
                                    tmp[(int)(height / 2d) + (int)nY[nY.Count - 1] + 1, (int)(width / 2d) + (int)nX[nY.Count - 1] + 1] = 1;
                                else
                                    nevirno++;
                            }
                        }
                    }

                }
            }

            MessageBox.Show("Кіл. точок не фону = " + count.ToString());
            MessageBox.Show("Pidozri= " + pidizri.ToString());
            MessageBox.Show("Nevorno= " + nevirno.ToString());

            count = 0;
            for (int y = 0; y < height + 2; y++)
            {
                for (int x = 0; x < width + 2; x++)
                {
                    if (tmp[y, x] == 1)
                    {
                        rgbN[0, y, x] = 255;
                        rgbN[1, y, x] = 255;
                        rgbN[2, y, x] = 255;

                        count++;
                    }
                    else
                    {
                        rgbN[0, y, x] = 0;
                        rgbN[1, y, x] = 0;
                        rgbN[2, y, x] = 0;
                    }


                }
            }

            MessageBox.Show("Кіл. точок без пропусків на зобр = " + count.ToString());

            Im.Add(RGB.RgbToBitmapNaive(rgbN));

            comboBox1.Items.Add(comboBox1.Text + "+Povorot");
            comboBox1.Text = comboBox1.Text + "+Povorot";

            label2.Text = "Povorot";

        }

        Bitmap rotIm(Bitmap inpt, float angel)
        {
            Bitmap res = new Bitmap(inpt.Width, inpt.Height);

            Graphics g = Graphics.FromImage(res);
            g.TranslateTransform((float)inpt.Width / 2, (float)inpt.Height / 2);
            g.RotateTransform(angel);
            g.TranslateTransform(-(float)inpt.Width / 2, -(float)inpt.Height / 2);
            //g.DrawImage(inpt, new Point((int)inpt.Width / 2, (int)inpt.Height / 2));
            g.DrawImage(inpt, new Point(0, 0));
            return res;
        }

        Bitmap korrectTest(Bitmap inpt)
        {
            Bitmap res;

            byte[,,] test = RGB.BitmapToByteRgbNaive(inpt);

            //byte[,,] testRes = new byte[3, res.Height, res.Width];

            for (int y = 0; y < inpt.Height; y++)
            {
                for (int x = 0; x < inpt.Width; x++)
                {
                    if (test[0, y, x] == 0 && test[1, y, x] == 0)
                    {
                        test[0, y, x] = 255;
                        test[1, y, x] = 255;
                        test[2, y, x] = 255;
                    }

                    if (test[0, y, x] == 0 && test[2, y, x] == 0)
                    {
                        test[0, y, x] = 255;
                        test[1, y, x] = 255;
                        test[2, y, x] = 255;
                    }

                    if (test[1, y, x] == 0 && test[2, y, x] == 0)
                    {
                        test[0, y, x] = 255;
                        test[1, y, x] = 255;
                        test[2, y, x] = 255;
                    }
                }
            }

            res = RGB.RgbToBitmapNaive(test);

            return res;
        }

        Bitmap privedenie_k_odnomu_razmeru(Bitmap inpt, int width, int height)
        {
            int iCy = (int)(inpt.Height / 2d);
            int iCx = (int)(inpt.Width / 2d);

            int Cy = (int)(height / 2d);
            int Cx = (int)(width / 2d);

            int Vy = Cy - iCy;
            int Vx = Cx - iCx;

            byte[,,] input = RGB.BitmapToByteRgbNaive(inpt);
            byte[,,] result = new byte[3, height, width];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    result[0, y, x] = 255;
                    result[1, y, x] = 255;
                    result[2, y, x] = 255;

                    if (Vy <= y && y < inpt.Height + Vy &&
                        Vx <= x && x < inpt.Width + Vx)
                    {
                        result[0, y, x] = input[0, y - Vy, x - Vx];
                        result[1, y, x] = input[1, y - Vy, x - Vx];
                        result[2, y, x] = input[2, y - Vy, x - Vx];
                    }
                }
            }

            return RGB.RgbToBitmapNaive(result);
        }

        //byte [,,] privedenie_k_odnomu_razmeru(byte[,,] input, int inpW,int inpH, int width,int height)
        //{
        //    int iCy = (int)(inpH / 2d);
        //    int iCx = (int)(inpW / 2d);

        //    int Cy = (int)(height / 2d);
        //    int Cx = (int)(width / 2d);

        //    int Vy = Cy - iCy;
        //    int Vx = Cx - iCx;

        //    byte[,,] result = new byte[3, height, width];

        //    for (int y = 0; y < height; y++)
        //    {
        //        for (int x = 0; x < width; x++)
        //        {
        //            if(y<inpH && x< inpW)
        //            {
        //                result[0, y + Vy, x + Vx] = input[0, y, x];
        //                result[1, y + Vy, x + Vx] = input[1, y, x];
        //                result[2, y + Vy, x + Vx] = input[2, y, x];
        //            }
        //            else
        //            {
        //                result[0, y, x] = 255;
        //                result[1, y, x] = 255;
        //                result[2, y, x] = 255;
        //            }

        //        }
        //    }

        //    return result;
        //}

        //+
        byte[,,] InterpolIm_null(byte[,,] rgb, int width, int height, int Interpol)
        {
            byte[,,] rgbN = new byte[3, height + 2 * Interpol, width + 2 * Interpol];

            for (int i = -Interpol; i < 0; i++)
            {
                for (int j = -Interpol; j < 0; j++)
                {
                    rgbN[0, Interpol + i, Interpol + j] = 255;//1
                    rgbN[1, Interpol + i, Interpol + j] = 255;
                    rgbN[2, Interpol + i, Interpol + j] = 255;

                    rgbN[0, Interpol + i, width + 2 * Interpol + j] = 255;
                    rgbN[1, Interpol + i, width + 2 * Interpol + j] = 255;
                    rgbN[2, Interpol + i, width + 2 * Interpol + j] = 255;

                    rgbN[0, height + 2 * Interpol + i, Interpol + j] = 255;//2
                    rgbN[1, height + 2 * Interpol + i, Interpol + j] = 255;
                    rgbN[2, height + 2 * Interpol + i, Interpol + j] = 255;

                    rgbN[0, height + 2 * Interpol + i, width + 2 * Interpol + j] = 255;//4
                    rgbN[1, height + 2 * Interpol + i, width + 2 * Interpol + j] = 255;
                    rgbN[2, height + 2 * Interpol + i, width + 2 * Interpol + j] = 255;
                }
            }

            for (int i = 0; i < height; i++)
            {
                for (int j = -Interpol; j < 0; j++)
                {
                    rgbN[0, Interpol + i, Interpol + j] = rgb[0, i, 0];
                    rgbN[1, Interpol + i, Interpol + j] = rgb[1, i, 0];
                    rgbN[2, Interpol + i, Interpol + j] = rgb[2, i, 0];

                    rgbN[0, Interpol + i, width + 2 * Interpol + j] = rgb[0, i, width - 1];
                    rgbN[1, Interpol + i, width + 2 * Interpol + j] = rgb[1, i, width - 1];
                    rgbN[2, Interpol + i, width + 2 * Interpol + j] = rgb[2, i, width - 1];
                }
            }

            for (int i = -Interpol; i < 0; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    rgbN[0, Interpol + i, Interpol + j] = rgb[0, 0, j];
                    rgbN[1, Interpol + i, Interpol + j] = rgb[1, 0, j];
                    rgbN[2, Interpol + i, Interpol + j] = rgb[2, 0, j];

                    rgbN[0, 2 * Interpol + height + i, Interpol + j] = rgb[0, height - 1, j];
                    rgbN[1, 2 * Interpol + height + i, Interpol + j] = rgb[1, height - 1, j];
                    rgbN[2, 2 * Interpol + height + i, Interpol + j] = rgb[2, height - 1, j];
                }
            }

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    rgbN[0, Interpol + i, Interpol + j] = rgb[0, i, j];
                    rgbN[1, Interpol + i, Interpol + j] = rgb[1, i, j];
                    rgbN[2, Interpol + i, Interpol + j] = rgb[2, i, j];
                }
            }

            return rgbN;
        }

        private void testToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            int width = Im[comboBox1.SelectedIndex].Width,
                height = Im[comboBox1.SelectedIndex].Height;
            //byte[,,] rgb = InterpolIm_null(RGB.BitmapToByteRgbNaive(Im[comboBox1.SelectedIndex]), width,height,1);
            byte[,,] rgb = RGB.BitmapToByteRgbNaive(Im[comboBox1.SelectedIndex]);

            double phi = Convert.ToDouble(toolStripTextBox6.Text);

            Bitmap rgbN = korrectTest(rotIm(RGB.RgbToBitmapNaive(rgb), (float)phi));

            double alpha = CalcOrientashion(RGB.BitmapToByteRgbNaive(rgbN), width, height);

            Test = rgbN;

            Im.Add(rgbN);

            comboBox1.Items.Add(comboBox1.Text + "+Povorot");
            comboBox1.Text = comboBox1.Text + "+Povorot";
        }

        double PowerToEPR(double K, double R)
        {
            return Math.Pow(R, 4) / K;
        }

        private void korrToolStripMenuItem_Click(object sender, EventArgs e)
        {
            byte[,,] test = RGB.BitmapToByteRgbNaive(Test);
            byte[,,] etal = RGB.BitmapToByteRgbNaive(Etalon);

            double ScalarEtal = 0;
            double sumEtal = 0;
            double EPR_Etal = 0;
            double Power_Etal = 0;

            double ScalarTest = 0;
            double sumTest = 0;
            double EPR_Test = 0;
            double Power_Test = 0;

            double KorrMax = 0;
            double Korr = 0;
            int IndexMaxKorr = 0;

            int width = 0,
                height = 0;

            // Задать рамеры 
            // лучше сначала делать выбор обьектов из базы данных по эпру, находить максимальные рамеры и сравнивать с тестом и приводить к
            //одному размеру
            if (Test.Height == Etalon.Height && Test.Width == Etalon.Width)
            {
                width = Test.Width;
                height = Test.Height;
            }
            else
            {
                width = Math.Max(Test.Width, Etalon.Width);
                height = Math.Max(Test.Height, Etalon.Height);
            }

            //Считывание ЕПР или Мощьности для эталона и теста
            Zadat_EPR EPR = new Zadat_EPR();
            EPR.ShowDialog();
            if (EPR.DialogResult == DialogResult.OK)
            {
                EPR_Etal = EPR.EPR_Etalon;
                EPR_Test = EPR.EPR_Test;
            }

            // Sum RGB 
            for (int y = 0; y < Etalon.Height; y++)
            {
                for (int x = 0; x < Etalon.Width; x++)
                {
                    sumEtal += 255 - etal[0, y, x];
                }
            }
            for (int y = 0; y < Etalon.Height; y++)
            {
                for (int x = 0; x < Etalon.Width; x++)
                {
                    //ScalarEtal += Math.Pow(255 - etal[0, y, x], 2) + Math.Pow(255 - etal[1, y, x], 2) + Math.Pow(255 - etal[2, y, x], 2);
                    ScalarEtal += Math.Pow((255 - etal[0, y, x]) * EPR_Etal / sumEtal, 2);
                }
            }

            // Scalar RBG to EPR
            // Если задан EPR
            if (EPR_Etal!=0 && EPR_Test!=0)
            {
                for (int y = 0; y < Test.Height; y++)
                {
                    for (int x = 0; x < Test.Width; x++)
                    {
                        sumTest += 255 - test[0, y, x];
                    }
                }
                for (int y = 0; y < Test.Height; y++)
                {
                    for (int x = 0; x < Test.Width; x++)
                    {
                        //ScalarTest += Math.Pow(255 - test[0, y, x], 2) + Math.Pow(255 - test[1, y, x], 2) + Math.Pow(255 - test[2, y, x], 2);
                        ScalarTest += Math.Pow((255 - test[0, y, x]) * EPR_Test / sumTest, 2);
                    }
                }
            }


            //!!!! Проверка по ЕПР, но без учета размера
            if (ScalarEtal < ScalarTest)
                MessageBox.Show("Порівняння не доцільне! Скаляр Еталона меньше чем у теста.");

            // Изменить размеры изображений чтобы были один к одному
            if (Test.Height == height && Test.Width == width)
                test = RGB.BitmapToByteRgbNaive(Test);
            else
            {
                Test = privedenie_k_odnomu_razmeru(Test, width, height);
                test = RGB.BitmapToByteRgbNaive(Test);
            }
            if (Etalon.Height == height && Etalon.Width == width)
                etal = RGB.BitmapToByteRgbNaive(Etalon);
            else
            {
                Etalon = privedenie_k_odnomu_razmeru(Etalon, width, height);
                etal = RGB.BitmapToByteRgbNaive(Etalon);
            }

            //Создание матрицы поворота и выбор максимальной корреляции
            for (int i = 0; i <= 360; i++)
            {
                etal = RGB.BitmapToByteRgbNaive(rotIm(Etalon, (float)i));
                Korr = 0;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        //Korr +=(255 - etal[0, y, x])* (255 - test[0, y, x]) + (255 - etal[1, y, x]) * (255 - test[1, y, x]) + (255 - etal[2, y, x]) * (255 - test[2, y, x]);
                        Korr += ((255 - etal[0, y, x]) * EPR_Etal / sumEtal) * ((255 - test[0, y, x]) * EPR_Test / sumTest);
                    }
                }

                if (Korr > KorrMax)
                {
                    IndexMaxKorr = i;
                    KorrMax = Korr;
                }
            }
            MessageBox.Show("Angle = " + IndexMaxKorr.ToString() + " korr = " + (KorrMax / ScalarEtal).ToString());
        }

    }
}
