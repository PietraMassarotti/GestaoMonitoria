using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class minhaConta : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["PerfilUsuario"] == null)
        {
            Response.Redirect("~/paginas/login.aspx");
            return;
        }

        if (!IsPostBack)
        {
            CarregarDadosConta();
        }
    }

    private void CarregarDadosConta()
    {
        string perfil = Session["PerfilUsuario"] as string;

        txtPerfil.Text = perfil;
        txtNome.Text = Session["NomeUsuario"].ToString();
        txtEmail.Text = Session["EmailUsuario"].ToString();

        if (perfil == "Aluno")
        {
            lblTipoId.Text = "RA";
            txtIdentificador.Text = Session["RA"].ToString();
        }
        else if (perfil == "Monitor")
        {
            lblTipoId.Text = "RM";
            txtIdentificador.Text = Session["RM"].ToString();
        }
        else if (perfil == "Coordenador")
        {
            lblTipoId.Text = "ID Coordenação";
            txtIdentificador.Text = Session["IdCoordenador"].ToString();
        }
    }

    protected void btnAlterarSenha_Click(object sender, EventArgs e)
    {
        if (!Page.IsValid)
        {
            return;
        }

        string perfil = Session["PerfilUsuario"].ToString();
        string senhaAtual = txtSenhaAtual.Text.Trim();
        string novaSenha = txtNovaSenha.Text.Trim();

        string tabela = "";
        string colunaId = "";
        string valorId = "";

        if (perfil == "Aluno")
        {
            tabela = "tb_alunos";
            colunaId = "ra";
            valorId = Session["RA"].ToString();
        }
        else if (perfil == "Monitor")
        {
            tabela = "tb_monitores";
            colunaId = "rm";
            valorId = Session["RM"].ToString();
        }
        else if (perfil == "Coordenador")
        {
            tabela = "tb_coordenadores";
            colunaId = "id_coordenador";
            valorId = Session["IdCoordenador"].ToString();
        }

        string strConexao = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        SqlConnection conn = new SqlConnection(strConexao);
        conn.Open();

        // Confere se a senha atual está correta
        SqlCommand cmdVerifica = new SqlCommand();
        cmdVerifica.Connection = conn;
        cmdVerifica.CommandText = "SELECT COUNT(*) FROM " + tabela + " WHERE " + colunaId + " = @id AND senha = @senhaAtual";
        cmdVerifica.Parameters.AddWithValue("@id", valorId);
        cmdVerifica.Parameters.AddWithValue("@senhaAtual", senhaAtual);

        int confere = (int)cmdVerifica.ExecuteScalar();
        if (confere == 0)
        {
            conn.Close();
            lblMensagem.Visible = true;
            lblMensagem.Text = "A senha atual está incorreta.";
            return;
        }

        // Atualiza a senha no banco
        SqlCommand cmdUpdate = new SqlCommand();
        cmdUpdate.Connection = conn;
        cmdUpdate.CommandText = "UPDATE " + tabela + " SET senha = @novaSenha WHERE " + colunaId + " = @id";
        cmdUpdate.Parameters.AddWithValue("@novaSenha", novaSenha);
        cmdUpdate.Parameters.AddWithValue("@id", valorId);
        cmdUpdate.ExecuteNonQuery();
        conn.Close();

        txtSenhaAtual.Text = "";
        txtNovaSenha.Text = "";
        lblMensagem.Visible = true;
        lblMensagem.Text = "Senha alterada com sucesso.";
    }
}