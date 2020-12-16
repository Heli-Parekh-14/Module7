using System;
using System.Data;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;

namespace WebApplication2_CRUD
{
    public partial class index : System.Web.UI.Page
    {
        string connectionString = @"Server=localhost;Database=rkit;Uid=root;Pwd=;";
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                Clear();
                GridFill();
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                using (MySqlConnection sqlCon = new MySqlConnection(connectionString))
                {
                    sqlCon.Open();
                    MySqlCommand sqlCmd = new MySqlCommand("ProductAddorEdit", sqlCon);
                    sqlCmd.CommandType = CommandType.StoredProcedure;
                    sqlCmd.Parameters.AddWithValue("_productid", Convert.ToInt32(hfProductID.Value == "" ? "0" : hfProductID.Value));
                    sqlCmd.Parameters.AddWithValue("_product", txtProduct.Text.Trim());
                    sqlCmd.Parameters.AddWithValue("_price", Convert.ToDecimal(txtPrice.Text.Trim()));
                    sqlCmd.Parameters.AddWithValue("_count", Convert.ToInt32(txtCount.Text.Trim()));
                    sqlCmd.Parameters.AddWithValue("_description", txtDescription.Text.Trim());
                    sqlCmd.ExecuteNonQuery();
                    GridFill();
                    Clear();
                    lblSuccessMessage.Text = "Saved Successfully";
                }
            }
            catch(Exception ex)
            {
                lblErrorMessage.Text = ex.Message;
            }            
        }

        void Clear()
        {
            hfProductID.Value = "";
            txtProduct.Text = txtPrice.Text = txtCount.Text = txtDescription.Text = "";
            btnSave.Text = "Save";
            btnDelete.Enabled = false;
            lblErrorMessage.Text = lblSuccessMessage.Text = "";
        }
        protected void btnClear_Click(object sender, EventArgs e)
        {
            Clear();
        }

        void GridFill()
        {
            using (MySqlConnection sqlCon = new MySqlConnection(connectionString))
            {
                sqlCon.Open();
                MySqlDataAdapter sqlDa = new MySqlDataAdapter("ProductViewAll", sqlCon);
                sqlDa.SelectCommand.CommandType = CommandType.StoredProcedure;
                DataTable dtbl = new DataTable();
                sqlDa.Fill(dtbl);
                gvProduct.DataSource = dtbl;
                gvProduct.DataBind();
            }
        }

        protected void lnkSelect_Click(object sender, EventArgs e)
        {
            int productid = Convert.ToInt32((sender as LinkButton).CommandArgument);
            using (MySqlConnection sqlCon = new MySqlConnection(connectionString))
            {
                sqlCon.Open();
                MySqlDataAdapter sqlDa = new MySqlDataAdapter("ProductViewByID", sqlCon);
                sqlDa.SelectCommand.CommandType = CommandType.StoredProcedure;
                sqlDa.SelectCommand.Parameters.AddWithValue("_productid", productid);
                DataTable dtbl = new DataTable();
                sqlDa.Fill(dtbl);

                txtProduct.Text = dtbl.Rows[0][1].ToString();
                txtPrice.Text = dtbl.Rows[0][2].ToString();
                txtCount.Text = dtbl.Rows[0][3].ToString();
                txtDescription.Text = dtbl.Rows[0][4].ToString();
                hfProductID.Value= dtbl.Rows[0][0].ToString();

                btnSave.Text = "Update";
                btnDelete.Enabled = true;
            }
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            using (MySqlConnection sqlCon = new MySqlConnection(connectionString))
            {
                sqlCon.Open();
                MySqlCommand sqlCmd = new MySqlCommand("ProductDeleteByID", sqlCon);
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.Parameters.AddWithValue("_productid", Convert.ToInt32(hfProductID.Value == "" ? "0" : hfProductID.Value));                
                sqlCmd.ExecuteNonQuery();
                GridFill();
                Clear();
                lblSuccessMessage.Text = "Deleted Successfully";
            }
        }
    }
}