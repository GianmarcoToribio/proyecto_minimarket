using CapaDatos;
using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapaNegocio;

namespace CapaNegocio
{
    public class CN_Cliente
    {
        private readonly CD_Cliente _objcd_Cliente = new CD_Cliente();
        private readonly IValidator<Cliente> _validator;

        /// Constructor principal: recibe la abstracción IValidator<Cliente>.
        
        public CN_Cliente(IValidator<Cliente> validator)
        {
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        
        /// Constructor por defecto: inyecta la implementación concreta ClienteValidator.
        
        public CN_Cliente() : this(new ClienteValidator())
        {
        }

        /// Lista todos los clientes.
        
        public List<Cliente> Listar()
        {
            return _objcd_Cliente.Listar();
        }

        
        /// Registra un nuevo cliente tras validar sus datos.
        
        public int Registrar(Cliente obj, out string Mensaje)
        {
            Mensaje = _validator.Validar(obj);
            if (!string.IsNullOrEmpty(Mensaje))
                return 0;

            return _objcd_Cliente.Registrar(obj, out Mensaje);
        }

        
        /// Edita un cliente existente tras validar sus datos.
        
        public bool Editar(Cliente obj, out string Mensaje)
        {
            Mensaje = _validator.Validar(obj);
            if (!string.IsNullOrEmpty(Mensaje))
                return false;

            return _objcd_Cliente.Editar(obj, out Mensaje);
        }

        
        /// Elimina un cliente por su identificador.
        
        public bool Eliminar(Cliente obj, out string Mensaje)
        {
            return _objcd_Cliente.Eliminar(obj, out Mensaje);
        }
    }
}


