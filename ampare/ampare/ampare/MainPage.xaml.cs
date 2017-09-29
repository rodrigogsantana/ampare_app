using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Net.Http;
using System.Collections.Generic;
using Newtonsoft.Json;
using Plugin.Connectivity.Abstractions;
using Plugin.Connectivity;

namespace ampare
{
    public partial class MainPage : ContentPage
    {
        private List<associado> listaAssociado = new List<associado>();
        string statusConexao = "conectado";

        public MainPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }




        void cpfMask(object sender, EventArgs e)
        {
            var ev = e as TextChangedEventArgs;

            if (ev.NewTextValue != ev.OldTextValue)
            {
                var entry = (Entry)sender;
                string text = Regex.Replace(ev.NewTextValue, @"[^0-9]", "");

                text = text.PadRight(11);

                // removendo todos os digitos excedentes 
                if (text.Length > 11)
                {
                    text = text.Remove(11);
                }

                text = text.Insert(3, ".").Insert(7, ".").Insert(11, "-").TrimEnd(new char[] { ' ', '.', '-' });
                if (entry.Text != text)
                    entry.Text = text;

            }
        }

        public async Task VerificaStatusAssociado()
        {
            try
            {
                if(string.IsNullOrWhiteSpace(txtcpf.Text))
                {
                    await DisplayAlert("Atenção!", "Digite o CPF do associado títular para pesquisar!", "Ok");
                }
                else { 
                var uri = new Uri("https://ampare.000webhostapp.com/consta_situacao_associado.php?cpf=" + txtcpf.Text.Trim());

                lblMensagem.IsVisible = true;
                lblMensagem.Text = "Aguarde, consultando...";

                using (HttpClient myClient = new HttpClient())
                {
                    var response = await myClient.GetAsync(uri);
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();

                        this.listaAssociado = JsonConvert.DeserializeObject<List<associado>>(content);

                        if (this.listaAssociado.Count > 0)
                        {
                            foreach (var item in this.listaAssociado)
                            {
                                if (item.status == "1")
                                {
                                    lblMensagem.Text = item.nome.ToUpper() + " ESTÁ ATIVO!";
                                    lblMensagem.TextColor = Color.Green;
                                }
                                else
                                {
                                    lblMensagem.Text = item.nome.ToUpper() + " ESTÁ INATIVO!";
                                    lblMensagem.TextColor = Color.Red;
                                }
                            }
                        }
                        else
                        {
                            await DisplayAlert("Atenção!", "Este CPF não está cadastrado!", "Ok");
                            lblMensagem.IsVisible = false;
                        }

                    }

                }
                     }

            }
            catch (Exception erro)
            {
                await DisplayAlert("Atenção!", "Não foi possível consultar. Detalhes: " + erro.Message, "Ok");
            }
        }

        private async void buttonConsultar_Clicked(object sender, EventArgs e)
        {
            if (CrossConnectivity.Current.IsConnected)
            {
                lblMensagem.TextColor = Color.Black;
                await VerificaStatusAssociado();
            } else
            {
                await DisplayAlert("Atenção!", "Você precisar estar conectado a internet. Verifique sua conexão!", "Ok");
            }

        }


        // private void Current_ConnectivityChanged(object sender, Plugin.Connectivity.Abstractions.ConnectivityChangedEventArgs e)
        //{
        //    if (e.IsConnected == true)
        //        this.statusConexao = "conectado";
        //    else
        //        this.statusConexao = "off";
                
        //}

        private void buttonCancel_Clicked(object sender, EventArgs e)
        {
            txtcpf.Text = "";
            txtcpf.Focus();
        }
    }
}
