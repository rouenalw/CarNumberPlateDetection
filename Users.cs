using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Vehicle_Verification_System.models;
using Vehicle_Verification_System.network.response;
using Vehicle_Verification_System.response;

namespace Vehicle_Verification_System
{
    public partial class Users : Form
    {
        public Users()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
        }

        private void Users_Load(object sender, EventArgs e)
        {
            comboBox1.Items.Add("Admin");
            comboBox1.Items.Add("Operator");

            loadUsers();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            saveUpdateUser("add");
        }

        private void saveUpdateUser(string action)
        {
            if(action!="delete" && textBox2.Text == "")
            {
                MessageBox.Show("All Fields are required", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (textBox1.Text != "" &&  comboBox1.Text != "")
            {
                string username = textBox1.Text;
                string password = textBox2.Text;
                string role = comboBox1.Text;

                using (var client = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate }))
                {
                    try
                    {
                        client.BaseAddress = new Uri(API.ip);
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", API.token);



                        var content = new FormUrlEncodedContent(new[]
                        {
                            new KeyValuePair<string, string>("username", username),
                            new KeyValuePair<string, string>("password", password),
                            new KeyValuePair<string, string>("role", role),
                            new KeyValuePair<string, string>("action", action),
                            new KeyValuePair<string, string>("id", ID)
                        });

                        HttpResponseMessage response = client.PostAsync("auth/add", content).Result;


                        response.EnsureSuccessStatusCode();
                        string result = response.Content.ReadAsStringAsync().Result;

                        ServerResponse resp = Newtonsoft.Json.JsonConvert.DeserializeObject<ServerResponse>(result);

                        MessageBox.Show(resp.message);
                        textBox1.Text = "";
                        textBox2.Text = "";
                        comboBox1.Text = "";

                        loadUsers();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Result: " + ex.Message);
                        MessageBox.Show("Error : " + ex.Message);
                    }
                }
                Console.ReadLine();


            }
            else
            {
                MessageBox.Show("All Fields are required", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void loadUsers()
        {
            using (var client = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate }))
            {
                try
                {
                    client.BaseAddress = new Uri(API.ip);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", API.token);

                    HttpResponseMessage response = client.GetAsync("auth/list").Result;


                    response.EnsureSuccessStatusCode();
                    string result = response.Content.ReadAsStringAsync().Result;

                    UsersList users = Newtonsoft.Json.JsonConvert.DeserializeObject<UsersList>(result);


                    var source = new BindingSource();
                    List<User> list = users.results;
                    source.DataSource = list;
                    dataGridView1.DataSource = source;

                    
            
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Result: " + ex.Message);
                    MessageBox.Show("Error : " + ex.Message);
                }
            }
            Console.ReadLine();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            saveUpdateUser("update");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            saveUpdateUser("delete");
        }

        string ID = "";
        private void DataGridView1_CellClick(object sender, System.Windows.Forms.DataGridViewCellEventArgs e)
        {
            ID = dataGridView1.SelectedCells[0].Value.ToString();
            string NAME = dataGridView1.SelectedCells[1].Value.ToString();
            string PASW = dataGridView1.SelectedCells[2].Value.ToString();
            string ROLE = dataGridView1.SelectedCells[3].Value.ToString();

            textBox1.Text = NAME;
            comboBox1.Text = ROLE;

        }

       
    }
}
