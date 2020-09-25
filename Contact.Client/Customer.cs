using Contact.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Contact.Client
{
    public partial class CustomerController : Form
    {
        public HttpClient client;
        //The URL of the WEB API Service
        private readonly string BaseUrl;
        //The HttpClient Class, this will be used for performing 
        //HTTP Operations, GET, POST, PUT, DELETE
        //Set the base address and the Header Formatter


        public CustomerController()
        {
            InitializeComponent();
            BaseUrl = ConfigurationSettings.AppSettings["BaseUrl"].ToString();
            BaseUrl = BaseUrl + "Customers";
            client = new HttpClient();
            client.BaseAddress = new Uri(BaseUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
        public async Task<List<Customer>> LoadCustomersData()
        {
            HttpResponseMessage responseMessage = await client.GetAsync(BaseUrl);

            List<Customer> CustomersList = new List<Customer>();
            if (responseMessage.IsSuccessStatusCode)
            {
                var responseData = responseMessage.Content.ReadAsStringAsync().Result;

                var RequestResponce = JsonConvert.DeserializeObject<List<Customer>>(responseData);

                CustomersList = RequestResponce;
            }
            return CustomersList.ToList().OrderByDescending(x => x.Id).ToList();
        }

        private async void Customer_Load(object sender, EventArgs e)
        {
            var data = await LoadCustomersData();
            CustomersGrid.DataSource = data.ToList();
        }
        private void Clear()
        {
            txtEmailsList.Text = string.Empty;
            txtFirstName.Text = string.Empty;
            txtPhoneList.Text = string.Empty;
            txtLastName.Text = string.Empty;
            txtEmailsList.Text = string.Empty;
        }
        private void EditClear()
        {
            txtEditFirstName.Text = string.Empty;
            txtEditLastName.Text = string.Empty;
            txtEditPhoneList.Text = string.Empty;
            txtEditEmailsList.Text = string.Empty;
            txtCustomerId.Text = string.Empty;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            Clear();
        }

        private void btnEditClear_Click(object sender, EventArgs e)
        {
            EditClear();
        }

        private async void btnAddCustomer_Click(object sender, EventArgs e)
        {
            
        }

        private async void txtSearch_TextChanged(object sender, EventArgs e)
        {
            var searchString = txtSearch.Text;
            List<Customer> customersList = new List<Customer>();
            customersList = await LoadCustomersData();
            var data = customersList.Where(x => x.FirstName.Contains(searchString)
                                        || x.LastName.ToString().Contains(searchString)
                                        || x.ListOfEmails.ToString().Contains(searchString) || x.ListOfPhoneNumbers.Contains(searchString)).ToList();
            var bindingList = new BindingList<Customer>(customersList);
            var source = new BindingSource(bindingList, null);
            CustomersGrid.DataSource = data.ToList();
        }
    }
}
