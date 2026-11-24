<%@ Page Title="" Language="C#" MasterPageFile="~/paginas/cabecalho/menu.master" AutoEventWireup="true" CodeFile="meusAgendamentos.aspx.cs" Inherits="meusAgendamentos" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="cardBox">
        <h1 class="tituloPagina">Meus agendamentos</h1>

        <asp:GridView ID="gvMeusAgendamentos" runat="server" AutoGenerateColumns="false" DataKeyNames="id_agendamento" CssClass="tabelaLista" EmptyDataText="Você ainda não possui monitorias agendadas." OnRowCommand="gvMeusAgendamentos_RowCommand">
            <Columns>
                <asp:BoundField DataField="data_agendamento" HeaderText="Data" DataFormatString="{0:dd/MM/yyyy}" />
                <asp:BoundField DataField="dia_semana" HeaderText="Dia" />
                <asp:BoundField DataField="horario" HeaderText="Horário" />
                <asp:BoundField DataField="nome_disciplina" HeaderText="Disciplina" />
                <asp:BoundField DataField="nome_monitor" HeaderText="Monitor" />
                <asp:BoundField DataField="status" HeaderText="Status" />
                <asp:TemplateField HeaderText="Ações">
                    <ItemTemplate>
                        <asp:Button ID="btnEditar" runat="server" Text="Editar" CommandName="EditarAgendamento" CommandArgument='<%# Eval("id_agendamento") %>' CssClass="btnEditar" CausesValidation="false" />
                        <asp:Button ID="btnExcluir" runat="server" Text="Cancelar" CommandName="ExcluirAgendamento" CommandArgument='<%# Eval("id_agendamento") %>' CssClass="btnExcluir" CausesValidation="false" OnClientClick="return confirm('Deseja realmente cancelar este agendamento?');" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>

        <asp:Panel ID="pnlEdicao" runat="server" Visible="false" CssClass="painelEdicao">
            <h2 class="subtituloSecao">Alterar agendamento</h2>
            <asp:HiddenField ID="hfIdAgendamento" runat="server" />

            <div class="formGrid">
                <div class="campoGrupo">
                    <label for="ddlEdicaoDisciplina">Disciplina</label>
                    <asp:DropDownList ID="ddlEdicaoDisciplina" runat="server" CssClass="controleForm" AutoPostBack="true" OnSelectedIndexChanged="ddlEdicaoDisciplina_SelectedIndexChanged"></asp:DropDownList>
                </div>

                <div class="campoGrupo">
                    <label for="ddlEdicaoMonitor">Monitor</label>
                    <asp:DropDownList ID="ddlEdicaoMonitor" runat="server" CssClass="controleForm" AutoPostBack="true" OnSelectedIndexChanged="ddlEdicaoMonitor_SelectedIndexChanged"></asp:DropDownList>
                </div>

                <div class="campoGrupo" style="grid-column: 1 / -1;">
                    <label for="ddlNovaDisponibilidade">Dia, data e horário disponíveis na semana</label>
                    <asp:DropDownList ID="ddlNovaDisponibilidade" runat="server" CssClass="controleForm"></asp:DropDownList>
                </div>
            </div>

            <div class="areaBotao">
                <asp:Button ID="btnSalvarEdicao" runat="server" Text="Salvar alteração" CssClass="btnPrimario" CausesValidation="false" OnClick="btnSalvarEdicao_Click" />
                <asp:Button ID="btnCancelarEdicao" runat="server" Text="Fechar" CssClass="btnExcluir" CausesValidation="false" OnClick="btnCancelarEdicao_Click" />
            </div>
        </asp:Panel>

        <asp:Label ID="lblMensagem" runat="server" CssClass="msgFeedback" Visible="false"></asp:Label>
    </div>
</asp:Content>