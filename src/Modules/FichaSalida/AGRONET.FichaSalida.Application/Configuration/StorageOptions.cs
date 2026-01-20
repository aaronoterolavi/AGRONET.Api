using System;
using System.Collections.Generic;
using System.Text;

namespace AGRONET.FichaSalida.Application.Configuration
{
    public sealed class StorageOptions
    {
        public string Mode { get; set; } = "Local"; // Local | UNC
        public string LocalBasePath { get; set; } = @"C:\AGRONET\Uploads\FichaSalida";
        public string UncBasePath { get; set; } = @"\\srvsiga\archivos\AGRONET\FichaSalida";
    }

    public sealed class UploadsOptions
    {
        public int MaxSizeMb { get; set; } = 25;
        public string[] AllowedExtensions { get; set; } = [".pdf", ".docx", ".jpg", ".jpeg", ".png"];
    }
}
