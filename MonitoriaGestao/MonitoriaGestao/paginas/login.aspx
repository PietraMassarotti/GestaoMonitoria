<%@ Page Title="" Language="C#" MasterPageFile="~/paginas/cabecalho/menu.master" AutoEventWireup="true" CodeFile="login.aspx.cs" Inherits="login" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="cardBox loginCard">
        <h1 class="tituloPagina">Entrar</h1>

        <div class="formLogin">
            <div class="campoLogin">
                <label for="ddlPerfil">Entrar como</label>
                <asp:DropDownList ID="ddlPerfil" runat="server" CssClass="escolhaUsuario">
                    <asp:ListItem Text="Selecione..." Value="0"></asp:ListItem>
                    <asp:ListItem Text="Aluno" Value="Aluno"></asp:ListItem>
                    <asp:ListItem Text="Monitor" Value="Monitor"></asp:ListItem>
                    <asp:ListItem Text="Coordenador" Value="Coordenador"></asp:ListItem>
                </asp:DropDownList>
                
            </div>

            <div class="campoLogin">
                <label for="txtEmail">E-mail</label>
                <asp:TextBox ID="txtEmail" runat="server" CssClass="inputLogin" TextMode="Email"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvEmail" runat="server" ControlToValidate="txtEmail" ErrorMessage="Informe o e-mail." CssClass="azul" Display="Dynamic"></asp:RequiredFieldValidator>
            </div>

            <div class="campoLogin">
                <label for="txtSenha">Senha</label>
                <asp:TextBox ID="txtSenha" runat="server" CssClass="inputLogin" TextMode="Password"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvSenha" runat="server" ControlToValidate="txtSenha" ErrorMessage="Informe a senha." CssClass="azul" Display="Dynamic"></asp:RequiredFieldValidator>
            </div>

            <div class="areaBotao">
                <asp:Button ID="btnEntrar" runat="server" Text="Entrar" CssClass="btnPrimario" OnClick="btnEntrar_Click" />
            </div>

            <asp:Label ID="lblMensagem" runat="server" CssClass="msgErro" Visible="false"></asp:Label>
        </div>
    </div>
</asp:Content>