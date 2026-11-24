using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

public partial class meusAgendamentos : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["PerfilUsuario"] == null || Session["PerfilUsuario"].ToString() != "Aluno")
        {
            Response.Redirect("~/paginas/login.aspx");
            return;
        }

        if (!IsPostBack)
        {
            CarregarMeusAgendamentos();
        }
    }

    private void CarregarMeusAgendamentos()
    {
        string strConexao = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        SqlConnection conn = new SqlConnection(strConexao);
        SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "SELECT a.id_agendamento, a.data_agendamento, a.dia_semana, a.horario, a.status, d.nome_disciplina, m.nome AS nome_monitor FROM tb_agendamentos a INNER JOIN tb_disciplinas d ON a.id_disciplina = d.id_disciplina INNER JOIN tb_monitores m ON a.rm = m.rm WHERE a.ra = @ra ORDER BY a.data_agendamento DESC, a.id_agendamento DESC;";
        cmd.Parameters.AddWithValue("@ra", Session["RA"].ToString());

        conn.Open();
        SqlDataAdapter da = new SqlDataAdapter(cmd);
        DataTable dt = new DataTable();
        da.Fill(dt);
        conn.Close();

        gvMeusAgendamentos.DataSource = dt;
        gvMeusAgendamentos.DataBind();
    }

    protected void gvMeusAgendamentos_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        int idAgendamento = Convert.ToInt32(e.CommandArgument);

        if (e.CommandName == "ExcluirAgendamento")
        {
            ExcluirAgendamento(idAgendamento);
        }
        else if (e.CommandName == "EditarAgendamento")
        {
            AbrirEdicao(idAgendamento);
        }
    }

    private void ExcluirAgendamento(int idAgendamento)
    {
        string strConexao = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        SqlConnection conn = new SqlConnection(strConexao);
        SqlCommand cmd = new SqlCommand("DELETE FROM tb_agendamentos WHERE id_agendamento = @id AND ra = @ra", conn);
        cmd.Parameters.AddWithValue("@id", idAgendamento);
        cmd.Parameters.AddWithValue("@ra", Session["RA"].ToString());

        conn.Open();
        cmd.ExecuteNonQuery();
        conn.Close();

        pnlEdicao.Visible = false;
        lblMensagem.Visible = true;
        lblMensagem.Text = "Agendamento cancelado com sucesso.";
        CarregarMeusAgendamentos();
    }

    private void CarregarDisciplinasEdicao(string idDisciplinaSelecionada)
    {
        string strConexao = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        SqlConnection conn = new SqlConnection(strConexao);
        SqlCommand cmd = new SqlCommand("SELECT id_disciplina, nome_disciplina FROM tb_disciplinas ORDER BY nome_disciplina", conn);

        conn.Open();
        SqlDataReader dr = cmd.ExecuteReader();
        ddlEdicaoDisciplina.DataSource = dr;
        ddlEdicaoDisciplina.DataTextField = "nome_disciplina";
        ddlEdicaoDisciplina.DataValueField = "id_disciplina";
        ddlEdicaoDisciplina.DataBind();
        dr.Close();
        conn.Close();

        ddlEdicaoDisciplina.Items.Insert(0, new ListItem("Selecione...", "0"));

        if (!string.IsNullOrEmpty(idDisciplinaSelecionada) && ddlEdicaoDisciplina.Items.FindByValue(idDisciplinaSelecionada) != null)
        {
            ddlEdicaoDisciplina.SelectedValue = idDisciplinaSelecionada;
        }
        else
        {
            ddlEdicaoDisciplina.SelectedIndex = 0;
        }
    }

    private void CarregarMonitoresEdicao(string idDisciplina, string rmSelecionado)
    {
        ddlEdicaoMonitor.Items.Clear();
        ddlEdicaoMonitor.Items.Insert(0, new ListItem("Selecione...", "0"));

        if (string.IsNullOrEmpty(idDisciplina) || idDisciplina == "0")
        {
            return;
        }

        string strConexao = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        SqlConnection conn = new SqlConnection(strConexao);
        SqlCommand cmd = new SqlCommand("SELECT rm, nome FROM tb_monitores WHERE id_disciplina = @id_disciplina ORDER BY nome", conn);
        cmd.Parameters.AddWithValue("@id_disciplina", idDisciplina);

        conn.Open();
        SqlDataReader dr = cmd.ExecuteReader();
        while (dr.Read())
        {
            ddlEdicaoMonitor.Items.Add(new ListItem(dr["nome"].ToString(), dr["rm"].ToString()));
        }
        dr.Close();
        conn.Close();

        if (!string.IsNullOrEmpty(rmSelecionado) && ddlEdicaoMonitor.Items.FindByValue(rmSelecionado) != null)
        {
            ddlEdicaoMonitor.SelectedValue = rmSelecionado;
        }
    }

    protected void ddlEdicaoDisciplina_SelectedIndexChanged(object sender, EventArgs e)
    {
        CarregarMonitoresEdicao(ddlEdicaoDisciplina.SelectedValue, null);
        CarregarHorariosDisponiveisEdicao(null, null, null);
    }

    protected void ddlEdicaoMonitor_SelectedIndexChanged(object sender, EventArgs e)
    {
        CarregarHorariosDisponiveisEdicao(null, null, null);
    }

    private void AbrirEdicao(int idAgendamento)
    {
        string idDisciplinaAtual = "";
        string rmAtual = "";
        string diaAtual = "";
        DateTime dataAtual = DateTime.MinValue;
        string horarioAtual = "";

        string strConexao = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        SqlConnection conn = new SqlConnection(strConexao);
        SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "SELECT id_agendamento, id_disciplina, rm, dia_semana, data_agendamento, horario FROM tb_agendamentos WHERE id_agendamento = @id AND ra = @ra;";
        cmd.Parameters.AddWithValue("@id", idAgendamento);
        cmd.Parameters.AddWithValue("@ra", Session["RA"].ToString());

        conn.Open();
        SqlDataReader dr = cmd.ExecuteReader();
        if (dr.Read())
        {
            hfIdAgendamento.Value = dr["id_agendamento"].ToString();
            idDisciplinaAtual = dr["id_disciplina"].ToString();
            rmAtual = dr["rm"].ToString();
            diaAtual = dr["dia_semana"].ToString();
            dataAtual = Convert.ToDateTime(dr["data_agendamento"]);
            horarioAtual = dr["horario"].ToString();
        }
        dr.Close();
        conn.Close();

        if (string.IsNullOrEmpty(hfIdAgendamento.Value))
        {
            return;
        }

        CarregarDisciplinasEdicao(idDisciplinaAtual);
        CarregarMonitoresEdicao(ddlEdicaoDisciplina.SelectedValue, rmAtual);

        pnlEdicao.Visible = true;
        lblMensagem.Visible = false;

        string valorAtual = diaAtual + "|" + dataAtual.ToString("yyyy-MM-dd") + "|" + horarioAtual;
        CarregarHorariosDisponiveisEdicao(diaAtual, dataAtual, valorAtual);
    }

    // Retorna o deslocamento de Segunda-feira (0) até Domingo (6)
    private int ObterIndiceDiaSemana(string diaSemana)
    {
        switch (diaSemana.Trim().ToLower())
        {
            case "segunda":
            case "segunda-feira": return 0;
            case "terça":
            case "terca":
            case "terça-feira":
            case "terca-feira": return 1;
            case "quarta":
            case "quarta-feira": return 2;
            case "quinta":
            case "quinta-feira": return 3;
            case "sexta":
            case "sexta-feira": return 4;
            case "sábado":
            case "sabado": return 5;
            case "domingo": return 6;
            default: return 0;
        }
    }

    // Calcula a data exata do dia da semana dentro da semana atual (começando na segunda-feira)
    private DateTime ObterDataNaSemanaAtual(string diaSemana)
    {
        DateTime hoje = DateTime.Today;
        int diasDesdeSegunda = (hoje.DayOfWeek == DayOfWeek.Sunday)
            ? -1
            : ((int)hoje.DayOfWeek - (int)DayOfWeek.Monday);

        DateTime segundaDaSemana = hoje.AddDays(-diasDesdeSegunda);
        int offsetDia = ObterIndiceDiaSemana(diaSemana);

        return segundaDaSemana.AddDays(offsetDia);
    }

    // Lista os dias/datas/horários da semana do monitor que ainda não foram ocupados por outro agendamento
    private void CarregarHorariosDisponiveisEdicao(string diaAtual, DateTime? dataAtual, string valorAtualParaManter)
    {
        ddlNovaDisponibilidade.Items.Clear();

        if (ddlEdicaoMonitor.SelectedValue == "0")
        {
            ddlNovaDisponibilidade.Items.Insert(0, new ListItem("Selecione...", "0"));
            return;
        }

        string strConexao = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        SqlConnection conn = new SqlConnection(strConexao);
        conn.Open();

        DateTime? dataInicioPeriodo = null;
        DateTime? dataFimPeriodo = null;

        SqlCommand cmdPeriodo = new SqlCommand(
            "SELECT data_inicio_periodo, data_fim_periodo FROM tb_monitores WHERE rm = @rm", conn);
        cmdPeriodo.Parameters.AddWithValue("@rm", ddlEdicaoMonitor.SelectedValue);

        SqlDataReader drPeriodo = cmdPeriodo.ExecuteReader();
        if (drPeriodo.Read())
        {
            if (drPeriodo["data_inicio_periodo"] != DBNull.Value)
                dataInicioPeriodo = Convert.ToDateTime(drPeriodo["data_inicio_periodo"]).Date;

            if (drPeriodo["data_fim_periodo"] != DBNull.Value)
                dataFimPeriodo = Convert.ToDateTime(drPeriodo["data_fim_periodo"]).Date;
        }
        drPeriodo.Close();

        SqlCommand cmdDisp = new SqlCommand("SELECT dia_semana, horario FROM tb_disponibilidades WHERE rm = @rm ORDER BY id_disponibilidade", conn);
        cmdDisp.Parameters.AddWithValue("@rm", ddlEdicaoMonitor.SelectedValue);

        List<string[]> listaDisp = new List<string[]>();
        SqlDataReader dr = cmdDisp.ExecuteReader();
        while (dr.Read())
        {
            listaDisp.Add(new string[] { dr["dia_semana"].ToString(), dr["horario"].ToString() });
        }
        dr.Close();

        string idAtualParam = string.IsNullOrEmpty(hfIdAgendamento.Value) ? "0" : hfIdAgendamento.Value;

        foreach (string[] item in listaDisp)
        {
            string diaSemana = item[0];
            string horario = item[1];
            DateTime dataCalculada = ObterDataNaSemanaAtual(diaSemana).Date;

            bool dentroDoPeriodo = true;
            if (dataInicioPeriodo.HasValue && dataCalculada < dataInicioPeriodo.Value)
                dentroDoPeriodo = false;
            if (dataFimPeriodo.HasValue && dataCalculada > dataFimPeriodo.Value)
                dentroDoPeriodo = false;

            if (!dentroDoPeriodo)
                continue;

            SqlCommand cmdOcupado = new SqlCommand();
            cmdOcupado.Connection = conn;
            cmdOcupado.CommandText = "SELECT COUNT(*) FROM tb_agendamentos WHERE rm = @rm AND data_agendamento = @data_agendamento AND horario = @horario AND id_agendamento <> @id_atual;";
            cmdOcupado.Parameters.AddWithValue("@rm", ddlEdicaoMonitor.SelectedValue);
            cmdOcupado.Parameters.AddWithValue("@data_agendamento", dataCalculada);
            cmdOcupado.Parameters.AddWithValue("@horario", horario);
            cmdOcupado.Parameters.AddWithValue("@id_atual", idAtualParam);

            int jaAgendado = (int)cmdOcupado.ExecuteScalar();
            if (jaAgendado == 0)
            {
                string texto = diaSemana + " - " + dataCalculada.ToString("dd/MM/yyyy") + " - " + horario;
                string valor = diaSemana + "|" + dataCalculada.ToString("yyyy-MM-dd") + "|" + horario;
                ddlNovaDisponibilidade.Items.Add(new ListItem(texto, valor));
            }
        }

        if (!string.IsNullOrEmpty(valorAtualParaManter) &&
            !string.IsNullOrEmpty(diaAtual) &&
            dataAtual.HasValue &&
            dataAtual.Value != DateTime.MinValue &&
            ddlNovaDisponibilidade.Items.FindByValue(valorAtualParaManter) == null)
        {
            string[] partesAtual = valorAtualParaManter.Split('|');
            if (partesAtual.Length == 3)
            {
                string textoAtual = diaAtual + " - " + dataAtual.Value.ToString("dd/MM/yyyy") + " - " + partesAtual[2];
                ddlNovaDisponibilidade.Items.Insert(0, new ListItem(textoAtual, valorAtualParaManter));
            }
        }

        conn.Close();

        ddlNovaDisponibilidade.Items.Insert(0, new ListItem("Selecione...", "0"));

        if (!string.IsNullOrEmpty(valorAtualParaManter) && ddlNovaDisponibilidade.Items.FindByValue(valorAtualParaManter) != null)
        {
            ddlNovaDisponibilidade.SelectedValue = valorAtualParaManter;
        }
    }

    // Atualiza disciplina, monitor, dia, data e horário do agendamento no banco
    protected void btnSalvarEdicao_Click(object sender, EventArgs e)
    {
        if (ddlEdicaoDisciplina.SelectedValue == "0" ||
            ddlEdicaoMonitor.SelectedValue == "0" ||
            ddlNovaDisponibilidade.SelectedValue == "0")
        {
            lblMensagem.Visible = true;
            lblMensagem.Text = "Selecione disciplina, monitor e um dia/horário disponível.";
            return;
        }

        string[] partes = ddlNovaDisponibilidade.SelectedValue.Split('|');
        string novoDia = partes[0];
        DateTime novaData = DateTime.Parse(partes[1]);
        string novoHorario = partes[2];

        string strConexao = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        SqlConnection conn = new SqlConnection(strConexao);
        conn.Open();

        SqlCommand cmdVerifica = new SqlCommand();
        cmdVerifica.Connection = conn;
        cmdVerifica.CommandText = "SELECT COUNT(*) FROM tb_agendamentos WHERE rm = @rm AND data_agendamento = @data AND horario = @horario AND id_agendamento <> @id";
        cmdVerifica.Parameters.AddWithValue("@rm", ddlEdicaoMonitor.SelectedValue);
        cmdVerifica.Parameters.AddWithValue("@data", novaData.Date);
        cmdVerifica.Parameters.AddWithValue("@horario", novoHorario);
        cmdVerifica.Parameters.AddWithValue("@id", hfIdAgendamento.Value);

        int ocupados = (int)cmdVerifica.ExecuteScalar();
        if (ocupados > 0)
        {
            conn.Close();
            lblMensagem.Visible = true;
            lblMensagem.Text = "Esse horário já está ocupado. Escolha outro.";
            CarregarHorariosDisponiveisEdicao(null, null, null);
            return;
        }

        SqlCommand cmdUpdate = new SqlCommand();
        cmdUpdate.Connection = conn;
        cmdUpdate.CommandText = "UPDATE tb_agendamentos SET id_disciplina = @id_disciplina, rm = @rm, dia_semana = @dia, data_agendamento = @data, horario = @horario WHERE id_agendamento = @id AND ra = @ra";
        cmdUpdate.Parameters.AddWithValue("@id_disciplina", ddlEdicaoDisciplina.SelectedValue);
        cmdUpdate.Parameters.AddWithValue("@rm", ddlEdicaoMonitor.SelectedValue);
        cmdUpdate.Parameters.AddWithValue("@dia", novoDia);
        cmdUpdate.Parameters.AddWithValue("@data", novaData.Date);
        cmdUpdate.Parameters.AddWithValue("@horario", novoHorario);
        cmdUpdate.Parameters.AddWithValue("@id", hfIdAgendamento.Value);
        cmdUpdate.Parameters.AddWithValue("@ra", Session["RA"].ToString());
        cmdUpdate.ExecuteNonQuery();
        conn.Close();

        pnlEdicao.Visible = false;
        lblMensagem.Visible = true;
        lblMensagem.Text = "Agendamento atualizado com sucesso.";
        CarregarMeusAgendamentos();
    }

    protected void btnCancelarEdicao_Click(object sender, EventArgs e)
    {
        pnlEdicao.Visible = false;
    }
}