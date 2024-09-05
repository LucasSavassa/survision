using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataContextManager.Entities
{
    [Table("tbCriticalCapture")]
    public class CriticalCaptureModel
    {
        [Column("CTC_Id")]
        public int Id { get; set; }

        [Column("SGR_Id")]
        public int SurgeryId { get; set; }

        [Column("CTC_EventDateTime")]
        public DateTime? EventDateTime { get; set; }

        [Column("CTC_EventTime")]
        public int EventTime { get; set; }

        [Column("CTC_CaptureName")]
        public string CaptureName { get; set; }

        [Column("CTC_CapturePath")]
        public string CapturePath { get; set; }

        [ForeignKey("SurgeryId")]
        public SurgeryModel SurgeryModel { get; set; }

        [NotMapped]
        public List<DetectionModel> Detections { get; set; }
    }
}