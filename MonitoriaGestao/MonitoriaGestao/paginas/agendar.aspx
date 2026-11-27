<%@ Page Title="" Language="C#" MasterPageFile="~/paginas/cabecalho/menu.master" AutoEventWireup="true" CodeFile="agendar.aspx.cs" Inherits="agendar" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="cardBox agendarCard">
        <h1 class="tituloPagina">Agendar monitoria</h1>

        <div class="formGrid">
            <div class="campoGrupo">
                <label for="ddlDisciplina">Disciplina</label>
                <asp:DropDownList ID="ddlDisciplina" runat="server" CssClass="controleForm" AutoPostBack="true" OnSelectedIndexChanged="ddlDisciplina_SelectedIndexChanged"></asp:DropDownList>
            </div>

            <div class="campoGrupo">
                <label for="ddlMonitor">Monitor</label>
                <asp:DropDownList ID="ddlMonitor" runat="server" CssClass="controleForm" AutoPostBack="true" OnSelectedIndexChanged="ddlMonitor_SelectedIndexChanged"></asp:DropDownList>
            </div>

            <div class="campoGrupo" style="grid-column: 1 / -1;">
                <label for="ddlDisponibilidade">Dia, data e horário disponíveis na semana</label>
                <asp:DropDownList ID="ddlDisponibilidade" runat="server" CssClass="controleForm"></asp:DropDownList>
            </div>
        </div>

        <div class="areaBotao">
            <asp:Button ID="btnAgendar" runat="server" Text="Agendar" CssClass="btnPrimario" OnClick="btnAgendar_Click" />
        </div>

        <asp:Label ID="lblMensagem" runat="server" CssClass="msgFeedback" Visible="false"></asp:Label>
    </div>
</asp:Content>