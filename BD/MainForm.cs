using System;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace BD
{
    public partial class MainForm : Form
    {
        public static readonly string connectionString = "server=localhost;database=vet;uid=root;pwd=1255;";
        public static readonly MySqlConnection database = new MySqlConnection(connectionString);
        private MySqlDataAdapter dataAdapter = new MySqlDataAdapter();
        private string selectedTable = ""; // Переменная для хранения имени выбранной таблицы

        public MainForm()
        {
            InitializeComponent();
            comboBoxTables.SelectedIndexChanged += ComboBoxTables_SelectedIndexChanged;
            GetData("show tables;", bindingSourceMySQL, dataGridViewDB);
            FillTablesComboBox();
        }

        private void FillTablesComboBox()
        {
            try
            {
                if (database.State != ConnectionState.Open)
                    database.Open();

                DataTable schema = database.GetSchema("Tables");
                foreach (DataRow row in schema.Rows)
                {
                    string tableName = row["TABLE_NAME"].ToString();
                    comboBoxTables.Items.Add(tableName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                if (database.State == ConnectionState.Open)
                    database.Close();
            }
        }

        private void ComboBoxTables_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedTable = comboBoxTables.SelectedItem.ToString(); // Обновляем выбранную таблицу
            string selectCommand = $"SELECT * FROM vet.{selectedTable};";
            GetData(selectCommand, bindingSourceMySQL, dataGridViewDB);
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            About aboutForm = new About();
            aboutForm.ShowDialog();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }
        
        private void dataGridViewDB_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            int columnIndex = e.ColumnIndex;
            int rowIndex = e.RowIndex;
            if (columnIndex >= 0 && rowIndex >= 0)
            {
                DataGridViewColumn column = dataGridViewDB.Columns[columnIndex];
                string columnName = column.Name;
                string newValue = dataGridViewDB.Rows[rowIndex].Cells[columnIndex].Value.ToString();
                string primaryKeyColumnName = dataGridViewDB.Columns[0].Name;
                string primaryKeyValue = dataGridViewDB.Rows[rowIndex].Cells[0].Value.ToString();
                string updateCommand = $"UPDATE {selectedTable} SET {columnName}='{newValue}' WHERE {primaryKeyColumnName}='{primaryKeyValue}';";
                UpdateData(updateCommand);
            }
        }

        private void UpdateData(string updateCommand)
        {
            try
            {
                if (database.State != ConnectionState.Open)
                    database.Open();
                MySqlCommand command = new MySqlCommand(updateCommand, database);
                command.ExecuteNonQuery();
                MessageBox.Show("Data updated successfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                if (database.State == ConnectionState.Open)
                    database.Close();
            }
        }

        private void buttonConfirm_Click(object sender, EventArgs e)
        {
            GetData($"SELECT * FROM {selectedTable};", bindingSourceMySQL, dataGridViewDB);
        }

        private void GetData(string selectCommand, BindingSource bindingSource, DataGridView dataGridView)
        {
            try
            {
                if (database.State != ConnectionState.Open)
                    database.Open();
                dataAdapter.SelectCommand = new MySqlCommand(selectCommand, database);
                DataTable table = new DataTable();
                dataAdapter.Fill(table);
                bindingSource.DataSource = table;
                dataGridView.DataSource = bindingSource;

                foreach (DataGridViewColumn column in dataGridView.Columns)
                {
                    column.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                if (database.State == ConnectionState.Open)
                    database.Close();
            }
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            try
            {
                // Открываем соединение с базой данных
                if (database.State != ConnectionState.Open)
                    database.Open();

                // Создаем команду для вставки новой записи
                string insertCommand = $"INSERT INTO {selectedTable} VALUES ();";
                MySqlCommand command = new MySqlCommand(insertCommand, database);
                command.ExecuteNonQuery();

                // Обновляем DataGridView
                GetData($"SELECT * FROM {selectedTable};", bindingSourceMySQL, dataGridViewDB);

                // Закрываем соединение с базой данных
                if (database.State == ConnectionState.Open)
                    database.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            try
            {
                // Проверяем, есть ли выбранные строки для удаления
                if (dataGridViewDB.SelectedRows.Count > 0)
                {
                    // Открываем соединение с базой данных
                    if (database.State != ConnectionState.Open)
                        database.Open();

                    // Удаляем выбранные строки
                    foreach (DataGridViewRow row in dataGridViewDB.SelectedRows)
                    {
                        // Получаем значение первичного ключа
                        string primaryKeyValue = row.Cells[0].Value.ToString();
                        // Создаем команду для удаления записи
                        string deleteCommand = $"DELETE FROM {selectedTable} WHERE {dataGridViewDB.Columns[0].Name}='{primaryKeyValue}';";
                        MySqlCommand command = new MySqlCommand(deleteCommand, database);
                        command.ExecuteNonQuery();
                    }

                    // Обновляем DataGridView
                    GetData($"SELECT * FROM {selectedTable};", bindingSourceMySQL, dataGridViewDB);

                    // Закрываем соединение с базой данных
                    if (database.State == ConnectionState.Open)
                        database.Close();
                }
                else
                {
                    MessageBox.Show("No rows selected for deletion.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
    }
}
