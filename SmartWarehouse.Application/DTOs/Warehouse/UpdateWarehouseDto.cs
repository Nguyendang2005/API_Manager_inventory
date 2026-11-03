using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SmartWarehouse.Application.DTOs.Warehouse
{
    public class UpdateWarehouseDto
    {

        [Required(ErrorMessage = "Warehouse code is required.")]
        [StringLength(20, ErrorMessage = "Warehouse code must not exceed 20 characters.")]
        public string Code { get; set; } = string.Empty;
        [Required(ErrorMessage = "Warehouse name is required.")]
        [StringLength(150, ErrorMessage = "Warehouse name must not exceed 150 characters.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(255,ErrorMessage = "Address must not exceed 255 characters.")]
        public string? Address { get; set; }
        [Required(ErrorMessage = "Warehouse active status is required.")]
        public bool?IsActive { get; set; }

    }
}
