<%@ Page Title="" Language="C#" MasterPageFile="~/paginas/cabecalho/menu.master" AutoEventWireup="true" CodeFile="minhaConta.aspx.cs" Inherits="minhaConta" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="cardBox contaCard">
        <h1 class="tituloPagina">Minha conta</h1>

        <div class="formGrid">
            <div class="campoGrupo">
                <label>Perfil</label>
                <asp:TextBox ID="txtPerfil" runat="server" CssClass="inputBloqueado" ReadOnly="true"></asp:TextBox>
            </div>

            <div class="campoGrupo">
                <label>
                    <asp:Label ID="lblTipoId" runat="server" Text="Identificador"></asp:Label></label>
                <asp:TextBox ID="txtIdentificador" runat="server" CssClass="inputBloqueado" ReadOnly="true"></asp:TextBox>
            </div>

            <div class="campoGrupo">
                <label>Nome</label>
                <asp:TextBox ID="txtNome" runat="server" CssClass="inputBloqueado" ReadOnly="true"></asp:TextBox>
            </div>

            <div class="campoGrupo">
                <label>E-mail</label>
                <asp:TextBox ID="txtEmail" runat="server" CssClass="inputBloqueado" ReadOnly="true"></asp:TextBox>
            </div>
        </div>

        <hr class="divisorConta" />

        <h2 class="subtitulo-conta">Alterar senha</h2>

        <div class="formGrid">
            <div class="campoGrupo">
                <label for="txtSenhaAtual">Senha atual</label>
                <asp:TextBox ID="txtSenhaAtual" runat="server" CssClass="inputClaro" TextMode="Password"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvSenhaAtual" runat="server" ControlToValidate="txtSenhaAtual" ErrorMessage="Informe a senha atual." CssClass="azul" Display="Dynamic"></asp:RequiredFieldValidator>
            </div>

            <div class="campoGrupo">
                <label for="txtNovaSenha">Nova senha</label>
                <asp:TextBox ID="txtNovaSenha" runat="server" CssClass="inputClaro" TextMode="Password"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvNovaSenha" runat="server" ControlToValidate="txtNovaSenha" ErrorMessage="Informe a nova senha." CssClass="azul" Display="Dynamic"></asp:RequiredFieldValidator>
            </div>
        </div>

        <div class="areaBotao">
            <asp:Button ID="btnAlterarSenha" runat="server" Text="Salvar nova senha" CssClass="btnPrimario" OnClick="btnAlterarSenha_Click" />
        </div>

        <asp:Label ID="lblMensagem" runat="server" CssClass="msgFeedback" Visible="false"></asp:Label>
    </div>
</asp:Content>

