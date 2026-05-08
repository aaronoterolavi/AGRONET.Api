using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace AGRONET.Bienes.Application.DTOs.Common;

public class PagedResultDto<T>
{
    public IReadOnlyList<T> items { get; set; } = new List<T>();
    public int total_count { get; set; }
    public int page_number { get; set; }
    public int page_size { get; set; }
    public int total_pages => (int)Math.Ceiling(total_count / (double)page_size);

    public static PagedResultDto<T> Empty => new PagedResultDto<T>();
}