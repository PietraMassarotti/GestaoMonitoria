using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class home : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            CarregarMural();
        }
    }

    private void CarregarMural()
    {
        string strConexao = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        SqlConnection conn = new SqlConnection(strConexao);
        SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = @"SELECT a.data_agendamento, a.dia_semana, a.horario, d.nome_disciplina, m.nome AS nome_monitor, al.nome AS nome_aluno, a.status FROM tb_agendamentos a INNER JOIN tb_disciplinas d ON a.id_disciplina = d.id_disciplina INNER JOIN tb_monitores m ON a.rm = m.rm INNER JOIN tb_alunos al ON a.ra = al.ra ORDER BY a.data_agendamento DESC, a.id_agendamento DESC;";
        conn.Open();
        SqlDataAdapter da = new SqlDataAdapter(cmd);
        DataTable dt = new DataTable();
        da.Fill(dt);
        conn.Close();

        gvMural.DataSource = dt;
        gvMural.DataBind();
    }
}