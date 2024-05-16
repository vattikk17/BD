using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace BD
{
    public partial class Login : Form
    {

        public Login()
        {
            InitializeComponent();
            textBoxPassword.UseSystemPasswordChar = true;
            textBoxPassword.KeyDown += textBoxPassword_KeyDown; // Привязываем обработчик события KeyDown
        }
        private void buttonLogin_Click(object sender, EventArgs e)
        {
            // Тут ви можете виконати перевірку введених даних для авторизації

            string username = textBoxUsername.Text;
            string password = textBoxPassword.Text;

            // Приклад перевірки, можна замінити на реальний механізм авторизації
            if (username == "Admin" && password == "1234")
            {
                // Авторизація успішна, відкриваємо головну форму
                MainForm mainForm = new MainForm();
                mainForm.Show();
                this.Hide(); // Ховаємо форму авторизації
            }
            else
            {
                MessageBox.Show("Неправильне ім'я користувача або пароль", "Помилка авторизації", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Приклад для форми авторизації

        private void checkBoxShowPassword_CheckedChanged(object sender, EventArgs e)
        {
            // Зміна властивості UseSystemPasswordChar залежно від стану прапорця
            textBoxPassword.UseSystemPasswordChar = !checkBoxShowPassword.Checked;
        }

        private void textBoxPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                // Если нажата клавиша Enter, вызываем метод для обработки логина
                buttonLogin_Click(sender, e);
            }
        }


    }
}
