using System.ComponentModel.DataAnnotations;

namespace Proyecto_alcaldia.Application.ViewModels;

public class ProyectoViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "La alcaldía es obligatoria")]
    [Display(Name = "Alcaldía")]
    public int AlcaldiaId { get; set; }

    [Display(Name = "NIT Alcaldía")]
    public string? NitAlcaldia { get; set; }

    [Display(Name = "Municipio")]
    public string? MunicipioAlcaldia { get; set; }

    [Required(ErrorMessage = "El código es obligatorio")]
    [StringLength(20, ErrorMessage = "El código no puede exceder 20 caracteres")]
    [Display(Name = "Código")]
    public string Codigo { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(200, ErrorMessage = "El nombre no puede exceder 200 caracteres")]
    [Display(Name = "Nombre")]
    public string Nombre { get; set; } = string.Empty;

    [Display(Name = "Descripción")]
    public string? Descripcion { get; set; }

    [Display(Name = "Código BPIN")]
    public decimal? CodigoBPIN { get; set; }

    [Display(Name = "Presupuesto Asignado")]
    [DataType(DataType.Currency)]
    public decimal? PresupuestoAsignado { get; set; }

    [Display(Name = "Presupuesto Ejecutado")]
    [DataType(DataType.Currency)]
    public decimal? PresupuestoEjecutado { get; set; }

    [Display(Name = "Valor Proyecto")]
    [DataType(DataType.Currency)]
    public decimal? ValorProyecto { get; set; }

    [Display(Name = "Responsable")]
    public int? ResponsableId { get; set; }

    [Display(Name = "Nombre Responsable")]
    public string? NombreResponsable { get; set; }

    [Display(Name = "Fecha Inicio")]
    [DataType(DataType.Date)]
    public DateTime? FechaInicio { get; set; }

    [Display(Name = "Fecha Fin")]
    [DataType(DataType.Date)]
    public DateTime? FechaFin { get; set; }

    [Display(Name = "Estado Proyecto")]
    public bool? EstadoProyecto { get; set; }

    [Display(Name = "Porcentaje Avance")]
    [Range(0, 100, ErrorMessage = "El porcentaje debe estar entre 0 y 100")]
    public decimal? PorcentajeAvance { get; set; }

    [Display(Name = "Activo")]
    public bool Activo { get; set; } = true;

    [Display(Name = "Actividades")]
    public int CantidadActividades { get; set; }
}
