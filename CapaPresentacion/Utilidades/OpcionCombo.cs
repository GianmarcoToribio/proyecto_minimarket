using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaPresentacion.Utilidades
{
    public class OpcionCombo
    {
        public string Texto { get; set; }
        public object Valor { get; set; }

        public override string ToString()
        {
            return Texto; // Esto permite que el ComboBox muestre el texto en lugar del objeto
        }
    }
}
