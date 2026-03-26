using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ParkYa.Models
{
    [Table("tipo_vehiculo")]
    public class TipoVehiculo
    {
        [Key]
        [Column("idTipo_vehiculo")]
        public int TipoVehiculoId { get; set; }

        [Column("Cod_TipoVehiculo")]
        public string cod_tipoVehiculo { get; set; } = string.Empty;

        [Column("nombre_tipo")]
        public string nombre_tipo { get; set; } = string.Empty;

    }
}