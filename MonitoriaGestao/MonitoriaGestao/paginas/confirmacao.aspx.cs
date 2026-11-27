using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class confirmar : System.Web.UI.Page
{
    // Na primeira carga da página (!IsPostBack), lê nome do aluno e data/horário
    // gravados na Session pela tela de agendamento. Se faltarem esses dados
    // (por exemplo, acesso direto pela URL sem agendar antes), redireciona de volta
    // para agendar.aspx; caso contrário, monta a mensagem em lblConfirmacao.
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