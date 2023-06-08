using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReceteX.Models
{
    public class Prescription:BaseModel
    {
        //sadece kendi yazdığı reçeteleri görüntüleyebilecek diğer doktorların yazdığı reçeteleri görmeyecek

        public Guid AppUserId { get; set; }

        //bir reçetenin içerisinde birden fazla reçete ilacı olabilir
        //ama bu şekilde yazılmıi ilaç sadece bir reçetede olabilir
        //bu şekilde otomatik olarak relation tablosu oluşturacak
        public ICollection<PrescriptionMedicine>? PrescriptionMedicines { get; set; }

        //bir reçetenin birden fazla açıklaması olabiliyor(açıklama türü)
    }
}
