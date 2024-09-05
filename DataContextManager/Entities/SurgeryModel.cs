using DataContextManager.EnumsDataBase;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataContextManager.Entities
{
    [Table("tbSurgery")]
    public class SurgeryModel
    {
        [Column("SGR_Id")]
        public int Id { get; set; }

        [Column("SGR_StartTime")]
        public DateTime? StartTime { get; set; }

        [Column("SGR_Identification")]
        public string Identification { get; set; }

        [Column("SGR_Duration")]
        public int? Duration { get; set; }

        [Column("SGR_PlannedStartTime")]
        public DateTime? PlannedStartTime { get; set; }

        [Column("SGR_SurgeryRoom")]
        public string SurgeryRoom { get; set; }

        [Column("SGR_PatientName")]
        public string PatientName { get; set; }

        [Column("SGR_SurgeryType")]
        public string SurgeryType { get; set; }

        [Column("SGR_SurgeryStatus")]
        public SurgeryStatus? SurgeryStatus { get; set; }

        [NotMapped]
        public List<CriticalCaptureModel> CriticalMoments { get; set; }
    }
}