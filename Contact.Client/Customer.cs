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
            DateTime date = dateOfBirthCustomer.Value;


            Customer _customer = new Customer()
            {
                Id = Guid.NewGuid(),
                FirstName = txtFirstName.Text,
                LastName = txtLastName.Text,
                ListOfEmails = txtEmailsList.Text,
                ListOfPhoneNumbers = txtPhoneList.Text,
                DateOfBirth = Convert.ToDateTime(date.ToString("yyyy-mm-dd"))
            };

            var postTask = client.PostAsJsonAsync<Customer>("Customers", _customer);

            postTask.Wait();

            var result = postTask.Result;

            if (result.IsSuccessStatusCode)
            {

                var readTask = result.Content.ReadAsAsync<Customer>();

                readTask.Wait();

                var insertedCustomer = readTask.Result;

                var data = await LoadCustomersData();
                CustomersGrid.DataSource = data.ToList();
                Clear();

                MessageBox.Show(result.StatusCode.ToString());
            }
            else
            {
                MessageBox.Show(result.StatusCode.ToString());
            }


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

        private async void btnUpdateCustomer_Click(object sender, EventArgs e)
        {
            DateTime date = dateOfBirthCustomer.Value;

            Customer _customer = new Customer()
            {
                Id = Guid.Parse(txtCustomerId.Text),
                FirstName = txtEditFirstName.Text,
                LastName = txtEditLastName.Text,
                ListOfEmails = txtEditEmailsList.Text,
                ListOfPhoneNumbers = txtPhoneList.Text,
                DateOfBirth = Convert.ToDateTime(date.ToString("yyyy-mm-dd"))
            };

            var postTask = client.PutAsJsonAsync<Customer>("Customers",  _customer);

            postTask.Wait();

            var result = postTask.Result;

            if (result.IsSuccessStatusCode)
            {

                var readTask = result.Content.ReadAsAsync<Customer>();

                readTask.Wait();

                var insertedCustomer = readTask.Result;

                var data = await LoadCustomersData();
                CustomersGrid.DataSource = data.ToList();

                EditClear();

                MessageBox.Show(result.StatusCode.ToString());
            }
            else
            {
                MessageBox.Show(result.StatusCode.ToString());
            }

            EditClear();
        }

        private void CustomersGrid_SelectionChanged(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in CustomersGrid.SelectedRows)
            {
                txtCustomerId.Text = row.Cells[0].Value.ToString();
                txtEditFirstName.Text = row.Cells[1].Value.ToString();
                txtEditLastName.Text = row.Cells[2].Value.ToString();
                txtEditPhoneList.Text = row.Cells[4].Value.ToString();
                txtEditEmailsList.Text = row.Cells[3].Value.ToString();
                //...
            }
        }

        private void CustomersGrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = CustomersGrid.Rows[e.RowIndex];
                txtCustomerId.Text = row.Cells["Id"].Value.ToString();
                txtEditFirstName.Text = row.Cells["FirstName"].Value.ToString();
                txtEditLastName.Text = row.Cells["LastName"].Value.ToString();
                txtEditPhoneList.Text = row.Cells["ListOfPhoneNumbers"].Value.ToString();
                txtEditEmailsList.Text = row.Cells["ListOfEmails"].Value.ToString();
            }
        }
    }
}
