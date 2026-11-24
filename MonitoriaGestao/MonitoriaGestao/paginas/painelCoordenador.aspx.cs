using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

public partial class painelCoordenador : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["PerfilUsuario"] == null || Session["PerfilUsuario"].ToString() != "Coordenador")
        {
            Response.Redirect("~/paginas/login.aspx");
            return;
        }

        if (!IsPostBack)
        {
            CarregarHorariosDropDowns();
            CarregarDisciplinas();
            CarregarMonitores();
            CarregarAlunos();
            CarregarTodosAgendamentos();

            AtivarAbaMonitores();
        }
    }

    private void CarregarHorariosDropDowns()
    {
        DropDownList[] combos = new DropDownList[]
        {
            ddlHoraInicioSegunda, ddlHoraFimSegunda,
            ddlHoraInicioTerca, ddlHoraFimTerca,
            ddlHoraInicioQuarta, ddlHoraFimQuarta,
            ddlHoraInicioQuinta, ddlHoraFimQuinta,
            ddlHoraInicioSexta, ddlHoraFimSexta,
            ddlHoraInicioSabado, ddlHoraFimSabado
        };

        foreach (DropDownList ddl in combos)
        {
            ddl.Items.Clear();
            ddl.Items.Add(new ListItem("Hora...", "0"));

            for (int hora = 7; hora <= 21; hora++)
            {
                string rotulo = hora + "h";
                ddl.Items.Add(new ListItem(rotulo, hora.ToString()));
            }
        }
    }

    private void AtivarAbaMonitores()
    {
        pnlMonitores.Visible = true;
        pnlAlunos.Visible = false;
        pnlAgendamentos.Visible = false;

        btnAbaMonitores.CssClass = "btnAbaAtiva";
        btnAbaAlunos.CssClass = "btnAba";
        btnAbaAgendamentos.CssClass = "btnAba";
        lblMensagem.Visible = false;
    }

    private void AtivarAbaAlunos()
    {
        pnlMonitores.Visible = false;
        pnlAlunos.Visible = true;
        pnlAgendamentos.Visible = false;

        btnAbaMonitores.CssClass = "btnAba";
        btnAbaAlunos.CssClass = "btnAbaAtiva";
        btnAbaAgendamentos.CssClass = "btnAba";
        lblMensagem.Visible = false;
    }

    private void AtivarAbaAgendamentos()
    {
        pnlMonitores.Visible = false;
        pnlAlunos.Visible = false;
        pnlAgendamentos.Visible = true;

        btnAbaMonitores.CssClass = "btnAba";
        btnAbaAlunos.CssClass = "btnAba";
        btnAbaAgendamentos.CssClass = "btnAbaAtiva";
        lblMensagem.Visible = false;
    }

    protected void btnAbaMonitores_Click(object sender, EventArgs e)
    {
        AtivarAbaMonitores();
        CarregarMonitores();
    }

    protected void btnAbaAlunos_Click(object sender, EventArgs e)
    {
        AtivarAbaAlunos();
        CarregarAlunos();
    }

    protected void btnAbaAgendamentos_Click(object sender, EventArgs e)
    {
        AtivarAbaAgendamentos();
        CarregarTodosAgendamentos();
    }

    protected void cblDiasMonitor_SelectedIndexChanged(object sender, EventArgs e)
    {
        AtivarAbaMonitores();
        AtualizarCamposHorarioPorDia();
    }

    private void AtualizarCamposHorarioPorDia()
    {
        bool seg = false, ter = false, qua = false, qui = false, sex = false, sab = false;

        foreach (ListItem item in cblDiasMonitor.Items)
        {
            if (!item.Selected) continue;

            switch (item.Value)
            {
                case "Segunda-feira": seg = true; break;
                case "Terça-feira": ter = true; break;
                case "Quarta-feira": qua = true; break;
                case "Quinta-feira": qui = true; break;
                case "Sexta-feira": sex = true; break;
                case "Sábado": sab = true; break;
            }
        }

        divHorSegunda.Visible = seg;
        divHorTerca.Visible = ter;
        divHorQuarta.Visible = qua;
        divHorQuinta.Visible = qui;
        divHorSexta.Visible = sex;
        divHorSabado.Visible = sab;

        pnlHorariosPorDia.Visible = (seg || ter || qua || qui || sex || sab);
    }

    private bool ObterIntervaloHorarioDoDia(string diaSemana, out string intervaloFormatado, out string erro)
    {
        intervaloFormatado = "";
        erro = "";

        DropDownList ddlInicio = null;
        DropDownList ddlFim = null;

        switch (diaSemana)
        {
            case "Segunda-feira":
                ddlInicio = ddlHoraInicioSegunda;
                ddlFim = ddlHoraFimSegunda;
                break;
            case "Terça-feira":
                ddlInicio = ddlHoraInicioTerca;
                ddlFim = ddlHoraFimTerca;
                break;
            case "Quarta-feira":
                ddlInicio = ddlHoraInicioQuarta;
                ddlFim = ddlHoraFimQuarta;
                break;
            case "Quinta-feira":
                ddlInicio = ddlHoraInicioQuinta;
                ddlFim = ddlHoraFimQuinta;
                break;
            case "Sexta-feira":
                ddlInicio = ddlHoraInicioSexta;
                ddlFim = ddlHoraFimSexta;
                break;
            case "Sábado":
                ddlInicio = ddlHoraInicioSabado;
                ddlFim = ddlHoraFimSabado;
                break;
        }

        if (ddlInicio == null || ddlFim == null)
        {
            erro = "Dia inválido.";
            return false;
        }

        if (ddlInicio.SelectedValue == "0" || ddlFim.SelectedValue == "0")
        {
            erro = "Selecione a hora de início e de fim para " + diaSemana + ".";
            return false;
        }

        int horaInicio = int.Parse(ddlInicio.SelectedValue);
        int horaFim = int.Parse(ddlFim.SelectedValue);

        if (horaFim <= horaInicio)
        {
            erro = "Em " + diaSemana + ", a hora de fim deve ser maior que a hora de início.";
            return false;
        }

        intervaloFormatado = horaInicio + "h às " + horaFim + "h";
        return true;
    }

    private void LimparCamposCadastroMonitor()
    {
        txtRmMonitor.Text = "";
        txtNomeMonitor.Text = "";
        txtEmailMonitor.Text = "";
        txtSenhaMonitor.Text = "";
        ddlDisciplinaMonitor.SelectedIndex = 0;
        ddlPeriodoMonitor.SelectedIndex = 0;

        foreach (ListItem item in cblDiasMonitor.Items)
        {
            item.Selected = false;
        }

        ddlHoraInicioSegunda.SelectedIndex = 0;
        ddlHoraFimSegunda.SelectedIndex = 0;
        ddlHoraInicioTerca.SelectedIndex = 0;
        ddlHoraFimTerca.SelectedIndex = 0;
        ddlHoraInicioQuarta.SelectedIndex = 0;
        ddlHoraFimQuarta.SelectedIndex = 0;
        ddlHoraInicioQuinta.SelectedIndex = 0;
        ddlHoraFimQuinta.SelectedIndex = 0;
        ddlHoraInicioSexta.SelectedIndex = 0;
        ddlHoraFimSexta.SelectedIndex = 0;
        ddlHoraInicioSabado.SelectedIndex = 0;
        ddlHoraFimSabado.SelectedIndex = 0;

        AtualizarCamposHorarioPorDia();
    }

    private void CarregarDisciplinas()
    {
        string strConexao = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        SqlConnection conn = new SqlConnection(strConexao);
        SqlCommand cmd = new SqlCommand("SELECT id_disciplina, nome_disciplina FROM tb_disciplinas ORDER BY nome_disciplina", conn);

        conn.Open();
        SqlDataReader dr = cmd.ExecuteReader();
        ddlDisciplinaMonitor.DataSource = dr;
        ddlDisciplinaMonitor.DataTextField = "nome_disciplina";
        ddlDisciplinaMonitor.DataValueField = "id_disciplina";
        ddlDisciplinaMonitor.DataBind();
        dr.Close();
        conn.Close();

        ddlDisciplinaMonitor.Items.Insert(0, new ListItem("Selecione...", "0"));
    }

    private void CarregarAlunos()
    {
        string strConexao = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        SqlConnection conn = new SqlConnection(strConexao);
        SqlCommand cmd = new SqlCommand("SELECT ra, nome, email FROM tb_alunos ORDER BY nome", conn);

        conn.Open();
        SqlDataAdapter da = new SqlDataAdapter(cmd);
        DataTable dt = new DataTable();
        da.Fill(dt);
        conn.Close();

        gvAlunos.DataSource = dt;
        gvAlunos.DataBind();
    }

    protected void btnCadastrarAluno_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtRaAluno.Text) ||
            string.IsNullOrWhiteSpace(txtNomeAluno.Text) ||
            string.IsNullOrWhiteSpace(txtEmailAluno.Text) ||
            string.IsNullOrWhiteSpace(txtSenhaAluno.Text))
        {
            AtivarAbaAlunos();
            MostrarMensagem("Preencha RA, nome, e-mail e senha do aluno.");
            return;
        }

        string strConexao = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        SqlConnection conn = new SqlConnection(strConexao);
        SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "INSERT INTO tb_alunos (ra, nome, email, senha) VALUES (@ra, @nome, @email, @senha)";
        cmd.Parameters.AddWithValue("@ra", txtRaAluno.Text.Trim());
        cmd.Parameters.AddWithValue("@nome", txtNomeAluno.Text.Trim());
        cmd.Parameters.AddWithValue("@email", txtEmailAluno.Text.Trim());
        cmd.Parameters.AddWithValue("@senha", txtSenhaAluno.Text.Trim());

        conn.Open();
        cmd.ExecuteNonQuery();
        conn.Close();

        txtRaAluno.Text = "";
        txtNomeAluno.Text = "";
        txtEmailAluno.Text = "";
        txtSenhaAluno.Text = "";

        AtivarAbaAlunos();
        CarregarAlunos();
        MostrarMensagem("Aluno cadastrado com sucesso.");
    }

    protected void gvAlunos_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "ExcluirAluno")
        {
            string ra = e.CommandArgument.ToString();
            string strConexao = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            SqlConnection conn = new SqlConnection(strConexao);
            conn.Open();

            SqlCommand cmdAg = new SqlCommand("DELETE FROM tb_agendamentos WHERE ra = @ra", conn);
            cmdAg.Parameters.AddWithValue("@ra", ra);
            cmdAg.ExecuteNonQuery();

            SqlCommand cmdAluno = new SqlCommand("DELETE FROM tb_alunos WHERE ra = @ra", conn);
            cmdAluno.Parameters.AddWithValue("@ra", ra);
            cmdAluno.ExecuteNonQuery();

            conn.Close();

            AtivarAbaAlunos();
            CarregarAlunos();
            CarregarTodosAgendamentos();
            MostrarMensagem("Aluno excluído.");
        }
    }

    private void CarregarMonitores()
    {
        string strConexao = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        SqlConnection conn = new SqlConnection(strConexao);
        SqlCommand cmd = new SqlCommand(@"
            SELECT m.rm, m.nome, m.email, d.nome_disciplina, m.periodo
            FROM tb_monitores m
            INNER JOIN tb_disciplinas d ON m.id_disciplina = d.id_disciplina
            ORDER BY m.nome", conn);

        conn.Open();
        SqlDataAdapter da = new SqlDataAdapter(cmd);
        DataTable dt = new DataTable();
        da.Fill(dt);
        conn.Close();

        gvMonitores.DataSource = dt;
        gvMonitores.DataBind();
    }

    protected void btnCadastrarMonitor_Click(object sender, EventArgs e)
    {
        AtivarAbaMonitores();
        AtualizarCamposHorarioPorDia();

        if (string.IsNullOrWhiteSpace(txtRmMonitor.Text) ||
            string.IsNullOrWhiteSpace(txtNomeMonitor.Text) ||
            string.IsNullOrWhiteSpace(txtEmailMonitor.Text) ||
            string.IsNullOrWhiteSpace(txtSenhaMonitor.Text) ||
            ddlDisciplinaMonitor.SelectedValue == "0" ||
            ddlPeriodoMonitor.SelectedValue == "0")
        {
            MostrarMensagem("Preencha RM, nome, e-mail, senha, disciplina e período de monitoria.");
            return;
        }

        List<string[]> disponibilidadesSelecionadas = new List<string[]>();

        foreach (ListItem item in cblDiasMonitor.Items)
        {
            if (item.Selected)
            {
                string dia = item.Value;
                string intervalo;
                string erro;

                if (!ObterIntervaloHorarioDoDia(dia, out intervalo, out erro))
                {
                    MostrarMensagem(erro);
                    return;
                }

                disponibilidadesSelecionadas.Add(new string[] { dia, intervalo });
            }
        }

        if (disponibilidadesSelecionadas.Count == 0)
        {
            MostrarMensagem("Selecione pelo menos um dia da semana para a monitoria.");
            return;
        }

        string[] partesPeriodo = ddlPeriodoMonitor.SelectedValue.Split('|');
        string nomePeriodo = partesPeriodo[0];
        DateTime dataInicio = DateTime.Parse(partesPeriodo[1]);
        DateTime dataFim = DateTime.Parse(partesPeriodo[2]);

        string strConexao = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        SqlConnection conn = new SqlConnection(strConexao);
        conn.Open();

        SqlCommand cmdMon = new SqlCommand();
        cmdMon.Connection = conn;
        cmdMon.CommandText = @"
            INSERT INTO tb_monitores (rm, nome, email, senha, id_disciplina, periodo, data_inicio_periodo, data_fim_periodo)
            VALUES (@rm, @nome, @email, @senha, @id_disciplina, @periodo, @data_inicio, @data_fim)";
        cmdMon.Parameters.AddWithValue("@rm", txtRmMonitor.Text.Trim());
        cmdMon.Parameters.AddWithValue("@nome", txtNomeMonitor.Text.Trim());
        cmdMon.Parameters.AddWithValue("@email", txtEmailMonitor.Text.Trim());
        cmdMon.Parameters.AddWithValue("@senha", txtSenhaMonitor.Text.Trim());
        cmdMon.Parameters.AddWithValue("@id_disciplina", ddlDisciplinaMonitor.SelectedValue);
        cmdMon.Parameters.AddWithValue("@periodo", nomePeriodo);
        cmdMon.Parameters.AddWithValue("@data_inicio", dataInicio.Date);
        cmdMon.Parameters.AddWithValue("@data_fim", dataFim.Date);
        cmdMon.ExecuteNonQuery();

        foreach (string[] disp in disponibilidadesSelecionadas)
        {
            SqlCommand cmdDisp = new SqlCommand();
            cmdDisp.Connection = conn;
            cmdDisp.CommandText = "INSERT INTO tb_disponibilidades (rm, dia_semana, horario) VALUES (@rm, @dia_semana, @horario)";
            cmdDisp.Parameters.AddWithValue("@rm", txtRmMonitor.Text.Trim());
            cmdDisp.Parameters.AddWithValue("@dia_semana", disp[0]);
            cmdDisp.Parameters.AddWithValue("@horario", disp[1]);
            cmdDisp.ExecuteNonQuery();
        }

        conn.Close();

        LimparCamposCadastroMonitor();
        CarregarMonitores();
        MostrarMensagem("Monitor e disponibilidades cadastrados com sucesso.");
    }

    protected void gvMonitores_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "ExcluirMonitor")
        {
            string rm = e.CommandArgument.ToString();
            string strConexao = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            SqlConnection conn = new SqlConnection(strConexao);
            conn.Open();

            SqlCommand cmdAg = new SqlCommand("DELETE FROM tb_agendamentos WHERE rm = @rm", conn);
            cmdAg.Parameters.AddWithValue("@rm", rm);
            cmdAg.ExecuteNonQuery();

            SqlCommand cmdDisp = new SqlCommand("DELETE FROM tb_disponibilidades WHERE rm = @rm", conn);
            cmdDisp.Parameters.AddWithValue("@rm", rm);
            cmdDisp.ExecuteNonQuery();

            SqlCommand cmdMon = new SqlCommand("DELETE FROM tb_monitores WHERE rm = @rm", conn);
            cmdMon.Parameters.AddWithValue("@rm", rm);
            cmdMon.ExecuteNonQuery();

            conn.Close();

            AtivarAbaMonitores();
            CarregarMonitores();
            CarregarTodosAgendamentos();
            MostrarMensagem("Monitor excluído.");
        }
    }

    private void CarregarTodosAgendamentos()
    {
        string strConexao = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        SqlConnection conn = new SqlConnection(strConexao);
        SqlCommand cmd = new SqlCommand(@"
            SELECT
                a.data_agendamento,
                a.dia_semana,
                a.horario,
                a.status,
                d.nome_disciplina,
                m.nome AS nome_monitor,
                al.nome AS nome_aluno
            FROM tb_agendamentos a
            INNER JOIN tb_disciplinas d ON a.id_disciplina = d.id_disciplina
            INNER JOIN tb_monitores m ON a.rm = m.rm
            INNER JOIN tb_alunos al ON a.ra = al.ra
            ORDER BY a.data_agendamento DESC, a.id_agendamento DESC", conn);

        conn.Open();
        SqlDataAdapter da = new SqlDataAdapter(cmd);
        DataTable dt = new DataTable();
        da.Fill(dt);
        conn.Close();

        gvTodosAgendamentos.DataSource = dt;
        gvTodosAgendamentos.DataBind();
    }

    private void MostrarMensagem(string texto)
    {
        lblMensagem.Visible = true;
        lblMensagem.Text = texto;
    }
}