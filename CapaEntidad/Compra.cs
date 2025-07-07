<<<<<<< HEAD
using System;
=======
﻿using System;
>>>>>>> 9b41abcb213c3712a82914f6c4b7c702576dd483
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class Compra
    {
        public int IdCompra { get; set; }
        public Usuario oUsuario { get; set; }
<<<<<<< HEAD
=======
        public Proveedor oProveedor { get; set; }
>>>>>>> 9b41abcb213c3712a82914f6c4b7c702576dd483
        public string TipoDocumento { get; set; }
        public string NumeroDocumento { get; set; }
        public decimal MontoTotal { get; set; }
        public List<Detalle_Compra> oDetalleCompra { get; set; }
        public string FechaRegistro { get; set; }
    }
}
