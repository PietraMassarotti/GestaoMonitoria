using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

public partial class meusAgendamentos : System.Web.UI.Page
{
    // Garante que apenas um aluno logado acesse a tela e carrega a tabela
    // apenas na primeira vez que a página abre.
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

    // Busca os agendamentos do aluno logado unindo com disciplinas e monitores
    // para exibir nomes legíveis no GridView.
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

    // Identifica qual botão da linha do GridView foi clicado ("Editar" ou "Cancelar")
    // e pega o id_agendamento guardado no CommandArgument.
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

    // Remove o agendamento do banco (filtrando também pelo RA do aluno logado),
    // fecha o painel de edição se estiver aberto e atualiza a lista.
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

    // Carrega todas as disciplinas no combo de edição.
    private void CarregarDisciplinasEdicao()
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
    }

    // Carrega os monitores da disciplina escolhida no painel de edição.
    private void CarregarMonitoresEdicao()
    {
        ddlEdicaoMonitor.Items.Clear();
        ddlEdicaoMonitor.Items.Insert(0, new ListItem("Selecione...", "0"));

        if (ddlEdicaoDisciplina.SelectedValue == "0")
        {
            return;
        }

        string strConexao = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        SqlConnection conn = new SqlConnection(strConexao);
        SqlCommand cmd = new SqlCommand("SELECT rm, nome FROM tb_monitores WHERE id_disciplina = @id_disciplina ORDER BY nome", conn);
        cmd.Parameters.AddWithValue("@id_disciplina", ddlEdicaoDisciplina.SelectedValue);

        conn.Open();
        SqlDataReader dr = cmd.ExecuteReader();
        ddlEdicaoMonitor.DataSource = dr;
        ddlEdicaoMonitor.DataTextField = "nome";
        ddlEdicaoMonitor.DataValueField = "rm";
        ddlEdicaoMonitor.DataBind();
        dr.Close();
        conn.Close();

        ddlEdicaoMonitor.Items.Insert(0, new ListItem("Selecione...", "0"));
    }

    // Ao mudar a disciplina na edição, atualiza os monitores e limpa os horários.
    protected void ddlEdicaoDisciplina_SelectedIndexChanged(object sender, EventArgs e)
    {
        CarregarMonitoresEdicao();
        CarregarHorariosDisponiveisEdicao();
    }

    // Ao mudar o monitor na edição, carrega os horários livres dele na semana.
    protected void ddlEdicaoMonitor_SelectedIndexChanged(object sender, EventArgs e)
    {
        CarregarHorariosDisponiveisEdicao();
    }

    // Busca os dados atuais do agendamento escolhido, preenche os combos
    // e abre o painel de edição na tela.
    private void AbrirEdicao(int idAgendamento)
    {
        string idDisciplinaAtual = "0";
        string rmAtual = "0";

        string strConexao = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        SqlConnection conn = new SqlConnection(strConexao);
        SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "SELECT id_agendamento, id_disciplina, rm FROM tb_agendamentos WHERE id_agendamento = @id AND ra = @ra;";
        cmd.Parameters.AddWithValue("@id", idAgendamento);
        cmd.Parameters.AddWithValue("@ra", Session["RA"].ToString());

        conn.Open();
        SqlDataReader dr = cmd.ExecuteReader();
        if (dr.Read())
        {
            hfIdAgendamento.Value = dr["id_agendamento"].ToString();
            idDisciplinaAtual = dr["id_disciplina"].ToString();
            rmAtual = dr["rm"].ToString();
        }
        dr.Close();
        conn.Close();

        CarregarDisciplinasEdicao();
        if (ddlEdicaoDisciplina.Items.FindByValue(idDisciplinaAtual) != null)
        {
            ddlEdicaoDisciplina.SelectedValue = idDisciplinaAtual;
        }

        CarregarMonitoresEdicao();
        if (ddlEdicaoMonitor.Items.FindByValue(rmAtual) != null)
        {
            ddlEdicaoMonitor.SelectedValue = rmAtual;
        }

        CarregarHorariosDisponiveisEdicao();
        pnlEdicao.Visible = true;
        lblMensagem.Visible = false;
    }

    // Converte o nome do dia da semana em deslocamento de 0 (Segunda) a 6 (Domingo).
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

    // Calcula a data exata do dia da semana na semana de referência (começando na segunda).
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

    // Lista os dias, datas e horários da semana do monitor que não estão ocupados
    // por outros agendamentos (ignorando o próprio agendamento que está sendo editado).
    private void CarregarHorariosDisponiveisEdicao()
    {
        ddlNovaDisponibilidade.Items.Clear();
        ddlNovaDisponibilidade.Items.Insert(0, new ListItem("Selecione...", "0"));

        if (ddlEdicaoMonitor.SelectedValue == "0")
        {
            return;
        }

        string strConexao = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        SqlConnection conn = new SqlConnection(strConexao);
        conn.Open();

        SqlCommand cmdDisp = new SqlCommand("SELECT dia_semana, horario FROM tb_disponibilidades WHERE rm = @rm ORDER BY id_disponibilidade", conn);
        cmdDisp.Parameters.AddWithValue("@rm", ddlEdicaoMonitor.SelectedValue);

        List<string[]> listaDisp = new List<string[]>();
        SqlDataReader dr = cmdDisp.ExecuteReader();
        while (dr.Read())
        {
            listaDisp.Add(new string[] { dr["dia_semana"].ToString(), dr["horario"].ToString() });
        }
        dr.Close();

        foreach (string[] item in listaDisp)
        {
            string diaSemana = item[0];
            string horario = item[1];
            DateTime dataCalculada = ObterDataNaSemanaAtual(diaSemana).Date;

            SqlCommand cmdOcupado = new SqlCommand();
            cmdOcupado.Connection = conn;
            cmdOcupado.CommandText = "SELECT COUNT(*) FROM tb_agendamentos WHERE rm = @rm AND data_agendamento = @data_agendamento AND horario = @horario AND id_agendamento <> @id_atual;";
            cmdOcupado.Parameters.AddWithValue("@rm", ddlEdicaoMonitor.SelectedValue);
            cmdOcupado.Parameters.AddWithValue("@data_agendamento", dataCalculada);
            cmdOcupado.Parameters.AddWithValue("@horario", horario);
            cmdOcupado.Parameters.AddWithValue("@id_atual", hfIdAgendamento.Value);

            int jaAgendado = (int)cmdOcupado.ExecuteScalar();
            if (jaAgendado == 0)
            {
                string texto = diaSemana + " - " + dataCalculada.ToString("dd/MM/yyyy") + " - " + horario;
                string valor = diaSemana + "|" + dataCalculada.ToString("yyyy-MM-dd") + "|" + horario;
                ddlNovaDisponibilidade.Items.Add(new ListItem(texto, valor));
            }
        }

        conn.Close();
    }

    // Valida os campos escolhidos, confere se ninguém pegou o horário nesse meio tempo
    // e atualiza o agendamento em tb_agendamentos.
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
        cmdVerifica.CommandText = "SELECT COUNT(*) FROM tb_agendamentos WHERE rm = @rm AND data_agendamento = @data_agendamento AND horario = @horario AND id_agendamento <> @id";
        cmdVerifica.Parameters.AddWithValue("@rm", ddlEdicaoMonitor.SelectedValue);
        cmdVerifica.Parameters.AddWithValue("@data_agendamento", novaData.Date);
        cmdVerifica.Parameters.AddWithValue("@horario", novoHorario);
        cmdVerifica.Parameters.AddWithValue("@id", hfIdAgendamento.Value);

        int ocupados = (int)cmdVerifica.ExecuteScalar();
        if (ocupados > 0)
        {
            conn.Close();
            lblMensagem.Visible = true;
            lblMensagem.Text = "Esse horário já está ocupado. Escolha outro.";
            CarregarHorariosDisponiveisEdicao();
            return;
        }

        SqlCommand cmdUpdate = new SqlCommand();
        cmdUpdate.Connection = conn;
        cmdUpdate.CommandText = "UPDATE tb_agendamentos SET id_disciplina = @id_disciplina, rm = @rm, dia_semana = @dia, data_agendamento = @data_agendamento, horario = @horario WHERE id_agendamento = @id AND ra = @ra";
        cmdUpdate.Parameters.AddWithValue("@id_disciplina", ddlEdicaoDisciplina.SelectedValue);
        cmdUpdate.Parameters.AddWithValue("@rm", ddlEdicaoMonitor.SelectedValue);
        cmdUpdate.Parameters.AddWithValue("@dia", novoDia);
        cmdUpdate.Parameters.AddWithValue("@data_agendamento", novaData.Date);
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

    // Esconde o painel de edição sem salvar alterações.
    protected void btnCancelarEdicao_Click(object sender, EventArgs e)
    {
        pnlEdicao.Visible = false;
    }
}