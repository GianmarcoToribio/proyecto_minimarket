using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaNegocio
{
    public class ClienteValidator : IValidator<Cliente>
    {
        public string Validar(Cliente obj)
        {
            StringBuilder sb = new StringBuilder();

            if (string.IsNullOrWhiteSpace(obj.Documento))
                sb.AppendLine("Es necesario el documento del Cliente");

            if (string.IsNullOrWhiteSpace(obj.NombreCompleto))
                sb.AppendLine("Es necesario el nombre completo del Cliente");

            if (string.IsNullOrWhiteSpace(obj.Correo))
                sb.AppendLine("Es necesario el correo del Cliente");

            return sb.ToString();
        }
    }
}


