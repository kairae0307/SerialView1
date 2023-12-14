using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.IO.Ports;
using System.Net;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Data.SqlClient;
using System.Configuration;


namespace WindowsApplication2
{
    public partial class Form1 : Form
    {
        public int temp =25;
        public int lux = 200;
        public int humidity = 64;
        public int co2 = 0xe8;
        private int richTextCnt = 0;
        public int DataCnt =0;
        public int lineEntercnt = 0;
        public int rcvCnt = 0;
        public int linecnt = 0;
        public bool Data_Start = false;
        private List<byte> FileData = new List<byte>();
        private List<byte> Image1 = new List<byte>();
        private List<byte> Image2 = new List<byte>();
        private List<byte> SwapData = new List<byte>();

        delegate void textCallbak(String txt);
        delegate void progressCallBack(int nProgressBar);
        private List<byte> rcvList = new List<byte>();
        private List<byte> hisList = new List<byte>();
        public int CheckSum = 0;
        public int length = 0;
        public bool lenCheck = false;
        public Form1()
        {
            InitializeComponent();
        }

        private void buttonOpen_Click(object sender, EventArgs e)
        {
            try
            {
                if (serialPort1.IsOpen)
                    serialPort1.Close();
            }
            catch
            {
                MessageBox.Show("Can't close already opened port", "Error");
                return;
            }

            serialPort1.PortName = comboBoxPort.SelectedItem.ToString();
            serialPort1.BaudRate = Convert.ToInt32(comboBoxBaud.SelectedItem);

            try
            {
                serialPort1.Open();

                label13.Text = comboBoxPort.SelectedItem.ToString() + " Open";
            }
            catch
            {
                MessageBox.Show("Can't open port", "Error");
                return;
            }
            timer1.Start();
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            try
            {
                if (serialPort1.IsOpen)
                    serialPort1.Close();
                label13.Text = comboBoxPort.SelectedItem.ToString() + " Close";

            }
            catch
            {
                MessageBox.Show("Can't close already opened port", "Error");
                return;
            }
        }

        [STAThread]
        private void AppendText(String str)
        {
            if (this.richTextBoxMsg.InvokeRequired)
            {
                textCallbak d = new textCallbak(AppendText);
                this.Invoke(d, new object[] { str });
            }
            else
            {
                this.richTextBoxMsg.Text += str;


            }
        }
        [STAThread]
        private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            byte bInData;
            string txt;

           
            for (int g = 0; g < 100; g++)
            {
                if (serialPort1.IsOpen)
                {
                    if (serialPort1.BytesToRead > 0)
                    {

                        richTextCnt++;
                        lineEntercnt++;
                        bInData = (byte)serialPort1.ReadByte();
                     //   FileData.Add(bInData);

                        if (richTextCnt == 1024)
                        {
                            linecnt++;
                            richTextCnt = 0;
                            label4.Text = linecnt.ToString();
                        }
                        if (bInData == 0x55)
                        {
                            txt = String.Format("\n{0:X2} ", bInData);
                        }
                        else
                        {

                            txt = String.Format("{0:X2} ", bInData);
                        }
                        richTextBoxMsg.AppendText(txt);
                      //  CheckSum += bInData;
                      //  txt = String.Format("{0:X2} ", bInData);

                         
                    //    richTextBoxMsg.AppendText(txt);
                      //  label4.Text = richTextCnt.ToString();

                     /*   if (richTextCnt == 20)
                        {
                            richTextCnt = 0;
                            richTextBoxMsg.AppendText("\r\n");

                        }H*/

                      //  richTextBoxMsg.AppendText(richTextCnt.ToString());



                    }
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;
            string[] ports = System.IO.Ports.SerialPort.GetPortNames();
        //    serialPort1.DataReceived += new SerialDataReceivedEventHandler(SP_DataReceived);

            foreach (string port in ports)//PC 에 있는 시리얼 포트 찾아서 저장
            {
                comboBoxPort.Items.Add(port);
            }
            richTextCnt = 0;


            comboBoxBaud.Items.Add("9600");
            comboBoxBaud.Items.Add("19200");
            comboBoxBaud.Items.Add("38400");
            comboBoxBaud.Items.Add("57600");
            comboBoxBaud.Items.Add("115200");

            comboBoxBaud.SelectedIndex = comboBoxBaud.Items.IndexOf("38400");
            comboBoxPort.SelectedIndex = 0;

            serialPort1.BaudRate = 38400;
            serialPort1.Encoding = Encoding.Default;
            serialPort1.DataBits = 8;
            serialPort1.StopBits = StopBits.One;

            richTextCnt = 0;
        }
        /*
        [STAThread]
        private void SP_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            byte bInData;
            string txt;


            for (int g = 0; g < 100; g++)
            {
                if (serialPort1.IsOpen)
                {
                    if (serialPort1.BytesToRead > 0)
                    {
                        bInData = (byte)serialPort1.ReadByte();
                        rcvList.Add(bInData);
                        richTextCnt++;
                        txt = String.Format("{0:X2} ", bInData);

                   
                        richTextBoxMsg.AppendText(txt);



                    }
                }
            }
        }
        */
        private void button6_Click(object sender, EventArgs e)
        {
            lineEntercnt = 0;
            richTextCnt = 0;
            linecnt = 0;
            length = 0;
            richTextBoxMsg.Clear();
            rcvList.Clear();
            serialPort1.DiscardInBuffer();
            serialPort1.DiscardOutBuffer();
            CheckSum = 0;

            label4.Text = "";
            label3.Text = "";

            /////////////////////////    포트 닫았다 다시 열자 

       /*     try
            {
                if (serialPort1.IsOpen)
                    serialPort1.Close();
                label13.Text = comboBoxPort.SelectedItem.ToString() + " Close";

            }
            catch
            {
                MessageBox.Show("Can't close already opened port", "Error");
                return;
            }
        * */
        }

        private void button1_Click(object sender, EventArgs e)
        {


            richTextCnt = 0;
            linecnt = 0;
            richTextBoxMsg.Clear();
            rcvList.Clear();
            serialPort1.DiscardInBuffer();
            serialPort1.DiscardOutBuffer();
            byte[] Data = new byte[6];

            Data[0] = 0x55;
            Data[1] = 0x15;
            Data[2] = 0x00;
            Data[3] = 0xcc;
            Data[4] = 0xfd;
            Data[5] = 0x00;


            if (serialPort1.IsOpen)
            {
                serialPort1.DiscardInBuffer();
                serialPort1.DiscardOutBuffer();
                serialPort1.Write(Data, 0, Data.Length);


            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            byte[] Data = new byte[1];

            Data[0] = rcvList[rcvList.Count - 1];

            richTextCnt = 0;
            linecnt = 0;
            richTextBoxMsg.Clear();
            rcvList.Clear();
          

            if (serialPort1.IsOpen)
            {
                serialPort1.DiscardInBuffer();
                serialPort1.DiscardOutBuffer();
                serialPort1.Write(Data, 0, Data.Length);


            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            byte[] Data = new byte[10];
            byte CheckSum = 0;

            if (temp < 40)
            {
                temp++;
            }
            else
            {
                temp = 25;
            }

            if (lux < 210)
            {
                lux++;
            }
            else
            {
                lux = 200;
            }

            if (humidity < 70)
            {
                humidity++;
            }
            else
            {
                humidity = 64;
            }

            if (co2 < 0xff)
            {
                co2++;
            }
            else
            {
                co2 = 0xe8;
            }

            Data[0] = 0x55;
            Data[1] = 0xA0;
            Data[2] = (byte)temp;
            Data[3] = 0x00;
            Data[4] = (byte)lux;
            Data[5] = (byte)humidity;
            Data[6] = 0x03;
            Data[7] = (byte)co2;
            Data[8] = 0x0c;

            for (int i = 0; i < 9; i++)
            {
                CheckSum += Data[i];
            }

            Data[9] = CheckSum;
        
           


            if (serialPort1.IsOpen)
            {
                serialPort1.DiscardInBuffer();
                serialPort1.DiscardOutBuffer();
                serialPort1.Write(Data, 0, Data.Length);


            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            byte[] Data = new byte[3];

            Data[0] = 0xAA;
            Data[1] = 0x09;
            CheckSum = 0;
            Data[2] = 0xAA + 0x09;



            if (serialPort1.IsOpen)
            {
                serialPort1.DiscardInBuffer();
                serialPort1.DiscardOutBuffer();
                serialPort1.Write(Data, 0, Data.Length);


            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            byte[] Data = new byte[3];

            Data[0] = 0xAA;
            Data[1] = 0x09;
            CheckSum = 0;
            Data[2] = 0xAA + 0x09;



            if (serialPort1.IsOpen)
            {
                serialPort1.DiscardInBuffer();
                serialPort1.DiscardOutBuffer();
                serialPort1.Write(Data, 0, Data.Length);


            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            byte[] Data = new byte[3];

            Data[0] = 0xAA;
            Data[1] = 0x01;
            CheckSum = 0;
            Data[2] = 0xAA + 0x01;



            if (serialPort1.IsOpen)
            {
                serialPort1.DiscardInBuffer();
                serialPort1.DiscardOutBuffer();
                serialPort1.Write(Data, 0, Data.Length);


            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            byte[] Data = new byte[3];

            Data[0] = 0xAA;
            Data[1] = 0x02;
            CheckSum = 0;
            Data[2] = 0xAA + 0x02;



            if (serialPort1.IsOpen)
            {
                serialPort1.DiscardInBuffer();
                serialPort1.DiscardOutBuffer();
                serialPort1.Write(Data, 0, Data.Length);


            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            byte[] Data = new byte[3];

            Data[0] = 0xAA;
            Data[1] = 0x04;
            CheckSum = 0;
            Data[2] = 0xAA + 0x04;



            if (serialPort1.IsOpen)
            {
                serialPort1.DiscardInBuffer();
                serialPort1.DiscardOutBuffer();
                serialPort1.Write(Data, 0, Data.Length);


            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            byte[] Data = new byte[3];

            Data[0] = 0xAA;
            Data[1] = 0x0A;
            CheckSum = 0;
            Data[2] = 0xAA + 0x0A;



            if (serialPort1.IsOpen)
            {
                serialPort1.DiscardInBuffer();
                serialPort1.DiscardOutBuffer();
                serialPort1.Write(Data, 0, Data.Length);


            }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            byte[] Data = new byte[2];

            Data[0] = 0xBA;
            Data[1] = 0xA0;
            richTextCnt = 0;
            linecnt = 0;
            FileData.Clear();
            if (serialPort1.IsOpen)
            {
                serialPort1.DiscardInBuffer();
                serialPort1.DiscardOutBuffer();
                serialPort1.Write(Data, 0, Data.Length);


            }
        }

        private void button13_Click(object sender, EventArgs e)
        {
            byte[] Data = new byte[2];

            Data[0] = 0xBA;
            Data[1] = 0xA1;

            FileData.Clear();
            if (serialPort1.IsOpen)
            {
                serialPort1.DiscardInBuffer();
                serialPort1.DiscardOutBuffer();
                serialPort1.Write(Data, 0, Data.Length);


            }
        }

        private void button14_Click(object sender, EventArgs e)
        {
            byte[] Data = new byte[2];

            Data[0] = 0xBA;
            Data[1] = 0xA2;
            FileData.Clear();

            if (serialPort1.IsOpen)
            {
                serialPort1.DiscardInBuffer();
                serialPort1.DiscardOutBuffer();
                serialPort1.Write(Data, 0, Data.Length);


            }
        }

        private void button15_Click(object sender, EventArgs e)
        {
            byte[] r5Data = new byte[FileData.Count];
            FileData.CopyTo(r5Data);

            string FileTariff = "";

         //   FileTariff = filename.Replace(".txt", "");
         //   FileTariff = filename.Replace(".TXT", "");

            FileTariff = "TEST.bin";
            //   FileTariff += ".bin";


            FileStream fsTMP = new FileStream(FileTariff, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            BinaryWriter bwTMP = new BinaryWriter(fsTMP);
            bwTMP.Write(r5Data);
            MessageBox.Show(FileTariff.ToString() + "  Maked file !");
            fsTMP.Close();
            bwTMP.Close();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            byte[] Data = new byte[10];
            byte CheckSum = 0;

            if (temp < 40)
            {
                temp++;
            }
            else
            {
                temp = 25;
            }

            if (lux < 210)
            {
                lux++;
            }
            else
            {
                lux = 200;
            }

            if (humidity < 70)
            {
                humidity++;
            }
            else
            {
                humidity = 64;
            }

            if (co2 < 0xff)
            {
                co2++;
            }
            else
            {
                co2 = 0xe8;
            }

            Data[0] = 0x55;
            Data[1] = 0xA0;
            Data[2] = (byte)temp;
            Data[3] = 0x00;
            Data[4] = (byte)lux;
            Data[5] = (byte)humidity;
            Data[6] = 0x03;
            Data[7] = (byte)co2;
            Data[8] = 0x0c;

            for (int i = 0; i < 9; i++)
            {
                CheckSum += Data[i];
            }

            Data[9] = CheckSum;




            if (serialPort1.IsOpen)
            {
                serialPort1.DiscardInBuffer();
                serialPort1.DiscardOutBuffer();
                serialPort1.Write(Data, 0, Data.Length);


            }
        }

    }
}