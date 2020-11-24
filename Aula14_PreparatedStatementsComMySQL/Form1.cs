using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using MySql.Data.MySqlClient;

namespace Aula14_PreparatedStatementsComMySQL
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            lstContatos.GridLines = true;
            lstContatos.View = View.Details;
            lstContatos.FullRowSelect = true;

            lstContatos.Columns.Add("Id", 30, HorizontalAlignment.Left);
            lstContatos.Columns.Add("Nome", 150, HorizontalAlignment.Left);
            lstContatos.Columns.Add("Email", 150, HorizontalAlignment.Left);
            lstContatos.Columns.Add("Telefone", 150, HorizontalAlignment.Left);

            carregar_lista_contatos();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Conectar no mysql (servidor) utilizando o usuário e senha.
            // Gravar os registros na tabela de contato
            // Avisar o usuário que deu tudo certo ou tudo errado

            try
            {
                // Abrindo a conexão com o MySQL.
                string data_source = "datasource=localhost;username=root;password=;database=db_agenda";
                MySqlConnection Conexao = new MySqlConnection(data_source);
                Conexao.Open();


                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = Conexao;

                // Se existir alguma coisa no txtId é porque estamos editando um contato, se não
                // é pq é um novo contato.
                if (txtId.Text == "")
                {
                    cmd.CommandText = "INSERT INTO contato (nome, email, telefone) VALUES (@nome, @email, @telefone)";
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@nome", txtNome.Text);
                    cmd.Parameters.AddWithValue("@email", txtEmail.Text);
                    cmd.Parameters.AddWithValue("@telefone", txtTelefone.Text);
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Contato inserido com sucesso.",
                                    "Deu certo!",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                } else
                {
                    // vou atualizar...

                    cmd.CommandText = "UPDATE contato SET nome=@nome, email=@email, telefone=@telefone WHERE id = @id";
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@nome", txtNome.Text);
                    cmd.Parameters.AddWithValue("@email", txtEmail.Text);
                    cmd.Parameters.AddWithValue("@telefone", txtTelefone.Text);
                    cmd.Parameters.AddWithValue("@id", txtId.Text);
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Contato atualizado com sucesso.",
                                    "Deu certo!",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                }

                zerar_formulario();

                carregar_lista_contatos();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,
                                    "Deu errado!",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            carregar_lista_contatos();
        }


        private void carregar_lista_contatos()
        {
            // Conectar no MySQL
            // Enviar comando de listagem de registros
            // Mostrados dados obtidos para o usuário

            try
            {
                // Abrindo a conexão com o MySQL.
                string data_source = "datasource=localhost;username=root;password=;database=db_agenda";
                MySqlConnection Conexao = new MySqlConnection(data_source);
                Conexao.Open();

                // Executando o comando pata retornar registros do MySQL com Preparated Statement
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = Conexao;
                cmd.CommandText = "SELECT id, nome, email, telefone FROM contato ORDER BY id DESC";


                // Estruturar os dados recebidos do mysql e mostrar ao usuário
                // reader = leitor
                MySqlDataReader reader = cmd.ExecuteReader();

                lstContatos.Items.Clear();

                while (reader.Read())
                {
                    string[] linha = { reader.GetString(0), //pega a id
                                       reader.GetString(1), // pega o nome
                                       reader.GetString(2), // pega o email
                                       reader.GetString(3), // pega o telefone
                                     };

                    ListViewItem estrutura_da_linha = new ListViewItem(linha);

                    lstContatos.Items.Add(estrutura_da_linha);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erro no Acesso ao BD", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(txtId.Text == "")
            {
                MessageBox.Show("Selecione um registro antes.",
                                "Ops!",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                return;
            }


            DialogResult confirmacao = MessageBox.Show("Tem certeza que deseja excluir?", 
                                                       "Cuidado", 
                                                       MessageBoxButtons.YesNo, 
                                                       MessageBoxIcon.Warning);
            if(confirmacao == DialogResult.Yes)
            {
                try
                {
                    // Abrindo a conexão com o MySQL.
                    string data_source = "datasource=localhost;username=root;password=;database=db_agenda";
                    MySqlConnection Conexao = new MySqlConnection(data_source);
                    Conexao.Open();

                    // Preparando para Executar o comando de Deleção no BD
                    MySqlCommand cmd = new MySqlCommand();
                    cmd.Connection = Conexao;
                    cmd.CommandText = "DELETE FROM contato WHERE id = @id";
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@id", txtId.Text);
                    cmd.ExecuteNonQuery();

                    zerar_formulario();

                    carregar_lista_contatos();

                    MessageBox.Show("Contato excluído com sucesso.",
                                    "Deu certo!",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);                    
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message,
                                    "Deu errado!",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);

                }
            }
        }

        private void lstContatos_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            ListView.SelectedListViewItemCollection itens_selecionados = lstContatos.SelectedItems;

            foreach(ListViewItem item in itens_selecionados)
            {
                txtId.Text = item.SubItems[0].Text;
                txtNome.Text = item.SubItems[1].Text;
                txtEmail.Text = item.SubItems[2].Text;
                txtTelefone.Text = item.SubItems[3].Text;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            zerar_formulario();

            txtNome.Focus();
        }

        private void zerar_formulario()
        {
            txtId.Text = "";
            txtNome.Text = "";
            txtEmail.Text = "";
            txtTelefone.Text = "";
        }
    }
}
