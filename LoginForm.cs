using Newtonsoft.Json;
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

namespace Vehicle_Verification_System
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            String username = textBox1.Text;
            String password = textBox2.Text;

            try
            {
                using (var client = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate }))
                {
                    try
                    {
                        client.BaseAddress = new Uri(network.response.API.ip);

                        var content = new FormUrlEncodedContent(new[]
                        {
                            new KeyValuePair<string, string>("username", username),
                            new KeyValuePair<string, string>("password", password),
                            
                        });

                        HttpResponseMessage response = client.PostAsync("token", content).Result;


                        response.EnsureSuccessStatusCode();
                        string result = response.Content.ReadAsStringAsync().Result;

                        TokenModel resp = JsonConvert.DeserializeObject<TokenModel>(result);

                        if (resp.access.Length < 10)
                        {
                            MessageBox.Show("Invalid User");
                        }
                        else
                        {
                            string access = resp.access;

                            API.token = access;

                            authorizeUser();
                        }
                       
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Result: " + ex.Message);
                        MessageBox.Show("Error : " + ex.Message);
                    }
                }
                Console.ReadLine();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void authorizeUser()
        {
            try
            {
                using (var client = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate }))
                {
                    try
                    {
                        client.BaseAddress = new Uri(network.response.API.ip);
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", API.token);

                      
                        HttpResponseMessage response = client.GetAsync("auth/role").Result;


                        response.EnsureSuccessStatusCode();
                        string result = response.Content.ReadAsStringAsync().Result;

                        AuthResponse resp = JsonConvert.DeserializeObject<AuthResponse>(result);

                        Form1.user = resp;
                        Form1 frm = new Form1();

                        textBox1.Text = "";
                        textBox2.Text = "";
                        this.Hide();
                        frm.ShowDialog();
                        this.Show();

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Result: " + ex.Message);
                        MessageBox.Show("Error : " + ex.Message);
                    }
                }
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
