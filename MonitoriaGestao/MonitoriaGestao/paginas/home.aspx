<%@ Page Title="" Language="C#" MasterPageFile="~/paginas/cabecalho/menu.master" AutoEventWireup="true" CodeFile="home.aspx.cs" Inherits="home" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="cardBox">
        <div class="topoMural">
            <div>
                <h1 class="tituloPagina" style="text-align: center; margin-bottom: 4px;">Mural de Monitorias</h1>
            </div>
        </div>

        <asp:Label ID="lblMensagem" runat="server" CssClass="azul" Visible="false"></asp:Label>

        <asp:GridView ID="gvMural" runat="server" AutoGenerateColumns="false" CssClass="tabelaMural" EmptyDataText="Nenhuma monitoria agendada até o momento." EmptyDataRowStyle-CssClass="vazioMural">
            <columns>
                <asp:BoundField DataField="data_agendamento" HeaderText="Data" DataFormatString="{0:dd/MM/yyyy}" />
                <asp:BoundField DataField="dia_semana" HeaderText="Dia" />
                <asp:BoundField DataField="horario" HeaderText="Horário" />
                <asp:BoundField DataField="nome_disciplina" HeaderText="Disciplina" />
                <asp:BoundField DataField="nome_monitor" HeaderText="Monitor" />
                <asp:BoundField DataField="nome_aluno" HeaderText="Aluno" />
                <asp:BoundField DataField="status" HeaderText="Status" />
            </columns>
        </asp:GridView>
    </div>
</asp:Content>

