using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class confirmar : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            string nomeAluno = Session["NomeAlunoAgendamento"] as string;
            string dataSelecionada = Session["DataAgendamentoSelecionada"] as string;

            if (string.IsNullOrEmpty(nomeAluno) || string.IsNullOrEmpty(dataSelecionada))
            {
                Response.Redirect("~/paginas/agendar.aspx");
                return;
            }

            lblConfirmacao.Text = "Parabéns, " + nomeAluno + "! Sua monitoria foi agendada para " + dataSelecionada + ".";
        }
    }
}