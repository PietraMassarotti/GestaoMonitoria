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
    // Garante que existe alguém logado na Session (qualquer perfil: Aluno, Monitor
    // ou Coordenador) e carrega os dados da conta apenas no primeiro acesso.
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

    // Preenche os Labels da tela com os dados salvos na Session e adapta o rótulo
    // e o valor do identificador conforme o perfil (RA, RM ou ID Coordenação).
    private void CarregarDadosConta()
    {
        string perfil = Session["PerfilUsuario"] as string;

        lblPerfil.Text = perfil;
        lblNome.Text = Session["NomeUsuario"].ToString();
        lblEmail.Text = Session["EmailUsuario"].ToString();

        if (perfil == "Aluno")
        {
            lblTipoId.Text = "RA";
            lblIdentificador.Text = Session["RA"].ToString();
        }
        else if (perfil == "Monitor")
        {
            lblTipoId.Text = "RM";
            lblIdentificador.Text = Session["RM"].ToString();
        }
        else if (perfil == "Coordenador")
        {
            lblTipoId.Text = "ID Coordenação";
            lblIdentificador.Text = Session["IdCoordenador"].ToString();
        }
    }

    // Valida os campos, mapeia qual tabela e coluna de ID pertencem ao perfil atual,
    // confere se a senha atual bate no banco e, se estiver certa, grava a nova senha.
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

        // Confere se a senha atual está correta antes de permitir a alteração.
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

        // Atualiza a senha no banco para o registro correspondente ao usuário logado.
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