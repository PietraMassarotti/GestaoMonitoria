using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class painelMonitor : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["PerfilUsuario"] == null || Session["PerfilUsuario"].ToString() != "Monitor")
        {
            Response.Redirect("~/paginas/login.aspx");
            return;
        }

        if (!IsPostBack)
        {
            CarregarMonitorias();
        }
    }

    private void CarregarMonitorias()
    {
        string strConexao = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        SqlConnection conn = new SqlConnection(strConexao);
        SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = @"
            SELECT
                a.id_agendamento,
                a.data_agendamento,
                a.dia_semana,
                a.horario,
                a.status,
                d.nome_disciplina,
                al.nome AS nome_aluno
            FROM tb_agendamentos a
            INNER JOIN tb_disciplinas d ON a.id_disciplina = d.id_disciplina
            INNER JOIN tb_alunos al ON a.ra = al.ra
            WHERE a.rm = @rm
            ORDER BY a.data_agendamento DESC, a.id_agendamento DESC";
        cmd.Parameters.AddWithValue("@rm", Session["RM"].ToString());

        conn.Open();
        SqlDataAdapter da = new SqlDataAdapter(cmd);
        DataTable dt = new DataTable();
        da.Fill(dt);
        conn.Close();

        gvMonitorias.DataSource = dt;
        gvMonitorias.DataBind();
    }

    // Atualiza o status para "Em andamento" ou "Finalizada"
    protected void gvMonitorias_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        int idAgendamento = Convert.ToInt32(e.CommandArgument);
        string novoStatus = "";

        if (e.CommandName == "MarcarAndamento")
        {
            novoStatus = "Em andamento";
        }
        else if (e.CommandName == "MarcarFinalizada")
        {
            novoStatus = "Finalizada";
        }
        else
        {
            return;
        }

        string strConexao = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        SqlConnection conn = new SqlConnection(strConexao);
        SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = @"
            UPDATE tb_agendamentos
            SET status = @status
            WHERE id_agendamento = @id AND rm = @rm";
        cmd.Parameters.AddWithValue("@status", novoStatus);
        cmd.Parameters.AddWithValue("@id", idAgendamento);
        cmd.Parameters.AddWithValue("@rm", Session["RM"].ToString());

        conn.Open();
        cmd.ExecuteNonQuery();
        conn.Close();

        lblMensagem.Visible = true;
        lblMensagem.Text = "Status atualizado para: " + novoStatus + ".";
        CarregarMonitorias();
    }
}