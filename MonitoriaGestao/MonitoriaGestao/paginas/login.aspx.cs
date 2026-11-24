using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class login : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
    }

    protected void btnEntrar_Click(object sender, EventArgs e)
    {
        string email = txtEmail.Text.Trim();
        string senha = txtSenha.Text.Trim();

        string strConexao = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        SqlConnection conn = new SqlConnection(strConexao);
        conn.Open();

        SqlCommand cmdAluno = new SqlCommand();
        cmdAluno.Connection = conn;
        cmdAluno.CommandText = "SELECT ra, nome, email FROM tb_alunos WHERE email = @email AND senha = @senha";
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

        SqlCommand cmdMonitor = new SqlCommand();
        cmdMonitor.Connection = conn;
        cmdMonitor.CommandText = "SELECT rm, nome, email, id_disciplina FROM tb_monitores WHERE email = @email AND senha = @senha";
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

        SqlCommand cmdCoord = new SqlCommand();
        cmdCoord.Connection = conn;
        cmdCoord.CommandText = "SELECT id_coordenador, nome, email FROM tb_coordenadores WHERE email = @email AND senha = @senha";
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
        conn.Close();

        lblMensagem.Visible = true;
        lblMensagem.Text = "E-mail ou senha inválidos.";
    }
}