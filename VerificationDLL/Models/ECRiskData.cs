using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VerificationDLL.Models
{
    public class ECRiskData
    {
        public int Id { get; set; }
        public string SurveyNo { get; set; }
        public string MeasuringArea { get; set; }
        public string Village { get; set; }
        public string Hobli { get; set; }
        public string Taluk { get; set; }
        public string District { get; set; }
        public bool IsLitigation { get; set; }
        public bool IsMortgaged { get; set; }
        public string OtherIssues { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}