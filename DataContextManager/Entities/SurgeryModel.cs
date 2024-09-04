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

        [NotMapped]
        public List<CriticalMomentModel> CriticalMoments { get; set; }
    }
}