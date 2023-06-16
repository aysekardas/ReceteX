using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReceteX.Models;
using ReceteX.Repository.Shared.Abstract;
using ReceteX.Utility;
using System.Xml;

namespace ReceteX.Web.Controllers
{
    public class MedicineController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly XmlRetriever xmlRetriever;
        //parametre olarak ekledik

        public MedicineController(IUnitOfWork unitOfWork, XmlRetriever xmlRetriever)
        {
            this.unitOfWork = unitOfWork;
            this.xmlRetriever = xmlRetriever;
        }

        public IActionResult Index()
        {
            return View();
        }

        //ayıklayacak
        public async Task ParseAndSaveFromXml(string xmlContent)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlContent);

            //ilaçların altındaki her bir ilaç için
            XmlNodeList medicinesFromXml = xmlDoc.SelectNodes("/ilaclar/ilac");
            //databasedeki tüm aktif nesneler
            IQueryable<Medicine> medicinesFromDb = unitOfWork.Medicines.GetAll().AsNoTracking().OrderBy(m => m.Name).ToList().AsQueryable<Medicine>();
            //database'deki tüm silinmiş nesneler
            IQueryable<Medicine> deletedMedicinesFromDb = unitOfWork.Medicines.GetAllDeleted().AsNoTracking().OrderBy(m => m.Name).ToList().AsQueryable<Medicine>();


            //Case 1 :
            //Yeni kayıtları aktaran döngümüz.
            foreach (XmlNode medicine in medicinesFromXml)
            {
                string barcodeFromXml = medicine.SelectSingleNode("barkod").InnerText;

                //Bu barkoda sahip en az bir kayıt var mı any ile yapıyoruz
                if (!medicinesFromDb.Any(m => m.Barcode == barcodeFromXml))
                {
                    Medicine med = new Medicine();
                    med.Name = medicine.SelectSingleNode("ad").InnerText;
                    med.Barcode = barcodeFromXml;
                    unitOfWork.Medicines.Add(med);

                    //Her seferinde save changes yapmıyorum, memoryde 8.000 tane çalışıyor
                }

                else
                {
                    Medicine medSilinmis = deletedMedicinesFromDb.FirstOrDefault(m => m.Barcode == barcodeFromXml);
                    if (medSilinmis != null)
                    {
                        medSilinmis.isDeleted = false;
                        unitOfWork.Medicines.Update(medSilinmis);
                    }
                }
            }
            //Hepsi bittikten sonra yapıyorum
            unitOfWork.Save();



            //Kaynaktan silinmiş olan ilaçları databasede isdelete=true yapan döngümüz
            //XmlNodeListe extension methodla sorgu atamıyoruz modifiye ederek yapıyoruz

            IEnumerable<XmlNode> medicinesFromXmlEnumarable =xmlDoc.SelectNodes("/ilaclar/ilac").Cast<XmlNode>();
            foreach (Medicine ilac in medicinesFromDb)
            {
               if(!medicinesFromXmlEnumarable.Any(x=>x.SelectSingleNode("barkod").InnerText==ilac.Barcode))
                {
                    //remove metodu zaten isDeleted true hale getiriyor denedik ama bu seferde db çok yordu
                    //ilac.isDeleted=true;
                    //remove metodu sadece guid id alıyor
                    //unitOfWork.Medicines.Remove(ilac.Id);

                    ilac.isDeleted = true;
                    unitOfWork.Medicines.Update(ilac);
                    
                }
                
            }


            unitOfWork.Save();
        }

        public async Task<IActionResult> UpdateMedicinesList()
        {
            //yapacağı işler bitince sayfaya gidince sayfaya yönlensin
            //ajax kullanmaycağım çok fazla data var

            // Case 1 :
            // string content = await xmlRetriever.GetXmlContent("https://www.ibys.com.tr/exe/ilaclar.xml");

            //Case 2 :
            //bakanlıkta olmayıp bizde olanları tespit etmek için yaptık
            string content = await xmlRetriever.GetXmlContent("https://www.ibys.com.tr/exe/ilaclar.xml");
            await ParseAndSaveFromXml(content);

            //Bu şekilde tek satırda da yazabiliriz.
            //await ParseAndSaveFromXml(await xmlRetriever.GetXmlContent("https://www.ibys.com.tr/exe/ilaclar.xml"));
            return RedirectToAction("Index");
        }
    }
}
