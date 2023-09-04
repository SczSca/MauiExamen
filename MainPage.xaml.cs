namespace MauiExamen;

using SQLite;

public partial class MainPage : ContentPage
{
    // Definición de la clase Cliente con atributos que representan sus propiedades
    public class Cliente
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Direccion { get; set; }
        public string Telefono { get; set; }
        public string CorreoElectronico { get; set; }
		public string UrlFoto {get; set;}
    }

    // Clase para administrar la base de datos
    public class DatabaseManager
    {
        readonly SQLiteConnection database;

        public DatabaseManager(string dbPath)
        {
            // Crear una instancia de SQLiteConnection con la ruta de la base de datos
            database = new SQLiteConnection(dbPath);
            // Crear la tabla "Cliente" en la base de datos si no existe
            database.CreateTable<Cliente>();
        }

        // Obtener todos los contactos de la base de datos
        public List<Cliente> ObtenerClientes()
        {
            return database.Table<Cliente>().ToList();
        }

        // Obtener un cliente por su nombre
        
        // Guardar o actualizar un cliente en la base de datos
        public int GuardarCliente(Cliente cliente)
        {
            if (cliente.Id != 0)
            {
                return database.Update(cliente);
            }
            else
            {
                return database.Insert(cliente);
            }
        }

        // Eliminar un cliente de la base de datos
        public int EliminarCliente(Cliente cliente)
        {
            return database.Delete(cliente);
        }
    }

    // Instancia de DatabaseManager para administrar la base de datos
    DatabaseManager dbManager;
    Cliente cliente;
	List<Cliente> contactosView;

    public MainPage()
    {
        InitializeComponent();

        // Ruta de la base de datos en el sistema de archivos del dispositivo
        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "clients.db");
        dbManager = new DatabaseManager(dbPath);
		
		MostrarClientesInterfaz();
    }

    // Evento al hacer clic en el botón "Guardar"
    private void Guardar_Clicked(object sender, EventArgs e)
    {
        // Código para guardar o modificar un cliente en la base de datos
		try
		{
			if(name.Text.Length > 0 && address.Text.Length > 0 && phone.Text.Length > 0 && email.Text.Length > 0)
			{
			cliente = new Cliente {Nombre = name.Text, Direccion = address.Text, Telefono = phone.Text, CorreoElectronico = email.Text, UrlFoto = url.Text};
			// cliente.Nombre = name.Text;
			// cliente.Direccion = address.Text;
			// cliente.Telefono = phone.Text;
			// cliente.CorreoElectronico = email.Text;
			dbManager.GuardarCliente(cliente);
			MostrarClientesInterfaz();
			LimpiarCampos();
			cliente = null;

			} else
			{
				DisplayAlert("Error","Faltan datos", "OK");
			}
		}
		catch (Exception ex)
		{
			DisplayAlert("Error",ex.Message, "OK");
		}
    }


    // Evento al hacer clic en el botón "Eliminar"
    private async void Eliminar_Clicked(object sender, ItemTappedEventArgs e)
    {
        // Código para eliminar un cliente de la base de datos
		if (e.Item != null && e.Item is Cliente clienteTapped)
		{
			// Mostrar un cuadro de diálogo preguntando al usuario si desea eliminar el cliente
			bool respuesta = await DisplayAlert("Eliminar cliente", $"¿Estás seguro de eliminar a {clienteTapped.Nombre}?", "Sí", "No");

			// Verificar la respuesta del usuario
			if (respuesta)
			{
				// Si el usuario seleccionó "Sí", se ejecuta este bloque de código
				cliente = clienteTapped;
				dbManager.EliminarCliente(cliente);
				MostrarClientesInterfaz();
			}
		}
    }

    // Evento al hacer clic en el botón "Modificar"
    private void Modificar_Clicked(object sender, EventArgs e)
    {
        // Código para habilitar campos y permitir modificaciones
		try
		{
		if(name.Text.Length > 0 && address.Text.Length > 0 && phone.Text.Length > 0 && email.Text.Length > 0 && url.Text.Length > 0)
			{
			cliente.Nombre = name.Text;
			cliente.Direccion = address.Text;
			cliente.Telefono = phone.Text;
			cliente.CorreoElectronico = email.Text;
			cliente.UrlFoto = url.Text;
			dbManager.GuardarContacto(cliente);
			MostrarClientesInterfaz();
			LimpiarCampos();
			ResetearBtns(sender, e);
			cliente = null;

			} else
			{
				DisplayAlert("Error","Faltan datos", "OK");
			}

		} catch (Exception ex)
		{
            DisplayAlert("Error", "Error: " + ex.Message, "OK");
		}
    }

	private async void contactsList_ItemTapped(object sender, ItemTappedEventArgs e)
	{
		string action = await DisplayActionSheet("Acciones:", "Cancelar", null, "Eliminar", "Editar");
		if(action == "Eliminar")
		{
			Eliminar_Clicked(sender, e);		
		}
		else if(action == "Editar")
		{
			if(e.Item != null && e.Item is Cliente clienteTapped)
			{
			name.Text = clienteTapped.Nombre;
			address.Text = clienteTapped.Direccion;
			phone.Text = clienteTapped.Telefono;
			email.Text = clienteTapped.CorreoElectronico;
			saveBtn.IsVisible = false;
			searchBtn.IsVisible = false;
			editBtn.IsVisible = true;
			cancelBtn.IsVisible = true;
			cliente = clienteTapped;
			}
		}
	}
	private void UrlEntry_TextChanged(object sender, TextChangedEventArgs e)
	{
		string url = e.NewTextValue;

		if (Uri.IsWellFormedUriString(url, UriKind.Absolute))
		{
			if (url.EndsWith(".jpg") || url.EndsWith(".png") || url.EndsWith(".jpeg"))
			{
				// La URL es válida y parece ser una URL de imagen
				imagenPreview.Source = ImageSource.FromUri(new Uri(url));
				imagenPreview.IsVisible = true;
				saveBtn.IsEnabled = true;
			}
			else
			{
				// La URL es válida pero no es una imagen
				imagenPreview.IsVisible = false;
			}
		}
		else
		{
			// La URL no es válida
			imagenPreview.IsVisible = false;
		}
	}
    // Método para limpiar los campos y restablecer estados
    private void LimpiarCampos()
    {
        // Código para limpiar y restablecer campos y estados
		name.Text = "";
		address.Text = "";
		phone.Text = "";
		email.Text = "";
		url.Text = "";
    }
	private void ResetearBtns(object sender, EventArgs e)
	{
		saveBtn.IsVisible = true;
		searchBtn.IsVisible = true;
		cancelBtn.IsVisible = false;
		editBtn.IsVisible = false;
		saveBtn.IsEnabled = false;
		LimpiarCampos();
	}
	private void MostrarClientesInterfaz()
	{
		contactosView = dbManager.ObtenerClientes();
		
		contactsList.ItemsSource = contactosView;
	}
}


	
