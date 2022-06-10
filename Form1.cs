using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.Data.SqlClient;
using System.IO.Ports;
using System.IO;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using Vehicle_Verification_System.response;
using Vehicle_Verification_System.models;
using Vehicle_Verification_System.network.response;
using System.Net.Http.Headers;
using System.Drawing;

namespace Vehicle_Verification_System
{
    public partial class Form1 : Form
    {
        // Serial Port
        SerialPort SP;
        internal static AuthResponse user;

        bool Allowed = false;

        public Form1()
        {
            InitializeComponent();
            string[] ports = SerialPort.GetPortNames();
            foreach (string port in ports)
            {
                CBPorts.Items.Add(port);
            }

            label2.Text = "Welcome " + user.fullname;
        }

        private void btnScan_Click(object sender, EventArgs e)
        {

            if (API.token.Length < 10)
            {
                LoginForm login = new LoginForm();
                login.Show();
                return;
            }

            try
            {
                TBData.Text = "";

                ProcessStartInfo psi = new ProcessStartInfo(@"python.exe");
                psi.Arguments = "script.py";
                psi.RedirectStandardOutput = true;
                psi.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                psi.UseShellExecute = false;
                Process proc = Process.Start(psi);
                System.IO.StreamReader myOutput = proc.StandardOutput;
                proc.WaitForExit();
                try
                {
                    if (proc.HasExited)
                    {
                        TBData.Text = myOutput.ReadToEnd();
                        File.AppendAllText("Log.txt", Environment.NewLine + Environment.NewLine + "Time: " + DateTime.Now.ToString() + '\t' + "Vehicle No: " + TBData.Text);


                        if (TBData.Text.Trim() != "")
                        {
                            try
                            {
                                //string ConStr = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\Data\VVS.mdf";
                                //SqlConnection Con = new SqlConnection(ConStr);
                                //Con.Open();
                                //SqlCommand Com = new SqlCommand("Select * From Data Where Vehicle_Num Like '%" + TBData.Text.Trim() + "%'", Con);
                                //SqlDataReader dr;
                                //dr = Com.ExecuteReader();
                                //dr.Read();




                                using (var client = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate }))
                                {
                                    try
                                    {
                                        String plate = TBData.Text.Trim();


                                        client.BaseAddress = new Uri(API.ip);
                                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", API.token);

                                        HttpResponseMessage response = client.GetAsync("v1/details/view/" + plate).Result;


                                        response.EnsureSuccessStatusCode();
                                        string result = response.Content.ReadAsStringAsync().Result;

                                        VehicleDetail vehicle = JsonConvert.DeserializeObject<VehicleDetail>(result);



                                        Add add = new Add();
                                        add.Title.Text = "WELCOME";
                                        add.TBVNum.Text = vehicle.vehicle_no;
                                        add.TBFullName.Text = vehicle.fullname;
                                        add.TBLNum.Text = vehicle.licence_no;
                                        add.TBDetails.Text = vehicle.details;
                                        //add.Pic.ImageLocation = vehicle.vehicle_no;
                                        add.TBAddress.Text = vehicle.address;
                                        add.pickerToken.Value = Convert.ToDateTime(vehicle.token);
                                        add.pickerDOB.Value = Convert.ToDateTime(vehicle.dob);
                                        add.TBChasis.Text = vehicle.chasis_no;
                                        add.ID = vehicle.id+"";
                                        add.image_path = vehicle.image_path;

                                        try
                                        {
                                            add.Pic.Image = Image.FromFile(@"" + vehicle.image_path);
                                        }
                                        catch (Exception ignore) {
                                            MessageBox.Show( vehicle.image_path);
                                        }

                                        Allowed = true;

                                        if (Convert.ToInt32(vehicle.status) == 1)
                                        {
                                            add.CBStolen.CheckState = CheckState.Checked;

                                            // Sending B1 To Arduino To Buzzer Because Vehicle Is Stolen
                                            TBData.Text += "Vehicle Was Marked As Stolen";
                                            File.AppendAllText("Log.txt", Environment.NewLine + "Vehicle Was Marked As Stolen" + Environment.NewLine + "Ringing Alert");
                                            SP.WriteLine("B1");
                                            Allowed = false;
                                        }


                                        if (add.pickerToken.Value < DateTime.Now)
                                        {
                                            // Sending B1 To Arduino To Buzzer Because Vehicle Token Expired
                                            TBData.Text += "Vehicle Token Expired!" + Environment.NewLine;
                                            File.AppendAllText("Log.txt", Environment.NewLine + "Vehicle Token Expired" + Environment.NewLine + "Ringing Alert");
                                            SP.WriteLine("B1");

                                            Allowed = false;
                                        }

                                        if (Allowed)
                                        {
                                            add.CBStolen.CheckState = CheckState.Unchecked;

                                            // Sending G1 To Arduino To Open Gate
                                            TBData.Text += "Opening Gate";
                                            File.AppendAllText("Log.txt", Environment.NewLine + "Vehicle verification successful!" + Environment.NewLine + "Opening Gate");
                                            SP.WriteLine("G1");
                                        }


                                        add.btnAdd.Visible = false;
                                        add.btnUpdate.Visible = true;

                                        add.ShowDialog();
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine("Result: " + ex.Message);
                                        MessageBox.Show("Error : " + ex.Message);
                                    }
                                }
                                Console.ReadLine();


                                /*Add add = new Add();
                                add.Title.Text = "WELCOME";
                                add.TBVNum.Text = dr.GetValue(0).ToString();
                                add.TBFullName.Text = dr.GetValue(1).ToString();
                                add.TBLNum.Text = dr.GetValue(2).ToString();
                                add.TBDetails.Text = dr.GetValue(3).ToString();
                                add.Pic.ImageLocation = dr.GetValue(4).ToString();
                                add.TBAddress.Text = dr.GetValue(5).ToString();
                                add.pickerToken.Value = Convert.ToDateTime(dr.GetValue(6).ToString());
                                add.pickerDOB.Value = Convert.ToDateTime(dr.GetValue(7).ToString());
                                add.TBChasis.Text = dr.GetValue(8).ToString();

                                if (Convert.ToInt32(dr.GetValue(9)) == 1)
                                {
                                    add.CBStolen.CheckState = CheckState.Checked;

                                    // Sending B1 To Arduino To Buzzer Because Vehicle Is Stolen
                                    TBData.Text += "Vehicle Was Marked As Stolen";
                                    File.AppendAllText("Log.txt", Environment.NewLine + "Vehicle Was Marked As Stolen" + Environment.NewLine + "Ringing Alert");
                                    SP.WriteLine("B1");
                                }
                                else
                                {
                                    add.CBStolen.CheckState = CheckState.Unchecked;

                                    // Sending G1 To Arduino To Open Gate
                                    TBData.Text += "Opening Gate";
                                    File.AppendAllText("Log.txt", Environment.NewLine + "Vehicle verification successful!" + Environment.NewLine + "Opening Gate");
                                    SP.WriteLine("G1");
                                }


                                add.btnAdd.Visible = false;
                                add.btnUpdate.Visible = true;

                                add.ShowDialog();*/


                            }
                            catch
                            {
                                TBData.Text += Environment.NewLine + "Couldn't Find Record";
                                File.AppendAllText("Log.txt", Environment.NewLine + "Verification Unsuccessful");
                            }

                            File.AppendAllText("Log.txt", "------------------------------------------------------------------------------");
                        }
                    }
                }
                catch (Exception ess)
                {
                    MessageBox.Show(ess.Message);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (API.token.Length < 10)
            {
                LoginForm login = new LoginForm();
                login.Show();
                return;
            }
            Add add = new Add();
            add.ShowDialog();
        }

        private void CBPorts_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                SP = new SerialPort();
                SP.PortName = CBPorts.Text;
                SP.BaudRate = 9600;
                SP.DtrEnable = true;
                SP.Open();
                SP.DataReceived += SP_DataReceived;
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SP_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string line = SP.ReadLine();
            this.BeginInvoke(new LineReceivedEvent(LineReceived), line);
        }

        private delegate void LineReceivedEvent(string line);
        private void LineReceived(string line)
        {

        }

        private void btnReloadPorts_Click(object sender, EventArgs e)
        {
            CBPorts.Items.Clear();
            string[] ports = SerialPort.GetPortNames();
            foreach (string port in ports)
            {
                CBPorts.Items.Add(port);
            }
            CBPorts.SelectedIndex = 0;
        }


        private void btnRecord_Click(object sender, EventArgs e)
        {

            if (API.token.Length < 10)
            {
                LoginForm login = new LoginForm();
                login.Show();
                return;
            }

            //Record R = new Record();
            //R.ShowDialog();


            using (var client = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate }))
            {
                try
                {
                    client.BaseAddress = new Uri(API.ip);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", API.token);

                    HttpResponseMessage response = client.GetAsync("v1/details/view").Result;


                    response.EnsureSuccessStatusCode();
                    string result = response.Content.ReadAsStringAsync().Result;

                    VehicleList vehicle = JsonConvert.DeserializeObject<VehicleList>(result);

                    Record R = new Record();
                    R.setData(vehicle);
                    R.ShowDialog();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Result: " + ex.Message);
                    MessageBox.Show("Error : " + ex.Message);
                }
            }
            Console.ReadLine();
        }

        private void Time_Tick(object sender, EventArgs e)
        {
            if (labNames.Left < -270)
            {
                labNames.Left = this.Width;
            }

            labNames.Left = labNames.Left - 2;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (!user.is_admin)
            {
                btnRecord.Enabled = false;
                btnRecord.Visible = false;
                btnUsers.Enabled = false;
                btnUsers.Visible = false;
                btnAdd.Enabled = false;
                btnAdd.Visible = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Users fm = new Users();
            fm.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
