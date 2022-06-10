using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using Vehicle_Verification_System.response;
using Vehicle_Verification_System.models;
using System.Net.Http;
using Vehicle_Verification_System.network.response;
using System.Net;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace Vehicle_Verification_System
{
    public partial class Record : Form
    {

        private VehicleList vehicle_list;
        public Record()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
            this.vehicle_list = new VehicleList();
        }


        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (DGV.SelectedRows.Count == 1)
            {
                
                try
                {
                    /*string ConStr = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\Data\VVS.mdf";
                    SqlConnection Con = new SqlConnection(ConStr);
                    Con.Open();
                    SqlCommand Com = new SqlCommand("Select * From Data Where Vehicle_Num = '" + ID + "'", Con);
                    SqlDataReader dr;
                    dr = Com.ExecuteReader();
                    dr.Read();*/

                    string ID = DGV.SelectedCells[0].Value.ToString();
                    string NAME = DGV.SelectedCells[1].Value.ToString();
                    string NUMBER = DGV.SelectedCells[2].Value.ToString();
                    string LICENCE = DGV.SelectedCells[3].Value.ToString();
                    string DETAIL = DGV.SelectedCells[4].Value.ToString();
                    string CHASIS = DGV.SelectedCells[5].Value.ToString();
                    string DOB = DGV.SelectedCells[6].Value.ToString();
                    string TOKEN = DGV.SelectedCells[7].Value.ToString();
                    string ADDRES = DGV.SelectedCells[8].Value.ToString();
                    string IMAGE = DGV.SelectedCells[9].Value.ToString();
                    string STATUS = DGV.SelectedCells[10].Value.ToString();

                   

                    Add add = new Add();
                    add.Title.Text = "WELCOME";
                    add.TBVNum.Text = NUMBER;
                    add.TBFullName.Text = NAME;
                    add.TBLNum.Text = LICENCE;
                    add.TBDetails.Text =DETAIL;
                    //add.Pic.ImageLocation = dr.GetValue(4).ToString();
                    add.image_path = DGV.SelectedCells[11].Value.ToString();
                    add.TBAddress.Text = ADDRES;
                    add.pickerToken.Value = Convert.ToDateTime(TOKEN);
                    add.pickerDOB.Value = Convert.ToDateTime(DOB);
                    add.TBChasis.Text = CHASIS;
                    add.ID = ID;

                    if (Convert.ToInt32(STATUS) == 1)
                    {
                        add.CBStolen.CheckState = CheckState.Checked;
                    }
                    else
                    {
                        add.CBStolen.CheckState = CheckState.Unchecked;
                    }


                    add.btnAdd.Visible = false;
                    add.btnUpdate.Visible = true;


                    this.Hide();

                    add.ShowDialog();
                }
                catch
                {
                    // ignore
                }
            }
        }

        private void Record_Load(object sender, EventArgs e)
        {
            try
            {
                //string ConStr = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\Data\VVS.mdf";
                //SqlConnection Con = new SqlConnection(ConStr);
                //Con.Open();
                //SqlDataAdapter da = new SqlDataAdapter("Select * From Data", Con);
                //DataTable DT = new DataTable();
                //da.Fill(DT);




                pullData();

            }
            catch
            {
                // ignore
            }
        }

        private void pullData()
        {
            var source = new BindingSource();
            List<VehicleDetail> list = vehicle_list.results;
            source.DataSource = list;
            DGV.DataSource = source;
        }

        class MyStruct
        {
            public string Name { get; set; }
            public string Adres { get; set; }


            public MyStruct(string name, string adress)
            {
                Name = name;
                Adres = adress;
            }
        }
        internal void setData(VehicleList vehicle)
        {
            this.vehicle_list = vehicle;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var resul = MessageBox.Show("Do you want to delete selected record?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (resul == DialogResult.No)
            {
                return;
            }


            try
            {
                string ID = DGV.SelectedCells[0].Value.ToString();

                /*string ConStr = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\Data\VVS.mdf";
                SqlConnection Con = new SqlConnection(ConStr);
                Con.Open();
                SqlCommand Com = new SqlCommand("Delete From Data Where Vehicle_Num = '" + ID + "'", Con);
                Com.ExecuteNonQuery();*/


                using (var client = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate }))
                {
                    try
                    {
                        client.BaseAddress = new Uri(API.ip);
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", API.token);



                        var content = new FormUrlEncodedContent(new[]
                        {
                            new KeyValuePair<string, string>("vid", ID)
                        });

                        HttpResponseMessage response = client.PostAsync("v1/details/delete/vehicle", content).Result;


                        response.EnsureSuccessStatusCode();
                        string result = response.Content.ReadAsStringAsync().Result;

                        ServerResponse resp = JsonConvert.DeserializeObject<ServerResponse>(result);

                        //MessageBox.Show(resp.message);

                        MessageBox.Show("Deleted Successfully", "Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadData();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Result: " + ex.Message);
                        MessageBox.Show("Error : " + ex.Message);
                    }
                }
                Console.ReadLine();

                

                
            }
            catch
            {
                MessageBox.Show("Deletion Failed", "Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void LoadData()
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

                    
                    setData(vehicle);
                    pullData();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Result: " + ex.Message);
                    MessageBox.Show("Error : " + ex.Message);
                }
            }
            Console.ReadLine();
        }
    }
}
