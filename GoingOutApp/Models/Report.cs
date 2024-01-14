using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoingOutApp.Models
{
    public class Report
    {
        [Key]
        public int Id { get; set; }
        public string ReportDesc { get; set; }
    }
}
