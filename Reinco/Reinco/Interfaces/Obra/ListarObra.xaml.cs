﻿using Reinco.Entidades;
using Reinco.Recursos;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Reinco.Interfaces.Obra
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ListarObra : ContentPage, INotifyPropertyChanged
    {
        string Color;


        #region +---- Services ----+
        HttpClient Cliente = new HttpClient();
        WebService Servicio = new WebService(); 
        #endregion

        #region +---- Eventos ----+
        new public event PropertyChangedEventHandler PropertyChanged; 
        #endregion

        #region +---- Atributos ----+
        public VentanaMensaje mensaje;
        string Mensaje;
        private bool isRefreshingObra { get; set; }
        #endregion

        #region +---- Propiedades ----+
        public ObservableCollection<ObraItem> ObraItems { get; set; }
        public bool IsRefreshingObra
        {
            set
            {
                if (isRefreshingObra != value)
                {
                    isRefreshingObra = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsRefreshingObra"));
                }
            }
            get
            {
                return isRefreshingObra;
            }
        }
        #endregion

        #region +---- Comandos ----+
        public ICommand CrearObra { get; private set; }
        public ICommand RefreshObraCommand { get; private set; }
        #endregion

        #region ==============Definiendo Propiedad Global De esta Pagina================
        protected override void OnAppearing()
        {
            base.OnAppearing();
            App.ListarObra = this;
        }
        #endregion

        #region========== Constructor sin parametros=============
        public ListarObra()
        {
            InitializeComponent();

            ObraItems = new ObservableCollection<ObraItem>();
            CargarObraItems();


            #region +---- Preparando Los Comandos ----+
            // Evento Crear Obra
            CrearObra = new Command(() =>
             {
                 Navigation.PushAsync(new AgregarObra());
             });

            // Evento Refrescar La Lista
            RefreshObraCommand = new Command( () =>
            {
                ObraItems.Clear();
                CargarObraItems();
                IsRefreshingObra = false;
            }); 
            #endregion

            //this.BindingContext = this; // Contexto de los Bindings Clase Actual Importante para que pueda funcionar el refresco de la lista con Gestos
        }
        #endregion 
        public ListarObra(int idPropietarioObra, int idObra)
        {
            eliminar(idPropietarioObra, idObra);
        }
        

        #region ============== Cargando las obras =============
        public async void CargarObraItems()
        {
            try
            {
                //servicioObra, mostrarObras--modificado
                dynamic obras = await Servicio.MetodoGet("ServicioPropietarioObra.asmx", "MostrarPropietarioObraDetalle");
                foreach (var item in obras)
                {
                    if (item.idPropietario == null || item.idUsuario_responsable == null)
                        Color = "#FF7777";
                    else
                        Color = "#77FF77";
                    ObraItems.Add(new ObraItem
                    {
                        idObra = item.idObra,
                        nombre = item.nombre,
                        codigo = item.codigo,
                        idPropietario =item.idPropietario==null?0: item.idPropietario,
                        idUsuario = item.idUsuario_responsable==null?0: item.idUsuario_responsable,
                        colorObra = Color,
                        idPropietarioObra=item.idPropietario_Obra == null ? 0 : item.idPropietario_Obra
                    });
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "Aceptar");
            }
        }
        #endregion

        #region ================= Evento Eliminar Obra=====================
        public async void eliminar(int idPropietarioObra, int idObra)
        {
            try { 
                bool respuesta= await DisplayAlert("Eliminar", "Eliminar idObra = " + idObra, "Aceptar","Cancelar");
                object[,] variables = new object[,] { { "idPropietarioObra", idPropietarioObra }, { "idObra", idObra } };
                dynamic result = await Servicio.MetodoGetString("ServicioPropietarioObra.asmx", "EliminarPropietarioObra", variables);
                Mensaje = Convert.ToString(result);
                if (result!=null)
                {
                    await App.Current.MainPage.DisplayAlert("Eliminar Obra", Mensaje, "OK");

                    // Recargando La lista
                    ObraItems.Clear();
                    CargarObraItems();
                    // 
                    return;
                }
                //
                // Evento Refrescar La Lista
            }
            catch (Exception ex)
            {
                await mensaje.MostrarMensaje("Eliminar Obra", "Error en el dispositivo o URL incorrecto: " + ex.ToString());
            }
            finally
            {
            }
        }
        #endregion

    }
}
