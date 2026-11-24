using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

public partial class agendar : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Garante que apenas aluno logado acesse a tela de agendamento
        if (Session["PerfilUsuario"] == null || Session["PerfilUsuario"].ToString() != "Aluno")
        {
            Response.Redirect("~/paginas/login.aspx");
            return;
        }

        if (!IsPostBack)
        {
            CarregarDisciplinas();
            ResetarMonitores();
            ResetarDisponibilidades();
        }
    }

    private void ResetarMonitores()
    {
        ddlMonitor.Items.Clear();
        ddlMonitor.Items.Insert(0, new ListItem("Selecione...", "0"));
    }

    private void ResetarDisponibilidades()
    {
        ddlDisponibilidade.Items.Clear();
        ddlDisponibilidade.Items.Insert(0, new ListItem("Selecione...", "0"));
    }

    // Carrega todas as disciplinas cadastradas
    private void CarregarDisciplinas()
    {
        string strConexao = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        SqlConnection conn = new SqlConnection(strConexao);
        SqlCommand cmd = new SqlCommand("SELECT id_disciplina, nome_disciplina FROM tb_disciplinas ORDER BY nome_disciplina", conn);

        conn.Open();
        SqlDataReader dr = cmd.ExecuteReader();
        ddlDisciplina.DataSource = dr;
        ddlDisciplina.DataTextField = "nome_disciplina";
        ddlDisciplina.DataValueField = "id_disciplina";
        ddlDisciplina.DataBind();
        dr.Close();
        conn.Close();

        ddlDisciplina.Items.Insert(0, new ListItem("Selecione...", "0"));
    }

    // Filtra monitores vinculados à disciplina escolhida
    protected void ddlDisciplina_SelectedIndexChanged(object sender, EventArgs e)
    {
        ResetarMonitores();
        ResetarDisponibilidades();

        if (ddlDisciplina.SelectedValue == "0")
        {
            return;
        }

        string strConexao = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        SqlConnection conn = new SqlConnection(strConexao);
        SqlCommand cmd = new SqlCommand("SELECT rm, nome FROM tb_monitores WHERE id_disciplina = @id_disciplina ORDER BY nome", conn);
        cmd.Parameters.AddWithValue("@id_disciplina", ddlDisciplina.SelectedValue);

        conn.Open();
        SqlDataReader dr = cmd.ExecuteReader();
        ddlMonitor.DataSource = dr;
        ddlMonitor.DataTextField = "nome";
        ddlMonitor.DataValueField = "rm";
        ddlMonitor.DataBind();
        dr.Close();
        conn.Close();

        ddlMonitor.Items.Insert(0, new ListItem("Selecione...", "0"));
    }

    protected void ddlMonitor_SelectedIndexChanged(object sender, EventArgs e)
    {
        CarregarHorariosDaSemana();
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
        // Se hoje for domingo (0), a semana já é a que começa amanhã (segunda-feira)
        int diasDesdeSegunda = (hoje.DayOfWeek == DayOfWeek.Sunday)
            ? -1
            : ((int)hoje.DayOfWeek - (int)DayOfWeek.Monday);

        DateTime segundaDaSemana = hoje.AddDays(-diasDesdeSegunda);
        int offsetDia = ObterIndiceDiaSemana(diaSemana);

        return segundaDaSemana.AddDays(offsetDia);
    }

    // Lista os dias/datas/horários da semana inteira do monitor que ainda não foram ocupados
    private void CarregarHorariosDaSemana()
    {
        ResetarDisponibilidades();

        if (ddlMonitor.SelectedValue == "0")
        {
            return;
        }

        string strConexao = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        SqlConnection conn = new SqlConnection(strConexao);
        conn.Open();

        // 1. Busca todas as disponibilidades fixas do monitor
        SqlCommand cmdDisp = new SqlCommand("SELECT dia_semana, horario FROM tb_disponibilidades WHERE rm = @rm ORDER BY id_disponibilidade", conn);
        cmdDisp.Parameters.AddWithValue("@rm", ddlMonitor.SelectedValue);

        List<string[]> listaDisp = new List<string[]>();
        SqlDataReader dr = cmdDisp.ExecuteReader();
        while (dr.Read())
        {
            listaDisp.Add(new string[] { dr["dia_semana"].ToString(), dr["horario"].ToString() });
        }
        dr.Close();

        // 2. Para cada disponibilidade, calcula a data na semana atual e verifica se está livre
        foreach (string[] item in listaDisp)
        {
            string diaSemana = item[0];
            string horario = item[1];
            DateTime dataCalculada = ObterDataNaSemanaAtual(diaSemana);

            SqlCommand cmdOcupado = new SqlCommand();
            cmdOcupado.Connection = conn;
            cmdOcupado.CommandText = @"
                SELECT COUNT(*)
                FROM tb_agendamentos
                WHERE rm = @rm AND data_agendamento = @data AND horario = @horario";
            cmdOcupado.Parameters.AddWithValue("@rm", ddlMonitor.SelectedValue);
            cmdOcupado.Parameters.AddWithValue("@data", dataCalculada.Date);
            cmdOcupado.Parameters.AddWithValue("@horario", horario);

            int jaAgendado = (int)cmdOcupado.ExecuteScalar();
            if (jaAgendado == 0)
            {
                string texto = diaSemana + " - " + dataCalculada.ToString("dd/MM/yyyy") + " - " + horario;
                string valor = diaSemana + "|" + dataCalculada.ToString("yyyy-MM-dd") + "|" + horario;
                ddlDisponibilidade.Items.Add(new ListItem(texto, valor));
            }
        }

        conn.Close();

        ddlDisponibilidade.Items.Insert(0, new ListItem("Selecione...", "0"));

        if (ddlDisponibilidade.Items.Count == 1)
        {
            lblMensagem.Visible = true;
            lblMensagem.Text = "Todos os horários deste monitor na semana já estão ocupados.";
        }
        else
        {
            lblMensagem.Visible = false;
        }
    }

    // Salva o agendamento no banco, guarda nome e data na Session e vai para confirmação
    protected void btnAgendar_Click(object sender, EventArgs e)
    {
        /*if (!Page.IsValid)
        {
            return;
        }*/

        string[] partes = ddlDisponibilidade.SelectedValue.Split('|');
        string diaSemana = partes[0];
        DateTime dataSelecionada = DateTime.Parse(partes[1]);
        string horario = partes[2];

        string strConexao = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        SqlConnection conn = new SqlConnection(strConexao);
        conn.Open();

        // Checagem final para evitar conflito caso o horário tenha sido ocupado
        SqlCommand cmdVerifica = new SqlCommand();
        cmdVerifica.Connection = conn;
        cmdVerifica.CommandText = @"
            SELECT COUNT(*)
            FROM tb_agendamentos
            WHERE rm = @rm AND data_agendamento = @data_agendamento AND horario = @horario";
        cmdVerifica.Parameters.AddWithValue("@rm", ddlMonitor.SelectedValue);
        cmdVerifica.Parameters.AddWithValue("@data_agendamento", dataSelecionada.Date);
        cmdVerifica.Parameters.AddWithValue("@horario", horario);

        int ocupados = (int)cmdVerifica.ExecuteScalar();
        if (ocupados > 0)
        {
            conn.Close();
            lblMensagem.Visible = true;
            lblMensagem.Text = "Esse horário acabou de ser reservado. Escolha outro.";
            CarregarHorariosDaSemana();
            return;
        }

        // Insere o novo agendamento
        SqlCommand cmdInsert = new SqlCommand();
        cmdInsert.Connection = conn;
        cmdInsert.CommandText = @"
            INSERT INTO tb_agendamentos (ra, rm, id_disciplina, dia_semana, data_agendamento, horario, status)
            VALUES (@ra, @rm, @id_disciplina, @dia_semana, @data_agendamento, @horario, 'Agendada')";
        cmdInsert.Parameters.AddWithValue("@ra", Session["RA"].ToString());
        cmdInsert.Parameters.AddWithValue("@rm", ddlMonitor.SelectedValue);
        cmdInsert.Parameters.AddWithValue("@id_disciplina", ddlDisciplina.SelectedValue);
        cmdInsert.Parameters.AddWithValue("@dia_semana", diaSemana);
        cmdInsert.Parameters.AddWithValue("@data_agendamento", dataSelecionada.Date);
        cmdInsert.Parameters.AddWithValue("@horario", horario);
        cmdInsert.ExecuteNonQuery();
        conn.Close();

        // Salva Nome do Aluno e Data/Horário selecionados na Session antes do redirecionamento
        string nomeAluno = Session["NomeUsuario"].ToString();
        string dataFormatada = dataSelecionada.ToString("dd/MM/yyyy") + " (" + diaSemana + " - " + horario + ")";

        Session["NomeAlunoAgendamento"] = nomeAluno;
        Session["DataAgendamentoSelecionada"] = dataFormatada;

        Response.Redirect("~/paginas/confirmacao.aspx");
    }

}