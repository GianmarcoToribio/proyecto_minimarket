using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CapaDatos;
using CapaEntidad;
namespace CapaNegocio
{
    public class CN_Usuario
    {

        private CD_Usuario objcd_usuario = new CD_Usuario();


        public List<Usuario> Listar()
        {
            return objcd_usuario.Listar();
        }

        public int Registrar(Usuario obj,out string Mensaje)
        {
            Mensaje = string.Empty;

            if (obj.Documento == "") {
                Mensaje += "Es necesario el documento del usuario\n";
            }

            if (obj.NombreCompleto == "")
            {
                Mensaje += "Es necesario el nombre completo del usuario\n";
            }
            
            if (obj.Clave == "")
            {
                Mensaje += "Es necesario la clave del usuario\n";
            }

            if (Mensaje != string.Empty)
            {
                return 0;
            }
            else {
                return objcd_usuario.Registrar(obj, out Mensaje);
            }

            
        }


        public bool Editar(Usuario obj, out string Mensaje)
        {

            Mensaje = string.Empty;

            if (obj.Documento == "")
            {
                Mensaje += "Es necesario el documento del usuario\n";
            }

            if (obj.NombreCompleto == "")
            {
                Mensaje += "Es necesario el nombre completo del usuario\n";
            }

            if (obj.Clave == "")
            {
                Mensaje += "Es necesario la clave del usuario\n";
            }


            if (Mensaje != string.Empty)
            {
                return false;
            }
            else
            {
                return objcd_usuario.Editar(obj, out Mensaje);
            }

            
        }


        public bool Eliminar(Usuario obj, out string Mensaje)
        {
            return objcd_usuario.Eliminar(obj, out Mensaje);
        }

        public Usuario IniciarSesion(string documento, string clave)
        {
            Usuario usuario = objcd_usuario.Listar().FirstOrDefault(u => u.Documento == documento && u.Clave == clave);

            // Verificar el estado del usuario
            if (usuario != null && usuario.Estado)
            {
                return usuario; // El usuario está activo
            }

            return null; // Usuario no encontrado o inactivo
        }


    }
}
