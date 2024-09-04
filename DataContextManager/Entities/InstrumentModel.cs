using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace DataContextManager.Entities
{
    [Table("tbInstrument")]
    public class InstrumentModel
    {
        [Column("ITM_Id")]
        public int Id { get; set; }

        [Column("ITM_Name")]
        public string Name { get; set; }

        [NotMapped]
        public List<DetectionModel> Detections { get; set; }
    }
}
