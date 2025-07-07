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
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CapaPresentacion
{
    public partial class frmReporteVentas : Form
    {
        public frmReporteVentas()
        {
            InitializeComponent();
        }

        private void frmReporteVentas_Load(object sender, EventArgs e)
        {

            // Limpiar las opciones anteriores en el ComboBox
            cbobusqueda.Items.Clear();

            // Añadir solo los filtros específicos en el ComboBox
            cbobusqueda.Items.Add(new OpcionCombo() { Valor = "NumeroDocumento", Texto = "NumeroDocumento" });
            cbobusqueda.Items.Add(new OpcionCombo() { Valor = "UsuarioRegistro", Texto = "UsuarioRegistro" });
            cbobusqueda.Items.Add(new OpcionCombo() { Valor = "NombreCliente", Texto = "NombreCliente" });
            cbobusqueda.Items.Add(new OpcionCombo() { Valor = "CodigoProducto", Texto = "Codigoproducto" });
            cbobusqueda.Items.Add(new OpcionCombo() { Valor = "NombreProducto", Texto = "Nombre Producto" });
            cbobusqueda.Items.Add(new OpcionCombo() { Valor = "Categoria", Texto = "Categoria" });
            cbobusqueda.Items.Add(new OpcionCombo() { Valor = "MetodoPago", Texto = "Método de Pago" });

            // Verificar si la columna "MetodoPago" ya existe en el DataGridView
            if (!dgvdata.Columns.Contains("MetodoPago"))
            {
                // Agregar la columna "MetodoPago" al DataGridView si no existe
                dgvdata.Columns.Add("MetodoPago", "Método de Pago");
            }

            // Configuración del ComboBox
            cbobusqueda.DisplayMember = "Texto";
            cbobusqueda.ValueMember = "Valor";
            cbobusqueda.SelectedIndex = 0; // Seleccionar la primera opción por defecto

        }

        private void btnbuscarreporte_Click(object sender, EventArgs e)
        {

            List<ReporteVenta> lista = new List<ReporteVenta>();

            // Obtener la lista de reportes
            lista = new CN_Reporte().Venta(txtfechainicio.Value.ToString(), txtfechafin.Value.ToString());

            // Limpiar el DataGridView antes de agregar nuevos registros
            dgvdata.Rows.Clear();

            // Verificar si la lista está vacía
            if (lista.Count == 0)
            {
                MessageBox.Show("No se encontraron reportes en el rango de fechas seleccionado.", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return; // Salir del método si no hay reportes
            }

            // Agregar los reportes al DataGridView
            foreach (ReporteVenta rv in lista)
            {
                dgvdata.Rows.Add(new object[] {
            rv.FechaRegistro,
            rv.TipoDocumento,
            rv.NumeroDocumento,
            rv.MontoTotal,
            rv.UsuarioRegistro,
            rv.DocumentoCliente,
            rv.NombreCliente,
            rv.CodigoProducto,
            rv.NombreProducto,
            rv.Categoria,
            rv.PrecioVenta,
            rv.Cantidad,
            rv.SubTotal,
            rv.MetodoPago // Agregar el método de pago aquí
        });
            }

        }

        /*private void btnbuscar_Click(object sender, EventArgs e)
        {

            string columnaFiltro = ((OpcionCombo)cbobusqueda.SelectedItem).Valor.ToString();
            bool found = false; // Variable para verificar si se encontró al menos un resultado

            if (dgvdata.Rows.Count > 0)
            {
                foreach (DataGridViewRow row in dgvdata.Rows)
                {
                    // Verificar si la celda contiene el texto buscado
                    if (row.Cells[columnaFiltro].Value != null &&
                        row.Cells[columnaFiltro].Value.ToString().Trim().ToUpper().Contains(txtbusqueda.Text.Trim().ToUpper()))
                    {
                        row.Visible = true; // Mostrar la fila si coincide con la búsqueda
                        found = true; // Se encontró al menos un resultado
                    }
                    else
                    {
                        row.Visible = false; // Ocultar la fila si no coincide
                    }
                }

                // Si no se encontró ningún resultado, mostrar un mensaje
                if (!found)
                {
                    MessageBox.Show("No se encontraron resultados para la búsqueda.", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show("No hay datos disponibles para buscar.", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }*/

        private void btnbuscar_Click(object sender, EventArgs e)
        {
            string columnaFiltro = ((OpcionCombo)cbobusqueda.SelectedItem).Valor.ToString();
            bool found = false; // Variable para verificar si se encontró al menos un resultado

            if (dgvdata.Rows.Count > 0)
            {
                foreach (DataGridViewRow row in dgvdata.Rows)
                {
                    // Verificar si la celda de la columna contiene el texto buscado
                    if (row.Cells[columnaFiltro].Value != null &&
                        row.Cells[columnaFiltro].Value.ToString().Trim().ToUpper().Contains(txtbusqueda.Text.Trim().ToUpper()))
                    {
                        row.Visible = true; // Mostrar la fila si coincide con la búsqueda
                        found = true; // Se encontró al menos un resultado
                    }
                    else
                    {
                        row.Visible = false; // Ocultar la fila si no coincide
                    }
                }

                // Si no se encontró ningún resultado, mostrar un mensaje
                if (!found)
                {
                    MessageBox.Show("No se encontraron resultados para la búsqueda.", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show("No hay datos disponibles para buscar.", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        /*private void btnexportar_Click(object sender, EventArgs e)
        {
            if (dgvdata.Rows.Count < 1)
            {

                MessageBox.Show("No hay registros para exportar", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

            }
            else
            {

                DataTable dt = new DataTable();

                foreach (DataGridViewColumn columna in dgvdata.Columns)
                {
                    dt.Columns.Add(columna.HeaderText, typeof(string));
                }

                foreach (DataGridViewRow row in dgvdata.Rows)
                {
                    if (row.Visible)
                        dt.Rows.Add(new object[] {
                            row.Cells[0].Value.ToString(),
                            row.Cells[1].Value.ToString(),
                            row.Cells[2].Value.ToString(),
                            row.Cells[3].Value.ToString(),
                            row.Cells[4].Value.ToString(),
                            row.Cells[5].Value.ToString(),
                            row.Cells[6].Value.ToString(),
                            row.Cells[7].Value.ToString(),
                            row.Cells[8].Value.ToString(),
                            row.Cells[9].Value.ToString(),
                            row.Cells[10].Value.ToString(),
                            row.Cells[11].Value.ToString(),
                            row.Cells[12].Value.ToString()
                        });
                }

                SaveFileDialog savefile = new SaveFileDialog();
                savefile.FileName = string.Format("ReporteVentas_{0}.xlsx", DateTime.Now.ToString("ddMMyyyyHHmmss"));
                savefile.Filter = "Excel Files | *.xlsx";

                if (savefile.ShowDialog() == DialogResult.OK)
                {

                    try
                    {
                        XLWorkbook wb = new XLWorkbook();
                        var hoja = wb.Worksheets.Add(dt, "Informe");
                        hoja.ColumnsUsed().AdjustToContents();
                        wb.SaveAs(savefile.FileName);
                        MessageBox.Show("Reporte Generado", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    }
                    catch
                    {
                        MessageBox.Show("Error al generar reporte", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }

                }

            }
        }*/

        private void btnexportar_Click(object sender, EventArgs e)
        {
            // Verificar si hay registros para exportar
            if (dgvdata.Rows.Count < 1)
            {
                MessageBox.Show("No hay registros para exportar", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return; // Salir del método si no hay registros
            }

            // Crear un DataTable para almacenar los datos del DataGridView
            DataTable dt = new DataTable();

            // Añadir columnas al DataTable, incluyendo "Método de Pago"
            foreach (DataGridViewColumn columna in dgvdata.Columns)
            {
                dt.Columns.Add(columna.HeaderText, typeof(string));
            }

            // Añadir filas al DataTable solo para filas visibles
            foreach (DataGridViewRow row in dgvdata.Rows)
            {
                if (row.Visible) // Solo agregar filas visibles
                {
                    var rowData = new object[dgvdata.Columns.Count];

                    // Llenar los datos de la fila
                    for (int i = 0; i < dgvdata.Columns.Count; i++)
                    {
                        rowData[i] = row.Cells[i].Value?.ToString(); // Usar null conditional para evitar NullReferenceException
                    }

                    dt.Rows.Add(rowData); // Añadir la fila al DataTable
                }
            }

            // Crear el diálogo para guardar el archivo
            using (SaveFileDialog savefile = new SaveFileDialog())
            {
                savefile.FileName = $"ReporteVentas_{DateTime.Now:ddMMyyyyHHmmss}.xlsx"; // Nombre del archivo
                savefile.Filter = "Excel Files | *.xlsx"; // Filtro para el tipo de archivo

                // Mostrar el diálogo para guardar el archivo
                if (savefile.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // Crear y guardar el archivo Excel
                        using (XLWorkbook wb = new XLWorkbook())
                        {
                            var hoja = wb.Worksheets.Add(dt, "Informe");
                            hoja.ColumnsUsed().AdjustToContents(); // Ajustar el contenido de las columnas
                            wb.SaveAs(savefile.FileName); // Guardar el archivo
                        }

                        MessageBox.Show("Reporte Generado", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex) // Capturar excepciones específicas
                    {
                        MessageBox.Show($"Error al generar reporte: {ex.Message}", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
            }
        }


    }
}
