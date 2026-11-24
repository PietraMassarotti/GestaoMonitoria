using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class masterPage : System.Web.UI.MasterPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ConfigurarMenu();
        }
    }

    private void ConfigurarMenu()
    {
        string nome = Session["NomeUsuario"] as string;
        string perfil = Session["PerfilUsuario"]as string; // Aluno, Monitor ou Coordenador

        bool logado = !string.IsNullOrEmpty(nome) && !string.IsNullOrEmpty(perfil);

        lblUsuarioLogado.Text = logado ? nome : "Visitante";
        lnkLogin.Visible = !logado;
        btnSair.Visible = logado;
        lnkMinhaConta.Visible = logado;

        if(perfil == "Aluno")
        {
            lnkAgendar.Visible = true;
            lnkMeusAgendamentos.Visible = true;
        }

        if (perfil == "Monitor")
        {
            lnkMonitor.Visible = true;
        }

        if (perfil == "Coordenador")
        {
            lnkCoordenacao.Visible = true;
        }

    }

    protected void btnSair_Click(object sender, EventArgs e)
    {
        Session.Clear();
        Session.Abandon();
        Response.Redirect("~/paginas/home.aspx");
    }
}
