using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataContextManager.Entities
{
    [Table("tbCriticalMoment")]
    public class CriticalMomentModel
    {
        [Column("CTM_Id")]
        public int Id { get; set; }

        [Column("SGR_Id")]
        public int SurgeryId { get; set; }

        [Column("CTM_EventDateTime")]
        public DateTime? EventDateTime { get; set; }

        [Column("CTM_EventTime")]
        public int EventTime { get; set; }

        [ForeignKey("SurgeryId")]
        public SurgeryModel SurgeryModel { get; set; }

        [NotMapped]
        public List<DetectionModel> Detections { get; set; }
    }
}