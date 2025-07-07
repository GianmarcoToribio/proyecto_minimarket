using CapaEntidad;
using CapaNegocio;
using CapaPresentacion.Utilidades;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CapaPresentacion
{
    public partial class frmProveedores : Form
    {
        public frmProveedores()
        {
            InitializeComponent();
        }

        private void frmProveedores_Load(object sender, EventArgs e)
        {
            // Cargar estados
            cboestado.Items.Add(new OpcionCombo() { Valor = 1, Texto = "Activo" });
            cboestado.Items.Add(new OpcionCombo() { Valor = 0, Texto = "No Activo" });
            cboestado.DisplayMember = "Texto";
            cboestado.ValueMember = "Valor";
            cboestado.SelectedIndex = 0;

            // Cargar categorías
            List<Categoria> listaCategorias = new CN_Categoria().Listar();
            foreach (Categoria item in listaCategorias)
            {
                cbocategoriaprov.Items.Add(new OpcionCombo() { Valor = item.IdCategoria, Texto = item.Descripcion });
            }
            cbocategoriaprov.DisplayMember = "Texto";
            cbocategoriaprov.ValueMember = "Valor";
            //cbocategoriaprov.SelectedIndex = 0;

            // Cargar proveedores
            List<Proveedor> lista = new CN_Proveedor().Listar();
            foreach (Proveedor item in lista)
            {
                dgvdata.Rows.Add(new object[] {
            "", item.IdProveedor, item.Documento, item.RazonSocial, item.Correo, item.Telefono,
            item.Estado == true ? 1 : 0,
            item.Estado == true ? "Activo" : "No Activo",
            item.oCategoria.IdCategoria, // Asegúrate de usar el IdCategoria
            item.oCategoria.Descripcion
        });
            }

            // Configurar opciones de búsqueda (sin incluir Correo y Teléfono)
            cbobusqueda.Items.Clear(); // Limpiar opciones anteriores
            cbobusqueda.Items.Add(new OpcionCombo() { Valor = "Documento", Texto = "Documento" });
            cbobusqueda.Items.Add(new OpcionCombo() { Valor = "RazonSocial", Texto = "Razón Social" });
            cbobusqueda.Items.Add(new OpcionCombo() { Valor = "CategoriaProv", Texto = "Categoría" });
            cbobusqueda.Items.Add(new OpcionCombo() { Valor = "Estado", Texto = "Estado" });

            cbobusqueda.DisplayMember = "Texto";
            cbobusqueda.ValueMember = "Valor";
            cbobusqueda.SelectedIndex = 0; // Seleccionar la primera opción por defecto
        }



        private void btnguardar_Click(object sender, EventArgs e)
        {
            string mensaje = string.Empty;

            int categoriaId = Convert.ToInt32(((OpcionCombo)cbocategoriaprov.SelectedItem).Valor);

            Proveedor obj = new Proveedor()
            {
                IdProveedor = Convert.ToInt32(txtid.Text),
                Documento = txtdocumento.Text,
                RazonSocial = txtrazonsocial.Text,
                Correo = txtcorreo.Text,
                Telefono = txttelefono.Text,
                Estado = Convert.ToInt32(((OpcionCombo)cboestado.SelectedItem).Valor) == 1,
                oCategoria = new Categoria() { IdCategoria = categoriaId }
            };

            if (obj.IdProveedor == 0) // Nuevo proveedor
            {
                int idgenerado = new CN_Proveedor().Registrar(obj, out mensaje);
                if (idgenerado != 0)
                {
                    dgvdata.Rows.Add(new object[] {
                        "", idgenerado, txtdocumento.Text, txtrazonsocial.Text, txtcorreo.Text, txttelefono.Text,
                        ((OpcionCombo)cboestado.SelectedItem).Valor.ToString(),
                        ((OpcionCombo)cboestado.SelectedItem).Texto.ToString(),
                        categoriaId, // Usar IdCategoria
                        ((OpcionCombo)cbocategoriaprov.SelectedItem).Texto
                    });
                    Limpiar();
                }
                else
                {
                    MessageBox.Show(mensaje);
                }
            }
            else // Edición de proveedor
            {
                bool resultado = new CN_Proveedor().Editar(obj, out mensaje);
                if (resultado)
                {
                    DataGridViewRow row = dgvdata.Rows[Convert.ToInt32(txtindice.Text)];
                    row.Cells["Id"].Value = txtid.Text;
                    row.Cells["Documento"].Value = txtdocumento.Text;
                    row.Cells["RazonSocial"].Value = txtrazonsocial.Text;
                    row.Cells["Correo"].Value = txtcorreo.Text;
                    row.Cells["Telefono"].Value = txttelefono.Text;
                    row.Cells["EstadoValor"].Value = ((OpcionCombo)cboestado.SelectedItem).Valor.ToString();
                    row.Cells["Estado"].Value = ((OpcionCombo)cboestado.SelectedItem).Texto.ToString();
                    row.Cells["IdCategoriaProv"].Value = categoriaId; // Usar IdCategoria
                    row.Cells["CategoriaProv"].Value = ((OpcionCombo)cbocategoriaprov.SelectedItem).Texto; // Usar descripción
                    Limpiar();
                }
                else
                {
                    MessageBox.Show(mensaje);
                }
            }
        }

        private void Limpiar()
        {
            txtindice.Text = "-1";
            txtid.Text = "0";
            txtdocumento.Text = "";
            txtrazonsocial.Text = "";
            txtcorreo.Text = "";
            txttelefono.Text = "";
            cboestado.SelectedIndex = 0;
            cbocategoriaprov.SelectedIndex = 0;
            txtdocumento.Select();
        }

        private void dgvdata_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0) return;

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
                    txtdocumento.Text = dgvdata.Rows[indice].Cells["Documento"].Value.ToString();
                    txtrazonsocial.Text = dgvdata.Rows[indice].Cells["RazonSocial"].Value.ToString();
                    txtcorreo.Text = dgvdata.Rows[indice].Cells["Correo"].Value.ToString();
                    txttelefono.Text = dgvdata.Rows[indice].Cells["Telefono"].Value.ToString();

                    foreach (OpcionCombo oc in cbocategoriaprov.Items)
                    {
                        if (Convert.ToInt32(oc.Valor) == Convert.ToInt32(dgvdata.Rows[indice].Cells["IdCategoriaProv"].Value))
                        {
                            int indice_combo = cbocategoriaprov.Items.IndexOf(oc);
                            cbocategoriaprov.SelectedIndex = indice_combo;
                            break;
                        }
                    }

                    // Asignar estado
                    foreach (OpcionCombo oc in cboestado.Items)
                    {
                        if (Convert.ToInt32(oc.Valor) == Convert.ToInt32(dgvdata.Rows[indice].Cells["EstadoValor"].Value))
                        {
                            cboestado.SelectedItem = oc;
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
                if (MessageBox.Show("¿Desea eliminar el proveedor?", "Mensaje", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    string mensaje;
                    Proveedor obj = new Proveedor { IdProveedor = Convert.ToInt32(txtid.Text) };

                    bool respuesta = new CN_Proveedor().Eliminar(obj, out mensaje);

                    if (respuesta)
                    {
                        // Elimina la fila del DataGridView
                        dgvdata.Rows.RemoveAt(Convert.ToInt32(txtindice.Text));
                        Limpiar();
                        MessageBox.Show(mensaje, "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show(mensaje, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
            }
        }

        private void btnbuscar_Click(object sender, EventArgs e)
        {
            // Verificar si hay una columna de búsqueda seleccionada
            if (cbobusqueda.SelectedItem == null)
            {
                MessageBox.Show("Seleccione una columna para buscar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string columnaFiltro = ((OpcionCombo)cbobusqueda.SelectedItem).Valor.ToString();

            // Limpiar el DataGridView si no hay datos
            if (dgvdata.Rows.Count == 0)
            {
                MessageBox.Show("No hay datos disponibles para buscar.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Primero, restablecer la visibilidad de todas las filas
            foreach (DataGridViewRow row in dgvdata.Rows)
            {
                row.Visible = true;
            }

            // Aplicar el filtro
            foreach (DataGridViewRow row in dgvdata.Rows)
            {
                var cellValue = row.Cells[columnaFiltro].Value;
                if (cellValue == null || !cellValue.ToString().Trim().ToUpper().Contains(txtbusqueda.Text.Trim().ToUpper()))
                {
                    row.Visible = false; // Ocultar fila si no coincide
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
    }
}