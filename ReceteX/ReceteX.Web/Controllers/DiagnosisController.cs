using Microsoft.AspNetCore.Mvc;
using ReceteX.Models;
using ReceteX.Repository.Shared.Abstract;
using ReceteX.Utility;
using System.Xml;

namespace ReceteX.Web.Controllers
{
    public class DiagnosisController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly XmlRetriever xmlRetriever;
		public DiagnosisController(IUnitOfWork unitOfWork, XmlRetriever xmlRetriever)
		{
			this.unitOfWork = unitOfWork;
			this.xmlRetriever = xmlRetriever;
		}

		public IActionResult Index()
        {
            return View();
        }

        public async Task ParseAndSaveFromXml(string xmlContent)
        {

            XmlDocument xmlDoc = new XmlDocument();
            XmlNodeList diagnoses = xmlDoc.SelectNodes("/tanilar/tani");


            foreach (XmlNode diagnosis in diagnoses)
            {
                Diagnosis dia = new Diagnosis();

                dia.Name = diagnosis.SelectSingleNode("ad").InnerText;
                dia.Code = diagnosis.SelectSingleNode("kod").InnerText;
                unitOfWork.Diagnoses.Add(dia);
            }
            unitOfWork.Save();
        }


        public async Task<IActionResult> UpdateDiagnosesList()
        {
            string content = await xmlRetriever.GetXmlContent("https://www.ibys.com.tr/exe/tanilar.xml");

            await ParseAndSaveFromXml(content);

            return RedirectToAction("Index"); 
        }


    }
}
