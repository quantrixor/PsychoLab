using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsychoLab.Model
{
    public partial class Session
    {
        [NotMapped] // Этот атрибут указывает, что свойство не должно маппиться в базу данных
        public bool IsSessionStartingInTwoHours
        {
            get
            {
                var sessionDateTime = SessionDate + StartTime;
                var timeSpanTillStart = sessionDateTime - DateTime.Now;
                return timeSpanTillStart <= TimeSpan.FromHours(2) && timeSpanTillStart > TimeSpan.Zero;
            }
        }
    }
}
