using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataContextManager.Entities
{
    [Table("tbCapture")]
    public class CaptureModel
    {
        [Column("CPT_Id")]
        public int Id { get; set; }

        [Column("CTP_Name")]
        public string Name { get; set; }

        [Column("CPT_Path")]
        public string Path { get; set; }

        [NotMapped]
        public List<DetectionModel> Detections { get; set; }
    }
}
