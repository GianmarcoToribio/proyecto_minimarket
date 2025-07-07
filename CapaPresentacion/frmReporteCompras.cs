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
    public partial class frmReporteCompras : Form
    {
        public frmReporteCompras()
        {
            InitializeComponent();
        }

        private void frmReporteCompras_Load(object sender, EventArgs e)
        {
<<<<<<< HEAD
            List<Proveedor> lista = new CN_Proveedor().Listar();
=======
            /*List<Proveedor> lista = new CN_Proveedor().Listar();

>>>>>>> diego
            cboproveedor.Items.Add(new OpcionCombo() { Valor = 0, Texto = "TODOS" });
            foreach (Proveedor item in lista)
            {
                cboproveedor.Items.Add(new OpcionCombo() { Valor = item.IdProveedor, Texto = item.RazonSocial });
            }
            cboproveedor.DisplayMember = "Texto";
            cboproveedor.ValueMember = "Valor";
            cboproveedor.SelectedIndex = 0;
<<<<<<< HEAD
=======


            foreach (DataGridViewColumn columna in dgvdata.Columns)
            {
                cbobusqueda.Items.Add(new OpcionCombo() { Valor = columna.Name, Texto = columna.HeaderText });
            }
            cbobusqueda.DisplayMember = "Texto";
            cbobusqueda.ValueMember = "Valor";
            cbobusqueda.SelectedIndex = 0;*/


            // Cargar proveedores en cboproveedor

            // Limpiar el ComboBox de búsqueda y agregar solo las opciones permitidas
>>>>>>> diego
            cbobusqueda.Items.Clear();
            cbobusqueda.Items.Add(new OpcionCombo() { Valor = "NumeroDocumento", Texto = "NumeroDocumento" });
            cbobusqueda.Items.Add(new OpcionCombo() { Valor = "UsuarioRegistro", Texto = "UsuarioRegistro" });
            cbobusqueda.Items.Add(new OpcionCombo() { Valor = "CodigoProducto", Texto = "CodigoProducto" });
            cbobusqueda.Items.Add(new OpcionCombo() { Valor = "NombreProducto", Texto = "NombreProducto" });
            cbobusqueda.Items.Add(new OpcionCombo() { Valor = "Categoria", Texto = "Categoría" });

            cbobusqueda.DisplayMember = "Texto";
            cbobusqueda.ValueMember = "Valor";
            cbobusqueda.SelectedIndex = 0;

        }

        private void btnbuscarresultado_Click(object sender, EventArgs e)
        {

            int idproveedor = Convert.ToInt32(((OpcionCombo)cboproveedor.SelectedItem).Valor.ToString());

            List<ReporteCompra> lista = new List<ReporteCompra>();

<<<<<<< HEAD
=======
            // Obtener la lista de reportes de compras
>>>>>>> diego
            lista = new CN_Reporte().Compra(
                txtfechainicio.Value.ToString(),
                txtfechafin.Value.ToString(),
                idproveedor
            );

<<<<<<< HEAD
            dgvdata.Rows.Clear();

            if (lista.Count == 0)
            {
                MessageBox.Show("No se encontraron reportes de compras en el rango de fechas seleccionado.", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

=======
            // Limpiar el DataGridView antes de agregar nuevos registros
            dgvdata.Rows.Clear();

            // Verificar si la lista está vacía
            if (lista.Count == 0)
            {
                MessageBox.Show("No se encontraron reportes de compras en el rango de fechas seleccionado.", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return; // Salir del método si no hay reportes
            }

            // Agregar los reportes al DataGridView
>>>>>>> diego
            foreach (ReporteCompra rc in lista)
            {
                dgvdata.Rows.Add(new object[] {
            rc.FechaRegistro,
            rc.TipoDocumento,
            rc.NumeroDocumento,
            rc.MontoTotal,
            rc.UsuarioRegistro,
            rc.DocumentoProveedor,
            rc.RazonSocial,
            rc.CodigoProducto,
            rc.NombreProducto,
            rc.Categoria,
            rc.PrecioCompra,
            rc.PrecioVenta,
            rc.Cantidad,
            rc.SubTotal
        });
            }

        }

        private void btnexportar_Click(object sender, EventArgs e)
        {
            if (dgvdata.Rows.Count < 1)
            {

                MessageBox.Show("No hay registros para exportar", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

            }
            else {

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
                            row.Cells[12].Value.ToString(),
                            row.Cells[13].Value.ToString()
                        });
                }

                SaveFileDialog savefile = new SaveFileDialog();
                savefile.FileName = string.Format("ReporteCompras_{0}.xlsx", DateTime.Now.ToString("ddMMyyyyHHmmss"));
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
        }
<<<<<<< HEAD
        private void btnbuscar_Click(object sender, EventArgs e)
        {

            string columnaFiltro = ((OpcionCombo)cbobusqueda.SelectedItem).Valor.ToString();
            bool found = false;
=======

        private void btnbuscar_Click(object sender, EventArgs e)
        {

            /*string columnaFiltro = ((OpcionCombo)cbobusqueda.SelectedItem).Valor.ToString();
            bool found = false; // Variable para verificar si se encontró al menos un resultado
>>>>>>> diego

            if (dgvdata.Rows.Count > 0)
            {
                foreach (DataGridViewRow row in dgvdata.Rows)
                {
<<<<<<< HEAD

                    if (row.Cells[columnaFiltro] != null && row.Cells[columnaFiltro].Value != null &&
                        row.Cells[columnaFiltro].Value.ToString().Trim().ToUpper().Contains(txtbusqueda.Text.Trim().ToUpper()))
                    {
                        row.Visible = true;
                        found = true; 
                    }
                    else
                    {
                        row.Visible = false;
                    }
                }

=======
                    // Verifica si la celda contiene el texto buscado
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
            }*/

            string columnaFiltro = ((OpcionCombo)cbobusqueda.SelectedItem).Valor.ToString();
            bool found = false; // Variable para verificar si se encontró al menos un resultado

            if (dgvdata.Rows.Count > 0)
            {
                foreach (DataGridViewRow row in dgvdata.Rows)
                {
                    // Verifica si la columna seleccionada existe en el DataGridView antes de buscar
                    if (row.Cells[columnaFiltro] != null && row.Cells[columnaFiltro].Value != null &&
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
>>>>>>> diego
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

        private void dgvdata_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
