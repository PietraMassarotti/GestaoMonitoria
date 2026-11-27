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

    // Lê os valores guardados na Session (definidos no login),
    // que tenta converter para texto e retorna null sem erro se a chave não existir
    // ou estiver vazia. Com isso, define se o usuário está logado, ajusta os links
    // padrão e habilita apenas os menus do perfil correspondente.
    private void ConfigurarMenu()
    {
        string nome = Session["NomeUsuario"] as string;
        string perfil = Session["PerfilUsuario"] as string; // Aluno, Monitor ou Coordenador

        // Só considera logado se nome e perfil estiverem preenchidos.
        bool logado = !string.IsNullOrEmpty(nome) && !string.IsNullOrEmpty(perfil);

        // Exibe o nome do usuário ou "Visitante", e alterna entre "Entrar" e "Sair / Minha conta".
        lblUsuarioLogado.Text = logado ? nome : "Visitante";
        lnkLogin.Visible = !logado;
        btnSair.Visible = logado;
        lnkMinhaConta.Visible = logado;

        // Libera os links específicos de cada perfil (os demais continuam ocultos por padrão).
        if (perfil == "Aluno")
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

    // Encerra a sessão do usuário (limpa e abandona a Session) e redireciona para o início.
    protected void btnSair_Click(object sender, EventArgs e)
    {
        Session.Clear();
        Session.Abandon();
        Response.Redirect("~/paginas/home.aspx");
    }
}