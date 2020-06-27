using LoginAndRegisterSystem.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LoginAndRegisterSystem
{
    public partial class RegisterForm : Form
    {
        bool passwordVisible = false;
        public RegisterForm()
        {
            InitializeComponent();
            textBox2.PasswordChar = '*';
            textBox3.PasswordChar = '*';

            KeyPreview = true;

            if (Control.IsKeyLocked(Keys.CapsLock))
                label5.Visible = true;
            else
                label5.Visible = false;

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Hide();
            LoginForm logForm = new LoginForm();
            logForm.Closed += (s, args) => this.Close();
            logForm.Show();
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

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            textBox1.Text = textBox1.Text.Trim();
            textBox1.SelectionStart = textBox1.TextLength;

            textBox2.Text = textBox2.Text.Trim();
            textBox2.SelectionStart = textBox2.TextLength;

            textBox3.Text = textBox3.Text.Trim();
            textBox3.SelectionStart = textBox3.TextLength;

            if (textBox1.Text.Length > 0 && textBox2.Text.Length > 0 && textBox2.Text == textBox3.Text)
            {
                Cursor.Current = Cursors.WaitCursor;
                try
                {
                    SqlConnection con = new SqlConnection(@"data source=(localdb)\MSSQLLocalDB;Initial Catalog=Users;Integrated Security=True;");
                    SqlDataAdapter sqa = new SqlDataAdapter($"Select * from Users where Login = '{textBox1.Text}'", con);
                    DataTable dt = new DataTable();
                    sqa.Fill(dt);

                    if (dt.Rows.Count >= 1)
                    {
                        DialogResult result = MessageBox.Show("User already exist. If it's your account, do you want to Sign In?", "Error",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                        if (result == DialogResult.Yes)
                        {
                            this.Hide();
                            LoginForm logForm = new LoginForm();
                            logForm.Closed += (s, args) => this.Close();
                            logForm.Show();
                        }
                    }
                    else
                    {
                        using (var context = new UsersDbContext())
                        {
                            context.Users.Add(new User(0, textBox1.Text, textBox2.Text));
                            context.SaveChanges();
                        }

                        MessageBox.Show($"You successfully registered an account with Login: {textBox1.Text}.", "Success!",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);

                        this.Hide();
                        LoginForm logForm = new LoginForm();
                        logForm.Closed += (s, args) => this.Close();
                        logForm.Show();
                    }

                    con.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Database error. Try again.\n\n{ex.Message}", "Error");
                }

                Cursor.Current = Cursors.Default;
            }
            else if (textBox2.Text != textBox3.Text)
            {
                MessageBox.Show("Passwords do not match.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox3.Text = string.Empty;
            }
            else
            {
                MessageBox.Show("Enter correct data.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox1.Text = string.Empty;
                textBox2.Text = string.Empty;
                textBox3.Text = string.Empty;
            }
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            if (passwordVisible)
            {
                passwordVisible = false;
                pictureBox4.Image = Resources.password_invisible;
                textBox2.PasswordChar = '*';
                textBox3.PasswordChar = '*';
            }
            else
            {
                passwordVisible = true;
                pictureBox4.Image = Resources.password_visible;
                textBox2.PasswordChar = '\0';
                textBox3.PasswordChar = '\0';
            }
        }

        private void RegisterForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == (char)Keys.Enter)
                pictureBox3_Click(null, null);
            else if (Control.IsKeyLocked(Keys.CapsLock))
                label5.Visible = true;
            else
                label5.Visible = false;
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                textBox1.Text = textBox1.Text.Trim();
                textBox1.SelectionStart = textBox1.TextLength;
            }
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                textBox2.Text = textBox2.Text.Trim();
                textBox2.SelectionStart = textBox2.TextLength;
            }
        }

        private void textBox3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                textBox3.Text = textBox3.Text.Trim();
                textBox3.SelectionStart = textBox3.TextLength;
            }
        }
    }
}
