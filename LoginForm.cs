using LoginAndRegisterSystem.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LoginAndRegisterSystem
{
    public partial class LoginForm : Form
    {
        bool passwordVisible = false;
        public LoginForm()
        {
            InitializeComponent();
            textBox2.PasswordChar = '*';
            KeyPreview = true;

            // Checking if CapsLock is locked
            if (Control.IsKeyLocked(Keys.CapsLock))
                label5.Visible = true;
            else
                label5.Visible = false;

            if (!CheckDatabaseExists(new SqlConnection(@"data source=(localdb)\MSSQLLocalDB;Initial Catalog=Users;Integrated Security=True;"), "Users"))
                using (var context = new UsersDbContext()) { context.Users.Add(new User(0, "admin", "admin")); }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Close();
        }

        Point lastPoint;
        private void panel2_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Left += e.X - lastPoint.X;
                Top += e.Y - lastPoint.Y;
            }
        }

        private void panel2_MouseDown(object sender, MouseEventArgs e)
        {
            lastPoint = new Point(e.X, e.Y);
        }

        private void label3_MouseEnter(object sender, EventArgs e)
        {
            label3.ForeColor = Color.Silver;
        }

        private void label3_MouseLeave(object sender, EventArgs e)
        {
            label3.ForeColor = Color.White;
        }

        private void LoginForm_KeyDown(object sender, KeyEventArgs e)
        {
            // Form keyDown logic
            if (e.KeyValue == (char)Keys.Enter)
                pictureBox3_Click(null, null);
            else if(Control.IsKeyLocked(Keys.CapsLock))
                label5.Visible = true;
            else
                label5.Visible = false;
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            // Visible/Invisible password logic
            if (passwordVisible)
            {
                passwordVisible = false;
                pictureBox4.Image = Resources.password_invisible;
                textBox2.PasswordChar = '*';
            }
            else
            {
                passwordVisible = true;
                pictureBox4.Image = Resources.password_visible;
                textBox2.PasswordChar = '\0';
            }
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            // Editing all textBoxes
            textBox1.Text = textBox1.Text.Trim();
            textBox1.SelectionStart = textBox1.TextLength;

            textBox2.Text = textBox2.Text.Trim();
            textBox2.SelectionStart = textBox2.TextLength;

            // If data is valid
            if (textBox1.Text.Length > 0 && textBox2.Text.Length > 0)
            {
                Cursor.Current = Cursors.WaitCursor;
                try
                {
                    SqlConnection con = new SqlConnection(@"data source=(localdb)\MSSQLLocalDB;Initial Catalog=Users;Integrated Security=True;");
                    SqlDataAdapter sqa = new SqlDataAdapter($"Select * from Users where Login = '{textBox1.Text}' and Password = '{textBox2.Text}'", con);
                    DataTable dt = new DataTable();
                    sqa.Fill(dt);

                    // If data is correct
                    if (dt.Rows.Count >= 1)
                    {
                        MessageBox.Show("You successfully loged in!", "Success!",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);

                        textBox1.Text = string.Empty;
                        textBox2.Text = string.Empty;
                    }
                    // If data is incorrect
                    else
                    {
                        MessageBox.Show("Username or Password is wrong. Enter correct data or Register.", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        textBox2.Text = "";
                    }

                    con.Close();
                }
                // If something go wrong
                catch (Exception ex)
                {
                    MessageBox.Show($"Database error. Try again.\n\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                Cursor.Current = Cursors.Default;
            }
            else
            {
                MessageBox.Show("Enter correct data.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox2.Text = "";
            }
        }

        // I have no idea what the hell is that, i copied it from Stackoverflow and that works
        private static bool CheckDatabaseExists(SqlConnection tmpConn, string databaseName)
        {
            string sqlCreateDBQuery;
            bool result = false;

            try
            {
                sqlCreateDBQuery = string.Format("SELECT database_id FROM sys.databases WHERE Name = '{0}'", databaseName);
                using (SqlCommand sqlCmd = new SqlCommand(sqlCreateDBQuery, tmpConn))
                {
                    tmpConn.Open();
                    object resultObj = sqlCmd.ExecuteScalar();
                    int databaseID = 0;
                    if (resultObj != null)
                    {
                        int.TryParse(resultObj.ToString(), out databaseID);
                    }
                    tmpConn.Close();
                    result = (databaseID > 0);
                }
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }

        private void label3_Click(object sender, EventArgs e)
        {
            // Going to registerForm
            this.Hide();
            RegisterForm regForm = new RegisterForm();
            regForm.Closed += (s, args) => this.Close();
            regForm.Show();
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            // Preveting Enters in textBox
            if (e.KeyCode == Keys.Enter)
            {
                textBox1.Text = textBox1.Text.Trim();
                textBox1.SelectionStart = textBox1.TextLength;
            }
        }
    }
}
