<%@ Page Title="" Language="C#"MasterPageFile="~/paginas/cabecalho/menu.master" AutoEventWireup="true" CodeFile="painelCoordenador.aspx.cs" Inherits="painelCoordenador" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="cardBox">
        <h1 class="tituloPagina">Painel da Coordenação</h1>

        <div class="abasCoord">
            <asp:Button ID="btnAbaMonitores" runat="server" Text="Monitores" CssClass="btnAbaAtiva" CausesValidation="false" OnClick="btnAbaMonitores_Click" />
            <asp:Button ID="btnAbaAlunos" runat="server" Text="Alunos" CssClass="btnAba" CausesValidation="false" OnClick="btnAbaAlunos_Click" />
            <asp:Button ID="btnAbaAgendamentos" runat="server" Text="Agendamentos" CssClass="btnAba" CausesValidation="false" OnClick="btnAbaAgendamentos_Click" />
        </div>

        <asp:Label ID="lblMensagem" runat="server" CssClass="msgFeedback" Visible="false"></asp:Label>

        <!-- PARTE 1: MONITORES -->
        <asp:Panel ID="pnlMonitores" runat="server" Visible="true">
            <h2 class="tituloSecao">Monitores</h2>

            <div class="formGrid">
                <div class="campoGrupo">
                    <label for="txtRmMonitor">RM</label>
                    <asp:TextBox ID="txtRmMonitor" runat="server" CssClass="inputClaro"></asp:TextBox>
                </div>

                <div class="campoGrupo">
                    <label for="txtNomeMonitor">Nome</label>
                    <asp:TextBox ID="txtNomeMonitor" runat="server" CssClass="inputClaro"></asp:TextBox>
                </div>

                <div class="campoGrupo">
                    <label for="txtEmailMonitor">E-mail</label>
                    <asp:TextBox ID="txtEmailMonitor" runat="server" CssClass="inputClaro" TextMode="Email"></asp:TextBox>
                </div>

                <div class="campoGrupo">
                    <label for="txtSenhaMonitor">Senha</label>
                    <asp:TextBox ID="txtSenhaMonitor" runat="server" CssClass="inputClaro" TextMode="Password"></asp:TextBox>
                </div>

                <div class="campoGrupo">
                    <label for="ddlDisciplinaMonitor">Disciplina</label>
                    <asp:DropDownList ID="ddlDisciplinaMonitor" runat="server" CssClass="controleForm"></asp:DropDownList>
                </div>

                <div class="campoGrupo">
                    <label for="ddlPeriodoMonitor">Período de monitoria</label>
                    <asp:DropDownList ID="ddlPeriodoMonitor" runat="server" CssClass="controleForm">
                        <asp:ListItem Text="Selecione..." Value="0"></asp:ListItem>
                        <asp:ListItem Text="1º Semestre (01/01/2026 a 30/06/2026)" Value="1º Semestre|2026-01-01|2026-06-30"></asp:ListItem>
                        <asp:ListItem Text="2º Semestre (01/07/2026 a 15/12/2026)" Value="2º Semestre|2026-07-01|2026-12-15"></asp:ListItem>
                    </asp:DropDownList>
                </div>

                <div class="campoGrupo" style="grid-column: 1 / -1;">
                    <label>Dias da semana da monitoria</label>
                    <asp:CheckBoxList ID="cblDiasMonitor" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" CssClass="diasLista" AutoPostBack="true" OnSelectedIndexChanged="cblDiasMonitor_SelectedIndexChanged">
                        <asp:ListItem Text="Segunda-feira" Value="Segunda-feira"></asp:ListItem>
                        <asp:ListItem Text="Terça-feira" Value="Terça-feira"></asp:ListItem>
                        <asp:ListItem Text="Quarta-feira" Value="Quarta-feira"></asp:ListItem>
                        <asp:ListItem Text="Quinta-feira" Value="Quinta-feira"></asp:ListItem>
                        <asp:ListItem Text="Sexta-feira" Value="Sexta-feira"></asp:ListItem>
                        <asp:ListItem Text="Sábado" Value="Sábado"></asp:ListItem>
                    </asp:CheckBoxList>
                </div>

                <!-- Hora início e hora fim por dia selecionado (7h às 21h) -->
                <asp:Panel ID="pnlHorariosPorDia" runat="server" Visible="false" CssClass="horarios-por-dia-box">
                    <span class="horariosDiaTitulo">Defina a hora de início e de fim para cada dia selecionado (7h às 21h)</span>

                    <div id="divHorSegunda" runat="server" visible="false" class="linhaHorarioDia">
                        <span class="rotuloDiaHorario">Segunda-feira:</span>
                        <asp:DropDownList ID="ddlHoraInicioSegunda" runat="server" CssClass="controleForm horaCurta"></asp:DropDownList>
                        <span>às</span>
                        <asp:DropDownList ID="ddlHoraFimSegunda" runat="server" CssClass="controleForm horaCurta"></asp:DropDownList>
                    </div>

                    <div id="divHorTerca" runat="server" visible="false" class="linhaHorarioDia">
                        <span class="rotuloDiaHorario">Terça-feira:</span>
                        <asp:DropDownList ID="ddlHoraInicioTerca" runat="server" CssClass="controleform horaCurta"></asp:DropDownList>
                        <span>às</span>
                        <asp:DropDownList ID="ddlHoraFimTerca" runat="server" CssClass="controleForm horaCurta"></asp:DropDownList>
                    </div>

                    <div id="divHorQuarta" runat="server" visible="false" class="linhaHorarioDia">
                        <span class="rotuloDiaHorario">Quarta-feira:</span>
                        <asp:DropDownList ID="ddlHoraInicioQuarta" runat="server" CssClass="controleForm horaCurta"></asp:DropDownList>
                        <span>às</span>
                        <asp:DropDownList ID="ddlHoraFimQuarta" runat="server" CssClass="controleForm horaCurta"></asp:DropDownList>
                    </div>

                    <div id="divHorQuinta" runat="server" visible="false" class="linhaHorarioDia">
                        <span class="rotuloDiaHorario">Quinta-feira:</span>
                        <asp:DropDownList ID="ddlHoraInicioQuinta" runat="server" CssClass="controleForm horaCurta"></asp:DropDownList>
                        <span>às</span>
                        <asp:DropDownList ID="ddlHoraFimQuinta" runat="server" CssClass="controleForm horaCurta"></asp:DropDownList>
                    </div>

                    <div id="divHorSexta" runat="server" visible="false" class="linhaHorarioDia">
                        <span class="rotuloDiaHorario">Sexta-feira:</span>
                        <asp:DropDownList ID="ddlHoraInicioSexta" runat="server" CssClass="controleForm horaCurta"></asp:DropDownList>
                        <span>às</span>
                        <asp:DropDownList ID="ddlHoraFimSexta" runat="server" CssClass="controleForm horaCurta"></asp:DropDownList>
                    </div>

                    <div id="divHorSabado" runat="server" visible="false" class="linhaHorarioDia">
                        <span class="rotuloDiaHorario">Sábado:</span>
                        <asp:DropDownList ID="ddlHoraInicioSabado" runat="server" CssClass="controleForm horaCurta"></asp:DropDownList>
                        <span>às</span>
                        <asp:DropDownList ID="ddlHoraFimSabado" runat="server" CssClass="controleForm horaCurta"></asp:DropDownList>
                    </div>
                </asp:Panel>
            </div>

            <div class="area-botao">
                <asp:Button ID="btnCadastrarMonitor" runat="server" Text="Cadastrar Monitor" CssClass="btnPrimario" CausesValidation="false" OnClick="btnCadastrarMonitor_Click" />
            </div>

            <asp:GridView ID="gvMonitores" runat="server" AutoGenerateColumns="false" DataKeyNames="rm" CssClass="tabelaCoord" EmptyDataText="Nenhum monitor cadastrado." OnRowCommand="gvMonitores_RowCommand">
                <Columns>
                    <asp:BoundField DataField="rm" HeaderText="RM" />
                    <asp:BoundField DataField="nome" HeaderText="Nome" />
                    <asp:BoundField DataField="email" HeaderText="E-mail" />
                    <asp:BoundField DataField="nome_disciplina" HeaderText="Disciplina" />
                    <asp:BoundField DataField="periodo" HeaderText="Período" />
                    <asp:TemplateField HeaderText="Ação">
                        <ItemTemplate>
                            <asp:Button ID="btnExcluirMonitor" runat="server" Text="Excluir" CommandName="ExcluirMonitor" CommandArgument='<%# Eval("rm") %>' CssClass="btnExcluir" CausesValidation="false" OnClientClick="return confirm('Deseja excluir este monitor?');" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </asp:Panel>

        <!-- PARTE 2: ALUNOS -->
        <asp:Panel ID="pnlAlunos" runat="server" Visible="false">
            <h2 class="tituloSecao">Alunos</h2>

            <div class="formGrid">
                <div class="campoGrupo">
                    <label for="txtRaAluno">RA</label>
                    <asp:TextBox ID="txtRaAluno" runat="server" CssClass="inputClaro"></asp:TextBox>
                </div>

                <div class="campoGrupo">
                    <label for="txtNomeAluno">Nome</label>
                    <asp:TextBox ID="txtNomeAluno" runat="server" CssClass="inputClaro"></asp:TextBox>
                </div>

                <div class="campoGrupo">
                    <label for="txtEmailAluno">E-mail</label>
                    <asp:TextBox ID="txtEmailAluno" runat="server" CssClass="inputClaro" TextMode="Email"></asp:TextBox>
                </div>

                <div class="campoGrupo">
                    <label for="txtSenhaAluno">Senha</label>
                    <asp:TextBox ID="txtSenhaAluno" runat="server" CssClass="inputClaro" TextMode="Password"></asp:TextBox>
                </div>
            </div>

            <div class="areaBotao">
                <asp:Button ID="btnCadastrarAluno" runat="server" Text="Cadastrar Aluno" CssClass="btnPrimario" CausesValidation="false" OnClick="btnCadastrarAluno_Click" />
            </div>

            <asp:GridView ID="gvAlunos" runat="server" AutoGenerateColumns="false" DataKeyNames="ra" CssClass="tabelaCoord" EmptyDataText="Nenhum aluno cadastrado." OnRowCommand="gvAlunos_RowCommand">
                <Columns>
                    <asp:BoundField DataField="ra" HeaderText="RA" />
                    <asp:BoundField DataField="nome" HeaderText="Nome" />
                    <asp:BoundField DataField="email" HeaderText="E-mail" />
                    <asp:TemplateField HeaderText="Ação">
                        <ItemTemplate>
                            <asp:Button ID="btnExcluirAluno" runat="server" Text="Excluir" CommandName="ExcluirAluno" CommandArgument='<%# Eval("ra") %>' CssClass="btnExcluir" CausesValidation="false" OnClientClick="return confirm('Deseja excluir este aluno?');" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </asp:Panel>

        <!-- PARTE 3: AGENDAMENTOS -->
        <asp:Panel ID="pnlAgendamentos" runat="server" Visible="false">
            <h2 class="tituloSecao">Agendamentos</h2>

            <asp:GridView ID="gvTodosAgendamentos" runat="server" AutoGenerateColumns="false" CssClass="tabelaCoord" EmptyDataText="Nenhum agendamento registrado.">
                <Columns>
                    <asp:BoundField DataField="data_agendamento" HeaderText="Data" DataFormatString="{0:dd/MM/yyyy}" />
                    <asp:BoundField DataField="dia_semana" HeaderText="Dia" />
                    <asp:BoundField DataField="horario" HeaderText="Horário" />
                    <asp:BoundField DataField="nome_disciplina" HeaderText="Disciplina" />
                    <asp:BoundField DataField="nome_monitor" HeaderText="Monitor" />
                    <asp:BoundField DataField="nome_aluno" HeaderText="Aluno" />
                    <asp:BoundField DataField="status" HeaderText="Status" />
                </Columns>
            </asp:GridView>
        </asp:Panel>
    </div>
</asp:Content>