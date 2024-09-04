using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataContextManager.Entities
{
    [Table("tbDetection")]
    public class DetectionModel
    {
        [Column("DTC_Id")]
        public int Id { get; set; }

        [Column("CTM_Id")]
        public int CriticalMomentId { get; set; }

        [Column("ITM_Id")]
        public int InstrumentId { get; set; }

        [Column("CPT_Id")]
        public int CaptureId { get; set; }

        [Column("DTC_Probability")]
        public double? Probability { get; set; }

        [Column("DTC_PositionX")]
        public double? PositionX { get; set; }

        [Column("DTC_PositionY")]
        public double? PositionY { get; set; }

        [Column("DTC_Width")]
        public double? Width { get; set; }

        [Column("DTC_Height")]
        public double? Height { get; set; }

        [ForeignKey("CriticalMomentId")]
        public CriticalMomentModel CriticalMomentModel { get; set; }

        [ForeignKey("InstrumentId")]
        public InstrumentModel InstrumentModel { get; set; }

        [ForeignKey("CaptureId")]
        public CaptureModel CaptureModel { get; set; }
    }
}