using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SmartWarehouse.Application.DTOs.Warehouse
{
    public class CreateWarehouseDto
    {
        [Required(ErrorMessage = "Warehouse code is required.")]
        [StringLength(20,ErrorMessage ="Warehouse code must not exeed 20 character")]
        [RegularExpression(@"^WH-[A-Z]{2,3}-\d{2,4}$", ErrorMessage = "Warehouse code  must follow format WH-DN-01")]

        public string Code { get; set; } = string.Empty;


        [Required(ErrorMessage = "Warehouse name is required.")]
        [StringLength(100,ErrorMessage = "Warehouse name must not exceed 150 characters.")]
        public string Name { get; set; } = string.Empty;


        [StringLength(255, ErrorMessage = "Address must not exceed 255 characters.")]
        public string? Address { get; set; }


        

    }
}
