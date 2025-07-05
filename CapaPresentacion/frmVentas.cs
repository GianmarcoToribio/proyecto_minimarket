using CapaEntidad;
using CapaNegocio;
using CapaPresentacion.Modales;
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
    public partial class frmVentas : Form
    {

        private Usuario _Usuario;
        public frmVentas(Usuario oUsuario = null)
        {
            _Usuario = oUsuario;
            InitializeComponent();
            CargarMetodosDePago(); // Asegúrate de cargar los métodos de pago
            cbotipopago.SelectedIndexChanged += cbotipopago_SelectedIndexChanged;
        }

        private void frmVentas_Load(object sender, EventArgs e)
        {
            cbotipodocumento.Items.Add(new OpcionCombo() { Valor = "Boleta", Texto = "Boleta" });
            cbotipodocumento.DisplayMember = "Texto";
            cbotipodocumento.ValueMember = "Valor";
            cbotipodocumento.SelectedIndex = 0;

            txtfecha.Text = DateTime.Now.ToString("dd/MM/yyyy");
            txtidproducto.Text = "0";
            txtpagocon.Text = "";
            txtcambio.Text = "";
            txttotalpagar.Text = "0";
        }


        private void btnbuscarcliente_Click(object sender, EventArgs e)
        {
            
        }

        private void btnbuscarproducto_Click(object sender, EventArgs e)
        {
            using (var modal = new mdProducto()) // No se pasa ID de categoría, muestra todos los productos
            {
                var result = modal.ShowDialog();

                if (result == DialogResult.OK && modal._Producto != null)
                {
                    txtidproducto.Text = modal._Producto.IdProducto.ToString();
                    txtcodproducto.Text = modal._Producto.Codigo;
                    txtproducto.Text = modal._Producto.Nombre;
                    txtprecio.Text = modal._Producto.PrecioVenta.ToString();
                    txtstock.Text = modal._Producto.Stock.ToString();
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
                    txtprecio.Text = oProducto.PrecioVenta.ToString("0.00");
                    txtstock.Text = oProducto.Stock.ToString();
                    txtcantidad.Select();
                }
                else
                {
                    txtcodproducto.BackColor = Color.MistyRose;
                    txtidproducto.Text = "0";
                    txtproducto.Text = "";
                    txtprecio.Text = "";
                    txtstock.Text = "";
                    txtcantidad.Value = 1;
                }
            }
        }

        private void btnagregarproducto_Click(object sender, EventArgs e)
        {
            if (int.Parse(txtidproducto.Text) == 0)
            {
                MessageBox.Show("Debe seleccionar un producto", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (!decimal.TryParse(txtprecio.Text, out decimal precio))
            {
                MessageBox.Show("Precio - Formato moneda incorrecto", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                txtprecio.Select();
                return;
            }

            if (Convert.ToInt32(txtstock.Text) < Convert.ToInt32(txtcantidad.Value))
            {
                MessageBox.Show("La cantidad no puede ser mayor al stock", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (!dgvdata.Rows.Cast<DataGridViewRow>().Any(fila => fila.Cells["IdProducto"].Value.ToString() == txtidproducto.Text))
            {
                if (new CN_Venta().RestarStock(Convert.ToInt32(txtidproducto.Text), Convert.ToInt32(txtcantidad.Value)))
                {
                    dgvdata.Rows.Add(new object[] {
                        txtidproducto.Text,
                        txtproducto.Text,
                        precio.ToString("0.00"),
                        txtcantidad.Value.ToString(),
                        (txtcantidad.Value * precio).ToString("0.00")
                    });

                    calcularTotal();
                    limpiarProducto();
                    txtcodproducto.Select();
                }
            }
        }

        private void CargarMetodosDePago()
        {

            cbotipopago.Items.Clear(); // Limpia las opciones anteriores
            cbotipopago.Items.Add(new OpcionCombo { Texto = "Yape", Valor = 1 });
            cbotipopago.Items.Add(new OpcionCombo { Texto = "Plin", Valor = 2 });
            cbotipopago.Items.Add(new OpcionCombo { Texto = "Efectivo", Valor = 3 });

            // Establece las propiedades de visualización y valor
            cbotipopago.DisplayMember = "Texto"; // Propiedad que se mostrará en el ComboBox
            cbotipopago.ValueMember = "Valor";    // Propiedad que será el valor del ComboBox
            cbotipopago.SelectedIndex = 0; // Opcional: seleccionar el primer elemento

        }

        private void ControlarCamposPago()
        {
            if (cbotipopago.SelectedItem is OpcionCombo metodoPagoSeleccionado)
            {
                // Asegúrate de mostrar el mensaje solo si hay una selección válida
                if (cbotipopago.SelectedIndex != -1)
                {
                    MessageBox.Show($"Método de pago seleccionado: {metodoPagoSeleccionado.Texto}");
                }

                if (metodoPagoSeleccionado.Texto == "Yape" || metodoPagoSeleccionado.Texto == "Plin")
                {
                    txtpagocon.Enabled = false;
                    txtcambio.Enabled = false;

                    // Limpia los campos para evitar valores previos
                    txtpagocon.Text = "";
                    txtcambio.Text = "";
                }
                else
                {
                    // Habilita los campos "Paga con" y "Cambio"
                    txtpagocon.Enabled = true;
                    txtcambio.Enabled = true;
                }
            }
        }

        // Asegúrate de llamar a este método en el evento SelectedIndexChanged del ComboBox
        private void cbotipopago_SelectedIndexChanged(object sender, EventArgs e)
        {
            ControlarCamposPago();
        }

        private void calcularTotal()
        {
            decimal total = 0;

            if (dgvdata.Rows.Count > 0)
            {
                foreach (DataGridViewRow row in dgvdata.Rows)
                {
                    // Verifica si la celda "SubTotal" no está vacía
                    if (row.Cells["SubTotal"].Value != null)
                    {
                        total += Convert.ToDecimal(row.Cells["SubTotal"].Value.ToString());
                    }
                }
            }

            // Actualiza el campo de texto donde se muestra el total
            txttotalpagar.Text = total.ToString("0.00");
        }


        private void limpiarProducto()
        {
            txtidproducto.Text = "0";
            txtcodproducto.Text = "";
            txtproducto.Text = "";
            txtprecio.Text = "";
            txtstock.Text = "";
            txtcantidad.Value = 1;
        }

        private void dgvdata_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0) return;

            // Verifica si es la fila de los encabezados
            if (e.RowIndex < 0)
                return;

            // Dibuja el ícono de eliminar (columna 5)
            if (e.ColumnIndex == 5)  // Columna para Eliminar
            {
                e.Paint(e.CellBounds, DataGridViewPaintParts.All);

                var w = Properties.Resources.delete25.Width;
                var h = Properties.Resources.delete25.Height;
                var x = e.CellBounds.Left + (e.CellBounds.Width - w) / 2;
                var y = e.CellBounds.Top + (e.CellBounds.Height - h) / 2;

                e.Graphics.DrawImage(Properties.Resources.delete25, new Rectangle(x, y, w, h));
                e.Handled = true;
            }

            // Dibuja el ícono de editar (columna 6)
            if (e.ColumnIndex == 6)  // Columna para Editar
            {
                e.Paint(e.CellBounds, DataGridViewPaintParts.All);

                var w = Properties.Resources.edit25.Width;  // Asegúrate de tener este ícono de editar
                var h = Properties.Resources.edit25.Height;
                var x = e.CellBounds.Left + (e.CellBounds.Width - w) / 2;
                var y = e.CellBounds.Top + (e.CellBounds.Height - h) / 2;

                e.Graphics.DrawImage(Properties.Resources.edit25, new Rectangle(x, y, w, h));
                e.Handled = true;
            }
        }

        /*private void dgvdata_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Verifica si se hizo clic en la columna de Eliminar
            if (e.ColumnIndex == 5)  // Columna 5 es para eliminar
            {
                int index = e.RowIndex;

                if (index >= 0)
                {
                    bool respuesta = new CN_Venta().SumarStock(
                        Convert.ToInt32(dgvdata.Rows[index].Cells["IdProducto"].Value.ToString()),
                        Convert.ToInt32(dgvdata.Rows[index].Cells["Cantidad"].Value.ToString())
                    );

                    if (respuesta)
                    {
                        dgvdata.Rows.RemoveAt(index);
                        calcularTotal();
                    }
                }
            }

            // Verifica si se hizo clic en la columna de Editar
            if (e.ColumnIndex == 6) // Editar
            {
                int index = e.RowIndex;
                if (index >= 0)
                {
                    int idProducto = Convert.ToInt32(dgvdata.Rows[index].Cells["IdProducto"].Value);
                    int cantidad = Convert.ToInt32(dgvdata.Rows[index].Cells["Cantidad"].Value);
                    decimal precioProducto = Convert.ToDecimal(dgvdata.Rows[index].Cells["Precio"].Value);

                    // Pide la nueva cantidad
                    var nuevaCantidad = Prompt.ShowDialog("Ingrese la nueva cantidad:", "Editar cantidad", cantidad.ToString());

                    if (int.TryParse(nuevaCantidad, out int cantidadEditada))
                    {
                        // 1. Restar el stock original
                        new CN_Venta().SumarStock(idProducto, cantidad);

                        // 2. Sumar el stock de la nueva cantidad
                        new CN_Venta().RestarStock(idProducto, cantidadEditada);

                        // Actualizar la fila del DataGridView
                        dgvdata.Rows[index].Cells["Cantidad"].Value = cantidadEditada;
                        decimal subTotal = precioProducto * cantidadEditada;
                        dgvdata.Rows[index].Cells["SubTotal"].Value = subTotal;

                        calcularTotal();
                    }
                    else
                    {
                        MessageBox.Show("La cantidad no es válida.");
                    }
                }
            }

        }*/


        private void dgvdata_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Verifica si se hizo clic en la columna de Eliminar
            if (e.ColumnIndex == 5)  // Columna 5 es para eliminar
            {
                int index = e.RowIndex;

                if (index >= 0)
                {
                    bool respuesta = new CN_Venta().SumarStock(
                        Convert.ToInt32(dgvdata.Rows[index].Cells["IdProducto"].Value.ToString()),
                        Convert.ToInt32(dgvdata.Rows[index].Cells["Cantidad"].Value.ToString())
                    );

                    if (respuesta)
                    {
                        dgvdata.Rows.RemoveAt(index);
                        calcularTotal();
                    }
                }
            }

            // Verifica si se hizo clic en la columna de Editar
            if (e.ColumnIndex == 6) // Editar
            {
                int index = e.RowIndex;
                if (index >= 0)
                {
                    int idProducto = Convert.ToInt32(dgvdata.Rows[index].Cells["IdProducto"].Value);
                    int cantidad = Convert.ToInt32(dgvdata.Rows[index].Cells["Cantidad"].Value);
                    decimal precioProducto = Convert.ToDecimal(dgvdata.Rows[index].Cells["Precio"].Value);

                    // Pide la nueva cantidad
                    var nuevaCantidad = Prompt.ShowDialog("Ingrese la nueva cantidad:", "Editar cantidad", cantidad.ToString());

                    if (int.TryParse(nuevaCantidad, out int cantidadEditada))
                    {
                        // Validar que la cantidad editada no sea mayor que el stock disponible
                        // Aquí solo verificamos que la cantidad no sea mayor que la cantidad original
                        if (cantidadEditada > cantidad)
                        {
                            MessageBox.Show("La cantidad no puede ser mayor que la cantidad original", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            return;  // Salir sin realizar cambios
                        }

                        // Validar que la cantidad editada no sea menor o igual a 0
                        if (cantidadEditada <= 0)
                        {
                            MessageBox.Show("La cantidad no puede ser menor o igual a 0", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            return;  // Salir sin realizar cambios
                        }


                        // 1. Restar el stock original
                        new CN_Venta().SumarStock(idProducto, cantidad);

                        // 2. Sumar el stock de la nueva cantidad
                        new CN_Venta().RestarStock(idProducto, cantidadEditada);

                        // Actualizar la fila del DataGridView
                        dgvdata.Rows[index].Cells["Cantidad"].Value = cantidadEditada;
                        decimal subTotal = precioProducto * cantidadEditada;
                        dgvdata.Rows[index].Cells["SubTotal"].Value = subTotal;

                        calcularTotal();
                    }
                    else
                    {
                        MessageBox.Show("La cantidad no es válida.");
                    }
                }
            }
        }



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


        private void txtprecio_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Char.IsDigit(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                if (txtprecio.Text.Trim().Length == 0 && e.KeyChar.ToString() == ".")
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

        private void txtpagocon_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Char.IsDigit(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                if (txtpagocon.Text.Trim().Length == 0 && e.KeyChar.ToString() == ".")
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

        private void calcularcambio()
        {

            if (txttotalpagar.Text.Trim() == "")
            {
                MessageBox.Show("No existen productos en la venta", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }


            decimal pagacon;
            decimal total = Convert.ToDecimal(txttotalpagar.Text);

            if (txtpagocon.Text.Trim() == "")
            {
                txtpagocon.Text = "0";
            }

            if (decimal.TryParse(txtpagocon.Text.Trim(), out pagacon))
            {

                if (pagacon < total)
                {
                    txtcambio.Text = "0.00";
                }
                else
                {
                    decimal cambio = pagacon - total;
                    txtcambio.Text = cambio.ToString("0.00");

                }
            }



        }

        private void txtpagocon_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                calcularcambio();
            }
        }

        private void btncrearventa_Click(object sender, EventArgs e)
        {
            // Verifica si el ComboBox tiene elementos
            if (cbotipopago.Items.Count == 0)
            {
                MessageBox.Show("No hay métodos de pago disponibles.", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Salir si no hay métodos de pago
            }

            // Verifica el método de pago seleccionado
            var metodoPagoSeleccionado = ((OpcionCombo)cbotipopago.SelectedItem)?.Texto;
            if (string.IsNullOrWhiteSpace(metodoPagoSeleccionado))
            {
                MessageBox.Show("Debe seleccionar un método de pago.", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Salir si no se ha seleccionado un método de pago
            }

            // Validación del campo "Pago con" solo si el método de pago es "Efectivo"
            decimal pagocon = 0;
            if (metodoPagoSeleccionado == "Efectivo")
            {
                if (decimal.TryParse(txtpagocon.Text, out pagocon))
                {
                    if (pagocon > 200)
                    {
                        MessageBox.Show("Solo se permite valor menor o igual a 200 en el campo 'Pago con'", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        txtpagocon.Select(); // Selecciona el campo para corregir
                        return; // Salir si hay error
                    }
                }
                else
                {
                    MessageBox.Show("Formato de entrada inválido en el campo 'Pago con'", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    txtpagocon.Select(); // Selecciona el campo para corregir
                    return; // Salir si el valor no es numérico
                }
            }

            // Validaciones de cliente y productos
            if (string.IsNullOrWhiteSpace(txtdocumentocliente.Text))
            {
                MessageBox.Show("Debe ingresar documento del cliente", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtnombrecliente.Text))
            {
                MessageBox.Show("Debe ingresar nombre del cliente", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (dgvdata.Rows.Count < 1)
            {
                MessageBox.Show("Debe ingresar productos en la venta", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            // Calcular el cambio solo si es efectivo
            if (metodoPagoSeleccionado == "Efectivo")
            {
                calcularcambio();
            }

            // Validar que el monto pagado sea mayor o igual al total solo si es efectivo
            decimal total = Convert.ToDecimal(txttotalpagar.Text);
            if (metodoPagoSeleccionado == "Efectivo" && pagocon < total)
            {
                MessageBox.Show("El monto pagado debe ser mayor o igual al total a pagar.", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            // Crear detalle de venta
            DataTable detalle_venta = new DataTable();
            detalle_venta.Columns.Add("IdProducto", typeof(int));
            detalle_venta.Columns.Add("PrecioVenta", typeof(decimal));
            detalle_venta.Columns.Add("Cantidad", typeof(int));
            detalle_venta.Columns.Add("SubTotal", typeof(decimal));

            foreach (DataGridViewRow row in dgvdata.Rows)
            {
                detalle_venta.Rows.Add(new object[] {
            row.Cells["IdProducto"].Value.ToString(),
            row.Cells["Precio"].Value.ToString(),
            row.Cells["Cantidad"].Value.ToString(),
            row.Cells["SubTotal"].Value.ToString()
        });
            }

            // Crear objeto venta
            int idcorrelativo = new CN_Venta().ObtenerCorrelativo();
            string numeroDocumento = string.Format("{0:00000}", idcorrelativo);

            Venta oVenta = new Venta()
            {
                oUsuario = new Usuario() { IdUsuario = _Usuario.IdUsuario },
                TipoDocumento = ((OpcionCombo)cbotipodocumento.SelectedItem).Texto,
                NumeroDocumento = numeroDocumento,
                DocumentoCliente = txtdocumentocliente.Text,
                NombreCliente = txtnombrecliente.Text,
                MontoPago = pagocon,
                MetodoPago = metodoPagoSeleccionado,
                MontoCambio = metodoPagoSeleccionado == "Efectivo" ? Convert.ToDecimal(txtcambio.Text) : 0, // Solo calcular cambio si es efectivo
                MontoTotal = total
            };

            // Registrar venta
            string mensaje = string.Empty;
            bool respuesta = new CN_Venta().Registrar(oVenta, detalle_venta, out mensaje);

            if (respuesta)
            {
                var result = MessageBox.Show("Numero de venta generada:\n" + numeroDocumento + "\n\n¿Desea copiar al portapapeles?", "Mensaje", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                if (result == DialogResult.Yes)
                    Clipboard.SetText(numeroDocumento);

                txtdocumentocliente.Text = "";
                txtnombrecliente.Text = "";
                dgvdata.Rows.Clear();
                calcularTotal();
                txtpagocon.Text = "";
                txtcambio.Text = "";
                btnbuscarcliente.Enabled = true;
            }
            else
            {
                MessageBox.Show(mensaje, "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
    }
}