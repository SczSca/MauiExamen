namespace MauiExamen.Pages;

using SQLite;

public partial class Productos : ContentPage
{
    // Definición de la clase Producto con atributos que representan sus propiedades
    public class Producto
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Cantidad { get; set; }
        public string Costo { get; set; }
		public string Venta { get; set; }
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
            // Crear la tabla "Producto" en la base de datos si no existe
            database.CreateTable<Producto>();
        }

        // Obtener todos los contactos de la base de datos
        public List<Producto> ObtenerProductos()
        {
            return database.Table<Producto>().ToList();
        }

        // Obtener un producto por su nombre
        
        // Guardar o actualizar un producto en la base de datos
        public int GuardarProducto(Producto producto)
        {
            if (producto.Id != 0)
            {
                return database.Update(producto);
            }
            else
            {
                return database.Insert(producto);
            }
        }

        // Eliminar un producto de la base de datos
        public int EliminarProducto(Producto producto)
        {
            return database.Delete(producto);
        }
    }

    // Instancia de DatabaseManager para administrar la base de datos
    DatabaseManager dbManager;
    Producto producto;
	List<Producto> productosView;
	//bandera que permite desactivar el TextChanged del entry url
	private bool validacionActive = true;

    public Productos()
    {
        InitializeComponent();

        // Ruta de la base de datos en el sistema de archivos del dispositivo
        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "products.db");
        dbManager = new DatabaseManager(dbPath);
		
		MostrarProductosInterfaz();
    }

    // Evento al hacer clic en el botón "Guardar"
    private void Guardar_Clicked(object sender, EventArgs e)
    {
        // Código para guardar o modificar un producto en la base de datos
		try
		{
			if(name.Text.Length > 0 && description.Text.Length > 0 && quantity.Text.Length > 0 && cost.Text.Length > 0 && sale.Text.Length > 0 && url.Text.Length > 0)
			{
			producto = new Producto {Nombre = name.Text, Descripcion = description.Text, Cantidad = quantity.Text, Costo = cost.Text, Venta =sale.Text, UrlFoto = url.Text};
			// producto.Nombre = name.Text;
			// producto.Descripcion = descripcion.Text;
			// producto.Cantidad = quantity.Text;
			// producto.Costo = cost.Text;
			validacionActive = false;
			dbManager.GuardarProducto(producto);
			MostrarProductosInterfaz();
			LimpiarCampos();
			producto = null;
			validacionActive = true;
			imagenPreview.IsVisible = false;
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
        // Código para eliminar un producto de la base de datos
		if (e.Item != null && e.Item is Producto productoTapped)
		{
			// Mostrar un cuadro de diálogo preguntando al usuario si desea eliminar el producto
			bool respuesta = await DisplayAlert("Eliminar producto", $"¿Estás seguro de eliminar a {productoTapped.Nombre}?", "Sí", "No");

			// Verificar la respuesta del usuario
			if (respuesta)
			{
				// Si el usuario seleccionó "Sí", se ejecuta este bloque de código
				producto = productoTapped;
				dbManager.EliminarProducto(producto);
				MostrarProductosInterfaz();
			}
		}
    }

    // Evento al hacer clic en el botón "Modificar"
    private void Modificar_Clicked(object sender, EventArgs e)
    {
        // Código para habilitar campos y permitir modificaciones
		try
		{
		if(name.Text.Length > 0 && description.Text.Length > 0 && quantity.Text.Length > 0 && cost.Text.Length > 0 && sale.Text.Length > 0 && url.Text.Length > 0)
			{
			producto.Nombre = name.Text;
			producto.Descripcion = description.Text;
			producto.Cantidad = quantity.Text;
			producto.Costo = cost.Text;
			producto.Venta = sale.Text;
			producto.UrlFoto = url.Text;
			validacionActive = false;
			dbManager.GuardarProducto(producto);
			MostrarProductosInterfaz();
			LimpiarCampos();
			ResetearBtns(sender, e);
			producto = null;
			validacionActive = true;
			imagenPreview.IsVisible = false;
			} else
			{
				DisplayAlert("Error","Faltan datos", "OK");
			}

		} catch (Exception ex)
		{
            DisplayAlert("Error", "Error: " + ex.Message, "OK");
		}
    }

	private async void productsList_ItemTapped(object sender, ItemTappedEventArgs e)
	{
		string action = await DisplayActionSheet("Acciones:", "Cancelar", null, "Eliminar", "Editar");
		if(action == "Eliminar")
		{
			Eliminar_Clicked(sender, e);		
		}
		else if(action == "Editar")
		{
			if(e.Item != null && e.Item is Producto productoTapped)
			{
			name.Text = productoTapped.Nombre;
			description.Text = productoTapped.Descripcion;
			quantity.Text = productoTapped.Cantidad;
			cost.Text = productoTapped.Costo;
			sale.Text = productoTapped.Venta;
			url.Text = productoTapped.UrlFoto;
			saveBtn.IsVisible = false;
			editBtn.IsVisible = true;
			cancelBtn.IsVisible = true;
			producto = productoTapped;
			}
		}
	}
	//método que permite validar que la URL sea una url y que sea de una imagen antes de guardar o editar
	private void UrlEntry_TextChanged(object sender, TextChangedEventArgs e)
	{
		if(!validacionActive) return;
		string urlT = e.NewTextValue;

		if (Uri.IsWellFormedUriString(urlT, UriKind.Absolute))
		{
			if (urlT.EndsWith(".jpg") || urlT.EndsWith(".png") || urlT.EndsWith(".jpeg"))
			{
				// La URL es válida y parece ser una URL de imagen
				imagenPreview.Source = ImageSource.FromUri(new Uri(urlT));
				imagenPreview.IsVisible = true;
				saveBtn.IsEnabled = true;
			}
			else
			{
				// La URL es válida pero no es una imagen
				imagenPreview.IsVisible = false;
				DisplayAlert("Error", "El texto introducido es una URL, pero no de una imagen", "OK");
				url.Text = "";
			}
		}
		else
		{
			// La URL no es válida
			imagenPreview.IsVisible = false;
			DisplayAlert("Error", "El texto introducido no es una URL", "OK");
			url.Text = "";
		}
	}
    // Método para limpiar los campos y restablecer estados
    private void LimpiarCampos()
    {
        // Código para limpiar y restablecer campos y estados
		name.Text = "";
		description.Text = "";
		quantity.Text = "";
		cost.Text = "";
		sale.Text = "";
		url.Text = "";
    }
	private void ResetearBtns(object sender, EventArgs e)
	{
		saveBtn.IsVisible = true;
		cancelBtn.IsVisible = false;
		editBtn.IsVisible = false;
		saveBtn.IsEnabled = false;
		validacionActive = false;
		LimpiarCampos();
		validacionActive = true;
		imagenPreview.IsVisible = false;
	}
	private void MostrarProductosInterfaz()
	{
		productosView = dbManager.ObtenerProductos();
		
		productsList.ItemsSource = productosView;
	}
}


	
