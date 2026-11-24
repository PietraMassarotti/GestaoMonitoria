<%@ Page Title="" Language="C#" MasterPageFile="~/paginas/cabecalho/menu.master" AutoEventWireup="true" CodeFile="painelMonitor.aspx.cs" Inherits="painelMonitor" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="cardBox">
        <h1 class="tituloPagina">Minhas monitorias</h1>

        <!-- Lista de monitorias atribuídas ao monitor logado -->
        <asp:GridView ID="gvMonitorias" runat="server" AutoGenerateColumns="false" DataKeyNames="id_agendamento" CssClass="tabelaMonitor" EmptyDataText="Nenhuma monitoria agendada para você no momento." OnRowCommand="gvMonitorias_RowCommand">
            <Columns>
                <asp:BoundField DataField="data_agendamento" HeaderText="Data" DataFormatString="{0:dd/MM/yyyy}" />
                <asp:BoundField DataField="dia_semana" HeaderText="Dia" />
                <asp:BoundField DataField="horario" HeaderText="Horário" />
                <asp:BoundField DataField="nome_disciplina" HeaderText="Disciplina" />
                <asp:BoundField DataField="nome_aluno" HeaderText="Aluno" />
                <asp:BoundField DataField="status" HeaderText="Status" />
                <asp:TemplateField HeaderText="Alterar status">
                    <ItemTemplate>
                        <asp:Button ID="btnAndamento" runat="server" Text="Em andamento" CommandName="MarcarAndamento" CommandArgument='<%# Eval("id_agendamento") %>' CssClass="btnStatusAndamento" CausesValidation="false" />
                        <asp:Button ID="btnFinalizada" runat="server" Text="Finalizada" CommandName="MarcarFinalizada" CommandArgument='<%# Eval("id_agendamento") %>' CssClass="btnStatusFinalizada" CausesValidation="false" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>

        <asp:Label ID="lblMensagem" runat="server" CssClass="msgFeedback" Visible="false"></asp:Label>
    </div>
</asp:Content>

