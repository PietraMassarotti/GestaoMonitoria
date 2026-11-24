<%@ Page Title="" Language="C#" MasterPageFile="~/paginas/cabecalho/menu.master" AutoEventWireup="true" CodeFile="confirmacao.aspx.cs" Inherits="confirmar" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="cardBox confirmacaoCard">
        <h1 class="tituloPagina">Agendamento Confirmado</h1>

        <asp:Label ID="lblConfirmacao" runat="server" CssClass="msgConfirmacao"></asp:Label>

        <div class="acoesConfirmacao">
            <asp:HyperLink ID="lnkMeusAgendamentos" runat="server" NavigateUrl="~/paginas/meusAgendamentos.aspx" CssClass="btnPrimario" Style="text-decoration: none;">
                Ver meus agendamentos
            </asp:HyperLink>
            <asp:HyperLink ID="lnkNovoAgendamento" runat="server" NavigateUrl="~/paginas/agendar.aspx" CssClass="linkSecundario">
                Agendar outra
            </asp:HyperLink>
        </div>
    </div>
</asp:Content>

