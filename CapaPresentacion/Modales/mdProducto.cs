using CapaEntidad;
using CapaNegocio;
using CapaPresentacion.Utilidades;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CapaPresentacion.Modales
{
    public partial class mdProducto : Form
    {
        public Producto _Producto { get; set; }
        public bool MostrarTodosLosProductos { get; set; } = true; // Por defecto, muestra todos los productos
        private int? _idCategoriaProveedor; // Para almacenar la categoría del proveedor, si se requiere

        public mdProducto(int? idCategoriaProveedor = null)
        {
            InitializeComponent();
            _idCategoriaProveedor = idCategoriaProveedor;
            MostrarTodosLosProductos = idCategoriaProveedor == null; // Si hay categoría, solo mostrar por categoría
        }

        private void mdProducto_Load(object sender, EventArgs e)
        {
            // Cargar columnas en el combo de búsqueda
            foreach (DataGridViewColumn columna in dgvdata.Columns)
            {
                if (columna.Visible)
                {
                    cbobusqueda.Items.Add(new OpcionCombo() { Valor = columna.Name, Texto = columna.HeaderText });
                }
            }
            cbobusqueda.DisplayMember = "Texto";
            cbobusqueda.ValueMember = "Valor";
            cbobusqueda.SelectedIndex = 0;

            // Cargar productos según el modo
            if (MostrarTodosLosProductos)
            {
                CargarProductos();
            }
            else if (_idCategoriaProveedor.HasValue)
            {
                CargarProductosPorCategoria(_idCategoriaProveedor.Value);
            }
        }

        // Método para cargar todos los productos
        private void CargarProductos()
        {
            dgvdata.Rows.Clear(); // Limpiar filas existentes
            List<Producto> lista = new CN__Producto().Listar().Where(p => p.Estado).ToList();

            foreach (Producto item in lista)
            {
                dgvdata.Rows.Add(new object[] {
                item.IdProducto,
                item.Codigo,
                item.Nombre,
                item.oCategoria != null ? item.oCategoria.Descripcion : "Sin Categoría",
                item.Stock,
                item.PrecioCompra,
                item.PrecioVenta
            });
            }

            if (lista.Count == 0)
            {
                MessageBox.Show("No hay productos disponibles.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // Método para cargar productos filtrados por categoría
        public void CargarProductosPorCategoria(int idCategoriaProveedor)
        {
            dgvdata.Rows.Clear(); // Limpiar filas existentes
            List<Producto> productosFiltrados = new CN__Producto().Listar()
                .Where(p => p.oCategoria != null && p.oCategoria.IdCategoria == idCategoriaProveedor && p.Estado)
                .ToList();

            foreach (Producto item in productosFiltrados)
            {
                dgvdata.Rows.Add(new object[] {
                item.IdProducto,
                item.Codigo,
                item.Nombre,
                item.oCategoria.Descripcion,
                item.Stock,
                item.PrecioCompra,
                item.PrecioVenta
            });
            }

            if (productosFiltrados.Count == 0)
            {
                MessageBox.Show("No hay productos disponibles para esta categoría.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    

        private void dgvdata_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            int iRow = e.RowIndex;
            int iColum = e.ColumnIndex;
            if (iRow >= 0 && iColum > 0)
            {
                _Producto = new Producto()
                {
                    IdProducto = Convert.ToInt32(dgvdata.Rows[iRow].Cells["Id"].Value.ToString()),
                    Codigo = dgvdata.Rows[iRow].Cells["Codigo"].Value.ToString(),
                    Nombre = dgvdata.Rows[iRow].Cells["Nombre"].Value.ToString(),
                    Stock = Convert.ToInt32(dgvdata.Rows[iRow].Cells["Stock"].Value.ToString()),
                    PrecioCompra = Convert.ToDecimal(dgvdata.Rows[iRow].Cells["PrecioCompra"].Value.ToString()),
                    PrecioVenta = Convert.ToDecimal(dgvdata.Rows[iRow].Cells["PrecioVenta"].Value.ToString()),
                };
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void btnbuscar_Click(object sender, EventArgs e)
        {
            string columnaFiltro = ((OpcionCombo)cbobusqueda.SelectedItem).Valor.ToString();

            if (dgvdata.Rows.Count > 0)
            {
                foreach (DataGridViewRow row in dgvdata.Rows)
                {
                    row.Visible = row.Cells[columnaFiltro].Value.ToString().Trim().ToUpper().Contains(txtbusqueda.Text.Trim().ToUpper());
                }
            }
        }

        private void btnlimpiarbuscador_Click(object sender, EventArgs e)
        {
            txtbusqueda.Text = "";
            foreach (DataGridViewRow row in dgvdata.Rows)
            {
                row.Visible = true;
            }
        }
    }
}
