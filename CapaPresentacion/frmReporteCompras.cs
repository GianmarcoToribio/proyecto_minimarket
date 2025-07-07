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



            cboproveedor.Items.Add(new OpcionCombo() { Valor = 0, Texto = "TODOS" });
            foreach (Proveedor item in lista)
            {
                cboproveedor.Items.Add(new OpcionCombo() { Valor = item.IdProveedor, Texto = item.RazonSocial });
            }
            cboproveedor.DisplayMember = "Texto";
            cboproveedor.ValueMember = "Valor";
            cboproveedor.SelectedIndex = 0;


            foreach (DataGridViewColumn columna in dgvdata.Columns)
            {
                cbobusqueda.Items.Add(new OpcionCombo() { Valor = columna.Name, Texto = columna.HeaderText });
            }
            cbobusqueda.DisplayMember = "Texto";
            cbobusqueda.ValueMember = "Valor";
            cbobusqueda.SelectedIndex = 0;*/


            // Cargar proveedores en cboproveedor
            List<Proveedor> lista = new CN_Proveedor().Listar();
            cboproveedor.Items.Add(new OpcionCombo() { Valor = 0, Texto = "TODOS" });
            foreach (Proveedor item in lista)
            {
                cboproveedor.Items.Add(new OpcionCombo() { Valor = item.IdProveedor, Texto = item.RazonSocial });
            }
            cboproveedor.DisplayMember = "Texto";
            cboproveedor.ValueMember = "Valor";
            cboproveedor.SelectedIndex = 0;

            // Limpiar el ComboBox de búsqueda y agregar solo las opciones permitidas
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

                txtfechainicio.Value.ToString(),
                txtfechafin.Value.ToString(),
                idproveedor
            );

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

            if (dgvdata.Rows.Count > 0)
            {
                foreach (DataGridViewRow row in dgvdata.Rows)
                {
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
