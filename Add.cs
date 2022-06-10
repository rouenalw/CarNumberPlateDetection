using System;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Net.Http;
using System.Net;
using Newtonsoft.Json;
using System.Collections.Generic;
using Vehicle_Verification_System.models;
using System.Net.Http.Headers;
using Vehicle_Verification_System.network.response;
using System.Drawing;

namespace Vehicle_Verification_System
{
    public partial class Add : Form
    {
        int status = 0;

        public string ID { get; internal set; }
        public string image_path { get; set; }

        public Add()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
        }

        private void Pic_DoubleClick(object sender, EventArgs e)
        {
            OpenFileDialog OFD = new OpenFileDialog();
            OFD.Multiselect = false;
            OFD.ShowDialog();
            OFD.Filter = "JPEG | *.jpg";
            Pic.ImageLocation = OFD.FileName;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            saveUpdate("add");

        }

        private void saveUpdate(string action)
        {
            try
            {
                if (CBStolen.CheckState == CheckState.Checked) status = 1;
                else status = 0;
                //string ConStr = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\Data\VVS.mdf;Integrated Security=True";
                //SqlConnection Con = new SqlConnection(ConStr);
                //Con.Open();
                //SqlCommand Com = new SqlCommand("Insert Into Data (Vehicle_Num, Full_Name, License_No, Details, Pic, Address, Token, DateOfBirth, Chasis_Num, Stolen) Values ('" + TBVNum.Text + "', '" + TBFullName.Text + "',
                //'" + TBLNum.Text + "', '" + TBDetails.Text + "', '" + Pic.ImageLocation.ToString() + "', '" + TBAddress.Text + "', '" + pickerToken.Value.Date + "', '" + pickerDOB.Value.Date + "', '" + TBChasis.Text + "', '" + status + "')", Con);
                //Com.ExecuteNonQuery();



                string number = TBVNum.Text;
                string name = TBFullName.Text;
                string detail = TBDetails.Text;
                string license = TBLNum.Text;
                string pic = "";//Pic.ImageLocation.ToString();
                string address = TBAddress.Text;
                string token = pickerToken.Text;
                string dob = pickerDOB.Text;
                string chasis = TBChasis.Text;


                using (var client = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate }))
                {
                    try
                    {
                        client.BaseAddress = new Uri(API.ip);
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", API.token);



                        var content = new FormUrlEncodedContent(new[]
                        {
                            new KeyValuePair<string, string>("number", number),
                            new KeyValuePair<string, string>("name", name),
                            new KeyValuePair<string, string>("detail", detail),
                            new KeyValuePair<string, string>("pic", pic),
                            new KeyValuePair<string, string>("address", address),
                            new KeyValuePair<string, string>("token", token),
                            new KeyValuePair<string, string>("dob", dob),
                            new KeyValuePair<string, string>("license", license),
                            new KeyValuePair<string, string>("chasis", chasis),
                            new KeyValuePair<string, string>("status", status+""),
                            new KeyValuePair<string, string>("action", action),
                            new KeyValuePair<string, string>("image_path", image_path),
                            new KeyValuePair<string, string>("id", ID)
                        });

                        HttpResponseMessage response = client.PostAsync("v1/details/add", content).Result;


                        response.EnsureSuccessStatusCode();
                        string result = response.Content.ReadAsStringAsync().Result;

                        ServerResponse resp = JsonConvert.DeserializeObject<ServerResponse>(result);

                        //MessageBox.Show(resp.message);

                        if (action == "add")
                        {
                            MessageBox.Show("Added Successfully", "Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                           
                            MessageBox.Show("Updated Successfully", "Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            this.Hide();
                            Record r = new Record();
                            r.LoadData();
                            r.ShowDialog();
                            

                           
                        }
                        Dispose();

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Result: " + ex.Message);
                        MessageBox.Show("Error : " + ex.Message);
                    }
                }
                Console.ReadLine();



                
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                /*if (CBStolen.CheckState == CheckState.Checked) status = 1;
                else status = 0;
                //string ConStr = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\Data\VVS.mdf;Integrated Security=True";
                //SqlConnection Con = new SqlConnection(ConStr);
                //Con.Open();
                //SqlCommand Com = new SqlCommand("UPDATE Data SET Full_Name = '" + TBFullName.Text + "', License_No = '" + TBLNum.Text + "', Details = '" + TBDetails.Text + "', Pic = '" + Pic.ImageLocation.ToString() + "', Address = '" + TBAddress.Text + "', Token = '" + pickerToken.Value.Date + "', DateOfBirth = '" + pickerDOB.Value.Date + "', Chasis_Num = '" + TBChasis.Text + "', Stolen = '" + status + "' WHERE Vehicle_Num = '" + TBVNum.Text + "'", Con);
                //Com.ExecuteNonQuery();

                MessageBox.Show("Updated Successfully", "Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Dispose();*/

                saveUpdate("update");
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Add_Load(object sender, EventArgs e)
        {
            pickerDOB.Format = DateTimePickerFormat.Custom;
            pickerToken.Format = DateTimePickerFormat.Custom;

            pickerDOB.CustomFormat = "yyyy-MM-dd";
            pickerToken.CustomFormat = "yyyy-MM-dd";

            try
            {
                Pic.Image = Image.FromFile(@""+image_path);
            }
            catch (Exception ignore) { }
        }

        private void Pic_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileOpen = new OpenFileDialog();
            fileOpen.Title = "Open Image file";
            //fileOpen.Filter = "JPG Files (*.jpg)| *.jpg";

            if (fileOpen.ShowDialog() == DialogResult.OK)
            {
               // MessageBox.Show(fileOpen.FileName);
                image_path = fileOpen.FileName;
                Pic.Image = Image.FromFile(fileOpen.FileName);
            }
            fileOpen.Dispose();
        }
    }
}
