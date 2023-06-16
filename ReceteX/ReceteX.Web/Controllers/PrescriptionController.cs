﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReceteX.Models;
using ReceteX.Repository.Shared.Abstract;
using System.Security.Claims;

namespace ReceteX.Web.Controllers
{
    [Authorize]
    public class PrescriptionController : Controller
    {
        private readonly IUnitOfWork unitOfWork;

        public PrescriptionController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }


        public IActionResult Index()
        {
            Prescription prescription = new Prescription();
            prescription.StatusId = unitOfWork.Statuses.GetFirstOrDefault(s => s.Name == "Taslak").Id;
            prescription.AppUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            unitOfWork.Prescriptions.Add(prescription);
            unitOfWork.Save();
            return View(prescription);


        }


        [HttpPost]
        public IActionResult Create(Prescription prescription)
        {
            unitOfWork.Prescriptions.Add(prescription);
            unitOfWork.Save();

            return Json(prescription);
        }

        [HttpPost]
        //reçeteye tanı ekleyeceğiz
        public IActionResult AddDiagnosis(Guid prescriptionId, Guid diagnosisId)
        {
            Prescription asil = unitOfWork.Prescriptions.GetAll(p => p.Id == prescriptionId).Include(p => p.Diagnoses).First();
            Diagnosis asilDiagnosis = unitOfWork.Diagnoses.GetFirstOrDefault(d => d.Id == diagnosisId);

            asil.Diagnoses.Add(asilDiagnosis);
            unitOfWork.Prescriptions.Update(asil);
            unitOfWork.Save();

            return Ok();

        }

        [HttpPost]
        public IActionResult GetDiagnoses(Guid prescriptionId)
        {
            return Json(unitOfWork.Prescriptions.GetAll(p => p.Id == prescriptionId).Include(p => p.Diagnoses));

        }

        [HttpPost]
        public IActionResult AddDescription(Guid prescriptionId, Description description)
        {
            Prescription asil = unitOfWork.Prescriptions.GetFirstOrDefault(p => p.Id == prescriptionId);
            asil.Descriptions.Add(description);
            unitOfWork.Prescriptions.Update(asil);
            unitOfWork.Save();
            return Ok();
        }

        [HttpPost]
        public IActionResult AddMedicine(Guid prescriptionId, PrescriptionMedicine prescriptionMedicine)
        {
            Prescription asil = unitOfWork.Prescriptions.GetFirstOrDefault(p => p.Id == prescriptionId);
            asil.PrescriptionMedicines.Add(prescriptionMedicine);
            unitOfWork.Prescriptions.Update(asil);
            unitOfWork.Save();
            return Ok();

        }

        [HttpPost]
        public IActionResult RemoveMedicine(Guid prescriptionId, Guid prescriptionMedicineId)
        {

            Prescription asil = unitOfWork.Prescriptions.GetFirstOrDefault(p => p.Id == prescriptionId);

            asil.PrescriptionMedicines.Remove(asil.PrescriptionMedicines.FirstOrDefault(pm => pm.Id == prescriptionMedicineId));

            unitOfWork.Prescriptions.Update(asil);
            unitOfWork.Save();
            return Ok();

        }

        [HttpPost]
        public IActionResult RemoveDescription(Guid prescriptionId, Guid descriptionId)
        {
            Prescription asil = unitOfWork.Prescriptions.GetFirstOrDefault(p => p.Id == prescriptionId);
            asil.Descriptions.Remove(asil.Descriptions.FirstOrDefault(d => d.Id == descriptionId));

            unitOfWork.Prescriptions.Update(asil);
            unitOfWork.Save();
            return Ok();

        }

        [HttpPost]
        public IActionResult RemoveDiagnosis(Guid prescriptionId, Guid diagnosisId)
        {
            Prescription asil = unitOfWork.Prescriptions.GetFirstOrDefault(p => p.Id == prescriptionId);
            asil.Diagnoses.Remove(asil.Diagnoses.FirstOrDefault(d => d.Id == diagnosisId));

            unitOfWork.Prescriptions.Update(asil);
            unitOfWork.Save();
            return Ok();

        }



    }
}

//GET: GET isteği, belirtilen bir kaynağın(örneğin bir web sayfası veya bir veri) alınmasını istemek için kullanılır. Bu yöntem, sunucudan veriyi almak için kullanılır ve genellikle bir kaynağın okunması veya görüntülenmesi için kullanılır. GET istekleri genellikle URL üzerinden parametrelerle iletilir ve verilerin sunucudan alınmasını sağlar.

//POST: POST isteği, sunucuya veri göndermek için kullanılır. Bu yöntem, bir kaynağa yeni bir veri eklemek veya mevcut bir kaynağı değiştirmek için kullanılır. POST istekleri genellikle bir formun sunucuya gönderilmesi veya bir API'ye veri gönderilmesi gibi durumlarda kullanılır.

//PUT: PUT isteği, belirtilen bir kaynağı güncellemek için kullanılır. Bu yöntem, belirli bir URL'deki kaynağın tamamen değiştirilmesini sağlar. Bir PUT isteği gönderildiğinde, sunucu istekte belirtilen kaynağı günceller veya oluşturur.

//DELETE: DELETE isteği, belirtilen bir kaynağı silmek için kullanılır. Bu yöntem, bir URL'deki kaynağın sunucu tarafından silinmesini sağlar. DELETE isteği gönderildiğinde, sunucu istekte belirtilen kaynağı siler.