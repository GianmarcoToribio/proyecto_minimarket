using CapaEntidad;
using CapaNegocio;
using CapaPresentacion.Modales;
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
using static CapaPresentacion.frmVentas;

namespace CapaPresentacion
{
    public partial class frmCompras : Form
    {

        private Usuario _Usuario;

        public frmCompras(Usuario oUsuario = null)
        {
            _Usuario = oUsuario;
            InitializeComponent();
        }

        private void frmCompras_Load(object sender, EventArgs e)
        {
            cbotipodocumento.Items.Add(new OpcionCombo() { Valor = "Boleta", Texto = "Boleta" });
            cbotipodocumento.DisplayMember = "Texto";
            cbotipodocumento.ValueMember = "Valor";
            cbotipodocumento.SelectedIndex = 0;

            txtfecha.Text = DateTime.Now.ToString("dd/MM/yyyy");

            txtidproveedor.Text = "0";
            txtidproducto.Text = "0";
            txtcategoriaProveedor.Text = "";
        }

        private int _idCategoriaProveedor; // Nueva variable para almacenar la categoría del proveedor

        private void btnbuscarproveedor_Click(object sender, EventArgs e)
        {
            using (var modal = new mdProveedor())
            {
                var result = modal.ShowDialog();

                if (result == DialogResult.OK)
                {
                    if (modal._Proveedor != null)
                    {
                        txtidproveedor.Text = modal._Proveedor.IdProveedor.ToString();
                        txtdocproveedor.Text = modal._Proveedor.Documento;
                        txtnombreproveedor.Text = modal._Proveedor.RazonSocial;

                        // Guardar la categoría del proveedor
                        if (modal._Proveedor.oCategoria != null)
                        {
                            _idCategoriaProveedor = modal._Proveedor.oCategoria.IdCategoria;
                            txtcategoriaProveedor.Text = modal._Proveedor.oCategoria.Descripcion;
                        }
                        else
                        {
                            txtcategoriaProveedor.Text = "Sin categoría";
                            _idCategoriaProveedor = 0; // Sin categoría
                        }

                        // Deshabilitar los campos del proveedor
                        txtidproveedor.Enabled = false;
                        btnbuscarproveedor.Enabled = false; // Deshabilitar botón de búsqueda de proveedor
                    }
                    else
                    {
                        MessageBox.Show("No se seleccionó ningún proveedor.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    txtdocproveedor.Select();
                }
            }

        }

        private void btnbuscarproducto_Click(object sender, EventArgs e)
        {
            int idProveedor = Convert.ToInt32(txtidproveedor.Text);
            Proveedor proveedor = new CN_Proveedor().ObtenerProveedorPorId(idProveedor);

            // Verificar que el proveedor tiene categoría
            if (proveedor?.oCategoria == null)
            {
                MessageBox.Show("El proveedor no tiene una categoría asignada.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var modal = new mdProducto(proveedor.oCategoria.IdCategoria)) // Pasa el ID de la categoría
            {
                var result = modal.ShowDialog();

                if (result == DialogResult.OK && modal._Producto != null)
                {
                    txtidproducto.Text = modal._Producto.IdProducto.ToString();
                    txtcodproducto.Text = modal._Producto.Codigo;
                    txtproducto.Text = modal._Producto.Nombre;
                    txtpreciocompra.Select();
                }
                else
                {
                    txtcodproducto.Select();
                }
            }
        }

        private void txtcodproducto_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {

                Producto oProducto = new CN__Producto().Listar().Where(p => p.Codigo == txtcodproducto.Text && p.Estado == true).FirstOrDefault();

                if (oProducto != null)
                {
                    txtcodproducto.BackColor = Color.Honeydew;
                    txtidproducto.Text = oProducto.IdProducto.ToString();
                    txtproducto.Text = oProducto.Nombre;
                    txtpreciocompra.Select();
                }
                else
                {
                    txtcodproducto.BackColor = Color.MistyRose;
                    txtidproducto.Text = "0";
                    txtproducto.Text = "";
                }


            }
        }

        private void btnagregarproducto_Click(object sender, EventArgs e)
        {
            

            decimal preciocompra = 0;
            decimal precioventa = 0;
            bool producto_existe = false;

            if (int.Parse(txtidproducto.Text) == 0)
            {
                MessageBox.Show("Debe seleccionar un producto", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (!decimal.TryParse(txtpreciocompra.Text, out preciocompra))
            {
                MessageBox.Show("Precio Compra - Formato moneda incorrecto", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                txtpreciocompra.Select();
                return;
            }

            if (preciocompra <= 0)
            {
                MessageBox.Show("El precio de compra debe ser mayor que 0", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                txtpreciocompra.Select();
                return;
            }

            if (!decimal.TryParse(txtprecioventa.Text, out precioventa))
            {
                MessageBox.Show("Precio Venta - Formato moneda incorrecto", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                txtprecioventa.Select();
                return;
            }

            if (precioventa <= 0)
            {
                MessageBox.Show("El precio de venta debe ser mayor que 0", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                txtprecioventa.Select();
                return;
            }

            // Nueva validación: Precio de venta no puede ser menor al precio de compra
            if (precioventa <= preciocompra)
            {
                MessageBox.Show("El precio de venta debe ser mayor al precio de compra", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                txtprecioventa.Select();
                return;
            }

            foreach (DataGridViewRow fila in dgvdata.Rows)
            {
                if (fila.Cells["IdProducto"].Value.ToString() == txtidproducto.Text)
                {
                    producto_existe = true;
                    break;
                }
            }

            if (!producto_existe)
            {
                dgvdata.Rows.Add(new object[] {
            txtidproducto.Text,
            txtproducto.Text,
            preciocompra.ToString("0.00"),
            precioventa.ToString("0.00"),
            txtcantidad.Value.ToString(),
            (txtcantidad.Value * preciocompra).ToString("0.00")
        });

                calcularTotal();
                limpiarProducto();
                txtcodproducto.Select();
            }
        }


        private void limpiarProducto()
        {
            txtidproducto.Text = "0";
            txtcodproducto.Text = "";
            txtcodproducto.BackColor = Color.White;
            txtproducto.Text = "";
            txtcategoriaProveedor.BackColor = Color.White;
            txtpreciocompra.Text = "";
            txtprecioventa.Text = "";
            txtcantidad.Value = 1;
        }

        /*private void calcularTotal()
        {
            decimal total = 0;
            if (dgvdata.Rows.Count > 0)
            {
                foreach (DataGridViewRow row in dgvdata.Rows)
                    total += Convert.ToDecimal(row.Cells["SubTotal"].Value.ToString());
            }
            txttotalpagar.Text = total.ToString("0.00");
        }*/

        private void calcularTotal()
        {
            decimal total = 0;

            foreach (DataGridViewRow row in dgvdata.Rows)
            {
                if (row.Cells["SubTotal"].Value != null)
                {
                    total += Convert.ToDecimal(row.Cells["SubTotal"].Value);
                }
            }

            txttotalpagar.Text = total.ToString("0.00"); // Actualiza el campo del total a pagar
        }


        /*private void dgvdata_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            if (e.ColumnIndex == 6)
            {

                e.Paint(e.CellBounds, DataGridViewPaintParts.All);

                var w = Properties.Resources.delete25.Width;
                var h = Properties.Resources.delete25.Height;
                var x = e.CellBounds.Left + (e.CellBounds.Width - w) / 2;
                var y = e.CellBounds.Top + (e.CellBounds.Height - h) / 2;

                e.Graphics.DrawImage(Properties.Resources.delete25, new Rectangle(x, y, w, h));
                e.Handled = true;
            }
        }

        private void dgvdata_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

            if (dgvdata.Columns[e.ColumnIndex].Name == "btneliminar")
            {
                int indice = e.RowIndex;

                if (indice >= 0)
                {
                    dgvdata.Rows.RemoveAt(indice);
                    calcularTotal();
                }
            }
        }*/


        // Este es un método para mostrar un cuadro de diálogo para ingresar un valor
        public static class Prompt
        {
            public static string ShowDialog(string text, string caption, string defaultValue = "")
            {
                Form prompt = new Form()
                {
                    Width = 400,
                    Height = 150,
                    Text = caption,
                    StartPosition = FormStartPosition.CenterScreen
                };

                Label textLabel = new Label() { Left = 50, Top = 20, Text = text };
                TextBox textBox = new TextBox() { Left = 50, Top = 50, Width = 300, Text = defaultValue };
                Button confirmation = new Button() { Text = "OK", Left = 250, Width = 100, Top = 70, DialogResult = DialogResult.OK };
                confirmation.Click += (sender, e) => { prompt.Close(); };
                prompt.Controls.Add(textLabel);
                prompt.Controls.Add(textBox);
                prompt.Controls.Add(confirmation);
                prompt.AcceptButton = confirmation;

                return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : defaultValue;
            }
        }

        private void dgvdata_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            // Ícono de Eliminar
            if (e.ColumnIndex == 6) // Ajusta el índice de columna si es necesario
            {
                e.Paint(e.CellBounds, DataGridViewPaintParts.All);

                var w = Properties.Resources.delete25.Width;
                var h = Properties.Resources.delete25.Height;
                var x = e.CellBounds.Left + (e.CellBounds.Width - w) / 2;
                var y = e.CellBounds.Top + (e.CellBounds.Height - h) / 2;

                e.Graphics.DrawImage(Properties.Resources.delete25, new Rectangle(x, y, w, h));
                e.Handled = true;
            }

            // Ícono de Editar
            if (e.ColumnIndex == 7) // Ajusta el índice de columna si es necesario
            {
                e.Paint(e.CellBounds, DataGridViewPaintParts.All);

                var w = Properties.Resources.edit25.Width;
                var h = Properties.Resources.edit25.Height;
                var x = e.CellBounds.Left + (e.CellBounds.Width - w) / 2;
                var y = e.CellBounds.Top + (e.CellBounds.Height - h) / 2;

                e.Graphics.DrawImage(Properties.Resources.edit25, new Rectangle(x, y, w, h));
                e.Handled = true;
            }
        }

        private void dgvdata_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Botón de Eliminar
            if (dgvdata.Columns[e.ColumnIndex].Name == "btneliminar")
            {
                int index = e.RowIndex;

                if (index >= 0)
                {
                    dgvdata.Rows.RemoveAt(index);
                    calcularTotal(); // Actualiza el total después de eliminar
                }
            }

            // Botón de Editar
            if (dgvdata.Columns[e.ColumnIndex].Name == "btneditar")
            {
                int index = e.RowIndex;

                if (index >= 0)
                {
                    // Obtener valores de la fila seleccionada
                    decimal precioCompraActual = Convert.ToDecimal(dgvdata.Rows[index].Cells["PrecioCompra"].Value);
                    decimal precioVentaActual = Convert.ToDecimal(dgvdata.Rows[index].Cells["PrecioVenta"].Value);
                    int cantidadActual = Convert.ToInt32(dgvdata.Rows[index].Cells["Cantidad"].Value);

                    // Solicitar nuevos valores al usuario
                    var nuevoPrecioCompra = Prompt.ShowDialog("Ingrese el nuevo precio de compra:", "Editar Precio Compra", precioCompraActual.ToString());
                    var nuevoPrecioVenta = Prompt.ShowDialog("Ingrese el nuevo precio de venta:", "Editar Precio Venta", precioVentaActual.ToString());
                    var nuevaCantidad = Prompt.ShowDialog("Ingrese la nueva cantidad:", "Editar Cantidad", cantidadActual.ToString());

                    // Validar y actualizar los valores
                    if (decimal.TryParse(nuevoPrecioCompra, out decimal precioCompraEditado) &&
                        decimal.TryParse(nuevoPrecioVenta, out decimal precioVentaEditado) &&
                        int.TryParse(nuevaCantidad, out int cantidadEditada))
                    {
                        // Actualizar valores en el DataGridView
                        dgvdata.Rows[index].Cells["PrecioCompra"].Value = precioCompraEditado;
                        dgvdata.Rows[index].Cells["PrecioVenta"].Value = precioVentaEditado;
                        dgvdata.Rows[index].Cells["Cantidad"].Value = cantidadEditada;

                        // Recalcular el SubTotal basado en PrecioCompra y Cantidad
                        dgvdata.Rows[index].Cells["SubTotal"].Value = precioCompraEditado * cantidadEditada;

                        // Recalcular el total general
                        calcularTotal();
                    }
                    else
                    {
                        MessageBox.Show("Valores no válidos. Por favor, ingrese datos numéricos correctos.");
                    }
                }
            }
        }


        private void txtpreciocompra_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Char.IsDigit(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                if (txtpreciocompra.Text.Trim().Length == 0 && e.KeyChar.ToString() == ".")
                {
                    e.Handled = true;
                }
                else
                {
                    if (Char.IsControl(e.KeyChar) || e.KeyChar.ToString() == ".")
                    {
                        e.Handled = false;
                    }
                    else
                    {
                        e.Handled = true;
                    }
                }
            }
        }

        private void txtprecioventa_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Char.IsDigit(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                if (txtprecioventa.Text.Trim().Length == 0 && e.KeyChar.ToString() == ".")
                {
                    e.Handled = true;
                }
                else
                {
                    if (Char.IsControl(e.KeyChar) || e.KeyChar.ToString() == ".")
                    {
                        e.Handled = false;
                    }
                    else
                    {
                        e.Handled = true;
                    }
                }
            }
        }

        private void btnregistrar_Click(object sender, EventArgs e)
        {
            if (Convert.ToInt32(txtidproveedor.Text) == 0)
            {
                MessageBox.Show("Debe seleccionar un proveedor", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (dgvdata.Rows.Count < 1)
            {
                MessageBox.Show("Debe ingresar productos en la compra", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            DataTable detalle_compra = new DataTable();

            detalle_compra.Columns.Add("IdProducto", typeof(int));
            detalle_compra.Columns.Add("PrecioCompra", typeof(decimal));
            detalle_compra.Columns.Add("PrecioVenta", typeof(decimal));
            detalle_compra.Columns.Add("Cantidad", typeof(int));
            detalle_compra.Columns.Add("MontoTotal", typeof(decimal));

            foreach (DataGridViewRow row in dgvdata.Rows)
            {
                detalle_compra.Rows.Add(
                    new object[] {
               Convert.ToInt32(row.Cells["IdProducto"].Value.ToString()),
               row.Cells["PrecioCompra"].Value.ToString(),
               row.Cells["PrecioVenta"].Value.ToString(),
               row.Cells["Cantidad"].Value.ToString(),
               row.Cells["SubTotal"].Value.ToString()
                    });
            }

            int idcorrelativo = new CN_Compra().ObtenerCorrelativo();
            string numerodocumento = string.Format("{0:00000}", idcorrelativo);

            Compra oCompra = new Compra()
            {
                oUsuario = new Usuario() { IdUsuario = _Usuario.IdUsuario },
                oProveedor = new Proveedor() { IdProveedor = Convert.ToInt32(txtidproveedor.Text) },
                TipoDocumento = ((OpcionCombo)cbotipodocumento.SelectedItem).Texto,
                NumeroDocumento = numerodocumento,
                MontoTotal = Convert.ToDecimal(txttotalpagar.Text)
            };

            string mensaje = string.Empty;
            bool respuesta = new CN_Compra().Registrar(oCompra, detalle_compra, out mensaje);

            if (respuesta)
            {
                var result = MessageBox.Show("Numero de compra generada:\n" + numerodocumento + "\n\n¿Desea copiar al portapapeles?", "Mensaje", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                if (result == DialogResult.Yes)
                    Clipboard.SetText(numerodocumento);

                // Volver a habilitar los campos del proveedor
                txtidproveedor.Enabled = true;
                btnbuscarproveedor.Enabled = true;

                txtidproveedor.Text = "0";
                txtdocproveedor.Text = "";
                txtcategoriaProveedor.Text = "";
                txtnombreproveedor.Text = "";
                
                dgvdata.Rows.Clear();
                calcularTotal();
            }
            else
            {
                MessageBox.Show(mensaje, "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

        }
    }
}