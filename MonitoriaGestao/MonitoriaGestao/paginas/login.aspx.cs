using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

public partial class login : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
    }

    /* Valida o perfil e os campos, consulta a tabela mcorrespondente ao perfil escolhido no SQL Server, 
    salva os dados na Session e redireciona para a tela inicial daquele tipo de usuário*/
    protected void btnEntrar_Click(object sender, EventArgs e)
    {
        // Valida primeiro se o usuário escolheu um perfil no DropDownList;
        // se estiver em "0", exibe o aviso
        if (ddlPerfil.SelectedValue == "0")
        {
            lblMensagem.Visible = true;
            lblMensagem.Text = "Escolha o tipo de Usuário.";
            return;
        }

        // Garante que os validadores do ASP.NET (como e-mail e senha) passaram
        // antes de prosseguir com a autenticação
        if (!Page.IsValid)
        {
            return;
        }

        // Lê o perfil selecionado e limpa espaços extras de email e senha
        string perfil = ddlPerfil.SelectedValue;
        string email = txtEmail.Text.Trim();
        string senha = txtSenha.Text.Trim();

        // Abre uma conexão com o SQL Server usando a string de conexão do Web.config
        string strConexao = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        SqlConnection conn = new SqlConnection(strConexao);
        conn.Open();

        // Autenticação de Aluno: busca em tb_alunos por email e senha usando parâmetros
        // Se encontrar, guarda RA, nome e
        // email na Session, fecha reader/conexão e leva para agendar.aspx.
        if (perfil == "Aluno")
        {
            SqlCommand cmdAluno = new SqlCommand(
                "SELECT ra, nome, email FROM tb_alunos WHERE email = @email AND senha = @senha", conn);
            cmdAluno.Parameters.AddWithValue("@email", email);
            cmdAluno.Parameters.AddWithValue("@senha", senha);

            SqlDataReader drAluno = cmdAluno.ExecuteReader();
            if (drAluno.Read())
            {
                Session["PerfilUsuario"] = "Aluno";
                Session["RA"] = drAluno["ra"].ToString();
                Session["NomeUsuario"] = drAluno["nome"].ToString();
                Session["EmailUsuario"] = drAluno["email"].ToString();
                drAluno.Close();
                conn.Close();
                Response.Redirect("~/paginas/agendar.aspx");
                return;
            }
            drAluno.Close();
        }
        // Autenticação de Monitor: faz a mesma verificação em tb_monitores, mas salva
        // RM e a disciplina vinculada (IdDisciplinaMonitor) na Session antes de ir para painelMonitor.aspx.
        else if (perfil == "Monitor")
        {
            SqlCommand cmdMonitor = new SqlCommand(
                "SELECT rm, nome, email, id_disciplina FROM tb_monitores WHERE email = @email AND senha = @senha", conn);
            cmdMonitor.Parameters.AddWithValue("@email", email);
            cmdMonitor.Parameters.AddWithValue("@senha", senha);

            SqlDataReader drMonitor = cmdMonitor.ExecuteReader();
            if (drMonitor.Read())
            {
                Session["PerfilUsuario"] = "Monitor";
                Session["RM"] = drMonitor["rm"].ToString();
                Session["IdDisciplinaMonitor"] = drMonitor["id_disciplina"].ToString();
                Session["NomeUsuario"] = drMonitor["nome"].ToString();
                Session["EmailUsuario"] = drMonitor["email"].ToString();
                drMonitor.Close();
                conn.Close();
                Response.Redirect("~/paginas/painelMonitor.aspx");
                return;
            }
            drMonitor.Close();
        }
        // Autenticação de Coordenador: confere as credenciais em tb_coordenadores,
        // registra IdCoordenador na Session e redireciona para painelCoordenador.aspx.
        else if (perfil == "Coordenador")
        {
            SqlCommand cmdCoord = new SqlCommand(
                "SELECT id_coordenador, nome, email FROM tb_coordenadores WHERE email = @email AND senha = @senha", conn);
            cmdCoord.Parameters.AddWithValue("@email", email);
            cmdCoord.Parameters.AddWithValue("@senha", senha);

            SqlDataReader drCoord = cmdCoord.ExecuteReader();
            if (drCoord.Read())
            {
                Session["PerfilUsuario"] = "Coordenador";
                Session["IdCoordenador"] = drCoord["id_coordenador"].ToString();
                Session["NomeUsuario"] = drCoord["nome"].ToString();
                Session["EmailUsuario"] = drCoord["email"].ToString();
                drCoord.Close();
                conn.Close();
                Response.Redirect("~/paginas/painelCoordenador.aspx");
                return;
            }
            drCoord.Close();
        }

        // Se nenhum registro bateu no perfil escolhido, fecha a conexão e exibe a mensagem de erro.
        conn.Close();
        lblMensagem.Visible = true;
        lblMensagem.Text = "Perfil, email ou senha inválidos.";
    }
}