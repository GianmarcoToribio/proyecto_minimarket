using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using CapaNegocio;
using CapaEntidad;

namespace CapaPresentacion
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }

        private void btncancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btningresar_Click(object sender, EventArgs e)
        {
            string documento = txtdocumento.Text; // Campo para el documento
            string clave = txtclave.Text; // Campo para la contraseña

            // Validar el inicio de sesión
            Usuario ousuario = new CN_Usuario().IniciarSesion(documento, clave);

            if (ousuario == null)
            {
                MessageBox.Show("Error: Documento o contraseña incorrectos o cuenta inactiva.", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                LimpiarCampos(); // Llamar al método de limpieza
                return; // Salir del método si las credenciales son incorrectas o el usuario está inactivo
            }

            // Si pasa las validaciones, iniciar sesión
            Inicio form = new Inicio(ousuario);
            form.Show();
            this.Hide();
            form.FormClosing += frm_closing;
        }

        // Método para limpiar los campos de texto
        private void LimpiarCampos()
        {
            txtdocumento.Text = ""; // Limpiar el campo de documento
            txtclave.Text = ""; // Limpiar el campo de contraseña
            txtdocumento.Focus(); // Opcional: Establecer el foco en el campo de documento
        }

        private void frm_closing(object sender, FormClosingEventArgs e)
        {
            LimpiarCampos(); // Limpiar los campos al cerrar el formulario de inicio
            this.Show();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
