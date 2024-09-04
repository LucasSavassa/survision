using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataContextManager.Entities
{
    [Table("tbConfiguration")]
    public class ConfigurationModel
    {
        [Column("CFG_Id")]
        public int Id { get; set; }

        [Column("CFG_CaptureFolder")]
        public string CaptureFolder {  get; set; }

        [Column("CFG_QueueFolder")]
        public string QueueFolder {  get; set; }

        [Column("CFG_ProcessingFolder")]
        public string ProcessingFolder {  get; set; }

        [Column("CFG_ProcessedFolder")]
        public string ProcessedFolder {  get; set; }

        [Column("CFG_SaveAllCaptures")]
        public bool? SaveAllCaptures {  get; set; }

        [Column("CFG_BackupFolder")]
        public string BackupFolder {  get; set; }

        [Column("CFG_CaptureInterval")]
        public int? CaptureInterval {  get; set; }
    }
}
