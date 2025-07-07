using CapaEntidad;
using CapaNegocio;
using CapaPresentacion.Utilidades;
using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CapaPresentacion
{
    public partial class frmProducto : Form
    {
        public frmProducto()
        {
            InitializeComponent();
        }

        private void frmProducto_Load(object sender, EventArgs e)
        {

            cboestado.Items.Add(new OpcionCombo() { Valor = 1, Texto = "Activo" });
            cboestado.Items.Add(new OpcionCombo() { Valor = 0, Texto = "No Activo" });
            cboestado.DisplayMember = "Texto";
            cboestado.ValueMember = "Valor";
            cboestado.SelectedIndex = 0;

            List<Categoria> listacategoria = new CN_Categoria().Listar();
            foreach (Categoria item in listacategoria.Where(c => c.Estado)) // Suponiendo que 'Estado' es un booleano
            {
                cbocategoria.Items.Add(new OpcionCombo() { Valor = item.IdCategoria, Texto = item.Descripcion });
            }

            cbocategoria.DisplayMember = "Texto";
            cbocategoria.ValueMember = "Valor";
            if (cbocategoria.Items.Count > 0)
            {
                cbocategoria.SelectedIndex = 0;
            }

            foreach (DataGridViewColumn columna in dgvdata.Columns)
            {
                if (columna.Visible == true && columna.Name != "btnseleccionar" && columna.Name != "Stock" && columna.Name != "PrecioCompra" && columna.Name != "PrecioVenta")
                {
                    cbobusqueda.Items.Add(new OpcionCombo() { Valor = columna.Name, Texto = columna.HeaderText });
                }
            }
            cbobusqueda.DisplayMember = "Texto";
            cbobusqueda.ValueMember = "Valor";
            if (cbobusqueda.Items.Count > 0)
            {
                cbobusqueda.SelectedIndex = 0;
            }

            //MOSTRAR TODOS LOS PRODUCTOS
            List<Producto> lista = new CN__Producto().Listar();
            foreach (Producto item in lista)
            {
                dgvdata.Rows.Add(new object[] {
            "",
            item.IdProducto,
            item.Codigo,
            item.Nombre,
            item.Descripcion,
            item.oCategoria.IdCategoria,
            item.oCategoria.Descripcion,
            item.Stock,
            item.PrecioCompra,
            item.PrecioVenta,
            item.Estado == true ? 1 : 0,
            item.Estado == true ? "Activo" : "No Activo"
        });
            }

            // Generar y mostrar el código del nuevo producto
            txtcodigo.Text = GenerateNextProductCode(lista);

            CargarDatosOrdenados();

        }

        private string GenerateNextProductCode(List<Producto> productos)
        {

            if (productos.Count == 0)
            {
                return "0001";
            }

            // Obtener todos los códigos existentes en la lista de productos
            List<int> existingCodes = productos
                .Select(p => int.TryParse(p.Codigo, out int code) ? code : 0)
                .Where(code => code > 0)
                .OrderBy(code => code)
                .ToList();

            // Buscar el primer hueco en los códigos secuenciales
            int nextCode = 1;
            foreach (int code in existingCodes)
            {
                if (code != nextCode)
                {
                    break;
                }
                nextCode++;
            }

            // Retornar el código formateado
            return nextCode.ToString("D4");

        }


        private void CargarDatosOrdenados()
        {
            // Obtén la lista de productos y ordénala por código
            List<Producto> productos = new CN__Producto().Listar()
                                          .OrderBy(p => Convert.ToInt32(p.Codigo))
                                          .ToList();

            // Limpia el DataGridView antes de cargar los datos
            dgvdata.Rows.Clear();

            // Cargar los datos ordenados en el DataGridView
            foreach (var producto in productos)
            {

                dgvdata.Rows.Add(new object[] {
    "",
    producto.IdProducto,
    producto.Codigo,
    producto.Nombre,
    producto.Descripcion,
    producto.oCategoria.IdCategoria.ToString(),
    producto.oCategoria.Descripcion,
    producto.Stock.ToString(), // Asegúrate de tener el valor real de Stock
    producto.PrecioCompra.ToString("0.00"), // Usa el valor de PrecioCompra
    producto.PrecioVenta.ToString("0.00"), // Usa el valor de PrecioVenta
    producto.Estado ? "1" : "0",
    producto.Estado ? "Activo" : "No Activo"
});

            }
        }



        private void btnguardar_Click(object sender, EventArgs e)
        {

            string mensaje = string.Empty;

            // Validación del nombre: solo letras, tildes, ñ, y espacios
            if (!Regex.IsMatch(txtnombre.Text, @"^[a-zA-ZñÑáéíóúÁÉÍÓÚ\s]+$"))
            {
                MessageBox.Show("El nombre debe contener solo letras, espacios y puede incluir tildes o 'ñ'.", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            // Validación de la descripción: letras, tildes, ñ, números, y puntos solo si están acompañados de letras
            if (!Regex.IsMatch(txtdescripcion.Text, @"^(?=.*[a-zA-ZñÑáéíóúÁÉÍÓÚ])[a-zA-ZñÑáéíóúÁÉÍÓÚ0-9\s]*(\.[a-zA-ZñÑáéíóúÁÉÍÓÚ0-9\s]*)?$"))
            {
                MessageBox.Show("La descripción debe contener al menos una letra, puede incluir números, tildes, ñ, y puntos solo si están acompañados de letras.", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            Producto obj = new Producto()
            {
                IdProducto = Convert.ToInt32(txtid.Text),
                Codigo = txtcodigo.Text,
                Nombre = txtnombre.Text,
                Descripcion = txtdescripcion.Text,
                oCategoria = new Categoria() { IdCategoria = Convert.ToInt32(((OpcionCombo)cbocategoria.SelectedItem).Valor) },
                Estado = Convert.ToInt32(((OpcionCombo)cboestado.SelectedItem).Valor) == 1 ? true : false
            };

            if (obj.IdProducto == 0) // Nuevo producto
            {
                int idgenerado = new CN__Producto().Registrar(obj, out mensaje);

                if (idgenerado != 0)
                {
                    // Agregar el nuevo producto a la tabla
                    dgvdata.Rows.Add(new object[] {
                "",
                idgenerado,
                txtcodigo.Text,
                txtnombre.Text,
                txtdescripcion.Text,
                ((OpcionCombo)cbocategoria.SelectedItem).Valor.ToString(),
                ((OpcionCombo)cbocategoria.SelectedItem).Texto.ToString(),
                "0",
                "0.00",
                "0.00",
                ((OpcionCombo)cboestado.SelectedItem).Valor.ToString(),
                ((OpcionCombo)cboestado.SelectedItem).Texto.ToString()
            });

                    Limpiar();

                    // Actualizar el siguiente código de producto disponible
                    txtcodigo.Text = GenerateNextProductCode(new CN__Producto().Listar());
                }
                else
                {
                    MessageBox.Show(mensaje);
                }
            }
            else // Edición de producto
            {
                bool resultado = new CN__Producto().Editar(obj, out mensaje);

                if (resultado)
                {
                    // Actualizar la fila del DataGridView
                    DataGridViewRow row = dgvdata.Rows[Convert.ToInt32(txtindice.Text)];
                    row.Cells["Id"].Value = txtid.Text;
                    row.Cells["Codigo"].Value = txtcodigo.Text;
                    row.Cells["Nombre"].Value = txtnombre.Text;
                    row.Cells["Descripcion"].Value = txtdescripcion.Text;
                    row.Cells["IdCategoria"].Value = ((OpcionCombo)cbocategoria.SelectedItem).Valor.ToString();
                    row.Cells["Categoria"].Value = ((OpcionCombo)cbocategoria.SelectedItem).Texto.ToString();
                    row.Cells["EstadoValor"].Value = ((OpcionCombo)cboestado.SelectedItem).Valor.ToString();
                    row.Cells["Estado"].Value = ((OpcionCombo)cboestado.SelectedItem).Texto.ToString();

                    Limpiar();

                    // Obtener el siguiente código de producto y mostrarlo en txtcodigo
                    txtcodigo.Text = GenerateNextProductCode(new CN__Producto().Listar());
                }
                else
                {
                    MessageBox.Show(mensaje);
                }
            }

            CargarDatosOrdenados();

        }

        private void Limpiar()
        {

            

            txtcodigo.Text = string.Empty;
            txtnombre.Text = string.Empty;
            txtdescripcion.Text = string.Empty;
            cbocategoria.SelectedIndex = 0;
            cboestado.SelectedIndex = 0;
            txtid.Text = "0"; // Reset ID for new entry

        }

        private void dgvdata_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            if (e.ColumnIndex == 0)
            {

                e.Paint(e.CellBounds, DataGridViewPaintParts.All);

                var w = Properties.Resources.check20.Width;
                var h = Properties.Resources.check20.Height;
                var x = e.CellBounds.Left + (e.CellBounds.Width - w) / 2;
                var y = e.CellBounds.Top + (e.CellBounds.Height - h) / 2;

                e.Graphics.DrawImage(Properties.Resources.check20, new Rectangle(x, y, w, h));
                e.Handled = true;
            }
        }

        private void dgvdata_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvdata.Columns[e.ColumnIndex].Name == "btnseleccionar")
            {

                int indice = e.RowIndex;

                if (indice >= 0)
                {

                    txtindice.Text = indice.ToString();
                    txtid.Text = dgvdata.Rows[indice].Cells["Id"].Value.ToString();

                    txtcodigo.Text = dgvdata.Rows[indice].Cells["Codigo"].Value.ToString();
                    txtnombre.Text = dgvdata.Rows[indice].Cells["Nombre"].Value.ToString();
                    txtdescripcion.Text = dgvdata.Rows[indice].Cells["Descripcion"].Value.ToString();
            

                    foreach (OpcionCombo oc in cbocategoria.Items)
                    {
                        if (Convert.ToInt32(oc.Valor) == Convert.ToInt32(dgvdata.Rows[indice].Cells["IdCategoria"].Value))
                        {
                            int indice_combo = cbocategoria.Items.IndexOf(oc);
                            cbocategoria.SelectedIndex = indice_combo;
                            break;
                        }
                    }


                    foreach (OpcionCombo oc in cboestado.Items)
                    {
                        if (Convert.ToInt32(oc.Valor) == Convert.ToInt32(dgvdata.Rows[indice].Cells["EstadoValor"].Value))
                        {
                            int indice_combo = cboestado.Items.IndexOf(oc);
                            cboestado.SelectedIndex = indice_combo;
                            break;
                        }
                    }


                }


            }
        }

        private void btneliminar_Click(object sender, EventArgs e)
        {

            if (Convert.ToInt32(txtid.Text) != 0)
            {
                if (MessageBox.Show("¿Desea eliminar el producto?", "Mensaje", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    string mensaje = string.Empty;

                    // Almacenar el código del producto antes de eliminarlo
                    string codigoEliminado = txtcodigo.Text;

                    // Crear un objeto Producto con solo el ID
                    Producto objProducto = new Producto
                    {
                        IdProducto = Convert.ToInt32(txtid.Text)
                    };

                    bool respuesta = new CN__Producto().Eliminar(objProducto, out mensaje);

                    if (respuesta)
                    {
                        // Remover la fila del DataGridView
                        dgvdata.Rows.RemoveAt(Convert.ToInt32(txtindice.Text));

                        // Limpiar los campos y asignar el código eliminado
                        Limpiar();
                        txtcodigo.Text = codigoEliminado;
                    }
                    else
                    {
                        MessageBox.Show(mensaje, "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
            }

            CargarDatosOrdenados();
        }

        private void btnbuscar_Click(object sender, EventArgs e)
        {
            string columnaFiltro = ((OpcionCombo)cbobusqueda.SelectedItem).Valor.ToString();

            if (dgvdata.Rows.Count > 0)
            {
                foreach (DataGridViewRow row in dgvdata.Rows)
                {

                    if (row.Cells[columnaFiltro].Value.ToString().Trim().ToUpper().Contains(txtbusqueda.Text.Trim().ToUpper()))
                        row.Visible = true;
                    else
                        row.Visible = false;
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

        private void btnlimpiar_Click(object sender, EventArgs e)
        {
            Limpiar();
        }

        private void btnexportar_Click(object sender, EventArgs e)
        {
            if (dgvdata.Rows.Count < 1)
            {
                MessageBox.Show("No hay datos para exportar", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else {
                DataTable dt = new DataTable();

                foreach (DataGridViewColumn columna in dgvdata.Columns) {
                    if (columna.HeaderText != "" && columna.Visible)
                        dt.Columns.Add(columna.HeaderText, typeof(string));
                }

                foreach (DataGridViewRow row in dgvdata.Rows) {
                    if (row.Visible)
                        dt.Rows.Add(new object[] {
                            row.Cells[2].Value.ToString(),
                            row.Cells[3].Value.ToString(),
                            row.Cells[4].Value.ToString(),
                            row.Cells[6].Value.ToString(),
                            row.Cells[7].Value.ToString(),
                            row.Cells[8].Value.ToString(),
                            row.Cells[9].Value.ToString(),
                            row.Cells[11].Value.ToString(),

                        });
                }

                SaveFileDialog savefile = new SaveFileDialog();
                savefile.FileName = string.Format("ReporteProducto_{0}.xlsx",DateTime.Now.ToString("ddMMyyyyHHmmss"));
                savefile.Filter = "Excel Files | *.xlsx";

                if (savefile.ShowDialog() == DialogResult.OK) {

                    try {
                        XLWorkbook wb = new XLWorkbook();
                        var hoja = wb.Worksheets.Add(dt, "Informe");
                        hoja.ColumnsUsed().AdjustToContents();
                        wb.SaveAs(savefile.FileName);
                        MessageBox.Show("Reporte Generado", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    }
                    catch {
                        MessageBox.Show("Error al generar reporte", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }

                }

            }
        }
    }
}
